using HoloCrew.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Servicio para manejar el carrito de compras: añadir, quitar, actualizar cantidades, aplicar cupón.
// Cuando el carrito cambia, lanza el evento CartUpdated para que la interfaz se refresque sola.

namespace HoloCrew.Services.Interfaces
{
    public interface ICartService
    {
        event EventHandler CartUpdated;
        Task AddToCartAsync(Product product, int quantity, string variant = null);
        Task UpdateQuantityAsync(int cartItemId, int newQuantity);
        Task RemoveFromCartAsync(int cartItemId);
        Task ClearCartAsync();
        Task<List<CartItem>> GetCartItemsAsync();
        Task<decimal> GetCartTotalAsync();
        int GetCartItemCount();
        Task<CouponResult> ApplyCouponAsync(string couponCode);
        decimal GetCurrentDiscount();
        string GetAppliedCouponCode();  // para pasar al checkout al pagar
        Task LoadCartAsync();
    }


    // Resultado de aplicar un cupón a través de la RPC validate_coupon
    public class CouponResult
    {
        public bool IsValid { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal? NewShippingCost { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}