using HoloCrew.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

// Repositorio para gestionar la lista de deseos (wishlist).
// Define las operaciones básicas: añadir, quitar, listar y comprobar.
// La implementación real conectará con la tabla wishlist_items de Supabase.

namespace HoloCrew.Repositories.Interfaces
{
    public interface IWishlistRepository
    {
        Task<List<Product>> GetByUserIdAsync(string userId);
        Task<bool> AddAsync(string userId, int productId);
        Task<bool> RemoveAsync(string userId, int productId);
        Task<bool> ExistsAsync(string userId, int productId);
        Task<int> CountAsync(string userId);
        Task ClearAsync(string userId);
    }
}