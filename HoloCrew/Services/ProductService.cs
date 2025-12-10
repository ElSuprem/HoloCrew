using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;

namespace HoloCrew.Services
{
    /// <summary>
    /// Servicio de productos con CATÁLOGO COMPLETO DE STREETWEAR
    /// 70 productos realistas adaptados a HoloCrew
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
            var allProducts = await _productRepository.GetAllAsync();

            if (allProducts == null || !allProducts.Any())
            {
                return GetStreetweaCatalog().Where(p => p.IsFeatured).ToList();
            }

            return allProducts.Where(p => p.IsFeatured).ToList();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            if (categoryId == 0)
            {
                var allProducts = await _productRepository.GetAllAsync();

                if (allProducts == null || !allProducts.Any())
                {
                    return GetStreetweaCatalog();
                }

                return allProducts;
            }

            var products = await _productRepository.GetByCategoryAsync(categoryId);

            if (products == null || !products.Any())
            {
                return GetStreetweaCatalog().Where(p => p.CategoryId == categoryId).ToList();
            }

            return products;
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                return GetStreetweaCatalog().FirstOrDefault(p => p.Id == productId);
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

            if (results == null || !results.Any())
            {
                return GetStreetweaCatalog()
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
                new Category { Id = 3, Name = "Footwear", IconUrl = "/Resources/Icons/footwear.png" },
                new Category { Id = 4, Name = "Accessories", IconUrl = "/Resources/Icons/accessories.png" }
            });
        }

        public async Task<List<Product>> GetRelatedProductsAsync(int productId)
        {
            var product = await GetProductByIdAsync(productId);

            if (product == null)
            {
                return new List<Product>();
            }

            var relatedProducts = await GetProductsByCategoryAsync(product.CategoryId);

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

        // ⭐⭐⭐ CATÁLOGO COMPLETO DE STREETWEAR ⭐⭐⭐
        private List<Product> GetStreetweaCatalog()
        {
            var products = new List<Product>();
            int id = 1;

            // ============================================
            // TOPS - 20 PRODUCTOS
            // ============================================

            // T-SHIRTS (5)
            products.Add(new Product
            {
                Id = id++,
                Name = "CLASSIC LOGO TEE",
                Description = "Essential cotton t-shirt",
                LongDescription = "100% premium cotton t-shirt with embroidered logo",
                Price = 34.99m,
                OriginalPrice = 44.99m,
                Stock = 150,
                MainImageUrl = "/Resources/Images/tshirt1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.5,
                ReviewCount = 234,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-60),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "VINTAGE WASH TEE",
                Description = "Relaxed fit vintage style",
                LongDescription = "Oversized t-shirt with vintage wash treatment",
                Price = 39.99m,
                Stock = 120,
                MainImageUrl = "/Resources/Images/tshirt2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.7,
                ReviewCount = 189,
                IsFeatured = true,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-5),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "GRAPHIC PRINT TEE",
                Description = "Bold artwork design",
                LongDescription = "Statement tee with exclusive graphic print",
                Price = 44.99m,
                OriginalPrice = 59.99m,
                Stock = 90,
                MainImageUrl = "/Resources/Images/tshirt3.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.6,
                ReviewCount = 156,
                IsFeatured = false,
                IsNew = false,
                Gender = "Men",
                CreatedAt = DateTime.Now.AddDays(-30),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "STRIPED LONG SLEEVE",
                Description = "Classic stripe pattern",
                LongDescription = "Long sleeve cotton tee with horizontal stripes",
                Price = 49.99m,
                Stock = 85,
                MainImageUrl = "/Resources/Images/tshirt4.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.4,
                ReviewCount = 98,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-45),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "POCKET TEE PACK",
                Description = "3-pack basic tees",
                LongDescription = "Essential pocket tees in neutral colors",
                Price = 69.99m,
                OriginalPrice = 89.99m,
                Stock = 200,
                MainImageUrl = "/Resources/Images/tshirt5.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.8,
                ReviewCount = 312,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-90),
                UpdatedAt = DateTime.Now
            });

            // HOODIES (5)
            products.Add(new Product
            {
                Id = id++,
                Name = "OVERSIZED HOODIE BLACK",
                Description = "Premium heavyweight cotton",
                LongDescription = "Ultra-comfortable oversized hoodie in premium cotton blend",
                Price = 79.99m,
                OriginalPrice = 99.99m,
                Stock = 75,
                MainImageUrl = "/Resources/Images/hoodie1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.9,
                ReviewCount = 456,
                IsFeatured = true,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-3),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "ZIP-UP HOODIE GRAY",
                Description = "Full zip with pockets",
                LongDescription = "Classic zip-up hoodie with kangaroo pockets",
                Price = 74.99m,
                Stock = 95,
                MainImageUrl = "/Resources/Images/hoodie2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.6,
                ReviewCount = 278,
                IsFeatured = true,
                IsNew = false,
                Gender = "Men",
                CreatedAt = DateTime.Now.AddDays(-25),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "EMBROIDERED LOGO HOODIE",
                Description = "Premium embroidery detail",
                LongDescription = "Luxury hoodie with embroidered chest logo",
                Price = 89.99m,
                Stock = 60,
                MainImageUrl = "/Resources/Images/hoodie3.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.7,
                ReviewCount = 201,
                IsFeatured = false,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-7),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "COLOR BLOCK HOODIE",
                Description = "Bold color combination",
                LongDescription = "Statement hoodie with contrasting color panels",
                Price = 84.99m,
                OriginalPrice = 109.99m,
                Stock = 55,
                MainImageUrl = "/Resources/Images/hoodie4.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.5,
                ReviewCount = 167,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-40),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "CROPPED HOODIE",
                Description = "Modern cropped fit",
                LongDescription = "Trendy cropped hoodie with raw hem detail",
                Price = 69.99m,
                Stock = 70,
                MainImageUrl = "/Resources/Images/hoodie5.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.4,
                ReviewCount = 134,
                IsFeatured = false,
                IsNew = true,
                Gender = "Women",
                CreatedAt = DateTime.Now.AddDays(-10),
                UpdatedAt = DateTime.Now
            });

            // TRACK JACKETS (3)
            products.Add(new Product
            {
                Id = id++,
                Name = "RETRO TRACK JACKET",
                Description = "Classic 90s inspired",
                LongDescription = "Vintage-style track jacket with contrast stripes",
                Price = 94.99m,
                Stock = 50,
                MainImageUrl = "/Resources/Images/track1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.6,
                ReviewCount = 145,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-35),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "TRICOT TRACK TOP",
                Description = "Smooth tricot fabric",
                LongDescription = "Premium tricot track jacket with side pockets",
                Price = 89.99m,
                OriginalPrice = 119.99m,
                Stock = 45,
                MainImageUrl = "/Resources/Images/track2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.7,
                ReviewCount = 189,
                IsFeatured = false,
                IsNew = false,
                Gender = "Men",
                CreatedAt = DateTime.Now.AddDays(-50),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "NYLON TRACK JACKET",
                Description = "Lightweight windbreaker",
                LongDescription = "Water-resistant nylon track jacket",
                Price = 79.99m,
                Stock = 65,
                MainImageUrl = "/Resources/Images/track3.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.5,
                ReviewCount = 112,
                IsFeatured = false,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-8),
                UpdatedAt = DateTime.Now
            });

            // JERSEYS (2)
            products.Add(new Product
            {
                Id = id++,
                Name = "MESH BASKETBALL JERSEY",
                Description = "Breathable mesh fabric",
                LongDescription = "Classic basketball jersey with team logo",
                Price = 64.99m,
                Stock = 80,
                MainImageUrl = "/Resources/Images/jersey1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.6,
                ReviewCount = 156,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-70),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "FOOTBALL JERSEY",
                Description = "American football style",
                LongDescription = "Oversized football jersey with number print",
                Price = 69.99m,
                OriginalPrice = 89.99m,
                Stock = 70,
                MainImageUrl = "/Resources/Images/jersey2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.4,
                ReviewCount = 98,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-55),
                UpdatedAt = DateTime.Now
            });

            // KNITWEAR (2)
            products.Add(new Product
            {
                Id = id++,
                Name = "CABLE KNIT SWEATER",
                Description = "Chunky knit design",
                LongDescription = "Warm cable knit sweater in premium wool blend",
                Price = 99.99m,
                Stock = 40,
                MainImageUrl = "/Resources/Images/knit1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.8,
                ReviewCount = 178,
                IsFeatured = true,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-12),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "CARDIGAN SWEATER",
                Description = "Button-up cardigan",
                LongDescription = "Classic cardigan with vintage buttons",
                Price = 94.99m,
                OriginalPrice = 124.99m,
                Stock = 35,
                MainImageUrl = "/Resources/Images/knit2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.6,
                ReviewCount = 134,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-80),
                UpdatedAt = DateTime.Now
            });

            // JACKETS (3)
            products.Add(new Product
            {
                Id = id++,
                Name = "BOMBER JACKET",
                Description = "Classic MA-1 style",
                LongDescription = "Timeless bomber jacket with ribbed cuffs",
                Price = 149.99m,
                Stock = 30,
                MainImageUrl = "/Resources/Images/bomber1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.9,
                ReviewCount = 267,
                IsFeatured = true,
                IsNew = false,
                Gender = "Men",
                CreatedAt = DateTime.Now.AddDays(-100),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "DENIM TRUCKER JACKET",
                Description = "Vintage wash denim",
                LongDescription = "Classic trucker jacket in premium denim",
                Price = 119.99m,
                OriginalPrice = 159.99m,
                Stock = 40,
                MainImageUrl = "/Resources/Images/denim1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.7,
                ReviewCount = 234,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-65),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "PUFFER JACKET",
                Description = "Insulated warmth",
                LongDescription = "Quilted puffer jacket with hood",
                Price = 179.99m,
                Stock = 25,
                MainImageUrl = "/Resources/Images/puffer1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Tops" },
                AverageRating = 4.8,
                ReviewCount = 198,
                IsFeatured = true,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-15),
                UpdatedAt = DateTime.Now
            });

            // ============================================
            // BOTTOMS - 20 PRODUCTOS
            // ============================================

            // DENIM PANTS (4)
            products.Add(new Product
            {
                Id = id++,
                Name = "SLIM FIT JEANS",
                Description = "Modern slim silhouette",
                LongDescription = "Comfortable slim fit jeans in dark wash",
                Price = 89.99m,
                Stock = 100,
                MainImageUrl = "/Resources/Images/jeans1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.5,
                ReviewCount = 289,
                IsFeatured = true,
                IsNew = false,
                Gender = "Men",
                CreatedAt = DateTime.Now.AddDays(-75),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "WIDE LEG JEANS",
                Description = "Relaxed wide fit",
                LongDescription = "Vintage-inspired wide leg denim",
                Price = 94.99m,
                OriginalPrice = 119.99m,
                Stock = 85,
                MainImageUrl = "/Resources/Images/jeans2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.7,
                ReviewCount = 312,
                IsFeatured = true,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-4),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "DISTRESSED JEANS",
                Description = "Ripped knee detail",
                LongDescription = "Edgy distressed jeans with authentic wear",
                Price = 99.99m,
                Stock = 75,
                MainImageUrl = "/Resources/Images/jeans3.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.4,
                ReviewCount = 178,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-45),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "BLACK SKINNY JEANS",
                Description = "Ultra-slim fit",
                LongDescription = "Stretch denim skinny jeans in jet black",
                Price = 84.99m,
                Stock = 110,
                MainImageUrl = "/Resources/Images/jeans4.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.6,
                ReviewCount = 267,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-90),
                UpdatedAt = DateTime.Now
            });

            // CARGO PANTS (3)
            products.Add(new Product
            {
                Id = id++,
                Name = "TACTICAL CARGO PANTS",
                Description = "Multi-pocket utility",
                LongDescription = "Military-inspired cargo pants with multiple pockets",
                Price = 79.99m,
                Stock = 90,
                MainImageUrl = "/Resources/Images/cargo1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.8,
                ReviewCount = 345,
                IsFeatured = true,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-6),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "PARACHUTE CARGO",
                Description = "Lightweight ripstop",
                LongDescription = "Parachute fabric cargo pants with adjustable cuffs",
                Price = 89.99m,
                OriginalPrice = 114.99m,
                Stock = 70,
                MainImageUrl = "/Resources/Images/cargo2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.6,
                ReviewCount = 198,
                IsFeatured = true,
                IsNew = false,
                Gender = "Men",
                CreatedAt = DateTime.Now.AddDays(-35),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "CANVAS CARGO PANTS",
                Description = "Heavy-duty canvas",
                LongDescription = "Durable canvas cargo pants for everyday wear",
                Price = 74.99m,
                Stock = 95,
                MainImageUrl = "/Resources/Images/cargo3.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.5,
                ReviewCount = 156,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-60),
                UpdatedAt = DateTime.Now
            });

            // JOGGERS (3)
            products.Add(new Product
            {
                Id = id++,
                Name = "PREMIUM SWEAT JOGGERS",
                Description = "Ultra-soft fleece",
                LongDescription = "Comfortable fleece joggers with tapered fit",
                Price = 64.99m,
                Stock = 120,
                MainImageUrl = "/Resources/Images/jogger1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.7,
                ReviewCount = 423,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-50),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "TECH FABRIC JOGGERS",
                Description = "Performance material",
                LongDescription = "Technical joggers with moisture-wicking fabric",
                Price = 69.99m,
                OriginalPrice = 89.99m,
                Stock = 100,
                MainImageUrl = "/Resources/Images/jogger2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.6,
                ReviewCount = 234,
                IsFeatured = false,
                IsNew = true,
                Gender = "Men",
                CreatedAt = DateTime.Now.AddDays(-9),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "ZIPPER POCKET JOGGERS",
                Description = "Secure zip pockets",
                LongDescription = "Joggers with zippered side pockets",
                Price = 74.99m,
                Stock = 85,
                MainImageUrl = "/Resources/Images/jogger3.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.5,
                ReviewCount = 189,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-40),
                UpdatedAt = DateTime.Now
            });

            // TRACK PANTS (2)
            products.Add(new Product
            {
                Id = id++,
                Name = "TRICOT TRACK PANTS",
                Description = "Classic athletic fit",
                LongDescription = "Retro track pants with side stripes",
                Price = 59.99m,
                Stock = 110,
                MainImageUrl = "/Resources/Images/trackpants1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.6,
                ReviewCount = 298,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-70),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "SNAP BUTTON TRACK PANTS",
                Description = "Tear-away design",
                LongDescription = "Track pants with snap buttons down the sides",
                Price = 69.99m,
                OriginalPrice = 94.99m,
                Stock = 65,
                MainImageUrl = "/Resources/Images/trackpants2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.4,
                ReviewCount = 145,
                IsFeatured = false,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-11),
                UpdatedAt = DateTime.Now
            });

            // JORTS (2)
            products.Add(new Product
            {
                Id = id++,
                Name = "VINTAGE DENIM SHORTS",
                Description = "Cut-off jean shorts",
                LongDescription = "Classic denim shorts with frayed hem",
                Price = 54.99m,
                Stock = 80,
                MainImageUrl = "/Resources/Images/jorts1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.3,
                ReviewCount = 167,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-120),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "BAGGY DENIM SHORTS",
                Description = "Relaxed loose fit",
                LongDescription = "Oversized denim shorts with cargo pockets",
                Price = 59.99m,
                Stock = 75,
                MainImageUrl = "/Resources/Images/jorts2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.4,
                ReviewCount = 134,
                IsFeatured = false,
                IsNew = true,
                Gender = "Men",
                CreatedAt = DateTime.Now.AddDays(-14),
                UpdatedAt = DateTime.Now
            });

            // SHORTS (2)
            products.Add(new Product
            {
                Id = id++,
                Name = "CARGO SHORTS",
                Description = "Multi-pocket utility",
                LongDescription = "Practical cargo shorts with side pockets",
                Price = 49.99m,
                Stock = 100,
                MainImageUrl = "/Resources/Images/shorts1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.5,
                ReviewCount = 223,
                IsFeatured = false,
                IsNew = false,
                Gender = "Men",
                CreatedAt = DateTime.Now.AddDays(-85),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "CHINO SHORTS",
                Description = "Classic summer style",
                LongDescription = "Tailored chino shorts in premium cotton",
                Price = 54.99m,
                OriginalPrice = 74.99m,
                Stock = 90,
                MainImageUrl = "/Resources/Images/shorts2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.6,
                ReviewCount = 189,
                IsFeatured = false,
                IsNew = false,
                Gender = "Men",
                CreatedAt = DateTime.Now.AddDays(-95),
                UpdatedAt = DateTime.Now
            });

            // SWIMSHORTS (2)
            products.Add(new Product
            {
                Id = id++,
                Name = "BOARD SHORTS",
                Description = "Quick-dry fabric",
                LongDescription = "Performance board shorts for water sports",
                Price = 44.99m,
                Stock = 70,
                MainImageUrl = "/Resources/Images/swim1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.4,
                ReviewCount = 156,
                IsFeatured = false,
                IsNew = false,
                Gender = "Men",
                CreatedAt = DateTime.Now.AddDays(-110),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "RETRO SWIM TRUNKS",
                Description = "Vintage pattern design",
                LongDescription = "Classic swim trunks with retro print",
                Price = 39.99m,
                Stock = 85,
                MainImageUrl = "/Resources/Images/swim2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.3,
                ReviewCount = 98,
                IsFeatured = false,
                IsNew = true,
                Gender = "Men",
                CreatedAt = DateTime.Now.AddDays(-16),
                UpdatedAt = DateTime.Now
            });

            // UNDERWEAR (2)
            products.Add(new Product
            {
                Id = id++,
                Name = "BOXER BRIEFS 3-PACK",
                Description = "Essential comfort",
                LongDescription = "Premium cotton boxer briefs in neutral colors",
                Price = 34.99m,
                Stock = 200,
                MainImageUrl = "/Resources/Images/underwear1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.7,
                ReviewCount = 412,
                IsFeatured = false,
                IsNew = false,
                Gender = "Men",
                CreatedAt = DateTime.Now.AddDays(-150),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "PERFORMANCE UNDERWEAR",
                Description = "Moisture-wicking tech",
                LongDescription = "Athletic underwear with breathable fabric",
                Price = 29.99m,
                OriginalPrice = 39.99m,
                Stock = 150,
                MainImageUrl = "/Resources/Images/underwear2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Bottoms" },
                AverageRating = 4.6,
                ReviewCount = 234,
                IsFeatured = false,
                IsNew = false,
                Gender = "Men",
                CreatedAt = DateTime.Now.AddDays(-130),
                UpdatedAt = DateTime.Now
            });

            // ============================================
            // FOOTWEAR - 15 PRODUCTOS
            // ============================================

            // ARMBO LOWS (3)
            products.Add(new Product
            {
                Id = id++,
                Name = "ARMBO LOW WHITE",
                Description = "Classic low-top sneaker",
                LongDescription = "Iconic low-top sneaker in premium leather",
                Price = 129.99m,
                Stock = 60,
                MainImageUrl = "/Resources/Images/armbo1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.8,
                ReviewCount = 567,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-80),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "ARMBO LOW BLACK",
                Description = "All-black edition",
                LongDescription = "Sleek all-black leather low-top",
                Price = 129.99m,
                OriginalPrice = 154.99m,
                Stock = 55,
                MainImageUrl = "/Resources/Images/armbo2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.9,
                ReviewCount = 489,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-75),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "ARMBO LOW VINTAGE",
                Description = "Distressed finish",
                LongDescription = "Vintage-style low-top with aged leather",
                Price = 139.99m,
                Stock = 45,
                MainImageUrl = "/Resources/Images/armbo3.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.7,
                ReviewCount = 312,
                IsFeatured = true,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-5),
                UpdatedAt = DateTime.Now
            });

            // VORTEX (3)
            products.Add(new Product
            {
                Id = id++,
                Name = "VORTEX RUNNER",
                Description = "Retro running shoe",
                LongDescription = "Classic runner with mesh and suede details",
                Price = 119.99m,
                Stock = 70,
                MainImageUrl = "/Resources/Images/vortex1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.6,
                ReviewCount = 278,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-60),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "VORTEX PREMIUM",
                Description = "Luxury materials",
                LongDescription = "Premium Vortex with premium leather and suede",
                Price = 149.99m,
                OriginalPrice = 189.99m,
                Stock = 40,
                MainImageUrl = "/Resources/Images/vortex2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.8,
                ReviewCount = 198,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-90),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "VORTEX TECH",
                Description = "Modern tech update",
                LongDescription = "Updated Vortex with technical mesh",
                Price = 134.99m,
                Stock = 55,
                MainImageUrl = "/Resources/Images/vortex3.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.5,
                ReviewCount = 145,
                IsFeatured = false,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-18),
                UpdatedAt = DateTime.Now
            });

            // VENTURE (3)
            products.Add(new Product
            {
                Id = id++,
                Name = "VENTURE HIGH-TOP",
                Description = "Classic basketball style",
                LongDescription = "High-top basketball sneaker in canvas",
                Price = 94.99m,
                Stock = 85,
                MainImageUrl = "/Resources/Images/venture1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.7,
                ReviewCount = 456,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-100),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "VENTURE PLATFORM",
                Description = "Chunky platform sole",
                LongDescription = "Venture with elevated platform design",
                Price = 109.99m,
                Stock = 50,
                MainImageUrl = "/Resources/Images/venture2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.4,
                ReviewCount = 189,
                IsFeatured = false,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-20),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "VENTURE LOW CANVAS",
                Description = "Low-top canvas version",
                LongDescription = "Classic low canvas sneaker",
                Price = 79.99m,
                OriginalPrice = 99.99m,
                Stock = 100,
                MainImageUrl = "/Resources/Images/venture3.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.6,
                ReviewCount = 512,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-120),
                UpdatedAt = DateTime.Now
            });

            // VITORIA (3)
            products.Add(new Product
            {
                Id = id++,
                Name = "VITORIA BOOT BLACK",
                Description = "Combat boot style",
                LongDescription = "Military-inspired leather boots",
                Price = 169.99m,
                Stock = 35,
                MainImageUrl = "/Resources/Images/vitoria1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.8,
                ReviewCount = 234,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-65),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "VITORIA CHELSEA",
                Description = "Classic Chelsea boot",
                LongDescription = "Sleek Chelsea boot with elastic sides",
                Price = 159.99m,
                OriginalPrice = 199.99m,
                Stock = 30,
                MainImageUrl = "/Resources/Images/vitoria2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.7,
                ReviewCount = 178,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-85),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "VITORIA HIKER",
                Description = "Outdoor hiking boot",
                LongDescription = "Rugged hiking boot with waterproof finish",
                Price = 179.99m,
                Stock = 25,
                MainImageUrl = "/Resources/Images/vitoria3.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.6,
                ReviewCount = 145,
                IsFeatured = false,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-22),
                UpdatedAt = DateTime.Now
            });

            // V-SLIDES (3)
            products.Add(new Product
            {
                Id = id++,
                Name = "V-SLIDE CLASSIC",
                Description = "Essential slide sandal",
                LongDescription = "Comfortable slide with cushioned footbed",
                Price = 44.99m,
                Stock = 150,
                MainImageUrl = "/Resources/Images/slide1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.5,
                ReviewCount = 389,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-140),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "V-SLIDE PLATFORM",
                Description = "Elevated platform slide",
                LongDescription = "Platform slide with extra height",
                Price = 54.99m,
                Stock = 100,
                MainImageUrl = "/Resources/Images/slide2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.4,
                ReviewCount = 267,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-95),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "V-SLIDE FLEECE",
                Description = "Cozy fleece lining",
                LongDescription = "Winter slide with warm fleece interior",
                Price = 59.99m,
                OriginalPrice = 74.99m,
                Stock = 80,
                MainImageUrl = "/Resources/Images/slide3.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Footwear" },
                AverageRating = 4.6,
                ReviewCount = 198,
                IsFeatured = false,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-25),
                UpdatedAt = DateTime.Now
            });

            // ============================================
            // ACCESSORIES - 15 PRODUCTOS
            // ============================================

            // CAPS (3)
            products.Add(new Product
            {
                Id = id++,
                Name = "CLASSIC DAD CAP",
                Description = "Adjustable baseball cap",
                LongDescription = "Comfortable dad cap with embroidered logo",
                Price = 29.99m,
                Stock = 200,
                MainImageUrl = "/Resources/Images/cap1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.6,
                ReviewCount = 567,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-110),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "SNAPBACK CAP",
                Description = "Flat brim snapback",
                LongDescription = "Classic snapback with flat brim",
                Price = 34.99m,
                Stock = 150,
                MainImageUrl = "/Resources/Images/cap2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.5,
                ReviewCount = 423,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-95),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "TRUCKER CAP",
                Description = "Mesh back trucker",
                LongDescription = "Breathable trucker cap with mesh panels",
                Price = 27.99m,
                OriginalPrice = 36.99m,
                Stock = 175,
                MainImageUrl = "/Resources/Images/cap3.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.4,
                ReviewCount = 298,
                IsFeatured = false,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-28),
                UpdatedAt = DateTime.Now
            });

            // BAGS (3)
            products.Add(new Product
            {
                Id = id++,
                Name = "BACKPACK UTILITY",
                Description = "Multi-compartment backpack",
                LongDescription = "Spacious backpack with laptop sleeve",
                Price = 89.99m,
                Stock = 60,
                MainImageUrl = "/Resources/Images/bag1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.7,
                ReviewCount = 345,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-70),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "CROSSBODY BAG",
                Description = "Compact shoulder bag",
                LongDescription = "Small crossbody with adjustable strap",
                Price = 64.99m,
                OriginalPrice = 84.99m,
                Stock = 80,
                MainImageUrl = "/Resources/Images/bag2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.5,
                ReviewCount = 234,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-55),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "DUFFLE BAG",
                Description = "Weekend travel bag",
                LongDescription = "Large duffle for gym or travel",
                Price = 99.99m,
                Stock = 45,
                MainImageUrl = "/Resources/Images/bag3.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.6,
                ReviewCount = 189,
                IsFeatured = false,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-30),
                UpdatedAt = DateTime.Now
            });

            // BEANIES (3)
            products.Add(new Product
            {
                Id = id++,
                Name = "RIBBED BEANIE",
                Description = "Classic knit beanie",
                LongDescription = "Warm ribbed beanie in soft acrylic",
                Price = 24.99m,
                Stock = 180,
                MainImageUrl = "/Resources/Images/beanie1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.6,
                ReviewCount = 478,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-130),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "CUFFED BEANIE",
                Description = "Double-fold cuff style",
                LongDescription = "Thick cuffed beanie for extra warmth",
                Price = 27.99m,
                Stock = 160,
                MainImageUrl = "/Resources/Images/beanie2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.5,
                ReviewCount = 356,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-115),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "SLOUCHY BEANIE",
                Description = "Relaxed oversized fit",
                LongDescription = "Trendy slouchy beanie with loose fit",
                Price = 29.99m,
                OriginalPrice = 39.99m,
                Stock = 140,
                MainImageUrl = "/Resources/Images/beanie3.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.4,
                ReviewCount = 267,
                IsFeatured = false,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-32),
                UpdatedAt = DateTime.Now
            });

            // CARDHOLDER (2)
            products.Add(new Product
            {
                Id = id++,
                Name = "LEATHER CARDHOLDER",
                Description = "Minimalist wallet",
                LongDescription = "Slim leather cardholder with 6 slots",
                Price = 39.99m,
                Stock = 100,
                MainImageUrl = "/Resources/Images/cardholder1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.7,
                ReviewCount = 298,
                IsFeatured = true,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-80),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "CANVAS CARDHOLDER",
                Description = "Durable canvas wallet",
                LongDescription = "Water-resistant canvas cardholder",
                Price = 29.99m,
                Stock = 120,
                MainImageUrl = "/Resources/Images/cardholder2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.5,
                ReviewCount = 189,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-100),
                UpdatedAt = DateTime.Now
            });

            // BELTS (2)
            products.Add(new Product
            {
                Id = id++,
                Name = "CANVAS WEB BELT",
                Description = "Military-style webbing",
                LongDescription = "Adjustable canvas belt with metal buckle",
                Price = 34.99m,
                Stock = 140,
                MainImageUrl = "/Resources/Images/belt1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.6,
                ReviewCount = 234,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-105),
                UpdatedAt = DateTime.Now
            });

            products.Add(new Product
            {
                Id = id++,
                Name = "LEATHER BELT",
                Description = "Classic leather belt",
                LongDescription = "Premium leather belt with brass buckle",
                Price = 49.99m,
                OriginalPrice = 69.99m,
                Stock = 90,
                MainImageUrl = "/Resources/Images/belt2.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.7,
                ReviewCount = 178,
                IsFeatured = false,
                IsNew = false,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-125),
                UpdatedAt = DateTime.Now
            });

            // RINGS (1)
            products.Add(new Product
            {
                Id = id++,
                Name = "SILVER BAND RING",
                Description = "Minimalist ring design",
                LongDescription = "Sterling silver band ring",
                Price = 44.99m,
                Stock = 75,
                MainImageUrl = "/Resources/Images/ring1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.5,
                ReviewCount = 156,
                IsFeatured = false,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-35),
                UpdatedAt = DateTime.Now
            });

            // RUGS (1)
            products.Add(new Product
            {
                Id = id++,
                Name = "LOGO RUG",
                Description = "Premium floor mat",
                LongDescription = "Decorative rug with embroidered logo",
                Price = 79.99m,
                Stock = 30,
                MainImageUrl = "/Resources/Images/rug1.jpg",
                ImageUrls = new List<string>(),
                CategoryId = 4,
                Category = new Category { Id = 4, Name = "Accessories" },
                AverageRating = 4.4,
                ReviewCount = 89,
                IsFeatured = false,
                IsNew = true,
                Gender = "Unisex",
                CreatedAt = DateTime.Now.AddDays(-38),
                UpdatedAt = DateTime.Now
            });

            return products;
        }
    }
}
