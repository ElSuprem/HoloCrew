using HoloCrew.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

// DTO que mapea la tabla 'profiles' de Supabase.
// Se enlaza con auth.users a través del campo 'id' (uuid).

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("profiles")]
    public class ProfileDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Column("membership_tier")]
        public string MembershipTier { get; set; } = "bronze";

        [Column("credits")]
        public int Credits { get; set; }

        [Column("lifetime_points")]
        public int LifetimePoints { get; set; }

        [Column("member_since")]
        public DateTime? MemberSince { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("last_login_at")]
        public DateTime? LastLoginAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }


        // Convierte este DTO al modelo User que usan los ViewModels.
        public User ToUser()
        {
            return new User
            {
                Id = Id.ToString(),
                Email = Email,
                FullName = $"{FirstName} {LastName}".Trim(),
                PhoneNumber = Phone ?? string.Empty,
                ProfileImageUrl = AvatarUrl ?? string.Empty,
                CreatedAt = CreatedAt,
                LastLoginAt = LastLoginAt,
                MembershipTier = MembershipTier,
                Credits = Credits,
                LifetimePoints = LifetimePoints,
                MemberSince = MemberSince ?? CreatedAt
            };
        }
    }
}