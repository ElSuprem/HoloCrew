using CommunityToolkit.Mvvm.ComponentModel;
using System;

// Notificación que recibe el usuario (pedidos, promociones, alertas, etc.)
// UserId es UUID de Supabase Auth.
// TimeAgo calcula el tiempo transcurrido en formato "5m ago", "2h ago", etc.

namespace HoloCrew.Models
{
    public partial class Notification : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string _userId = string.Empty;  // UUID

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _message = string.Empty;

        [ObservableProperty]
        private string _type = string.Empty; // "order_status", "promotion", "wishlist_sale", etc.

        [ObservableProperty]
        private DateTime _createdAt;

        [ObservableProperty]
        private bool _isRead;

        [ObservableProperty]
        private string _icon = string.Empty;

        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - CreatedAt;
                if (timeSpan.TotalMinutes < 1)
                    return "Just now";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes}m ago";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours}h ago";
                if (timeSpan.TotalDays < 7)
                    return $"{(int)timeSpan.TotalDays}d ago";
                if (timeSpan.TotalDays < 30)
                    return $"{(int)(timeSpan.TotalDays / 7)}w ago";
                return CreatedAt.ToString("MMM dd");
            }
        }
    }
}