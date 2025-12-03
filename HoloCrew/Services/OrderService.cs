using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;

namespace HoloCrew.Services
{
    /// <summary>
    /// Implementación del servicio de pedidos
    /// </summary>
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

            // Establecer valores iniciales
            order.OrderDate = DateTime.Now;
            order.Status = OrderStatus.Pending;
            order.EstimatedDeliveryDate = DateTime.Now.AddDays(5); // 5 días de estimación

            // Crear historial inicial
            order.StatusHistory = new List<OrderStatusHistory>
            {
                new OrderStatusHistory
                {
                    Status = OrderStatus.Pending,
                    Timestamp = DateTime.Now,
                    Note = "Pedido creado"
                }
            };

            // Guardar en repositorio
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

                // Solo se puede cancelar si está en Pending o Confirmed
                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                {
                    return false;
                }

                // Actualizar estado
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

            // TODO: Consultar API de transporte (DHL, FedEx, etc.)
            // Por ahora, retornar datos mock
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