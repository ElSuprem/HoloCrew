using HoloCrew.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

// Interfaz del repositorio de membresía.

namespace HoloCrew.Repositories.Interfaces
{
    public interface IMembershipRepository
    {
        Task<List<MembershipTierConfig>> GetAllTiersAsync();
        Task<List<CreditTransaction>> GetUserTransactionsAsync(string userId, int limit = 20);
    }
}