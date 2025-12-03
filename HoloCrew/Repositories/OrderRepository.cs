using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloCrew.Repositories
{
    /// <summary>
    /// Implementación del repositorio de pedidos
    /// NOTA: Usa datos MOCK en memoria
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private static List<Order> _orders;
        private static int _nextId = 1;

        public OrderRepository()
        {
            if (_orders == null)
            {
                InitializeMockData();
            }
        }

        public Task<Order> CreateAsync(Order order)
        {
            order.Id = _nextId++;
            order.OrderDate = DateTime.Now;

            // Asegurar que las listas no sean null
            order.Items ??= new List<OrderItem>();
            order.StatusHistory ??= new List<OrderStatusHistory>();

            _orders.Add(order);
            return Task.FromResult(order);
        }

        public Task<List<Order>> GetByUserIdAsync(int userId)
        {
            var userOrders = _orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            return Task.FromResult(userOrders);
        }

        public Task<Order> GetByIdAsync(int id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            return Task.FromResult(order);
        }

        public Task<Order> GetByOrderNumberAsync(string orderNumber)
        {
            var order = _orders.FirstOrDefault(o => o.OrderNumber == orderNumber);
            return Task.FromResult(order);
        }

        public Task<Order> UpdateAsync(Order order)
        {
            var existing = _orders.FirstOrDefault(o => o.Id == order.Id);
            if (existing != null)
            {
                var index = _orders.IndexOf(existing);
                _orders[index] = order;
                return Task.FromResult(order);
            }
            return Task.FromResult<Order>(null);
        }

        public Task<List<Order>> GetAllAsync()
        {
            return Task.FromResult(_orders.OrderByDescending(o => o.OrderDate).ToList());
        }

        public Task<List<Order>> GetByStatusAsync(OrderStatus status)
        {
            var orders = _orders
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            return Task.FromResult(orders);
        }

        public Task<List<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var orders = _orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            return Task.FromResult(orders);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                _orders.Remove(order);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private void InitializeMockData()
        {
            _orders = new List<Order>
            {
                new Order
                {
                    Id = _nextId++,
                    OrderNumber = "ORD-20241201-00001",
                    UserId = 1,
                    Status = OrderStatus.Delivered,
                    OrderDate = DateTime.Now.AddDays(-15),
                    EstimatedDeliveryDate = DateTime.Now.AddDays(-10),
                    DeliveredDate = DateTime.Now.AddDays(-12),
                    Subtotal = 729.98m,
                    ShippingCost = 0m,
                    Tax = 153.30m,
                    Discount = 0m,
                    Total = 883.28m,
                    TrackingNumber = "ES123456789",
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            Id = 1,
                            ProductId = 1,
                            Quantity = 1,
                            UnitPrice = 699.99m
                        },
                        new OrderItem
                        {
                            Id = 2,
                            ProductId = 4,
                            Quantity = 1,
                            UnitPrice = 29.99m
                        }
                    },
                    ShippingAddress = new Address
                    {
                        FullName = "Juan Pérez",
                        AddressLine1 = "Calle Mayor 123",
                        City = "Madrid",
                        PostalCode = "28001",
                        Country = "España"
                    },
                    StatusHistory = new List<OrderStatusHistory>
                    {
                        new OrderStatusHistory { Status = OrderStatus.Pending, Timestamp = DateTime.Now.AddDays(-15), Note = "Pedido creado" },
                        new OrderStatusHistory { Status = OrderStatus.Confirmed, Timestamp = DateTime.Now.AddDays(-15).AddHours(2), Note = "Pedido confirmado" },
                        new OrderStatusHistory { Status = OrderStatus.Processing, Timestamp = DateTime.Now.AddDays(-14), Note = "Preparando envío" },
                        new OrderStatusHistory { Status = OrderStatus.Shipped, Timestamp = DateTime.Now.AddDays(-13), Note = "Enviado" },
                        new OrderStatusHistory { Status = OrderStatus.Delivered, Timestamp = DateTime.Now.AddDays(-12), Note = "Entregado" }
                    }
                },
                new Order
                {
                    Id = _nextId++,
                    OrderNumber = "ORD-20241125-00002",
                    UserId = 1,
                    Status = OrderStatus.Shipped,
                    OrderDate = DateTime.Now.AddDays(-5),
                    EstimatedDeliveryDate = DateTime.Now.AddDays(2),
                    Subtotal = 279.99m,
                    ShippingCost = 5.99m,
                    Tax = 58.80m,
                    Discount = 0m,
                    Total = 344.78m,
                    TrackingNumber = "ES987654321",
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            Id = 3,
                            ProductId = 2,
                            Quantity = 1,
                            UnitPrice = 279.99m
                        }
                    },
                    ShippingAddress = new Address
                    {
                        FullName = "Juan Pérez",
                        AddressLine1 = "Calle Mayor 123",
                        City = "Madrid",
                        PostalCode = "28001",
                        Country = "España"
                    },
                    StatusHistory = new List<OrderStatusHistory>
                    {
                        new OrderStatusHistory { Status = OrderStatus.Pending, Timestamp = DateTime.Now.AddDays(-5), Note = "Pedido creado" },
                        new OrderStatusHistory { Status = OrderStatus.Confirmed, Timestamp = DateTime.Now.AddDays(-5).AddHours(1), Note = "Pedido confirmado" },
                        new OrderStatusHistory { Status = OrderStatus.Processing, Timestamp = DateTime.Now.AddDays(-4), Note = "Preparando envío" },
                        new OrderStatusHistory { Status = OrderStatus.Shipped, Timestamp = DateTime.Now.AddDays(-3), Note = "Enviado" }
                    }
                }
            };
        }
    }
}