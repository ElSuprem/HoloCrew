using System;
using System.Collections.Generic;

// Niveles de membresía (Bronce, Plata, Oro, Platino).
// Cada nivel tiene beneficios: descuento, envío gratis, acceso anticipado, etc.
// Los usuarios ganan puntos por compras y suben de nivel.

namespace HoloCrew.Models
{
    public enum MembershipLevel
    {
        None = 0,
        Bronze = 1,
        Silver = 2,
        Gold = 3,
        Platinum = 4
    }

    // Beneficios de cada nivel
    public class MembershipTier
    {
        public MembershipLevel Level { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public int PointsRequired { get; set; }        // puntos necesarios para llegar a este nivel
        public int DiscountPercentage { get; set; }    // % de descuento que da
        public bool FreeShipping { get; set; }
        public bool EarlyAccess { get; set; }          // acceso anticipado a lanzamientos
        public bool BirthdayGift { get; set; }
        public bool ExclusiveProducts { get; set; }    // productos solo para este nivel
        public int PointsMultiplier { get; set; }      // 1x, 2x, 3x puntos por cada euro gastado
        public List<string> Benefits { get; set; } = new();
    }

    // Datos de membresía de un usuario concreto
    public class UserMembership
    {
        public int UserId { get; set; }
        public MembershipLevel CurrentLevel { get; set; }
        public int TotalPoints { get; set; }           // todos los puntos acumulados
        public int CurrentLevelPoints { get; set; }    // puntos que tiene dentro del nivel actual
        public int PointsToNextLevel { get; set; }     // puntos que le faltan para subir
        public decimal TotalSpent { get; set; }
        public int TotalOrders { get; set; }
        public DateTime MemberSince { get; set; }
        public DateTime? NextLevelDate { get; set; }

        public double ProgressPercentage { get; set; } // porcentaje de progreso hacia el siguiente nivel
        public MembershipLevel NextLevel { get; set; }  // cuál es el siguiente nivel
    }

    // Recompensa canjeable con puntos (descuento, producto gratis, etc.)
    public class MembershipReward
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int PointsCost { get; set; }             // cuántos puntos cuesta
        public MembershipLevel MinimumLevel { get; set; } // nivel mínimo para poder canjearla
        public bool IsRedeemed { get; set; }            // si el usuario ya la ha canjeado
        public DateTime? RedeemedDate { get; set; }
    }
}