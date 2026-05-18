using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

// Producto de la tienda con toda su información: nombre, precio, stock, imágenes,
// categorías, puntuación, si tiene descuento, si es nuevo, tallas disponibles, etc.
// El stock se guarda en dos niveles:
//   - Stock global (columna stock de la tabla products) → suma total.
//   - StockBySize (Dictionary<string, int>) → stock por talla individual, viene
//     de la tabla product_sizes. Esto es lo que permite marcar tallas concretas
//     como "Sold Out".

namespace HoloCrew.Models
{
    public partial class Product : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LongDescription { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
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
        public List<string> AvailableSizes { get; set; } = new List<string>();
        public List<string> AvailableColors { get; set; } = new List<string>();
        public string Color { get; set; } = string.Empty;

        // Stock detallado por talla. Clave = talla ("XS", "S", "M", ...), valor = unidades disponibles.
        // Se rellena desde la tabla product_sizes. Permite marcar tallas concretas como Sold Out.
        public Dictionary<string, int> StockBySize { get; set; } = new Dictionary<string, int>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // descuento
        public bool HasDiscount => OriginalPrice.HasValue && OriginalPrice > Price;
        public decimal DiscountPercentage => HasDiscount
            ? Math.Round(((OriginalPrice!.Value - Price) / OriginalPrice.Value) * 100, 0)
            : 0;
        public decimal Savings => HasDiscount ? OriginalPrice!.Value - Price : 0;
        public bool IsInStock => Stock > 0;
        public bool IsLowStock => Stock > 0 && Stock <= 10;

        // True si todas las tallas tienen stock 0 (producto completamente agotado).
        // Se usa para mostrar el badge SOLD OUT en el catálogo / home.
        // Si no hay info de stock por talla, cae al check de Stock global.
        public bool IsCompletelyOutOfStock
        {
            get
            {
                if (StockBySize != null && StockBySize.Any())
                    return StockBySize.Values.All(stock => stock <= 0);
                return Stock <= 0;
            }
        }

        // Devuelve true si la talla indicada tiene stock disponible.
        public bool HasStockForSize(string size)
        {
            if (StockBySize == null || !StockBySize.ContainsKey(size))
                return Stock > 0; // fallback al stock global si no hay info por talla
            return StockBySize[size] > 0;
        }

        // wishlist
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
                    OnPropertyChanged(nameof(IsCompletelyOutOfStock));
                }
            }
        }
    }
}