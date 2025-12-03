namespace HoloCrew.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Address> Addresses { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public NotificationSettings NotificationPreferences { get; set; }
    }
}