namespace HoloCrew.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int Stock { get; set; }
        public string MainImageUrl { get; set; }
        public List<string> ImageUrls { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Propiedades calculadas
        public bool HasDiscount => OriginalPrice.HasValue && OriginalPrice > Price;
        public decimal DiscountPercentage => HasDiscount
            ? Math.Round(((OriginalPrice.Value - Price) / OriginalPrice.Value) * 100, 0)
            : 0;
        public bool IsInStock => Stock > 0;
    }
}