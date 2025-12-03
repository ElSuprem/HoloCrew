using HoloCrew.Models;

namespace HoloCrew.Services.Interfaces
{
    /// <summary>
    /// Servicio para gestión del carrito de compras
    /// </summary>
    public interface ICartService
    {
        /// <summary>
        /// Evento que se dispara cuando el carrito se actualiza
        /// </summary>
        event EventHandler CartUpdated;

        /// <summary>
        /// Agrega un producto al carrito
        /// </summary>
        Task AddToCartAsync(Product product, int quantity, string variant = null);

        /// <summary>
        /// Actualiza la cantidad de un item del carrito
        /// </summary>
        Task UpdateQuantityAsync(int cartItemId, int newQuantity);

        /// <summary>
        /// Elimina un item del carrito
        /// </summary>
        Task RemoveFromCartAsync(int cartItemId);

        /// <summary>
        /// Vacía completamente el carrito
        /// </summary>
        Task ClearCartAsync();

        /// <summary>
        /// Obtiene todos los items del carrito
        /// </summary>
        Task<List<CartItem>> GetCartItemsAsync();

        /// <summary>
        /// Calcula el total del carrito
        /// </summary>
        Task<decimal> GetCartTotalAsync();

        /// <summary>
        /// Obtiene la cantidad de items en el carrito
        /// </summary>
        int GetCartItemCount();

        /// <summary>
        /// Aplica un cupón de descuento
        /// </summary>
        Task<bool> ApplyCouponAsync(string couponCode);

        /// <summary>
        /// Obtiene el descuento actual aplicado
        /// </summary>
        decimal GetCurrentDiscount();
    }
}