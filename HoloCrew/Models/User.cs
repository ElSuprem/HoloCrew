using System;
using System.Collections.Generic;

namespace HoloCrew.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<Address> Addresses { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public NotificationSettings NotificationPreferences { get; set; }
    }

    // ========================================
    // ⭐ AGREGADO: Configuración de la app
    // ========================================

    /// <summary>
    /// Configuración de la aplicación guardada en JSON
    /// Ubicación: %AppData%/HoloCrew/settings.json
    /// </summary>
    public class AppSettings
    {
        // APARIENCIA
        public bool IsDarkMode { get; set; } = false;
        public string SelectedLanguage { get; set; } = "Español";

        // NOTIFICACIONES
        public bool NotificationsEnabled { get; set; } = true;
        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = true;
        public bool OrderUpdates { get; set; } = true;
        public bool PromotionalEmails { get; set; } = false;

        // PRIVACIDAD
        public bool DataCollectionEnabled { get; set; } = true;
        public bool PersonalizedAds { get; set; } = false;
        public bool ShareDataWithPartners { get; set; } = false;

        // METADATA
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}