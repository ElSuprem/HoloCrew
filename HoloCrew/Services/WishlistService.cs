using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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

        public event EventHandler? WishlistUpdated;

        public ObservableCollection<Product> WishlistItems => _wishlistItems;
        public int WishlistCount => _wishlistItems.Count;

        public WishlistService(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
            _wishlistItems = new ObservableCollection<Product>();
        }

        /// <summary>
        /// Añade un producto usando la referencia directa (mantiene el objeto de la UI)
        /// </summary>
        public async Task AddToWishlistAsync(Product product)
        {
            if (product == null) return;

            if (_wishlistItems.Any(p => p.Id == product.Id))
                return;

            _wishlistItems.Add(product);
            OnWishlistUpdated();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Añade un producto por ID (carga nueva instancia)
        /// </summary>
        public async Task AddToWishlistAsync(int productId)
        {
            if (_wishlistItems.Any(p => p.Id == productId))
                return;

            var product = await _productService.GetProductByIdAsync(productId);
            if (product != null)
            {
                _wishlistItems.Add(product);
                OnWishlistUpdated();
            }
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

        public async Task ClearWishlistAsync()
        {
            _wishlistItems.Clear();
            OnWishlistUpdated();
            await Task.CompletedTask;
        }

        public async Task LoadWishlistAsync()
        {
            await Task.CompletedTask;
        }

        public async Task<List<Product>> GetWishlistAsync(int userId)
        {
            return await Task.FromResult(_wishlistItems.ToList());
        }

        public async Task<int> GetWishlistCountAsync(int userId)
        {
            return await Task.FromResult(_wishlistItems.Count);
        }

        public async Task MoveAllToCartAsync(int userId)
        {
            foreach (var product in _wishlistItems.ToList())
            {
                await _cartService.AddToCartAsync(product, 1);
            }
            await Task.CompletedTask;
        }

        private void OnWishlistUpdated()
        {
            WishlistUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}