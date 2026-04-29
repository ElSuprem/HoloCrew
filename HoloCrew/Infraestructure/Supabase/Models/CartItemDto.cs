using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

// DTO que mapea la tabla 'cart_items' de Supabase.
// Solo se usa para borrar y actualizar cantidad.
// Para añadir usamos la RPC add_to_cart (que valida stock).
// Para listar usamos CartDetailDto que ya trae el producto enrichido.

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("cart_items")]
    public class CartItemDto : BaseModel
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
    }
}