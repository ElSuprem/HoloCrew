using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using System.Collections.ObjectModel;

namespace HoloCrew.Services
{
    /// <summary>
    /// Servicio de wishlist FUNCIONAL con datos MOCK
    /// </summary>
    public class WishlistService : IWishlistService
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private ObservableCollection<Product> _wishlistItems;

        public event EventHandler WishlistUpdated;

        public WishlistService(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
            InitializeMockWishlist();
        }

        private void InitializeMockWishlist()
        {
            // Empezar con wishlist vacía por defecto
            // Los productos mock se pueden agregar al hacer debug
            _wishlistItems = new ObservableCollection<Product>();

            // ⭐ DESCOMENTA ESTO PARA VER PRODUCTOS INICIALES
            /*
            _wishlistItems = new ObservableCollection<Product>
            {
                new Product
                {
                    Id = 6,
                    Name = "OVERSIZED HOODIE BLACK",
                    Description = "Premium heavyweight cotton",
                    Price = 79.99m,
                    OriginalPrice = 99.99m,
                    Stock = 75,
                    MainImageUrl = "/Resources/Images/hoodie1.jpg",
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Tops" },
                    AverageRating = 4.9,
                    ReviewCount = 456,
                    IsFeatured = true,
                    IsNew = true,
                    Gender = "Unisex"
                }
            };
            */
        }

        public async Task<List<Product>> GetWishlistAsync(int userId)
        {
            return await Task.FromResult(_wishlistItems.ToList());
        }

        public async Task AddToWishlistAsync(int productId)
        {
            try
            {
                // Verificar si ya está en wishlist
                if (_wishlistItems.Any(p => p.Id == productId))
                {
                    return; // Ya está en wishlist
                }

                // Cargar el producto completo desde ProductService
                var product = await _productService.GetProductByIdAsync(productId);

                if (product != null)
                {
                    _wishlistItems.Add(product);
                    OnWishlistUpdated();
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine($"Error adding to wishlist: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        public async Task RemoveFromWishlistAsync(int productId)
        {
            var item = _wishlistItems.FirstOrDefault(p => p.Id == productId);
            if (item != null)
            {
                _wishlistItems.Remove(item);
                OnWishlistUpdated();
            }
            await Task.CompletedTask;
        }

        public async Task<bool> IsInWishlistAsync(int productId)
        {
            return await Task.FromResult(_wishlistItems.Any(p => p.Id == productId));
        }

        public async Task<int> GetWishlistCountAsync(int userId)
        {
            return await Task.FromResult(_wishlistItems.Count);
        }

        public async Task ClearWishlistAsync()
        {
            _wishlistItems.Clear();
            OnWishlistUpdated();
            await Task.CompletedTask;
        }

        public async Task MoveAllToCartAsync(int userId)
        {
            // Agregar todos los items al carrito
            foreach (var product in _wishlistItems.ToList())
            {
                await _cartService.AddToCartAsync(product, 1);
            }

            // Opcionalmente, limpiar la wishlist después
            // await ClearWishlistAsync();

            await Task.CompletedTask;
        }

        private void OnWishlistUpdated()
        {
            WishlistUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}