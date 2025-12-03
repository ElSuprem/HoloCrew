namespace HoloCrew.Models
{
    public class NotificationSettings
    {
        public bool OrderUpdatesEnabled { get; set; }
        public bool PromotionsEnabled { get; set; }
        public bool NewProductsEnabled { get; set; }
        public bool PriceAlertsEnabled { get; set; }
        public bool EmailNotificationsEnabled { get; set; }
        public bool PushNotificationsEnabled { get; set; }
    }
}