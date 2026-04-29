using HoloCrew.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

// Repositorio para gestionar notificaciones del usuario contra Supabase.

namespace HoloCrew.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByUserIdAsync(string userId);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(string userId);
        Task<bool> DeleteAsync(int notificationId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<bool> CreateAsync(string userId, string type, string title, string message);
    }
}