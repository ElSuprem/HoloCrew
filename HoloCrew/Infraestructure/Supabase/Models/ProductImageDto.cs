using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

// DTO que mapea la tabla 'product_images' de Supabase.

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("product_images")]
    public class ProductImageDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [Column("alt_text")]
        public string? AltText { get; set; }

        [Column("sort_order")]
        public int SortOrder { get; set; }

        [Column("is_primary")]
        public bool IsPrimary { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}