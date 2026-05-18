using System.Collections.Generic;

// Configuración de un nivel de membresía (Bronze, Silver, Gold, Platinum).
// Viene de la tabla membership_tiers_config de Supabase.

namespace HoloCrew.Models
{
    public class MembershipTierConfig
    {
        public string Tier { get; set; } = string.Empty;          // "bronze", "silver", "gold", "platinum"
        public string Name { get; set; } = string.Empty;          // "BRONZE", "SILVER", etc.
        public int MinPoints { get; set; }
        public int MaxPoints { get; set; }
        public decimal PointsPerEuro { get; set; }
        public int EarlyAccessHours { get; set; }
        public decimal FreeShippingThreshold { get; set; }
        public int BirthdayBonusCredits { get; set; }
        public List<string> Benefits { get; set; } = new();
        public string ImageUrl { get; set; } = string.Empty;
    }
}