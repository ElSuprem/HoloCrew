using HoloCrew.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Servicio para gestionar notificaciones del usuario.
// Puede mostrar mensajes emergentes (toast) y avisa con NotificationReceived cuando llega una nueva.

namespace HoloCrew.Services.Interfaces
{
    public interface INotificationService
    {
        event EventHandler<Notification> NotificationReceived;

        Task<List<Notification>> GetNotificationsAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task DeleteNotificationAsync(int notificationId);
        int GetUnreadCount();
        void ShowToast(string title, string message, NotificationType type);
        Task SendNotificationAsync(string userId, Notification notification);
        Task MarkAllAsReadAsync(string userId);
        Task LoadNotificationsAsync();  // recarga la caché desde BD
    }
}