using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

// Servicio del carrito conectado a Supabase a través del repositorio.
// Mantiene una caché en memoria de los items para que GetCartItemCount sea instantáneo.
// Los cupones se validan en BD a través de la RPC validate_coupon que ya hace
// todas las comprobaciones (existencia, fechas, usos máximos, monto mínimo, etc).

namespace HoloCrew.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;
        private readonly IAuthenticationService _authService;
        private readonly Supabase.Client _supabase;
        private readonly ObservableCollection<CartItem> _cartItems;

        private string _appliedCouponCode = string.Empty;
        private decimal _appliedDiscount = 0;
        private decimal? _appliedShippingOverride = null;

        public event EventHandler? CartUpdated;

        public CartService(
            ICartRepository repository,
            IAuthenticationService authService,
            Supabase.Client supabase)
        {
            _repository = repository;
            _authService = authService;
            _supabase = supabase;
            _cartItems = new ObservableCollection<CartItem>();
        }


        public async Task AddToCartAsync(Product product, int quantity, string variant = null)
        {
            if (product == null) return;

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return;

            // Determinar talla
            string size;
            if (!string.IsNullOrEmpty(variant))
                size = variant;
            else if (product.AvailableSizes != null && product.AvailableSizes.Any())
                size = product.AvailableSizes.First();
            else
                size = string.Empty;

            // Determinar color
            string color;
            if (product.AvailableColors != null && product.AvailableColors.Any())
                color = product.AvailableColors.First();
            else
                color = string.Empty;

            var ok = await _repository.AddAsync(product.Id, quantity, size, color);
            if (ok)
            {
                await LoadCartAsync();
            }
        }


        public async Task UpdateQuantityAsync(int cartItemId, int newQuantity)
        {
            var ok = await _repository.UpdateQuantityAsync(cartItemId, newQuantity);
            if (ok)
            {
                await LoadCartAsync();
            }
        }


        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var ok = await _repository.RemoveAsync(cartItemId);
            if (ok)
            {
                var item = _cartItems.FirstOrDefault(i => i.Id == cartItemId);
                if (item != null) _cartItems.Remove(item);
                OnCartUpdated();
            }
        }


        public async Task ClearCartAsync()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return;

            var ok = await _repository.ClearAsync(userId);
            if (ok)
            {
                _cartItems.Clear();
                _appliedCouponCode = string.Empty;
                _appliedDiscount = 0;
                _appliedShippingOverride = null;
                OnCartUpdated();
            }
        }


        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            if (!_cartItems.Any())
                await LoadCartAsync();
            return _cartItems.ToList();
        }


        public async Task<decimal> GetCartTotalAsync()
        {
            if (!_cartItems.Any())
                await LoadCartAsync();

            var subtotal = _cartItems.Sum(i => i.Subtotal);
            var shipping = _appliedShippingOverride ?? (subtotal > 50 ? 0 : 5.99m);
            var tax = subtotal * 0.21m;
            return subtotal + shipping + tax - _appliedDiscount;
        }


        public int GetCartItemCount()
        {
            return _cartItems.Sum(i => i.Quantity);
        }


        // Aplica un cupón llamando a la RPC validate_coupon de Supabase.
        // La RPC valida: que exista, que esté activo, fechas de validez, usos máximos,
        // monto mínimo del pedido, y devuelve el descuento calculado o un mensaje de error.
        public async Task<CouponResult> ApplyCouponAsync(string couponCode)
        {
            if (string.IsNullOrWhiteSpace(couponCode))
            {
                return new CouponResult
                {
                    IsValid = false,
                    Message = "Please enter a coupon code"
                };
            }

            try
            {
                var subtotal = _cartItems.Sum(i => i.Subtotal);

                var parameters = new Dictionary<string, object>
                {
                    { "p_code", couponCode.Trim().ToUpper() },
                    { "p_subtotal", subtotal }
                };

                var response = await _supabase.Rpc("validate_coupon", parameters);

                if (response?.Content == null)
                {
                    return new CouponResult { IsValid = false, Message = "Server error" };
                }

                var rawContent = response.Content.Trim();
                System.Diagnostics.Debug.WriteLine($"[Coupon] Raw response: {rawContent}");

                // La RPC devuelve TABLE(...) que llega como un array JSON con una fila
                ValidateCouponRow? row = null;
                try
                {
                    var array = JsonConvert.DeserializeObject<List<ValidateCouponRow>>(rawContent);
                    row = array?.FirstOrDefault();
                }
                catch
                {
                    // Por si viene como objeto suelto en lugar de array
                    try
                    {
                        row = JsonConvert.DeserializeObject<ValidateCouponRow>(rawContent);
                    }
                    catch { }
                }

                if (row == null)
                {
                    return new CouponResult { IsValid = false, Message = "Invalid response from server" };
                }

                if (row.IsValid)
                {
                    _appliedCouponCode = couponCode.Trim().ToUpper();
                    _appliedDiscount = row.DiscountAmount ?? 0;
                    _appliedShippingOverride = row.NewShippingCost;
                    OnCartUpdated();

                    return new CouponResult
                    {
                        IsValid = true,
                        DiscountAmount = row.DiscountAmount ?? 0,
                        NewShippingCost = row.NewShippingCost,
                        Message = $"✓ Coupon applied! You save €{(row.DiscountAmount ?? 0):N2}"
                    };
                }
                else
                {
                    // Limpiamos cualquier cupón previo si este es inválido
                    _appliedCouponCode = string.Empty;
                    _appliedDiscount = 0;
                    _appliedShippingOverride = null;
                    OnCartUpdated();

                    return new CouponResult
                    {
                        IsValid = false,
                        Message = $"✗ {row.ErrorMessage ?? "Invalid coupon"}"
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Coupon] Apply error: {ex.Message}");
                return new CouponResult
                {
                    IsValid = false,
                    Message = "Error applying coupon"
                };
            }
        }


        public decimal GetCurrentDiscount()
        {
            return _appliedDiscount;
        }


        public string GetAppliedCouponCode()
        {
            return _appliedCouponCode;
        }


        public async Task LoadCartAsync()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                _cartItems.Clear();
                OnCartUpdated();
                return;
            }

            var items = await _repository.GetByUserIdAsync(userId);

            _cartItems.Clear();
            foreach (var item in items)
                _cartItems.Add(item);

            OnCartUpdated();
        }


        // ==================== HELPERS ====================

        private string GetCurrentUserId()
        {
            var user = _authService.GetCurrentUser();
            return user?.Id ?? string.Empty;
        }

        private void OnCartUpdated()
        {
            CartUpdated?.Invoke(this, EventArgs.Empty);
        }


        // Clase interna para deserializar la respuesta de la RPC validate_coupon
        private class ValidateCouponRow
        {
            [JsonProperty("is_valid")]
            public bool IsValid { get; set; }

            [JsonProperty("coupon_id")]
            public int? CouponId { get; set; }

            [JsonProperty("discount_amount")]
            public decimal? DiscountAmount { get; set; }

            [JsonProperty("new_shipping_cost")]
            public decimal? NewShippingCost { get; set; }

            [JsonProperty("error_message")]
            public string? ErrorMessage { get; set; }
        }
    }
}