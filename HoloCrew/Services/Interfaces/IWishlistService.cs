using HoloCrew.Models;

namespace HoloCrew.Services.Interfaces
{
    /// <summary>
    /// Servicio para gestión de lista de deseos
    /// </summary>
    public interface IWishlistService
    {
        /// <summary>
        /// Evento que se dispara cuando la wishlist cambia
        /// </summary>
        event EventHandler WishlistUpdated;

        /// <summary>
        /// Agrega un producto a la wishlist (por ID - carga nuevo producto)
        /// </summary>
        Task AddToWishlistAsync(int productId);

        /// <summary>
        /// Agrega un producto a la wishlist (usa el mismo objeto)
        /// </summary>
        Task AddToWishlistAsync(Product product);

        /// <summary>
        /// Elimina un producto de la wishlist
        /// </summary>
        Task RemoveFromWishlistAsync(int productId);

        /// <summary>
        /// Obtiene todos los productos en la wishlist de un usuario
        /// </summary>
        Task<List<Product>> GetWishlistAsync(int userId);

        /// <summary>
        /// Verifica si un producto está en la wishlist
        /// </summary>
        Task<bool> IsInWishlistAsync(int productId);

        /// <summary>
        /// Vacía completamente la wishlist
        /// </summary>
        Task ClearWishlistAsync();

        /// <summary>
        /// Obtiene la cantidad de productos en la wishlist
        /// </summary>
        Task<int> GetWishlistCountAsync(int userId);

        /// <summary>
        /// Mueve todos los items de la wishlist al carrito
        /// </summary>
        Task MoveAllToCartAsync(int userId);
    }
}