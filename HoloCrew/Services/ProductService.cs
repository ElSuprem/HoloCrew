using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;

namespace HoloCrew.Services
{
    /// <summary>
    /// Implementación del servicio de productos CON DATOS DE PRUEBA
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetFeaturedProductsAsync()
        {
            // Intentar obtener del repositorio
            var allProducts = await _productRepository.GetAllAsync();

            // Si no hay productos, devolver datos de prueba
            if (allProducts == null || !allProducts.Any())
            {
                return GetMockProducts().Where(p => p.IsFeatured).ToList();
            }

            return allProducts.Where(p => p.IsFeatured).ToList();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            if (categoryId == 0)
            {
                // 0 = todas las categorías
                var allProducts = await _productRepository.GetAllAsync();

                // Si no hay productos, devolver datos de prueba
                if (allProducts == null || !allProducts.Any())
                {
                    return GetMockProducts();
                }

                return allProducts;
            }

            var products = await _productRepository.GetByCategoryAsync(categoryId);

            // Si no hay productos, devolver datos de prueba filtrados
            if (products == null || !products.Any())
            {
                return GetMockProducts().Where(p => p.CategoryId == categoryId).ToList();
            }

            return products;
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            // Si no existe, buscar en mock
            if (product == null)
            {
                return GetMockProducts().FirstOrDefault(p => p.Id == productId);
            }

            return product;
        }

