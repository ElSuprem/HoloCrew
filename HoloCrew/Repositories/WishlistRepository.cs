using HoloCrew.Infraestructure.Supabase.Models;
using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using PgConstants = Supabase.Postgrest.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Repositorio de wishlist conectado a Supabase.
// - Para listar: usa la vista wishlist_with_details (un solo SELECT con JOIN incluido).
// - Para añadir/quitar: usa la RPC toggle_wishlist (atómica, valida auth automáticamente).

namespace HoloCrew.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly Supabase.Client _supabase;

        public WishlistRepository(Supabase.Client supabase)
        {
            _supabase = supabase;
        }


        public async Task<List<Product>> GetByUserIdAsync(string userId)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                    return new List<Product>();

                var response = await _supabase
                    .From<WishlistDetailDto>()
                    .Where(w => w.UserId == userGuid)
                    .Where(w => w.ProductIsActive == true)
                    .Order("added_at", PgConstants.Ordering.Descending)
                    .Get();

                return response.Models
                    .Select(w => w.ToProduct())
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Wishlist] GetByUser error: {ex.Message}");
                return new List<Product>();
            }
        }


        public async Task<bool> AddAsync(string userId, int productId)
        {
            // Usamos la RPC toggle_wishlist que ya existe en BD.
            // Si el producto no está, lo añade. Si está, lo quita.
            // Esta primera implementación usa toggle directo: si quieres garantizar
            // "añadir" puro, llamamos primero a ExistsAsync.
            try
            {
                var exists = await ExistsAsync(userId, productId);
                if (exists)
                    return true; // ya está, no hacemos nada

                return await ToggleAsync(productId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Wishlist] Add error: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> RemoveAsync(string userId, int productId)
        {
            try
            {
                var exists = await ExistsAsync(userId, productId);
                if (!exists)
                    return true; // ya no está, no hacemos nada

                return await ToggleAsync(productId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Wishlist] Remove error: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> ExistsAsync(string userId, int productId)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                    return false;

                var response = await _supabase
                    .From<WishlistItemDto>()
                    .Where(w => w.UserId == userGuid)
                    .Where(w => w.ProductId == productId)
                    .Get();

                return response.Models.Any();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Wishlist] Exists error: {ex.Message}");
                return false;
            }
        }


        public async Task<int> CountAsync(string userId)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                    return 0;

                var response = await _supabase
                    .From<WishlistItemDto>()
                    .Where(w => w.UserId == userGuid)
                    .Get();

                return response.Models.Count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Wishlist] Count error: {ex.Message}");
                return 0;
            }
        }


        public async Task ClearAsync(string userId)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                    return;

                await _supabase
                    .From<WishlistItemDto>()
                    .Where(w => w.UserId == userGuid)
                    .Delete();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Wishlist] Clear error: {ex.Message}");
            }
        }


        // Helper: llama a la RPC toggle_wishlist
        private async Task<bool> ToggleAsync(int productId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "p_product_id", productId }
                };

                await _supabase.Rpc("toggle_wishlist", parameters);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Wishlist] Toggle error: {ex.Message}");
                return false;
            }
        }
    }
}