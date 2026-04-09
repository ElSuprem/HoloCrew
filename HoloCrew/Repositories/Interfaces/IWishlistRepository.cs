using HoloCrew.Models;

// Repositorio para la lista de deseos (wishlist) del usuario.
// Guarda qué productos ha marcado cada usuario.

namespace HoloCrew.Repositories.Interfaces
{
    public interface IWishlistRepository
    {
        Task<bool> AddProductAsync(int userId, int productId);       // añadir a favoritos
        Task<bool> RemoveProductAsync(int userId, int productId);    // quitar de favoritos
        Task<List<int>> GetProductIdsAsync(int userId);              // sacar todos los ids de productos favoritos
        Task<bool> ContainsProductAsync(int userId, int productId);  // comprobar si ya está en favoritos
        Task<bool> ClearAsync(int userId);                           // vaciar toda la wishlist
        Task<int> GetCountAsync(int userId);                         // cuántos productos tiene en favoritos
    }
}