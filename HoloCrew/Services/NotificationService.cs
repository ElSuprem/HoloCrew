using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;
using Supabase.Realtime;
using Supabase.Realtime.Interfaces;
using Supabase.Realtime.PostgresChanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static Supabase.Realtime.Constants;
using static Supabase.Realtime.PostgresChanges.PostgresChangesOptions;

// Servicio de notificaciones conectado a Supabase.
// Mantiene una caché en memoria para que GetUnreadCount() sea instantáneo.
// Se suscribe a Supabase Realtime para recibir notificaciones nuevas en tiempo real:
// cuando un trigger SQL inserta una notificación en la tabla, esta clase se entera al
// instante vía WebSocket y dispara NotificationReceived. La UI (badge del navbar) se
// refresca sin polling.

namespace HoloCrew.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IAuthenticationService _authService;
        private readonly Supabase.Client _supabase;
        private List<Notification> _cache = new();
        private RealtimeChannel? _realtimeChannel;

        public event EventHandler<Notification>? NotificationReceived;

        public NotificationService(
            INotificationRepository repository,
            IAuthenticationService authService,
            Supabase.Client supabase)
        {
            _repository = repository;
            _authService = authService;
            _supabase = supabase;

            // Recargamos caché y resuscribimos a Realtime al cambiar el estado de auth
            _authService.AuthStateChanged += async (s, e) =>
            {
                await LoadNotificationsAsync();
                await SubscribeToRealtimeAsync();
            };
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
                if (item != null) item.IsRead = true;

                // Notificar a la UI (badge del navbar) que el contador ha cambiado
                FireNotificationChanged();
            }
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            var ok = await _repository.DeleteAsync(notificationId);
            if (ok)
            {
                var item = _cache.FirstOrDefault(n => n.Id == notificationId);
                if (item != null) _cache.Remove(item);

                FireNotificationChanged();
            }
        }

        public int GetUnreadCount()
        {
            return _cache.Count(n => !n.IsRead);
        }

        public void ShowToast(string title, string message, NotificationType type)
        {
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
                foreach (var n in _cache) n.IsRead = true;

                FireNotificationChanged();
            }



        }

        // Dispara el evento NotificationReceived para avisar a la UI (badge del navbar
        // principalmente) de que el contador o estado de notificaciones ha cambiado.
        // Se llama desde MarkAsRead, DeleteNotification y MarkAllAsRead, no solo desde
        // SendNotification, para que el badge se mantenga sincronizado.
        private void FireNotificationChanged()
        {
            try
            {
                System.Windows.Application.Current?.Dispatcher.Invoke(() =>
                {
                    NotificationReceived?.Invoke(this, new Notification());
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Notifications] Fire event error: {ex.Message}");
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


        // ==================== REALTIME ====================

        // Se suscribe a inserciones en la tabla notifications para el usuario actual.
        // Cuando el trigger SQL inserta una notificación (por ejemplo al crear un pedido),
        // este método recibe el evento y dispara NotificationReceived para que la UI
        // (badge del navbar, etc.) se refresque al instante sin polling.
        private async Task SubscribeToRealtimeAsync()
        {
            try
            {
                // Desuscribir el canal anterior si existía (cambio de usuario)
                if (_realtimeChannel != null)
                {
                    _realtimeChannel.Unsubscribe();
                    _realtimeChannel = null;
                }

                var user = _authService.GetCurrentUser();
                if (user == null || string.IsNullOrEmpty(user.Id)) return;

                // CRÍTICO: hay que conectar el socket de Realtime antes de suscribirse a canales.
                // Sin esto, Supabase lanza "Socket must exist, was Connect called?".
                // Connect() es idempotente: si ya está conectado, no hace nada.
                await _supabase.Realtime.ConnectAsync();

                // Suscribirse a INSERTs en la tabla notifications
                _realtimeChannel = _supabase.Realtime.Channel("realtime", "public", "notifications");
                _realtimeChannel.AddPostgresChangeHandler(ListenType.Inserts, OnRealtimeNotificationInserted);

                await _realtimeChannel.Subscribe();

                System.Diagnostics.Debug.WriteLine($"[Realtime] Subscribed to notifications for user {user.Id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Realtime] Subscribe error: {ex.Message}");
            }
        }

        // Callback que se dispara cuando se inserta una notificación nueva en BBDD.
        // Filtra por usuario y dispara el evento en el hilo de UI.
        private async void OnRealtimeNotificationInserted(IRealtimeChannel sender, PostgresChangesResponse change)
        {
            try
            {
                var user = _authService.GetCurrentUser();
                if (user == null || string.IsNullOrEmpty(user.Id)) return;

                // Recargar la caché completa (más simple que parsear el payload manualmente)
                await LoadNotificationsAsync();

                // Disparar el evento en el hilo de UI para que el binding se actualice bien
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    NotificationReceived?.Invoke(this, new Notification());
                });

                System.Diagnostics.Debug.WriteLine("[Realtime] New notification received and cache refreshed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Realtime] Insert handler error: {ex.Message}");
            }
        }
    }
}