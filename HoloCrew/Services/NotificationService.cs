using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Servicio de notificaciones conectado a Supabase a través del repositorio.
// Mantiene una caché en memoria para que GetUnreadCount() sea instantáneo (lo llama la UI mucho).
// La caché se actualiza al hacer login y tras cada operación.

namespace HoloCrew.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IAuthenticationService _authService;
        private List<Notification> _cache = new();

        public event EventHandler<Notification>? NotificationReceived;

        public NotificationService(
            INotificationRepository repository,
            IAuthenticationService authService)
        {
            _repository = repository;
            _authService = authService;

            // Recargamos la caché cuando cambia el estado de auth
            _authService.AuthStateChanged += async (s, e) => await LoadNotificationsAsync();
        }


        public async Task<List<Notification>> GetNotificationsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Notification>();

            var notifications = await _repository.GetByUserIdAsync(userId);
            _cache = notifications;
            return notifications;
        }


        public async Task MarkAsReadAsync(int notificationId)
        {
            var ok = await _repository.MarkAsReadAsync(notificationId);
            if (ok)
            {
                var item = _cache.FirstOrDefault(n => n.Id == notificationId);
                if (item != null)
                    item.IsRead = true;
            }
        }


        public async Task DeleteNotificationAsync(int notificationId)
        {
            var ok = await _repository.DeleteAsync(notificationId);
            if (ok)
            {
                var item = _cache.FirstOrDefault(n => n.Id == notificationId);
                if (item != null)
                    _cache.Remove(item);
            }
        }


        public int GetUnreadCount()
        {
            return _cache.Count(n => !n.IsRead);
        }


        public void ShowToast(string title, string message, NotificationType type)
        {
            // Aquí se podría mostrar un toast en pantalla.
            // De momento solo escribe en la consola de depuración.
            System.Diagnostics.Debug.WriteLine($"[TOAST] {type}: {title} - {message}");
        }


        public async Task SendNotificationAsync(string userId, Notification notification)
        {
            if (string.IsNullOrEmpty(userId) || notification == null) return;

            var ok = await _repository.CreateAsync(
                userId,
                notification.Type ?? "info",
                notification.Title ?? string.Empty,
                notification.Message ?? string.Empty);

            if (ok)
            {
                // Recargamos la caché para que aparezca la nueva
                await LoadNotificationsAsync();
                NotificationReceived?.Invoke(this, notification);
            }
        }


        public async Task MarkAllAsReadAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return;

            var ok = await _repository.MarkAllAsReadAsync(userId);
            if (ok)
            {
                foreach (var n in _cache)
                    n.IsRead = true;
            }
        }


        // Recarga la caché desde BD usando el usuario actualmente logueado.
        public async Task LoadNotificationsAsync()
        {
            var user = _authService.GetCurrentUser();
            if (user == null || string.IsNullOrEmpty(user.Id))
            {
                _cache.Clear();
                return;
            }

            _cache = await _repository.GetByUserIdAsync(user.Id);
        }
    }
}