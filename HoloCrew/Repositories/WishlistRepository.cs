using HoloCrew.Repositories;
using HoloCrew.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloCrew.Repositories
{
    /// <summary>
    /// Implementación del repositorio de wishlist
    /// NOTA: Usa datos MOCK en memoria
    /// Almacena pares (userId, productId)
    /// </summary>
    public class WishlistRepository : IWishlistRepository
    {
        // Estructura: Dictionary<UserId, List<ProductId>>
        private static Dictionary<int, List<int>> _wishlists = new();

        public Task<bool> AddProductAsync(int userId, int productId)
        {
            if (!_wishlists.ContainsKey(userId))
            {
                _wishlists[userId] = new List<int>();
            }

            if (!_wishlists[userId].Contains(productId))
            {
                _wishlists[userId].Add(productId);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> RemoveProductAsync(int userId, int productId)
        {
            if (_wishlists.ContainsKey(userId))
            {
                var removed = _wishlists[userId].Remove(productId);
                return Task.FromResult(removed);
            }

            return Task.FromResult(false);
        }

        public Task<List<int>> GetProductIdsAsync(int userId)
        {
            if (_wishlists.ContainsKey(userId))
            {
                return Task.FromResult(_wishlists[userId].ToList());
            }

            return Task.FromResult(new List<int>());
        }

        public Task<bool> ContainsProductAsync(int userId, int productId)
        {
            if (_wishlists.ContainsKey(userId))
            {
                return Task.FromResult(_wishlists[userId].Contains(productId));
            }

            return Task.FromResult(false);
        }

        public Task<bool> ClearAsync(int userId)
        {
            if (_wishlists.ContainsKey(userId))
            {
                _wishlists[userId].Clear();
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<int> GetCountAsync(int userId)
        {
            if (_wishlists.ContainsKey(userId))
            {
                return Task.FromResult(_wishlists[userId].Count);
            }

            return Task.FromResult(0);
        }
    }
}