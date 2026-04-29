using HoloCrew.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;

// DTO que mapea la tabla 'orders' de Supabase.
// Mantiene los campos esenciales que necesita la UI; el resto (notas internas,
// timestamps de cambio de estado, etc.) los gestiona la BD automáticamente vía triggers.

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("orders")]
    public class OrderDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("order_number")]
        public string OrderNumber { get; set; } = string.Empty;

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("status")]
        public string Status { get; set; } = "pending";

        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "pending";

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("shipping_cost")]
        public decimal ShippingCost { get; set; }

        [Column("tax")]
        public decimal Tax { get; set; }

        [Column("discount")]
        public decimal Discount { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("coupon_code")]
        public string? CouponCode { get; set; }

        [Column("payment_method")]
        public string? PaymentMethod { get; set; }

        [Column("tracking_number")]
        public string? TrackingNumber { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        [Column("shipped_at")]
        public DateTime? ShippedAt { get; set; }

        [Column("delivered_at")]
        public DateTime? DeliveredAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }


        public Order ToOrder(List<OrderItemDto>? items = null)
        {
            return new Order
            {
                Id = Id,
                OrderNumber = OrderNumber,
                UserId = UserId.ToString(),
                Status = ParseStatus(Status),
                Subtotal = Subtotal,
                ShippingCost = ShippingCost,
                Tax = Tax,
                Discount = Discount,
                Total = Total,
                OrderDate = OrderDate,
                DeliveredDate = DeliveredAt,
                TrackingNumber = TrackingNumber,
                Items = items?.Select(i => i.ToOrderItem()).ToList() ?? new List<OrderItem>()
            };
        }

        // Convierte el string del enum de Postgres al enum C#
        public static OrderStatus ParseStatus(string status) => status?.ToLower() switch
        {
            "pending" => OrderStatus.Pending,
            "confirmed" => OrderStatus.Confirmed,
            "processing" => OrderStatus.Processing,
            "shipped" => OrderStatus.Shipped,
            "delivered" => OrderStatus.Delivered,
            "cancelled" => OrderStatus.Cancelled,
            "refunded" => OrderStatus.Refunded,
            _ => OrderStatus.Pending
        };

        // Convierte el enum C# al string que entiende Postgres
        public static string StatusToString(OrderStatus status) => status switch
        {
            OrderStatus.Pending => "pending",
            OrderStatus.Confirmed => "confirmed",
            OrderStatus.Processing => "processing",
            OrderStatus.Shipped => "shipped",
            OrderStatus.Delivered => "delivered",
            OrderStatus.Cancelled => "cancelled",
            OrderStatus.Refunded => "refunded",
            _ => "pending"
        };
    }
}