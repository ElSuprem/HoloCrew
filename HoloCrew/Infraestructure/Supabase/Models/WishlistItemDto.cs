using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

// DTO que mapea la tabla 'wishlist_items' de Supabase.
// Solo se usa para insertar/borrar. Para listar usamos WishlistDetailDto que ya trae el producto.

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("wishlist_items")]
    public class WishlistItemDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("price_at_add")]
        public decimal? PriceAtAdd { get; set; }

        [Column("notify_on_sale")]
        public bool NotifyOnSale { get; set; }

        [Column("added_at")]
        public DateTime AddedAt { get; set; }
    }
}