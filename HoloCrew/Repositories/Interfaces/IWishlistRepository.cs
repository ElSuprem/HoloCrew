using HoloCrew.Models;

namespace HoloCrew.Repositories.Interfaces
{
    /// <summary>
    /// Repositorio para acceso a datos de wishlist
    /// </summary>
    public interface IWishlistRepository
    {
        /// <summary>
        /// Agrega un producto a la wishlist
        /// </summary>
        Task<bool> AddProductAsync(int userId, int productId);

        /// <summary>
        /// Elimina un producto de la wishlist
        /// </summary>
        Task<bool> RemoveProductAsync(int userId, int productId);

        /// <summary>
        /// Obtiene todos los IDs de productos en la wishlist de un usuario
        /// </summary>
        Task<List<int>> GetProductIdsAsync(int userId);

        /// <summary>
        /// Verifica si un producto está en la wishlist
        /// </summary>
        Task<bool> ContainsProductAsync(int userId, int productId);

        /// <summary>
        /// Limpia toda la wishlist de un usuario
        /// </summary>
        Task<bool> ClearAsync(int userId);

        /// <summary>
        /// Obtiene la cantidad de productos en la wishlist
        /// </summary>
        Task<int> GetCountAsync(int userId);
    }
}