using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Repositorio de productos con datos falsos en memoria (mock).
// Las categorías son: 1=Tops, 2=Bottoms, 3=Footwear, 4=Accessories.
// Los datos de ejemplo están en InitializeMockData().

namespace HoloCrew.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private static List<Product> _products;
        private static int _nextId = 1;

        public ProductRepository()
        {
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
            var products = _products.Where(p => p.CategoryId == categoryId).ToList();
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
            var featured = _products.Where(p => p.IsFeatured).ToList();
            return Task.FromResult(featured);
        }

        public Task<List<Product>> GetNewProductsAsync()
        {
            var newProducts = _products.Where(p => p.IsNew).OrderByDescending(p => p.CreatedAt).ToList();
            return Task.FromResult(newProducts);
        }

        // productos de ejemplo para probar sin base de datos real
        private void InitializeMockData()
        {
            _products = new List<Product>
            {
                // ========== TOPS (CategoryId = 1) ==========
                new Product
                {
                    Id = _nextId++,
                    Name = "PREMIUM LOGO TEE",
                    Description = "Camiseta básica de algodón con logo bordado",
                    LongDescription = "Camiseta 100% algodón. Corte relajado, cuello redondo acanalado.",
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
                    Description = "Sudadera con capucha oversized",
                    LongDescription = "Mezcla de algodón 450GSM con corte oversize.",
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
                    Description = "Sudadera clásica con cuello redondo",
                    LongDescription = "Interior de felpa suave, puños y dobladillo acanalados.",
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

                // ========== BOTTOMS (CategoryId = 2) ==========
                new Product
                {
                    Id = _nextId++,
                    Name = "TACTICAL CARGO PANTS",
                    Description = "Pantalón cargo estilo militar",
                    LongDescription = "Tela ripstop duradera, múltiples bolsillos.",
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
                    Description = "Vaqueros negros ajustados clásicos",
                    LongDescription = "Denim elástico para mayor comodidad.",
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
                    Description = "Joggers de alto rendimiento",
                    LongDescription = "Tejido que absorbe la humedad, bolsillos con cremallera.",
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

                // ========== FOOTWEAR (CategoryId = 3) ==========
                new Product
                {
                    Id = _nextId++,
                    Name = "ARMBO LOW WHITE",
                    Description = "Zapatillas de cuero minimalistas",
                    LongDescription = "Cuero de primera calidad, plantilla acolchada.",
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
                    Description = "Caña alta de lona clásicas",
                    LongDescription = "Parte superior de lona duradera, suela de goma vulcanizada.",
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
                    Description = "Zapatillas Chunky retro",
                    LongDescription = "Suela de espuma multicapa, parte superior de malla y ante.",
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

                // ========== ACCESSORIES (CategoryId = 4) ==========
                new Product
                {
                    Id = _nextId++,
                    Name = "LOGO BASEBALL CAP",
                    Description = "Gorra clásica de 6 paneles",
                    LongDescription = "Correa ajustable, logo bordado, visera curva.",
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
                    Description = "Bolso bandolera compacto",
                    LongDescription = "Nailon resistente al agua, correa ajustable.",
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
                    Description = "Gorro de punto acanalado clásico",
                    LongDescription = "Mezcla suave de acrílico, textura acanalada.",
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
                    Description = "Tarjetero minimalista",
                    LongDescription = "Cuero genuino, 6 ranuras para tarjetas.",
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
                    Description = "Cinturón de lona estilo militar",
                    LongDescription = "Tejido de lona duradero, hebilla de metal.",
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