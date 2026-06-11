using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Servicio de membresía. Centraliza la lógica del Members Club:
// cálculo de tier actual y siguiente, progreso y consultas de transacciones.

namespace HoloCrew.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;

        public MembershipService(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }


        public async Task<List<MembershipTierConfig>> GetAllTiersAsync()
        {
            var tiers = await _membershipRepository.GetAllTiersAsync();

            // Convertir rutas relativas de las imágenes a URLs completas del bucket de Supabase.
            // En la BBDD las URLs están como "/images/Bronze3D.png" pero el bucket sirve desde
            // https://<proyecto>.supabase.co/storage/v1/object/public/product-images/<archivo>.png
            // El tier con más puntos mínimos es el tope: se muestra abierto ("X+"),
            // porque su max_points es un valor enorme/arbitrario que no aporta nada.
            var topTier = tiers.OrderByDescending(t => t.MinPoints).FirstOrDefault();

            foreach (var tier in tiers)
            {
                tier.ImageUrl = NormalizeImageUrl(tier.ImageUrl);
                // Pasar el nombre del tier a mayúsculas para mostrarlo bien.
                tier.Name = tier.Name?.ToUpper() ?? string.Empty;

                // Rango para mostrar: el tope abierto ("X+"), el resto "X – Y".
                tier.RangeDisplay = tier == topTier
                    ? $"{tier.MinPoints:N0}+"
                    : $"{tier.MinPoints:N0} – {tier.MaxPoints:N0}";
            }

            return tiers;
        }

        private const string SUPABASE_BUCKET_BASE_URL =
            "https://unaptwklykdhbonuzhwm.supabase.co/storage/v1/object/public/product-images/";

        private string NormalizeImageUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            // Si ya es URL completa, devolverla tal cual.
            if (url.StartsWith("http://") || url.StartsWith("https://"))
                return url;

            // Quitar "/images/" si lo trae al inicio (formato que tiene tu BBDD).
            var fileName = url;
            if (fileName.StartsWith("/images/"))
                fileName = fileName.Substring("/images/".Length);
            else if (fileName.StartsWith("/"))
                fileName = fileName.Substring(1);

            return SUPABASE_BUCKET_BASE_URL + fileName;
        }


        public Task<List<CreditTransaction>> GetUserTransactionsAsync(string userId, int limit = 20)
            => _membershipRepository.GetUserTransactionsAsync(userId, limit);


        // El tier actual es aquel cuyo rango [min_points, max_points] contiene
        // los lifetime_points del usuario.
        public MembershipTierConfig? GetCurrentTier(int lifetimePoints, List<MembershipTierConfig> allTiers)
        {
            return allTiers
                .OrderBy(t => t.MinPoints)
                .FirstOrDefault(t => lifetimePoints >= t.MinPoints && lifetimePoints <= t.MaxPoints)
                ?? allTiers.OrderByDescending(t => t.MinPoints).FirstOrDefault();
            // Si los puntos exceden todos los rangos (caso raro), devolvemos el tier más alto.
        }


        public MembershipTierConfig? GetNextTier(int lifetimePoints, List<MembershipTierConfig> allTiers)
        {
            return allTiers
                .OrderBy(t => t.MinPoints)
                .FirstOrDefault(t => t.MinPoints > lifetimePoints);
        }


        public double GetProgressPercentage(int lifetimePoints, List<MembershipTierConfig> allTiers)
        {
            var current = GetCurrentTier(lifetimePoints, allTiers);
            var next = GetNextTier(lifetimePoints, allTiers);

            if (current == null) return 0;
            if (next == null) return 100; // ya está en el tier máximo

            var rangeStart = current.MinPoints;
            var rangeEnd = next.MinPoints;
            var range = rangeEnd - rangeStart;
            if (range <= 0) return 100;

            var pointsInCurrent = lifetimePoints - rangeStart;
            return Math.Min(100, Math.Max(0, (pointsInCurrent / (double)range) * 100));
        }


        public int GetPointsToNextTier(int lifetimePoints, List<MembershipTierConfig> allTiers)
        {
            var next = GetNextTier(lifetimePoints, allTiers);
            if (next == null) return 0;
            return Math.Max(0, next.MinPoints - lifetimePoints);
        }
    }
}