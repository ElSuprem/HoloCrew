using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

// Producto de la tienda con toda su información: nombre, precio, stock, imágenes,
// categorías, puntuación, si tiene descuento, si es nuevo, tallas disponibles, etc.

namespace HoloCrew.Models
{
    public partial class Product : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LongDescription { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }   // si tiene descuento, aquí está el precio antes
        public int Stock { get; set; }
        public string MainImageUrl { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new List<string>();

        // categorías
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategorySlug { get; set; } = string.Empty;
        public Category? Category { get; set; }

        // reseñas
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        // etiquetas especiales
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }
        public bool IsBlackWeek { get; set; }
        public bool IsSoftsCollection { get; set; }
        public bool IsClassicCollection { get; set; }

        // variantes
        public string Gender { get; set; } = "Unisex";
        public List<string> AvailableSizes { get; set; } = new List<string> { "S", "M", "L", "XL" };
        public List<string> AvailableColors { get; set; } = new List<string> { "Black", "White" };
        public string Color { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // si tiene descuento y cuánto es
        public bool HasDiscount => OriginalPrice.HasValue && OriginalPrice > Price;
        public decimal DiscountPercentage => HasDiscount
            ? Math.Round(((OriginalPrice!.Value - Price) / OriginalPrice.Value) * 100, 0)
            : 0;
        public decimal Savings => HasDiscount ? OriginalPrice!.Value - Price : 0;

        public bool IsInStock => Stock > 0;
        public bool IsLowStock => Stock > 0 && Stock <= 10;

        // si el producto está en la lista de deseos del usuario
        private bool _isInWishlist;
        public bool IsInWishlist
        {
            get => _isInWishlist;
            set
            {
                if (_isInWishlist != value)
                {
                    _isInWishlist = value;
                    OnPropertyChanged(nameof(IsInWishlist));
                }
            }
        }
    }
}