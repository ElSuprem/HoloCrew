using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

// DTO que mapea la tabla 'product_sizes' de Supabase.

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("product_sizes")]
    public class ProductSizeDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("size")]
        public string Size { get; set; } = string.Empty;

        [Column("stock")]
        public int Stock { get; set; }
    }
}