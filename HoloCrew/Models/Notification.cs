using CommunityToolkit.Mvvm.ComponentModel;

namespace HoloCrew.Models
{
    /// <summary>
    /// Modelo de notificación
    /// </summary>
    public partial class Notification : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private int _userId;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _message;

        [ObservableProperty]
        private string _type; // "Order", "Promotion", "Alert", "Info"

        [ObservableProperty]
        private DateTime _createdAt;

        [ObservableProperty]
        private bool _isRead;

        [ObservableProperty]
        private string _icon; // Emoji o icono

        /// <summary>
        /// Tiempo transcurrido desde la notificación
        /// </summary>
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