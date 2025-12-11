using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloCrew.Repositories
{
    /// <summary>
    /// Implementación del repositorio de productos
    /// ⭐ CORREGIDO: Categorías unificadas con ProductService (Streetwear)
    /// - CategoryId 1: Tops
    /// - CategoryId 2: Bottoms
    /// - CategoryId 3: Footwear
    /// - CategoryId 4: Accessories
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private static List<Product> _products;
        private static int _nextId = 1;

        public ProductRepository()
        {
            // Inicializar datos mock solo una vez
            if (_products == null)
            {
                InitializeMockData();
            }
        }

        public Task<List<Product>> GetAllAsync()
        {
            return Task.FromResult(_products.ToList());
        }

        public Task<Product> GetByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<List<Product>> GetByCategoryAsync(int categoryId)
        {
            var products = _products
                .Where(p => p.CategoryId == categoryId)
                .ToList();
            return Task.FromResult(products);
        }

        public Task<List<Product>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Task.FromResult(new List<Product>());
            }

            var lowerQuery = query.ToLower();
            var results = _products
                .Where(p => p.Name.ToLower().Contains(lowerQuery) ||
                           (p.Description != null && p.Description.ToLower().Contains(lowerQuery)))
                .ToList();

            return Task.FromResult(results);
        }

        public Task<Product> CreateAsync(Product product)
        {
            product.Id = _nextId++;
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            _products.Add(product);
            return Task.FromResult(product);
        }

        public Task<Product> UpdateAsync(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing != null)
            {
                var index = _products.IndexOf(existing);
                product.UpdatedAt = DateTime.Now;
                _products[index] = product;
                return Task.FromResult(product);
            }
            return Task.FromResult<Product>(null);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<List<Product>> GetFeaturedAsync()
        {
            var featured = _products
                .Where(p => p.IsFeatured)
                .ToList();
            return Task.FromResult(featured);
        }

        public Task<List<Product>> GetNewProductsAsync()
        {
            var newProducts = _products
                .Where(p => p.IsNew)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
            return Task.FromResult(newProducts);
        }

        /// <summary>
        /// Inicializa datos mock con las mismas categorías que ProductService
        /// ⭐ CORREGIDO: Categorías unificadas (Tops, Bottoms, Footwear, Accessories)
        /// </summary>
        private void InitializeMockData()
        {
            _products = new List<Product>
            {
                // ============================================
                // TOPS (CategoryId = 1)
                // ============================================
                new Product
                {
                    Id = _nextId++,
                    Name = "PREMIUM LOGO TEE",
                    Description = "Essential cotton t-shirt with embroidered logo",
                    LongDescription = "100% premium cotton t-shirt. Relaxed fit, ribbed crew neck. Available in multiple colors.",
                    Price = 34.99m,
                    OriginalPrice = 44.99m,
                    Stock = 150,
                    MainImageUrl = "/Resources/Images/Products/tee1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/tee1.jpg", "/Resources/Images/Products/tee1-2.jpg" },
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Tops" },
                    AverageRating = 4.5,
                    ReviewCount = 234,
                    IsFeatured = true,
                    IsNew = false,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddMonths(-3),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "OVERSIZED HOODIE",
                    Description = "Premium heavyweight hoodie",
                    LongDescription = "450GSM cotton blend hoodie with oversized fit. Kangaroo pocket, adjustable drawstring hood.",
                    Price = 79.99m,
                    Stock = 85,
                    MainImageUrl = "/Resources/Images/Products/hoodie1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/hoodie1.jpg" },
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Tops" },
                    AverageRating = 4.8,
                    ReviewCount = 312,
                    IsFeatured = true,
                    IsNew = true,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddDays(-15),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "CREWNECK SWEATSHIRT",
                    Description = "Classic crew neck sweatshirt",
                    LongDescription = "Soft fleece interior, ribbed cuffs and hem. Perfect layering piece.",
                    Price = 64.99m,
                    OriginalPrice = 79.99m,
                    Stock = 95,
                    MainImageUrl = "/Resources/Images/Products/crew1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/crew1.jpg" },
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Tops" },
                    AverageRating = 4.6,
                    ReviewCount = 178,
                    IsFeatured = false,
                    IsNew = false,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddMonths(-2),
                    UpdatedAt = DateTime.Now
                },

                // ============================================
                // BOTTOMS (CategoryId = 2)
                // ============================================
                new Product
                {
                    Id = _nextId++,
                    Name = "TACTICAL CARGO PANTS",
                    Description = "Military-inspired cargo pants",
                    LongDescription = "Durable ripstop fabric, multiple utility pockets, adjustable waist. Perfect for urban exploration.",
                    Price = 89.99m,
                    Stock = 65,
                    MainImageUrl = "/Resources/Images/Products/cargo1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/cargo1.jpg" },
                    CategoryId = 2,
                    Category = new Category { Id = 2, Name = "Bottoms" },
                    AverageRating = 4.7,
                    ReviewCount = 267,
                    IsFeatured = true,
                    IsNew = false,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddMonths(-1),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "SLIM FIT DENIM",
                    Description = "Classic black slim fit jeans",
                    LongDescription = "Stretch denim for comfort. Slim through hip and thigh, narrow leg opening.",
                    Price = 69.99m,
                    Stock = 100,
                    MainImageUrl = "/Resources/Images/Products/jeans1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/jeans1.jpg" },
                    CategoryId = 2,
                    Category = new Category { Id = 2, Name = "Bottoms" },
                    AverageRating = 4.5,
                    ReviewCount = 189,
                    IsFeatured = false,
                    IsNew = true,
                    Gender = "Men",
                    CreatedAt = DateTime.Now.AddDays(-10),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "TECH JOGGERS",
                    Description = "Performance jogger pants",
                    LongDescription = "Moisture-wicking fabric, zippered pockets, elastic cuffs. From gym to street.",
                    Price = 59.99m,
                    OriginalPrice = 74.99m,
                    Stock = 120,
                    MainImageUrl = "/Resources/Images/Products/jogger1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/jogger1.jpg" },
                    CategoryId = 2,
                    Category = new Category { Id = 2, Name = "Bottoms" },
                    AverageRating = 4.6,
                    ReviewCount = 223,
                    IsFeatured = true,
                    IsNew = false,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddMonths(-2),
                    UpdatedAt = DateTime.Now
                },

                // ============================================
                // FOOTWEAR (CategoryId = 3)
                // ============================================
                new Product
                {
                    Id = _nextId++,
                    Name = "ARMBO LOW WHITE",
                    Description = "Clean minimal leather sneakers",
                    LongDescription = "Premium full-grain leather, cushioned insole, durable rubber outsole. Timeless design.",
                    Price = 129.99m,
                    Stock = 55,
                    MainImageUrl = "/Resources/Images/Products/sneaker1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/sneaker1.jpg" },
                    CategoryId = 3,
                    Category = new Category { Id = 3, Name = "Footwear" },
                    AverageRating = 4.8,
                    ReviewCount = 345,
                    IsFeatured = true,
                    IsNew = false,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddMonths(-4),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "HIGH-TOP CANVAS",
                    Description = "Classic canvas high-tops",
                    LongDescription = "Durable canvas upper, vulcanized rubber sole, metal eyelets. Street style essential.",
                    Price = 79.99m,
                    OriginalPrice = 99.99m,
                    Stock = 80,
                    MainImageUrl = "/Resources/Images/Products/hightop1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/hightop1.jpg" },
                    CategoryId = 3,
                    Category = new Category { Id = 3, Name = "Footwear" },
                    AverageRating = 4.6,
                    ReviewCount = 198,
                    IsFeatured = false,
                    IsNew = true,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddDays(-7),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "CHUNKY RUNNER",
                    Description = "Retro-inspired chunky sneakers",
                    LongDescription = "Multi-layer foam sole, mesh and suede upper, maximum cushioning. Dad shoe vibes.",
                    Price = 149.99m,
                    Stock = 40,
                    MainImageUrl = "/Resources/Images/Products/chunky1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/chunky1.jpg" },
                    CategoryId = 3,
                    Category = new Category { Id = 3, Name = "Footwear" },
                    AverageRating = 4.5,
                    ReviewCount = 156,
                    IsFeatured = true,
                    IsNew = false,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddMonths(-2),
                    UpdatedAt = DateTime.Now
                },

                // ============================================
                // ACCESSORIES (CategoryId = 4)
                // ============================================
                new Product
                {
                    Id = _nextId++,
                    Name = "LOGO BASEBALL CAP",
                    Description = "Classic 6-panel cap",
                    LongDescription = "Adjustable strap, embroidered logo, curved brim. One size fits most.",
                    Price = 29.99m,
                    Stock = 200,
                    MainImageUrl = "/Resources/Images/Products/cap1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/cap1.jpg" },
                    CategoryId = 4,
                    Category = new Category { Id = 4, Name = "Accessories" },
                    AverageRating = 4.7,
                    ReviewCount = 456,
                    IsFeatured = true,
                    IsNew = false,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddMonths(-5),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "CROSSBODY BAG",
                    Description = "Compact everyday bag",
                    LongDescription = "Water-resistant nylon, adjustable strap, multiple compartments. Perfect for essentials.",
                    Price = 49.99m,
                    Stock = 90,
                    MainImageUrl = "/Resources/Images/Products/bag1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/bag1.jpg" },
                    CategoryId = 4,
                    Category = new Category { Id = 4, Name = "Accessories" },
                    AverageRating = 4.6,
                    ReviewCount = 234,
                    IsFeatured = false,
                    IsNew = true,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddDays(-5),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "RIBBED BEANIE",
                    Description = "Classic knit beanie",
                    LongDescription = "Soft acrylic blend, ribbed texture, fold-over cuff. Warm and stylish.",
                    Price = 24.99m,
                    Stock = 180,
                    MainImageUrl = "/Resources/Images/Products/beanie1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/beanie1.jpg" },
                    CategoryId = 4,
                    Category = new Category { Id = 4, Name = "Accessories" },
                    AverageRating = 4.6,
                    ReviewCount = 478,
                    IsFeatured = false,
                    IsNew = false,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddMonths(-6),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "LEATHER CARDHOLDER",
                    Description = "Minimalist wallet",
                    LongDescription = "Genuine leather, 6 card slots, slim profile. Fits front pocket perfectly.",
                    Price = 39.99m,
                    Stock = 100,
                    MainImageUrl = "/Resources/Images/Products/wallet1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/wallet1.jpg" },
                    CategoryId = 4,
                    Category = new Category { Id = 4, Name = "Accessories" },
                    AverageRating = 4.7,
                    ReviewCount = 298,
                    IsFeatured = true,
                    IsNew = false,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddMonths(-3),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "CANVAS BELT",
                    Description = "Military-style web belt",
                    LongDescription = "Durable canvas webbing, metal D-ring buckle, adjustable length. Versatile accessory.",
                    Price = 24.99m,
                    OriginalPrice = 34.99m,
                    Stock = 150,
                    MainImageUrl = "/Resources/Images/Products/belt1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/belt1.jpg" },
                    CategoryId = 4,
                    Category = new Category { Id = 4, Name = "Accessories" },
                    AverageRating = 4.5,
                    ReviewCount = 167,
                    IsFeatured = false,
                    IsNew = false,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddMonths(-4),
                    UpdatedAt = DateTime.Now
                }
            };
        }
    }
}