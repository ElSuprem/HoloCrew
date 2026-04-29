using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Servicio de pedidos. Delega en el OrderRepository conectado a Supabase.
// La creación usa la RPC create_order_from_cart que es transaccional.

namespace HoloCrew.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAuthenticationService _authService;
        private readonly ICartService _cartService;

        public OrderService(
            IOrderRepository orderRepository,
            IAuthenticationService authService,
            ICartService cartService)
        {
            _orderRepository = orderRepository;
            _authService = authService;
            _cartService = cartService;
        }


        public async Task<Order?> CreateOrderFromCartAsync(
            Address shippingAddress,
            string paymentMethod,
            string? couponCode = null)
        {
            var user = _authService.GetCurrentUser();
            if (user == null || string.IsNullOrEmpty(user.Id))
                return null;

            var order = await _orderRepository.CreateFromCartAsync(
                user.Id,
                shippingAddress,
                paymentMethod,
                couponCode);

            if (order != null)
            {
                // Tras crear el pedido, recargamos el carrito (que la BD ya vació)
                await _cartService.LoadCartAsync();
            }

            return order;
        }


        public async Task<List<Order>> GetUserOrdersAsync(string userId)
        {
            return await _orderRepository.GetByUserIdAsync(userId);
        }


        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }


        public async Task<bool> CancelOrderAsync(int orderId)
        {
            return await _orderRepository.CancelAsync(orderId);
        }


        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            return await _orderRepository.UpdateStatusAsync(orderId, newStatus);
        }


        public async Task<TrackingInfo?> GetTrackingInfoAsync(string trackingNumber)
        {
            // La integración real con la API de la empresa de mensajería (DHL, Correos, etc.)
            // se haría aquí. De momento, datos de ejemplo.
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return null;

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


        public async Task<List<OrderStatusHistory>> GetOrderStatusHistoryAsync(int orderId)
        {
            // El historial está en la tabla order_status_history.
            // Si quieres mostrarlo en la UI lo implementamos como DTO en otro paso.
            // De momento devolvemos lista vacía.
            return await Task.FromResult(new List<OrderStatusHistory>());
        }
    }
}