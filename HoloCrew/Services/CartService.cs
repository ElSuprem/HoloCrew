using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using System.Collections.ObjectModel;

// Servicio del carrito con datos falsos en memoria (mock).
// Permite añadir productos, cambiar cantidades, quitar, aplicar cupones.
// Al cambiar el carrito, lanza CartUpdated para que la interfaz se refresque.

namespace HoloCrew.Services
{
    public class CartService : ICartService
    {
        private ObservableCollection<CartItem> _cartItems;
        private decimal _appliedDiscount = 0;

        public event EventHandler CartUpdated;

        public CartService()
        {
            InitializeMockCart();
        }

        // datos de ejemplo para probar sin base de datos real
        private void InitializeMockCart()
        {
            _cartItems = new ObservableCollection<CartItem>
            {
                new CartItem
                {
                    Id = 1,
                    ProductId = 6,
                    ProductName = "OVERSIZED HOODIE BLACK",
                    Price = 79.99m,
                    Quantity = 2,
                    Size = "L",
                    Color = "Black",
                    ImageUrl = "/Resources/Images/hoodie1.jpg"
                },
                new CartItem
                {
                    Id = 2,
                    ProductId = 22,
                    ProductName = "TACTICAL CARGO PANTS",
                    Price = 79.99m,
                    Quantity = 1,
                    Size = "M",
                    Color = "Olive",
                    ImageUrl = "/Resources/Images/cargo1.jpg"
                },
                new CartItem
                {
                    Id = 3,
                    ProductId = 41,
                    ProductName = "ARMBO LOW WHITE",
                    Price = 129.99m,
                    Quantity = 1,
                    Size = "42",
                    Color = "White",
                    ImageUrl = "/Resources/Images/armbo1.jpg"
                }
            };
        }

        public async Task AddToCartAsync(Product product, int quantity, string variant = null)
        {
            string size = variant ?? "M"; // si no viene talla, se pone M por defecto

            // si ya hay un producto igual (mismo id y misma talla), se suma cantidad
            var existingItem = _cartItems.FirstOrDefault(i =>
                i.ProductId == product.Id &&
                i.Size == size);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new CartItem
                {
                    Id = _cartItems.Any() ? _cartItems.Max(i => i.Id) + 1 : 1,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    Size = size,
                    Color = "Default",
                    ImageUrl = product.MainImageUrl
                };
                _cartItems.Add(newItem);
            }

            OnCartUpdated();
            await Task.CompletedTask;
        }

        public async Task UpdateQuantityAsync(int cartItemId, int newQuantity)
        {
            var item = _cartItems.FirstOrDefault(i => i.Id == cartItemId);
            if (item != null)
            {
                item.Quantity = newQuantity;
                OnCartUpdated();
            }
            await Task.CompletedTask;
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var item = _cartItems.FirstOrDefault(i => i.Id == cartItemId);
            if (item != null)
            {
                _cartItems.Remove(item);
                OnCartUpdated();
            }
            await Task.CompletedTask;
        }

        public async Task ClearCartAsync()
        {
            _cartItems.Clear();
            _appliedDiscount = 0;
            OnCartUpdated();
            await Task.CompletedTask;
        }

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            return await Task.FromResult(_cartItems.ToList());
        }

        public async Task<decimal> GetCartTotalAsync()
        {
            var subtotal = _cartItems.Sum(i => i.Subtotal);
            var shipping = subtotal > 50 ? 0 : 5.99m;   // envío gratis si el pedido supera 50€
            var tax = subtotal * 0.21m;                 // IVA 21%
            return await Task.FromResult(subtotal + shipping + tax - _appliedDiscount);
        }

        public int GetCartItemCount()
        {
            return _cartItems.Sum(i => i.Quantity);
        }

        // cupones de ejemplo
        public async Task<bool> ApplyCouponAsync(string couponCode)
        {
            var validCoupons = new Dictionary<string, decimal>
            {
                { "WELCOME10", 10m },
                { "SAVE20", 20m },
                { "CREW50", 50m }
            };

            if (validCoupons.TryGetValue(couponCode.ToUpper(), out var discount))
            {
                _appliedDiscount = discount;
                OnCartUpdated();
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        public decimal GetCurrentDiscount()
        {
            return _appliedDiscount;
        }

        private void OnCartUpdated()
        {
            CartUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}