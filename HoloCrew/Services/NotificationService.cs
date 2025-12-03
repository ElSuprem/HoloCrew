using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace HoloCrew.Services
{
    /// <summary>
    /// Implementación del servicio de notificaciones
    /// </summary>
    public class NotificationService : INotificationService
    {
        private List<Notification> _notifications = new();
        private int _nextNotificationId = 1;

        public event EventHandler<Notification> NotificationReceived;

        public Task<List<Notification>> GetNotificationsAsync(int userId)
        {
            var userNotifications = _notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return Task.FromResult(userNotifications);
        }

        public Task MarkAsReadAsync(int notificationId)
        {
            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);

            if (notification != null)
            {
                notification.IsRead = true;
            }

            return Task.CompletedTask;
        }

        public Task DeleteNotificationAsync(int notificationId)
        {
            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);

            if (notification != null)
            {
                _notifications.Remove(notification);
            }

            return Task.CompletedTask;
        }

        public int GetUnreadCount()
        {
            return _notifications.Count(n => !n.IsRead);
        }

        public void ShowToast(string title, string message, NotificationType type)
        {
            // TODO: Implementar notificación toast en la UI
            // Podría usar un sistema de ventanas emergentes o un área de notificaciones

            System.Diagnostics.Debug.WriteLine($"[TOAST] {type}: {title} - {message}");
        }

        public Task SendNotificationAsync(int userId, Notification notification)
        {
            notification.Id = _nextNotificationId++;
            notification.UserId = userId;
            notification.CreatedAt = DateTime.Now;
            notification.IsRead = false;

            _notifications.Add(notification);

            // Disparar evento
            OnNotificationReceived(notification);

            return Task.CompletedTask;
        }

        public Task MarkAllAsReadAsync(int userId)
        {
            var userNotifications = _notifications.Where(n => n.UserId == userId);

            foreach (var notification in userNotifications)
            {
                notification.IsRead = true;
            }

            return Task.CompletedTask;
        }

        private void OnNotificationReceived(Notification notification)
        {
            NotificationReceived?.Invoke(this, notification);
        }
    }
}