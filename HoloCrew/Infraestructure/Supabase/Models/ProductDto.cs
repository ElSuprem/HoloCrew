using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using HoloCrew.Models;
using System;
using System.Collections.Generic;
using System.Linq;

// DTO que mapea la tabla 'products' de Supabase.
// Solo se usa para hablar con la base de datos.
// El método ToProduct() convierte el DTO al modelo Product.cs que usan los ViewModels.

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("products")]
    public class ProductDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("slug")]
        public string Slug { get; set; } = string.Empty;

        [Column("sku")]
        public string? Sku { get; set; }

        [Column("short_description")]
        public string? ShortDescription { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("original_price")]
        public decimal? OriginalPrice { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("gender")]
        public string? Gender { get; set; }

        [Column("stock")]
        public int Stock { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("is_new")]
        public bool IsNew { get; set; }

        [Column("is_featured")]
        public bool IsFeatured { get; set; }

        [Column("is_best_seller")]
        public bool IsBestSeller { get; set; }

        [Column("is_black_week")]
        public bool IsBlackWeek { get; set; }

        [Column("is_flash_sale")]
        public bool IsFlashSale { get; set; }

        [Column("is_app_exclusive")]
        public bool IsAppExclusive { get; set; }

        [Column("collection")]
        public string? Collection { get; set; }

        [Column("avg_rating")]
        public decimal AvgRating { get; set; }

        [Column("review_count")]
        public int ReviewCount { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }


        // Convierte este DTO al modelo Product que usan los ViewModels.
        // Recibe las imágenes, tallas (con stock) y colores que ya hayan sido cargadas
        // desde sus respectivas tablas, para juntarlas en un solo objeto.
        public Product ToProduct(
            IEnumerable<ProductImageDto>? images = null,
            IEnumerable<ProductSizeDto>? sizes = null,
            IEnumerable<ProductColorDto>? colors = null)
        {
            var imageList = images?
                .Where(i => i.ProductId == Id)
                .OrderBy(i => i.SortOrder)
                .ToList()
                ?? new List<ProductImageDto>();

            var mainImage = imageList.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
                ?? imageList.FirstOrDefault()?.ImageUrl
                ?? string.Empty;

            // Tallas y stock por talla del producto.
            var productSizes = sizes?
                .Where(s => s.ProductId == Id)
                .ToList()
                ?? new List<ProductSizeDto>();

            // Construye el diccionario talla → stock. Permite saber si una talla concreta
            // está agotada sin tener que recargar la BBDD.
            var stockBySize = productSizes
                .GroupBy(s => s.Size)
                .ToDictionary(g => g.Key, g => g.First().Stock);

            return new Product
            {
                Id = Id,
                Name = Name,
                Description = ShortDescription ?? string.Empty,
                LongDescription = Description ?? string.Empty,
                Price = Price,
                OriginalPrice = OriginalPrice,
                Stock = Stock,
                MainImageUrl = mainImage,
                ImageUrls = imageList.Select(i => i.ImageUrl).ToList(),
                CategoryId = CategoryId,
                AverageRating = (double)AvgRating,
                ReviewCount = ReviewCount,
                IsFeatured = IsFeatured,
                IsNew = IsNew,
                IsBlackWeek = IsBlackWeek,
                IsSoftsCollection = Collection == "softs",
                IsClassicCollection = Collection == "classic",
                Gender = string.IsNullOrEmpty(Gender)
                    ? "Unisex"
                    : char.ToUpper(Gender[0]) + Gender.Substring(1),
                AvailableSizes = productSizes
                    .Select(s => s.Size)
                    .ToList(),
                AvailableColors = colors?
                    .Where(c => c.ProductId == Id)
                    .Select(c => c.ColorName)
                    .ToList()
                    ?? new List<string>(),
                StockBySize = stockBySize,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt
            };
        }
    }
}