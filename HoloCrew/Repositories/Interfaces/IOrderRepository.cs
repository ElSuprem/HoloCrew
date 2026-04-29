using HoloCrew.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Repositorio para manejar pedidos contra Supabase.

namespace HoloCrew.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> CreateFromCartAsync(string userId, Address shippingAddress, string paymentMethod, string? couponCode = null);
        Task<List<Order>> GetByUserIdAsync(string userId);
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByOrderNumberAsync(string orderNumber);
        Task<bool> CancelAsync(int orderId);
        Task<bool> UpdateStatusAsync(int orderId, OrderStatus newStatus);
    }
}