using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using HoloCrew.Models;

// DTO que mapea la tabla 'categories' de Supabase.
// Soporta jerarquía padre/hijo (parent_id).

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("categories")]
    public class CategoryDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("slug")]
        public string Slug { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("icon")]
        public string? Icon { get; set; }

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Column("parent_id")]
        public int? ParentId { get; set; }

        [Column("depth")]
        public int Depth { get; set; }

        [Column("sort_order")]
        public int SortOrder { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }


        // Convierte este DTO al modelo Category que usan los ViewModels.
        public Category ToCategory()
        {
            return new Category
            {
                Id = Id,
                Name = Name
                // Si tu Category tiene más campos (Slug, Icon...), añádelos aquí
            };
        }
    }
}