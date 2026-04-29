using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

// Servicio de wishlist conectado a Supabase a través del repositorio.
// Mantiene una ObservableCollection en memoria para que la UI se actualice al instante,
// y sincroniza los cambios con la base de datos.

namespace HoloCrew.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _repository;
        private readonly ICartService _cartService;
        private readonly IAuthenticationService _authService;
        private readonly ObservableCollection<Product> _wishlistItems;

        public event EventHandler? WishlistUpdated;
        public ObservableCollection<Product> WishlistItems => _wishlistItems;
        public int WishlistCount => _wishlistItems.Count;

        public WishlistService(
            IWishlistRepository repository,
            ICartService cartService,
            IAuthenticationService authService)
        {
            _repository = repository;
            _cartService = cartService;
            _authService = authService;
            _wishlistItems = new ObservableCollection<Product>();
        }


        public async Task AddToWishlistAsync(Product product)
        {
            if (product == null) return;
            if (_wishlistItems.Any(p => p.Id == product.Id))
                return;

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return;

            var ok = await _repository.AddAsync(userId, product.Id);
            if (ok)
            {
                product.IsInWishlist = true;
                _wishlistItems.Add(product);
                OnWishlistUpdated();
            }
        }

        public async Task AddToWishlistAsync(int productId)
        {
            if (_wishlistItems.Any(p => p.Id == productId))
                return;

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return;

            var ok = await _repository.AddAsync(userId, productId);
            if (ok)
            {
                // Recargar la wishlist para que aparezca el producto recién añadido
                await LoadWishlistAsync();
            }
        }

        public async Task RemoveFromWishlistAsync(int productId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return;

            var ok = await _repository.RemoveAsync(userId, productId);
            if (ok)
            {
                var item = _wishlistItems.FirstOrDefault(p => p.Id == productId);
                if (item != null)
                {
                    _wishlistItems.Remove(item);
                    OnWishlistUpdated();
                }
            }
        }

        public async Task<bool> IsInWishlistAsync(int productId)
        {
            // Primero comprobamos en memoria (rápido)
            if (_wishlistItems.Any(p => p.Id == productId))
                return true;

            // Si no, comprobamos en BD (por si la lista en memoria no está cargada)
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return false;

            return await _repository.ExistsAsync(userId, productId);
        }

        public async Task ClearWishlistAsync()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return;

            await _repository.ClearAsync(userId);
            _wishlistItems.Clear();
            OnWishlistUpdated();
        }

        public async Task LoadWishlistAsync()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                _wishlistItems.Clear();
                OnWishlistUpdated();
                return;
            }

            var items = await _repository.GetByUserIdAsync(userId);

            _wishlistItems.Clear();
            foreach (var item in items)
            {
                item.IsInWishlist = true;
                _wishlistItems.Add(item);
            }

            OnWishlistUpdated();
        }

        public async Task<List<Product>> GetWishlistAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Product>();

            return await _repository.GetByUserIdAsync(userId);
        }

        public async Task<int> GetWishlistCountAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return 0;

            return await _repository.CountAsync(userId);
        }

        public async Task MoveAllToCartAsync(string userId)
        {
            foreach (var product in _wishlistItems.ToList())
            {
                await _cartService.AddToCartAsync(product, 1);
            }
            // Limpiamos la wishlist tras moverlo todo
            await ClearWishlistAsync();
        }


        // ==================== HELPERS ====================

        private string GetCurrentUserId()
        {
            var user = _authService.GetCurrentUser();
            return user?.Id ?? string.Empty;
        }

        private void OnWishlistUpdated()
        {
            WishlistUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}