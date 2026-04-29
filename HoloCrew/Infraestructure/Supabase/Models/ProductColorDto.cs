using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

// DTO que mapea la tabla 'product_colors' de Supabase.

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("product_colors")]
    public class ProductColorDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("color_name")]
        public string ColorName { get; set; } = string.Empty;

        [Column("color_hex")]
        public string ColorHex { get; set; } = string.Empty;

        [Column("image_url")]
        public string? ImageUrl { get; set; }
    }
}