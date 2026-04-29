using HoloCrew.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

// DTO que mapea la tabla 'notifications' de Supabase.
// Las notificaciones se crean automáticamente desde triggers en BD
// (ej: cuando un pedido cambia a 'shipped', se inserta una notificación).

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("notifications")]
    public class NotificationDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("type")]
        public string Type { get; set; } = string.Empty;

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("message")]
        public string Message { get; set; } = string.Empty;

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Column("action_type")]
        public string? ActionType { get; set; }

        [Column("action_reference")]
        public string? ActionReference { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; }

        [Column("read_at")]
        public DateTime? ReadAt { get; set; }

        [Column("metadata")]
        public string? Metadata { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }


        public Notification ToNotification()
        {
            return new Notification
            {
                Id = Id,
                UserId = UserId.ToString(),
                Type = Type,
                Title = Title,
                Message = Message,
                IsRead = IsRead,
                CreatedAt = CreatedAt,
                Icon = GetIconForType(Type)
            };
        }


        // Asigna un emoji según el tipo de notificación (puramente visual)
        private static string GetIconForType(string type) => type?.ToLower() switch
        {
            "order_status" => "📦",
            "order_shipped" => "🚚",
            "order_delivered" => "✅",
            "promotion" => "🎁",
            "wishlist_sale" => "💝",
            "stock_back" => "📢",
            "review_reply" => "💬",
            _ => "🔔"
        };
    }
}