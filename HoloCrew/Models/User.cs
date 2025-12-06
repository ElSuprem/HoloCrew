using System;
using System.Collections.Generic;

namespace HoloCrew.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // ⭐ AGREGADO - Necesario para AuthenticationService
        public string PhoneNumber { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; } // ⭐ AGREGADO - Para tracking de login
        public List<Address> Addresses { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public NotificationSettings NotificationPreferences { get; set; }
    }
}