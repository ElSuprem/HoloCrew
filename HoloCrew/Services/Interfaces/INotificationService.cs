using HoloCrew.Models;

// Servicio para gestionar notificaciones del usuario.
// Puede mostrar mensajes emergentes (toast) y avisa con NotificationReceived cuando llega una nueva.

namespace HoloCrew.Services.Interfaces
{
    public interface INotificationService
    {
        event EventHandler<Notification> NotificationReceived;  // salta cuando llega una notificación nueva

        Task<List<Notification>> GetNotificationsAsync(int userId);  // todas las notificaciones de un usuario
        Task MarkAsReadAsync(int notificationId);                    // marcar una como leída
        Task DeleteNotificationAsync(int notificationId);            // borrar una notificación
        int GetUnreadCount();                                        // cuántas no leídas hay
        void ShowToast(string title, string message, NotificationType type);  // mensaje emergente en pantalla
        Task SendNotificationAsync(int userId, Notification notification);     // enviar notificación a un usuario
        Task MarkAllAsReadAsync(int userId);                         // marcar todas como leídas
    }
}