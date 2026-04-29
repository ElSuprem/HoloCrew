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
        ObservableCollection<Product> WishlistItems { get; }
        int WishlistCount { get; }
        event EventHandler? WishlistUpdated;

        Task AddToWishlistAsync(Product product);
        Task AddToWishlistAsync(int productId);
        Task RemoveFromWishlistAsync(int productId);
        Task<bool> IsInWishlistAsync(int productId);
        Task ClearWishlistAsync();
        Task LoadWishlistAsync();
        Task<List<Product>> GetWishlistAsync(string userId);
        Task<int> GetWishlistCountAsync(string userId);
        Task MoveAllToCartAsync(string userId);
    }
}