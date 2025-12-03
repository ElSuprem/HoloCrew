using HoloCrew.Models;

namespace HoloCrew.Services.Interfaces
{
    /// <summary>
    /// Servicio para gestión de notificaciones
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Evento que se dispara cuando se recibe una nueva notificación
        /// </summary>
        event EventHandler<Notification> NotificationReceived;

        /// <summary>
        /// Obtiene todas las notificaciones de un usuario
        /// </summary>
        Task<List<Notification>> GetNotificationsAsync(int userId);

        /// <summary>
        /// Marca una notificación como leída
        /// </summary>
        Task MarkAsReadAsync(int notificationId);

        /// <summary>
        /// Elimina una notificación
        /// </summary>
        Task DeleteNotificationAsync(int notificationId);

        /// <summary>
        /// Obtiene el conteo de notificaciones no leídas
        /// </summary>
        int GetUnreadCount();

        /// <summary>
        /// Muestra una notificación toast en la interfaz
        /// </summary>
        void ShowToast(string title, string message, NotificationType type);

        /// <summary>
        /// Envía una notificación push al usuario
        /// </summary>
        Task SendNotificationAsync(int userId, Notification notification);

        /// <summary>
        /// Marca todas las notificaciones como leídas
        /// </summary>
        Task MarkAllAsReadAsync(int userId);
    }
}