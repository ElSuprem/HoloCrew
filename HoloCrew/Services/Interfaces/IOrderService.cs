using HoloCrew.Models;

namespace HoloCrew.Services.Interfaces
{
    /// <summary>
    /// Servicio para gestión de pedidos
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Crea un nuevo pedido
        /// </summary>
        Task<Order> CreateOrderAsync(Order order);

        /// <summary>
        /// Obtiene todos los pedidos de un usuario
        /// </summary>
        Task<List<Order>> GetUserOrdersAsync(int userId);

        /// <summary>
        /// Obtiene un pedido por su ID
        /// </summary>
        Task<Order> GetOrderByIdAsync(int orderId);

        /// <summary>
        /// Cancela un pedido
        /// </summary>
        Task<bool> CancelOrderAsync(int orderId);

        /// <summary>
        /// Obtiene información de tracking de un pedido
        /// </summary>
        Task<TrackingInfo> GetTrackingInfoAsync(string trackingNumber);

        /// <summary>
        /// Actualiza el estado de un pedido
        /// </summary>
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);

        /// <summary>
        /// Obtiene el historial de estados de un pedido
        /// </summary>
        Task<List<OrderStatusHistory>> GetOrderStatusHistoryAsync(int orderId);
    }
}