        public async Task<List<Product>> SearchProductsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<Product>();
            }

            var results = await _productRepository.SearchAsync(query);

            // Si no hay resultados, buscar en mock
            if (results == null || !results.Any())
            {
                return GetMockProducts()
                    .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               p.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return results;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await Task.FromResult(new List<Category>
            {
                new Category { Id = 1, Name = "Tops", IconUrl = "/Resources/Icons/tops.png" },
                new Category { Id = 2, Name = "Bottoms", IconUrl = "/Resources/Icons/bottoms.png" },
                new Category { Id = 3, Name = "Accessories", IconUrl = "/Resources/Icons/accessories.png" },
                new Category { Id = 4, Name = "Footwear", IconUrl = "/Resources/Icons/footwear.png" }
            });
        }

        public async Task<List<Product>> GetRelatedProductsAsync(int productId)
        {
            var product = await GetProductByIdAsync(productId);

            if (product == null)
            {
                return new List<Product>();
            }

            // Obtener productos de la misma categoría
            var relatedProducts = await GetProductsByCategoryAsync(product.CategoryId);

            // Excluir el producto actual y tomar solo 4
            return relatedProducts
                .Where(p => p.Id != productId)
                .Take(4)
                .ToList();
        }

        public async Task<List<Product>> GetBestSellersAsync()
        {
            var allProducts = await GetProductsByCategoryAsync(0);

            return allProducts
                .OrderByDescending(p => p.ReviewCount)
                .Take(10)
                .ToList();
        }

        public async Task<List<Product>> GetNewProductsAsync()
        {
            var allProducts = await GetProductsByCategoryAsync(0);

            return allProducts
                .Where(p => p.IsNew)
                .OrderByDescending(p => p.CreatedAt)
                .Take(10)
                .ToList();
        }

        // ⭐ DATOS DE PRUEBA
        private List<Product> GetMockProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "OVERSIZED HOODIE",
                    Description = "Premium Cotton Blend",
                    LongDescription = "Ultra-comfortable oversized hoodie made from premium cotton blend",
                    Price = 59.99m,
                    OriginalPrice = 79.99m,
                    Stock = 50,
                    MainImageUrl = "/Resources/Images/hoodie.jpg",
                    ImageUrls = new List<string>(),
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Tops" },
                    AverageRating = 4.5,
                    ReviewCount = 128,
                    IsFeatured = true,
                    IsNew = true,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddDays(-10),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 2,
                    Name = "CARGO PANTS",
                    Description = "Tactical Style",
                    LongDescription = "Modern cargo pants with multiple pockets",
                    Price = 79.99m,
                    Stock = 30,
                    MainImageUrl = "/Resources/Images/cargo.jpg",
                    ImageUrls = new List<string>(),
                    CategoryId = 2,
                    Category = new Category { Id = 2, Name = "Bottoms" },
                    AverageRating = 4.7,
                    ReviewCount = 95,
                    IsFeatured = true,
                    IsNew = false,
                    Gender = "Men",
                    CreatedAt = DateTime.Now.AddDays(-30),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 3,
                    Name = "GRAPHIC TEE",
                    Description = "Limited Edition Print",
                    LongDescription = "Exclusive graphic t-shirt with limited edition design",
                    Price = 39.99m,
                    OriginalPrice = 49.99m,
                    Stock = 100,
                    MainImageUrl = "/Resources/Images/tee.jpg",
                    ImageUrls = new List<string>(),
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Tops" },
                    AverageRating = 4.3,
                    ReviewCount = 67,
                    IsFeatured = true,
                    IsNew = true,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddDays(-5),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 4,
                    Name = "BOMBER JACKET",
                    Description = "Classic Streetwear",
                    LongDescription = "Timeless bomber jacket perfect for any season",
                    Price = 129.99m,
                    Stock = 25,
                    MainImageUrl = "/Resources/Images/bomber.jpg",
                    ImageUrls = new List<string>(),
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Tops" },
                    AverageRating = 4.8,
                    ReviewCount = 143,
                    IsFeatured = true,
                    IsNew = false,
                    Gender = "Men",
                    CreatedAt = DateTime.Now.AddDays(-60),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 5,
                    Name = "SNEAKERS",
                    Description = "Limited Release",
                    LongDescription = "Exclusive sneakers with premium materials",
                    Price = 149.99m,
                    OriginalPrice = 199.99m,
                    Stock = 15,
                    MainImageUrl = "/Resources/Images/sneakers.jpg",
                    ImageUrls = new List<string>(),
                    CategoryId = 4,
                    Category = new Category { Id = 4, Name = "Footwear" },
                    AverageRating = 4.9,
                    ReviewCount = 201,
                    IsFeatured = true,
                    IsNew = true,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddDays(-3),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 6,
                    Name = "CAP",
                    Description = "Embroidered Logo",
                    LongDescription = "Classic cap with embroidered brand logo",
                    Price = 29.99m,
                    Stock = 80,
                    MainImageUrl = "/Resources/Images/cap.jpg",
                    ImageUrls = new List<string>(),
                    CategoryId = 3,
                    Category = new Category { Id = 3, Name = "Accessories" },
                    AverageRating = 4.4,
                    ReviewCount = 54,
                    IsFeatured = false,
                    IsNew = false,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddDays(-90),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 7,
                    Name = "JOGGERS",
                    Description = "Comfort Fit",
                    LongDescription = "Ultra-comfortable joggers perfect for lounging",
                    Price = 69.99m,
                    Stock = 45,
                    MainImageUrl = "/Resources/Images/joggers.jpg",
                    ImageUrls = new List<string>(),
                    CategoryId = 2,
                    Category = new Category { Id = 2, Name = "Bottoms" },
                    AverageRating = 4.6,
                    ReviewCount = 112,
                    IsFeatured = true,
                    IsNew = false,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddDays(-45),
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 8,
                    Name = "BACKPACK",
                    Description = "Water Resistant",
                    LongDescription = "Durable backpack with water-resistant coating",
                    Price = 89.99m,
                    OriginalPrice = 109.99m,
                    Stock = 20,
                    MainImageUrl = "/Resources/Images/backpack.jpg",
                    ImageUrls = new List<string>(),
                    CategoryId = 3,
                    Category = new Category { Id = 3, Name = "Accessories" },
                    AverageRating = 4.7,
                    ReviewCount = 89,
                    IsFeatured = false,
                    IsNew = true,
                    Gender = "Unisex",
                    CreatedAt = DateTime.Now.AddDays(-7),
                    UpdatedAt = DateTime.Now
                }
            };
        }
    }
}