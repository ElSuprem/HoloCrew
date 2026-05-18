using HoloCrew.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

// Servicio de membresía. Centraliza la lógica del Members Club.

namespace HoloCrew.Services.Interfaces
{
    public interface IMembershipService
    {
        Task<List<MembershipTierConfig>> GetAllTiersAsync();
        Task<List<CreditTransaction>> GetUserTransactionsAsync(string userId, int limit = 20);

        // Calcula el tier actual basándose en los lifetime_points del usuario.
        MembershipTierConfig? GetCurrentTier(int lifetimePoints, List<MembershipTierConfig> allTiers);

        // Calcula el siguiente tier (null si ya está en Platinum).
        MembershipTierConfig? GetNextTier(int lifetimePoints, List<MembershipTierConfig> allTiers);

        // Calcula el % de progreso hacia el siguiente tier (0-100).
        double GetProgressPercentage(int lifetimePoints, List<MembershipTierConfig> allTiers);

        // Puntos que faltan para subir de tier.
        int GetPointsToNextTier(int lifetimePoints, List<MembershipTierConfig> allTiers);
    }
}