using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;

// Servicio de pedidos. Usa IOrderRepository para guardar y consultar pedidos.
// Se encarga de crear pedidos, cancelarlos, actualizar estados, y obtener información de tracking.

namespace HoloCrew.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
            {
                return null;
            }

            order.OrderDate = DateTime.Now;
            order.Status = OrderStatus.Pending;
            order.EstimatedDeliveryDate = DateTime.Now.AddDays(5); // se estiman 5 días de envío

            // historial inicial del pedido
            order.StatusHistory = new List<OrderStatusHistory>
            {
                new OrderStatusHistory
                {
                    Status = OrderStatus.Pending,
                    Timestamp = DateTime.Now,
                    Note = "Pedido creado"
                }
            };

            var createdOrder = await _orderRepository.CreateAsync(order);
            return createdOrder;
        }

        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            return await _orderRepository.GetByUserIdAsync(userId);
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);

                if (order == null)
                {
                    return false;
                }

                // solo se puede cancelar si está pendiente o confirmado (no si ya está enviado o entregado)
                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                {
                    return false;
                }

                order.Status = OrderStatus.Cancelled;
                order.StatusHistory.Add(new OrderStatusHistory
                {
                    Status = OrderStatus.Cancelled,
                    Timestamp = DateTime.Now,
                    Note = "Pedido cancelado por el usuario"
                });

                await _orderRepository.UpdateAsync(order);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<TrackingInfo> GetTrackingInfoAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
            {
                return null;
            }

            // en producción se consultaría la API de la empresa de paquetería (DHL, Correos, etc.)
            // de momento, datos falsos de ejemplo
            return await Task.FromResult(new TrackingInfo
            {
                TrackingNumber = trackingNumber,
                Carrier = "DHL Express",
                ShipDate = DateTime.Now.AddDays(-2),
                EstimatedDelivery = DateTime.Now.AddDays(3),
                CurrentStatus = "En tránsito",
                CurrentLocation = "Madrid, España",
                Events = new List<TrackingEvent>
                {
                    new TrackingEvent
                    {
                        Timestamp = DateTime.Now.AddDays(-2),
                        Location = "Almacén Madrid",
                        Status = "Empaquetado",
                        Description = "Pedido empaquetado y listo para envío"
                    },
                    new TrackingEvent
                    {
                        Timestamp = DateTime.Now.AddDays(-1),
                        Location = "Centro de distribución Madrid",
                        Status = "En tránsito",
                        Description = "Paquete en ruta"
                    }
                }
            });
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);

                if (order == null)
                {
                    return false;
                }

                order.Status = newStatus;
                order.StatusHistory.Add(new OrderStatusHistory
                {
                    Status = newStatus,
                    Timestamp = DateTime.Now,
                    Note = $"Estado actualizado a {newStatus}"
                });

                if (newStatus == OrderStatus.Delivered)
                {
                    order.DeliveredDate = DateTime.Now;
                }

                await _orderRepository.UpdateAsync(order);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<OrderStatusHistory>> GetOrderStatusHistoryAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            return order?.StatusHistory ?? new List<OrderStatusHistory>();
        }
    }
}