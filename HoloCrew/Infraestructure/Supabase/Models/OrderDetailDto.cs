using HoloCrew.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

// DTO que mapea la vista 'orders_with_details' de Supabase.
// La vista enriquece cada pedido con: total de items, cantidad total y la primera imagen
// (útil para listar el historial sin tener que cargar todos los items).

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("orders_with_details")]
    public class OrderDetailDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("order_number")]
        public string OrderNumber { get; set; } = string.Empty;

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("status")]
        public string Status { get; set; } = "pending";

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

        [Column("tracking_number")]
        public string? TrackingNumber { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        [Column("delivered_at")]
        public DateTime? DeliveredAt { get; set; }

        [Column("item_count")]
        public long ItemCount { get; set; }

        [Column("total_quantity")]
        public long TotalQuantity { get; set; }

        [Column("first_item_image")]
        public string? FirstItemImage { get; set; }


        public Order ToOrder()
        {
            return new Order
            {
                Id = Id,
                OrderNumber = OrderNumber,
                UserId = UserId.ToString(),
                Status = OrderDto.ParseStatus(Status),
                Subtotal = Subtotal,
                ShippingCost = ShippingCost,
                Tax = Tax,
                Discount = Discount,
                Total = Total,
                OrderDate = OrderDate,
                DeliveredDate = DeliveredAt,
                TrackingNumber = TrackingNumber,
                ItemCount = (int)TotalQuantity,
                Items = new System.Collections.Generic.List<OrderItem>()
            };
        }
    }
}