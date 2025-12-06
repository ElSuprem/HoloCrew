using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloCrew.Services
{
    /// <summary>
    /// Implementación del servicio de wishlist
    /// </summary>
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductRepository _productRepository;
        private int _currentUserId = 1; // ⭐ TEMPORAL: Usar ID fijo hasta implementar autenticación

        public WishlistService(
            IWishlistRepository wishlistRepository,
            IProductRepository productRepository)
        {
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
        }

        public async Task AddToWishlistAsync(int productId)
        {
            // ⭐ IMPLEMENTADO: Usar el repositorio
            await _wishlistRepository.AddProductAsync(_currentUserId, productId);
        }

        public async Task RemoveFromWishlistAsync(int productId)
        {
            // ⭐ IMPLEMENTADO: Usar el repositorio
            await _wishlistRepository.RemoveProductAsync(_currentUserId, productId);
        }

        public async Task<List<Product>> GetWishlistAsync(int userId)
        {
            // ⭐ IMPLEMENTADO: Obtener IDs del repositorio y luego los productos
            var productIds = await _wishlistRepository.GetProductIdsAsync(userId);
            var products = new List<Product>();

            foreach (var productId in productIds)
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product != null)
                {
                    products.Add(product);
                }
            }

            return products;
        }

        public async Task<bool> IsInWishlistAsync(int productId)
        {
            // ⭐ IMPLEMENTADO: Verificar con repositorio
            return await _wishlistRepository.ContainsProductAsync(_currentUserId, productId);
        }

        public async Task ClearWishlistAsync()
        {
            // ⭐ IMPLEMENTADO: Usar repositorio
            await _wishlistRepository.ClearAsync(_currentUserId);
        }

        public async Task<int> GetWishlistCountAsync(int userId)
        {
            // ⭐ IMPLEMENTADO: Obtener del repositorio
            return await _wishlistRepository.GetCountAsync(userId);
        }

        public async Task MoveAllToCartAsync(int userId)
        {
            // TODO: Implementar cuando tengas acceso a ICartService
            // Por ahora, solo limpiamos la wishlist
            await _wishlistRepository.ClearAsync(userId);
            await Task.CompletedTask;
        }
    }
}