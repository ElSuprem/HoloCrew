using HoloCrew.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
        Task<List<Product>> GetWishlistAsync(int userId);
        Task<int> GetWishlistCountAsync(int userId);
        Task MoveAllToCartAsync(int userId);
    }
}