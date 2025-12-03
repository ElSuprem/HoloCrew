using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace HoloCrew.Services
{
    /// <summary>
    /// Implementación del servicio de wishlist
    /// </summary>
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductRepository _productRepository;

        public WishlistService(
            IWishlistRepository wishlistRepository,
            IProductRepository productRepository)
        {
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
        }

        public async Task AddToWishlistAsync(int productId)
        {
            // TODO: Implementar con repositorio real
            await Task.CompletedTask;
        }

        public async Task RemoveFromWishlistAsync(int productId)
        {
            // TODO: Implementar con repositorio real
            await Task.CompletedTask;
        }

        public async Task<List<Product>> GetWishlistAsync(int userId)
        {
            // TODO: Obtener IDs de productos en wishlist desde repositorio
            // Por ahora, retornar lista vacía
            return await Task.FromResult(new List<Product>());
        }

        public async Task<bool> IsInWishlistAsync(int productId)
        {
            // TODO: Verificar con repositorio
            return await Task.FromResult(false);
        }

        public async Task ClearWishlistAsync()
        {
            // TODO: Implementar con repositorio
            await Task.CompletedTask;
        }

        public async Task<int> GetWishlistCountAsync(int userId)
        {
            var wishlist = await GetWishlistAsync(userId);
            return wishlist.Count;
        }

        public async Task MoveAllToCartAsync(int userId)
        {
            // TODO: Implementar
            // 1. Obtener wishlist
            // 2. Agregar cada producto al carrito
            // 3. Limpiar wishlist
            await Task.CompletedTask;
        }
    }
}