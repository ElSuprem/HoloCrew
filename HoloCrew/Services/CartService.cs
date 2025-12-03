using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace HoloCrew.Services
{
    /// <summary>
    /// Implementación del servicio de carrito
    /// Mantiene el carrito en memoria (en producción podría ser persistente)
    /// </summary>
    public class CartService : ICartService
    {
        private List<CartItem> _cartItems = new();
        private decimal _currentDiscount = 0;
        private int _nextCartItemId = 1;

        public event EventHandler CartUpdated;

        public Task AddToCartAsync(Product product, int quantity, string variant = null)
        {
            if (product == null || quantity <= 0)
            {
                return Task.CompletedTask;
            }

            // Verificar si el producto ya está en el carrito
            var existingItem = _cartItems.FirstOrDefault(ci =>
                ci.ProductId == product.Id && ci.SelectedVariant == variant);

            if (existingItem != null)
            {
                // Incrementar cantidad
                existingItem.Quantity += quantity;
            }
            else
            {
                // Agregar nuevo item
                var cartItem = new CartItem
                {
                    Id = _nextCartItemId++,
                    ProductId = product.Id,
                    Product = product,
                    Quantity = quantity,
                    SelectedVariant = variant,
                    UnitPrice = product.Price
                };

                _cartItems.Add(cartItem);
            }

            OnCartUpdated();
            return Task.CompletedTask;
        }

        public Task UpdateQuantityAsync(int cartItemId, int newQuantity)
        {
            var item = _cartItems.FirstOrDefault(ci => ci.Id == cartItemId);

            if (item != null)
            {
                if (newQuantity <= 0)
                {
                    _cartItems.Remove(item);
                }
                else
                {
                    item.Quantity = newQuantity;
                }

                OnCartUpdated();
            }

            return Task.CompletedTask;
        }

        public Task RemoveFromCartAsync(int cartItemId)
        {
            var item = _cartItems.FirstOrDefault(ci => ci.Id == cartItemId);

            if (item != null)
            {
                _cartItems.Remove(item);
                OnCartUpdated();
            }

            return Task.CompletedTask;
        }

        public Task ClearCartAsync()
        {
            _cartItems.Clear();
            _currentDiscount = 0;
            OnCartUpdated();
            return Task.CompletedTask;
        }

        public Task<List<CartItem>> GetCartItemsAsync()
        {
            return Task.FromResult(_cartItems.ToList());
        }

        public Task<decimal> GetCartTotalAsync()
        {
            var subtotal = _cartItems.Sum(item => item.Subtotal);
            var total = subtotal - _currentDiscount;
            return Task.FromResult(total);
        }

        public int GetCartItemCount()
        {
            return _cartItems.Sum(item => item.Quantity);
        }

        public Task<bool> ApplyCouponAsync(string couponCode)
        {
            if (string.IsNullOrWhiteSpace(couponCode))
            {
                return Task.FromResult(false);
            }

            // TODO: Validar cupón con el servidor
            // Por ahora, cupones hardcodeados para desarrollo
            var validCoupons = new Dictionary<string, decimal>
            {
                { "DESCUENTO10", 10m },
                { "BIENVENIDO", 5m },
                { "VERANO2024", 15m }
            };

            if (validCoupons.TryGetValue(couponCode.ToUpper(), out var discount))
            {
                _currentDiscount = discount;
                OnCartUpdated();
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public decimal GetCurrentDiscount()
        {
            return _currentDiscount;
        }

        private void OnCartUpdated()
        {
            CartUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}