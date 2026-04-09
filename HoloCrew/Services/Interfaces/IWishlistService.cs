using HoloCrew.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

// Servicio para gestionar la lista de deseos (wishlist) del usuario.
// Permite añadir, quitar, consultar si un producto está en favoritos, mover todo al carrito, etc.
// WishlistUpdated avisa a la interfaz cuando hay cambios.

namespace HoloCrew.Services.Interfaces
{
    public interface IWishlistService
    {
        ObservableCollection<Product> WishlistItems { get; }  // lista de productos favoritos (se actualiza sola en la interfaz)
        int WishlistCount { get; }                            // cuántos productos tiene en favoritos

        event EventHandler? WishlistUpdated;                  // salta cuando cambia la wishlist

        Task AddToWishlistAsync(Product product);             // añadir producto a favoritos
        Task AddToWishlistAsync(int productId);               // añadir por id
        Task RemoveFromWishlistAsync(int productId);          // quitar de favoritos
        Task<bool> IsInWishlistAsync(int productId);          // comprobar si ya está en favoritos
        Task ClearWishlistAsync();                            // vaciar toda la wishlist
        Task LoadWishlistAsync();                             // cargar los favoritos del usuario
        Task<List<Product>> GetWishlistAsync(int userId);     // obtener lista de favoritos de un usuario
        Task<int> GetWishlistCountAsync(int userId);          // cuántos favoritos tiene un usuario
        Task MoveAllToCartAsync(int userId);                  // mover todos los favoritos al carrito
    }
}