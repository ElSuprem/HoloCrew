using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using System.Collections.ObjectModel;

namespace HoloCrew.Services
{
    /// <summary>
    /// Servicio de wishlist funcional
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
            // WISHLIST VACÍA - Los productos se agregan desde la app
            _wishlistItems = new ObservableCollection<Product>();
        }

        public async Task<List<Product>> GetWishlistAsync(int userId)
        {
            return await Task.FromResult(_wishlistItems.ToList());
        }

        /// <summary>
        /// Agrega producto usando el MISMO objeto (mantiene referencia para UI)
        /// </summary>
        public async Task AddToWishlistAsync(Product product)
        {
            if (product == null) return;

            try
            {
                // Verificar si ya está
                if (_wishlistItems.Any(p => p.Id == product.Id))
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Product {product.Id} already in wishlist");
                    return;
                }

                // Usar el MISMO objeto que viene de la UI
                _wishlistItems.Add(product);
                System.Diagnostics.Debug.WriteLine($"✅ Added product {product.Id} ({product.Name}) to wishlist. Total: {_wishlistItems.Count}");
                OnWishlistUpdated();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error adding to wishlist: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Agrega producto por ID (carga nuevo producto - NO mantiene referencia UI)
        /// </summary>
        public async Task AddToWishlistAsync(int productId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔍 Attempting to add product {productId} to wishlist");

                // Verificar si ya está
                if (_wishlistItems.Any(p => p.Id == productId))
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Product {productId} already in wishlist");
                    return;
                }

                // Cargar producto (NOTA: esto crea nueva instancia)
                var product = await _productService.GetProductByIdAsync(productId);

                if (product != null)
                {
                    _wishlistItems.Add(product);
                    System.Diagnostics.Debug.WriteLine($"✅ Added product {productId} ({product.Name}) to wishlist. Total: {_wishlistItems.Count}");
                    OnWishlistUpdated();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Product {productId} not found");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error adding to wishlist: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            await Task.CompletedTask;
        }

        public async Task RemoveFromWishlistAsync(int productId)
        {
            var item = _wishlistItems.FirstOrDefault(p => p.Id == productId);
            if (item != null)
            {
                _wishlistItems.Remove(item);
                System.Diagnostics.Debug.WriteLine($"✅ Removed product {productId} from wishlist. Remaining: {_wishlistItems.Count}");
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
            System.Diagnostics.Debug.WriteLine("✅ Wishlist cleared");
            OnWishlistUpdated();
            await Task.CompletedTask;
        }

        public async Task MoveAllToCartAsync(int userId)
        {
            foreach (var product in _wishlistItems.ToList())
            {
                await _cartService.AddToCartAsync(product, 1);
            }
            System.Diagnostics.Debug.WriteLine($"✅ Moved {_wishlistItems.Count} items to cart");
            await Task.CompletedTask;
        }

        private void OnWishlistUpdated()
        {
            System.Diagnostics.Debug.WriteLine($"📢 WishlistUpdated event fired. Items: {_wishlistItems.Count}");
            WishlistUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}