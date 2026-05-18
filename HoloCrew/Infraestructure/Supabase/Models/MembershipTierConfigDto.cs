using HoloCrew.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Collections.Generic;

// DTO que mapea la tabla 'membership_tiers_config' de Supabase.

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("membership_tiers_config")]
    public class MembershipTierConfigDto : BaseModel
    {
        // El PK de esta tabla es el campo 'tier' (USER-DEFINED enum).
        // En el SDK lo tratamos como string.
        [PrimaryKey("tier", false)]
        public string Tier { get; set; } = string.Empty;

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("min_points")]
        public int MinPoints { get; set; }

        [Column("max_points")]
        public int? MaxPoints { get; set; }

        [Column("points_per_euro")]
        public decimal PointsPerEuro { get; set; }

        [Column("early_access_hours")]
        public int EarlyAccessHours { get; set; }

        [Column("free_shipping_threshold")]
        public decimal FreeShippingThreshold { get; set; }

        [Column("birthday_bonus_credits")]
        public int BirthdayBonusCredits { get; set; }

        // El campo benefits viene como jsonb (array de strings).
        // El SDK lo deserializa directamente a List<string>.
        [Column("benefits")]
        public List<string>? Benefits { get; set; }

        [Column("image_url")]
        public string? ImageUrl { get; set; }


        // Convierte este DTO al modelo MembershipTierConfig.
        public MembershipTierConfig ToModel()
        {
            return new MembershipTierConfig
            {
                Tier = Tier,
                Name = Name,
                MinPoints = MinPoints,
                MaxPoints = MaxPoints ?? int.MaxValue,
                PointsPerEuro = PointsPerEuro,
                EarlyAccessHours = EarlyAccessHours,
                FreeShippingThreshold = FreeShippingThreshold,
                BirthdayBonusCredits = BirthdayBonusCredits,
                Benefits = Benefits ?? new List<string>(),
                ImageUrl = ImageUrl ?? string.Empty
            };
        }
    }
}