using HoloCrew.Infraestructure.Supabase.Models;
using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using PgConstants = Supabase.Postgrest.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Repositorio del carrito conectado a Supabase.
// - Para listar: usa la vista cart_with_details (un solo SELECT con JOIN incluido).
// - Para añadir: usa la RPC add_to_cart (atómica, valida stock automáticamente).
// - Para actualizar/borrar: queries directas a la tabla cart_items.

namespace HoloCrew.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly Supabase.Client _supabase;

        public CartRepository(Supabase.Client supabase)
        {
            _supabase = supabase;
        }


        public async Task<List<CartItem>> GetByUserIdAsync(string userId)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                    return new List<CartItem>();

                var response = await _supabase
                    .From<CartDetailDto>()
                    .Where(c => c.UserId == userGuid)
                    .Where(c => c.ProductIsActive == true)
                    .Order("added_at", PgConstants.Ordering.Descending)
                    .Get();

                return response.Models
                    .Select(c => c.ToCartItem())
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Cart] GetByUser error: {ex.Message}");
                return new List<CartItem>();
            }
        }


        public async Task<bool> AddAsync(int productId, int quantity, string size, string color)
        {
            // Usamos la RPC add_to_cart que ya existe en BD.
            // Esta función:
            //  - Valida que el producto tenga stock suficiente.
            //  - Si el producto+talla+color ya está en el carrito, suma la cantidad.
            //  - Si no, lo crea.
            //  - Maneja la auth automáticamente (auth.uid()).
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "p_product_id", productId },
                    { "p_quantity", quantity },
                    { "p_size", size ?? string.Empty },
                    { "p_color", color ?? string.Empty }
                };

                await _supabase.Rpc("add_to_cart", parameters);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Cart] Add error: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> UpdateQuantityAsync(int cartItemId, int newQuantity)
        {
            try
            {
                if (newQuantity <= 0)
                {
                    // Si la cantidad nueva es 0 o menos, lo borramos
                    return await RemoveAsync(cartItemId);
                }

                await _supabase
                    .From<CartItemDto>()
                    .Where(c => c.Id == cartItemId)
                    .Set(c => c.Quantity, newQuantity)
                    .Update();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Cart] UpdateQuantity error: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> RemoveAsync(int cartItemId)
        {
            try
            {
                await _supabase
                    .From<CartItemDto>()
                    .Where(c => c.Id == cartItemId)
                    .Delete();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Cart] Remove error: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> ClearAsync(string userId)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                    return false;

                await _supabase
                    .From<CartItemDto>()
                    .Where(c => c.UserId == userGuid)
                    .Delete();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Cart] Clear error: {ex.Message}");
                return false;
            }
        }


        public async Task<int> GetItemCountAsync(string userId)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                    return 0;

                var response = await _supabase
                    .From<CartItemDto>()
                    .Where(c => c.UserId == userGuid)
                    .Get();

                return response.Models.Sum(c => c.Quantity);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Cart] Count error: {ex.Message}");
                return 0;
            }
        }
    }
}