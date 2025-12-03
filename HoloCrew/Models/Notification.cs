namespace HoloCrew.Models
{
    // ===== ENUM =====
    public enum NotificationType
    {
        OrderUpdate = 0,
        Promotion = 1,
        NewProduct = 2,
        PriceAlert = 3,
        System = 4
    }

    // ===== CLASE PRINCIPAL =====
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string IconUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ActionUrl { get; set; }
    }
}