using HoloCrew.Models;

// Servicio para gestionar pedidos: crear, cancelar, consultar, tracking, cambiar estado.
// Se conecta con los modelos Order, OrderStatus, TrackingInfo, OrderStatusHistory.

namespace HoloCrew.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order);                               // crear nuevo pedido
        Task<List<Order>> GetUserOrdersAsync(int userId);                        // pedidos de un usuario
        Task<Order> GetOrderByIdAsync(int orderId);                              // buscar pedido por id
        Task<bool> CancelOrderAsync(int orderId);                                // cancelar pedido
        Task<TrackingInfo> GetTrackingInfoAsync(string trackingNumber);          // seguimiento del envío
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);   // cambiar estado (admin)
        Task<List<OrderStatusHistory>> GetOrderStatusHistoryAsync(int orderId);  // historial de cambios de estado
    }
}