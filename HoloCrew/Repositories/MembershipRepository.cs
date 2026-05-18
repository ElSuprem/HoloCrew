using HoloCrew.Infraestructure.Supabase.Models;
using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using PgConstants = Supabase.Postgrest.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Repositorio de membresía conectado a Supabase.
// Lee de las tablas: membership_tiers_config, credit_transactions.

namespace HoloCrew.Repositories
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly Supabase.Client _supabase;

        public MembershipRepository(Supabase.Client supabase)
        {
            _supabase = supabase;
        }


        // Devuelve los 4 niveles configurados en Supabase (Bronze, Silver, Gold, Platinum),
        // ordenados por min_points ascendente.
        public async Task<List<MembershipTierConfig>> GetAllTiersAsync()
        {
            var response = await _supabase
                .From<MembershipTierConfigDto>()
                .Order("min_points", PgConstants.Ordering.Ascending)
                .Get();

            return response.Models
                .Select(dto => dto.ToModel())
                .ToList();
        }


        // Devuelve las últimas N transacciones de créditos del usuario, ordenadas
        // de más reciente a más antigua.
        public async Task<List<CreditTransaction>> GetUserTransactionsAsync(string userId, int limit = 20)
        {
            if (!Guid.TryParse(userId, out var userGuid))
                return new List<CreditTransaction>();

            var response = await _supabase
                .From<CreditTransactionDto>()
                .Where(t => t.UserId == userGuid)
                .Order("created_at", PgConstants.Ordering.Descending)
                .Limit(limit)
                .Get();

            return response.Models
                .Select(dto => dto.ToModel())
                .ToList();
        }
    }
}