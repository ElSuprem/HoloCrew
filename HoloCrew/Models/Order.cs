using System;
using System.Collections.Generic;

// Estados del pedido (pendiente, confirmado, enviado, etc.).
// Pedido completo con sus productos, direcciones, pagos e historial de cambios.

namespace HoloCrew.Models
{
    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5,
        Refunded = 6
    }

    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;  // UUID del usuario
        public User? User { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; }
        public Address? ShippingAddress { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string? TrackingNumber { get; set; }
        public List<OrderStatusHistory> StatusHistory { get; set; } = new();
        public int ItemCount { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string SelectedVariant { get; set; } = string.Empty;
        public decimal Subtotal => UnitPrice * Quantity;
    }

    public class OrderStatusHistory
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}