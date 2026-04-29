using HoloCrew.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

// Repositorio para gestionar el carrito de compras.
// Define las operaciones contra Supabase: listar, añadir, actualizar cantidad, eliminar y vaciar.

namespace HoloCrew.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<List<CartItem>> GetByUserIdAsync(string userId);
        Task<bool> AddAsync(int productId, int quantity, string size, string color);
        Task<bool> UpdateQuantityAsync(int cartItemId, int newQuantity);
        Task<bool> RemoveAsync(int cartItemId);
        Task<bool> ClearAsync(string userId);
        Task<int> GetItemCountAsync(string userId);
    }
}