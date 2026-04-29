using HoloCrew.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

// DTO que mapea la tabla 'order_items' de Supabase.
// Almacena un snapshot del producto en el momento del pedido (nombre, precio, imagen)
// para que aunque luego cambien los datos del producto, el pedido se mantenga estable.

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("order_items")]
    public class OrderItemDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [Column("product_image_url")]
        public string? ProductImageUrl { get; set; }

        [Column("product_sku")]
        public string? ProductSku { get; set; }

        [Column("size")]
        public string? Size { get; set; }

        [Column("color")]
        public string? Color { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("discount")]
        public decimal Discount { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }


        public OrderItem ToOrderItem()
        {
            return new OrderItem
            {
                Id = Id,
                OrderId = OrderId,
                ProductId = ProductId,
                Quantity = Quantity,
                UnitPrice = UnitPrice,
                SelectedVariant = $"{Size ?? ""}/{Color ?? ""}".Trim('/'),
                Product = new Product
                {
                    Id = ProductId,
                    Name = ProductName,
                    Price = UnitPrice,
                    MainImageUrl = ProductImageUrl ?? string.Empty
                }
            };
        }
    }
}