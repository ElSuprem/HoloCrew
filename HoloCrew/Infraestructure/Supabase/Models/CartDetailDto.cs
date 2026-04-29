using HoloCrew.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

// DTO que mapea la vista 'cart_with_details' de Supabase.
// La vista hace JOIN con products y devuelve los datos enrichidos en una sola query.
// Se usa para listar el carrito con toda la info del producto.

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("cart_with_details")]
    public class CartDetailDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("size")]
        public string? Size { get; set; }

        [Column("color")]
        public string? Color { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price_at_add")]
        public decimal PriceAtAdd { get; set; }

        [Column("added_at")]
        public DateTime AddedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [Column("product_slug")]
        public string ProductSlug { get; set; } = string.Empty;

        [Column("current_price")]
        public decimal CurrentPrice { get; set; }

        [Column("original_price")]
        public decimal? OriginalPrice { get; set; }

        [Column("product_is_active")]
        public bool ProductIsActive { get; set; }

        [Column("main_image_url")]
        public string? MainImageUrl { get; set; }

        [Column("stock_available")]
        public int StockAvailable { get; set; }

        [Column("line_total")]
        public decimal LineTotal { get; set; }

        [Column("price_change")]
        public string? PriceChange { get; set; }


        // Convierte este DTO al modelo CartItem que usan los ViewModels.
        public CartItem ToCartItem()
        {
            return new CartItem
            {
                Id = Id,
                ProductId = ProductId,
                ProductName = ProductName,
                Price = CurrentPrice,
                Quantity = Quantity,
                Size = Size ?? string.Empty,
                Color = Color ?? string.Empty,
                ImageUrl = MainImageUrl ?? string.Empty
            };
        }
    }
}