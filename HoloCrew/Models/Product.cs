using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace HoloCrew.Models
{
    /// <summary>
    /// Modelo de producto con soporte para subcategorías
    /// </summary>
    public partial class Product : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int Stock { get; set; }
        public string MainImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();

        // Categorías
        public int CategoryId { get; set; }           // ID de categoría principal (1-5)
        public int SubCategoryId { get; set; }        // ID de subcategoría (10-59)
        public string SubCategorySlug { get; set; }   // Slug para filtrado rápido
        public Category Category { get; set; }

        // Reviews y rating
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        // Flags
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }
        public bool IsBlackWeek { get; set; }         // Nuevo: para ofertas Black Week
        public bool IsSoftsCollection { get; set; }   // Nuevo: para colección Softs
        public bool IsClassicCollection { get; set; } // Nuevo: para colección Classic

        // Variantes
        public string Gender { get; set; } // "Men", "Women", "Unisex"
        public List<string> AvailableSizes { get; set; } = new List<string> { "S", "M", "L", "XL" };
        public List<string> AvailableColors { get; set; } = new List<string> { "Black", "White" };
        public string Color { get; set; } // Color principal

        // Fechas
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Propiedades calculadas
        public bool HasDiscount => OriginalPrice.HasValue && OriginalPrice > Price;
        public decimal DiscountPercentage => HasDiscount
            ? Math.Round(((OriginalPrice.Value - Price) / OriginalPrice.Value) * 100, 0)
            : 0;
        public bool IsInStock => Stock > 0;
        public bool IsLowStock => Stock > 0 && Stock <= 10;

        // Estado de wishlist para UI reactiva
        [ObservableProperty]
        private bool _isInWishlist;
    }
}