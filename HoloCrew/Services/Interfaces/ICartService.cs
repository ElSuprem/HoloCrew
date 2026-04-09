using HoloCrew.Models;

// Servicio para manejar el carrito de compras: añadir, quitar, actualizar cantidades, aplicar cupón.
// Cuando el carrito cambia, lanza el evento CartUpdated para que la interfaz se refresque sola.

namespace HoloCrew.Services.Interfaces
{
    public interface ICartService
    {
        event EventHandler CartUpdated;  // se avisa cuando algo cambia en el carrito

        Task AddToCartAsync(Product product, int quantity, string variant = null);  // añadir producto
        Task UpdateQuantityAsync(int cartItemId, int newQuantity);                  // cambiar cantidad
        Task RemoveFromCartAsync(int cartItemId);                                   // quitar un producto
        Task ClearCartAsync();                                                      // vaciar todo
        Task<List<CartItem>> GetCartItemsAsync();                                   // lista de productos en el carrito
        Task<decimal> GetCartTotalAsync();                                          // suma total
        int GetCartItemCount();                                                     // cuántos productos (sumando cantidades)
        Task<bool> ApplyCouponAsync(string couponCode);                             // aplicar descuento
        decimal GetCurrentDiscount();                                               // descuento actual aplicado
    }
}