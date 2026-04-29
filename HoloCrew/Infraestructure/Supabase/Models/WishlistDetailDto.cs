using HoloCrew.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;

// DTO que mapea la vista 'wishlist_with_details' de Supabase.
// La vista hace JOIN con products y devuelve los datos del producto en una sola query.
// Se usa solo para listar la wishlist; las inserciones y borrados van por la RPC toggle_wishlist.

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("wishlist_with_details")]
    public class WishlistDetailDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("added_at")]
        public DateTime AddedAt { get; set; }

        [Column("notify_on_sale")]
        public bool NotifyOnSale { get; set; }

        [Column("price_at_add")]
        public decimal? PriceAtAdd { get; set; }

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

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("avg_rating")]
        public decimal AvgRating { get; set; }

        [Column("review_count")]
        public int ReviewCount { get; set; }

        [Column("main_image_url")]
        public string? MainImageUrl { get; set; }

        [Column("total_stock")]
        public int TotalStock { get; set; }

        [Column("price_drop_amount")]
        public decimal? PriceDropAmount { get; set; }


        // Convierte este DTO al modelo Product que usan los ViewModels.
        // Como la vista ya nos da los datos enrichidos, no hacen falta más queries.
        public Product ToProduct()
        {
            return new Product
            {
                Id = ProductId,
                Name = ProductName,
                Description = string.Empty,
                LongDescription = string.Empty,
                Price = CurrentPrice,
                OriginalPrice = OriginalPrice,
                Stock = TotalStock,
                MainImageUrl = MainImageUrl ?? string.Empty,
                ImageUrls = new List<string>(),
                CategoryId = CategoryId,
                AverageRating = (double)AvgRating,
                ReviewCount = ReviewCount,
                AvailableSizes = new List<string>(),
                AvailableColors = new List<string>(),
                IsInWishlist = true,
                CreatedAt = AddedAt,
                UpdatedAt = AddedAt
            };
        }
    }
}