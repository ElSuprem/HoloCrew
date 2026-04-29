using HoloCrew.Infraestructure.Supabase.Models;
using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using PgConstants = Supabase.Postgrest.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Repositorio de notificaciones conectado a Supabase.
// La mayoría de notificaciones las crean automáticamente los triggers de BD;
// este repositorio se encarga de listarlas, marcarlas y borrarlas.

namespace HoloCrew.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly Supabase.Client _supabase;

        public NotificationRepository(Supabase.Client supabase)
        {
            _supabase = supabase;
        }


        public async Task<List<Notification>> GetByUserIdAsync(string userId)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                    return new List<Notification>();

                var response = await _supabase
                    .From<NotificationDto>()
                    .Where(n => n.UserId == userGuid)
                    .Order("created_at", PgConstants.Ordering.Descending)
                    .Get();

                return response.Models
                    .Select(n => n.ToNotification())
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Notif] GetByUser error: {ex.Message}");
                return new List<Notification>();
            }
        }


        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            try
            {
                await _supabase
                    .From<NotificationDto>()
                    .Where(n => n.Id == notificationId)
                    .Set(n => n.IsRead, true)
                    .Set(n => n.ReadAt, DateTime.UtcNow)
                    .Update();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Notif] MarkAsRead error: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                    return false;

                await _supabase
                    .From<NotificationDto>()
                    .Where(n => n.UserId == userGuid)
                    .Where(n => n.IsRead == false)
                    .Set(n => n.IsRead, true)
                    .Set(n => n.ReadAt, DateTime.UtcNow)
                    .Update();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Notif] MarkAllAsRead error: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> DeleteAsync(int notificationId)
        {
            try
            {
                await _supabase
                    .From<NotificationDto>()
                    .Where(n => n.Id == notificationId)
                    .Delete();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Notif] Delete error: {ex.Message}");
                return false;
            }
        }


        public async Task<int> GetUnreadCountAsync(string userId)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                    return 0;

                var response = await _supabase
                    .From<NotificationDto>()
                    .Where(n => n.UserId == userGuid)
                    .Where(n => n.IsRead == false)
                    .Get();

                return response.Models.Count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Notif] GetUnreadCount error: {ex.Message}");
                return 0;
            }
        }


        public async Task<bool> CreateAsync(string userId, string type, string title, string message)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                    return false;

                var dto = new NotificationDto
                {
                    UserId = userGuid,
                    Type = type,
                    Title = title,
                    Message = message,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _supabase.From<NotificationDto>().Insert(dto);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Notif] Create error: {ex.Message}");
                return false;
            }
        }
    }
}