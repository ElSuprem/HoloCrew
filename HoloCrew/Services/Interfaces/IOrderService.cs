using HoloCrew.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Servicio de pedidos: crear, cancelar, consultar, cambiar estado.

namespace HoloCrew.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderFromCartAsync(Address shippingAddress, string paymentMethod, string? couponCode = null);
        Task<List<Order>> GetUserOrdersAsync(string userId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<bool> CancelOrderAsync(int orderId);
        Task<TrackingInfo?> GetTrackingInfoAsync(string trackingNumber);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        Task<List<OrderStatusHistory>> GetOrderStatusHistoryAsync(int orderId);
    }
}