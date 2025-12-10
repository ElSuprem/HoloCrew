namespace HoloCrew.Models
{
    /// <summary>
    /// Niveles de membresía del HoloCrew Members Club
    /// </summary>
    public enum MembershipLevel
    {
        None = 0,
        Bronze = 1,
        Silver = 2,
        Gold = 3,
        Platinum = 4
    }

    /// <summary>
    /// Información de un nivel de membresía
    /// </summary>
    public class MembershipTier
    {
        public MembershipLevel Level { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public int PointsRequired { get; set; }
        public int DiscountPercentage { get; set; }
        public bool FreeShipping { get; set; }
        public bool EarlyAccess { get; set; }
        public bool BirthdayGift { get; set; }
        public bool ExclusiveProducts { get; set; }
        public int PointsMultiplier { get; set; } // 1x, 2x, 3x, 4x
        public List<string> Benefits { get; set; } = new();
    }

    /// <summary>
    /// Información de membresía del usuario
    /// </summary>
    public class UserMembership
    {
        public int UserId { get; set; }
        public MembershipLevel CurrentLevel { get; set; }
        public int TotalPoints { get; set; }
        public int CurrentLevelPoints { get; set; }
        public int PointsToNextLevel { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalOrders { get; set; }
        public DateTime MemberSince { get; set; }
        public DateTime? NextLevelDate { get; set; }

        // Propiedades calculadas
        public double ProgressPercentage { get; set; }
        public MembershipLevel NextLevel { get; set; }
    }

    /// <summary>
    /// Recompensa o beneficio obtenido
    /// </summary>
    public class MembershipReward
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int PointsCost { get; set; }
        public MembershipLevel MinimumLevel { get; set; }
        public bool IsRedeemed { get; set; }
        public DateTime? RedeemedDate { get; set; }
    }
}