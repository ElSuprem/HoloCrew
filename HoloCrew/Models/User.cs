using System;
using System.Collections.Generic;

// Usuario de la aplicación. Guarda sus datos personales, direcciones,
// métodos de pago y preferencias de notificación.
// La configuración de la app se guarda en %AppData%/HoloCrew/settings.json

namespace HoloCrew.Models
{
    public class User
    {
        public string Id { get; set; } = string.Empty;   // UUID de Supabase Auth
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }           // ya no se usa con Supabase Auth
        public string PhoneNumber { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<Address> Addresses { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public NotificationSettings NotificationPreferences { get; set; }

        // Datos de membresía (vienen de la tabla profiles).
        public string MembershipTier { get; set; } = "bronze";
        public int Credits { get; set; }
        public int LifetimePoints { get; set; }
        public DateTime MemberSince { get; set; }
    }

    // Configuración de la app: modo oscuro, idioma, notificaciones, privacidad, etc.
    public class AppSettings
    {
        public bool IsDarkMode { get; set; } = false;
        public string SelectedLanguage { get; set; } = "Español";
        public bool NotificationsEnabled { get; set; } = true;
        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = true;
        public bool OrderUpdates { get; set; } = true;
        public bool PromotionalEmails { get; set; } = false;
        public bool DataCollectionEnabled { get; set; } = true;
        public bool PersonalizedAds { get; set; } = false;
        public bool ShareDataWithPartners { get; set; } = false;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }


}