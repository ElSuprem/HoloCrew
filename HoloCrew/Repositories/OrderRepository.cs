using HoloCrew.Infraestructure.Supabase.Models;
using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using Newtonsoft.Json;
using PgConstants = Supabase.Postgrest.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Repositorio de pedidos conectado a Supabase.
// - Crear: usa la RPC create_order_from_cart (transaccional, valida stock, vacía carrito).
// - Listar: usa la vista orders_with_details (un solo SELECT, ya viene enrichida).
// - Detalle: hace 2 queries (la orden + sus items).
// - Cancelar/actualizar estado: queries directas a la tabla orders.

namespace HoloCrew.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Supabase.Client _supabase;

        public OrderRepository(Supabase.Client supabase)
        {
            _supabase = supabase;
        }


        public async Task<Order?> CreateFromCartAsync(
             string userId,
             Address shippingAddress,
             string paymentMethod,
            string? couponCode = null)
        {
            try
            {
                var parameters = new Dictionary<string, object>
        {
            { "p_shipping_address", JsonConvert.SerializeObject(shippingAddress) },
            { "p_billing_address", JsonConvert.SerializeObject(shippingAddress) },
            { "p_payment_method", paymentMethod }
        };

                if (!string.IsNullOrEmpty(couponCode))
                    parameters.Add("p_coupon_code", couponCode);

                // La RPC devuelve la fila completa del pedido recién creado (tipo 'orders')
                var response = await _supabase.Rpc("create_order_from_cart", parameters);

                if (response?.Content == null)
                {
                    System.Diagnostics.Debug.WriteLine("[Order] CreateFromCart: respuesta vacía");
                    return null;
                }

                var rawContent = response.Content.Trim();

                // Deserializar la respuesta como un OrderDto (la RPC devuelve la fila completa)
                OrderDto? createdOrderDto = null;
                try
                {
                    createdOrderDto = JsonConvert.DeserializeObject<OrderDto>(rawContent);
                }
                catch
                {
                    // Por si devuelve la fila envuelta en un array
                    try
                    {
                        var array = JsonConvert.DeserializeObject<List<OrderDto>>(rawContent);
                        createdOrderDto = array?.FirstOrDefault();
                    }
                    catch { }
                }

                if (createdOrderDto == null || createdOrderDto.Id <= 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[Order] CreateFromCart: no se pudo deserializar. Raw: {rawContent}");
                    return null;
                }

                // Cargar los items del pedido recién creado para tener todos los datos
                var itemsResponse = await _supabase
                    .From<OrderItemDto>()
                    .Where(i => i.OrderId == createdOrderDto.Id)
                    .Get();

                return createdOrderDto.ToOrder(itemsResponse.Models);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Order] CreateFromCart error: {ex.Message}");
                return null;
            }
        }


        public async Task<List<Order>> GetByUserIdAsync(string userId)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                    return new List<Order>();

                var response = await _supabase
                    .From<OrderDetailDto>()
                    .Where(o => o.UserId == userGuid)
                    .Order("order_date", PgConstants.Ordering.Descending)
                    .Get();

                return response.Models
                    .Select(o => o.ToOrder())
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Order] GetByUser error: {ex.Message}");
                return new List<Order>();
            }
        }


        public async Task<Order?> GetByIdAsync(int id)
        {
            try
            {
                var orderResponse = await _supabase
                    .From<OrderDto>()
                    .Where(o => o.Id == id)
                    .Single();

                if (orderResponse == null)
                    return null;

                // Cargar los items del pedido
                var itemsResponse = await _supabase
                    .From<OrderItemDto>()
                    .Where(i => i.OrderId == id)
                    .Get();

                return orderResponse.ToOrder(itemsResponse.Models);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Order] GetById error: {ex.Message}");
                return null;
            }
        }


        public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
        {
            try
            {
                var orderResponse = await _supabase
                    .From<OrderDto>()
                    .Where(o => o.OrderNumber == orderNumber)
                    .Single();

                if (orderResponse == null)
                    return null;

                var itemsResponse = await _supabase
                    .From<OrderItemDto>()
                    .Where(i => i.OrderId == orderResponse.Id)
                    .Get();

                return orderResponse.ToOrder(itemsResponse.Models);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Order] GetByOrderNumber error: {ex.Message}");
                return null;
            }
        }


        public async Task<bool> CancelAsync(int orderId)
        {
            return await UpdateStatusAsync(orderId, OrderStatus.Cancelled);
        }


        public async Task<bool> UpdateStatusAsync(int orderId, OrderStatus newStatus)
        {
            try
            {
                var statusString = OrderDto.StatusToString(newStatus);

                await _supabase
                    .From<OrderDto>()
                    .Where(o => o.Id == orderId)
                    .Set(o => o.Status, statusString)
                    .Update();

                // El trigger log_order_status_change registrará el cambio en el historial automáticamente.
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Order] UpdateStatus error: {ex.Message}");
                return false;
            }
        }
    }
}