using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Servicio de productos con catálogo completo de streetwear.
// Organizado por subcategorías igual que la web de HoloCrew.
// Los productos están en GetStreetwearCatalog() como datos mock.

namespace HoloCrew.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private List<Product> _cachedProducts;  // guarda los productos en memoria para no recargar siempre

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetFeaturedProductsAsync()
        {
            var products = await GetAllProductsAsync();
            return products.Where(p => p.IsFeatured).Take(8).ToList();  // hasta 8 destacados
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await GetAllProductsAsync();

            if (categoryId == 0) return products;

            // categorías principales (1-5) o subcategorías (10+)
            if (categoryId >= 1 && categoryId <= 5)
            {
                return products.Where(p => p.CategoryId == categoryId).ToList();
            }

            return products.Where(p => p.SubCategoryId == categoryId).ToList();
        }

        // obtiene productos por slug de subcategoría (usado desde el mega menú)
        public async Task<List<Product>> GetProductsBySlugAsync(string slug)
        {
            var products = await GetAllProductsAsync();

            if (string.IsNullOrEmpty(slug) || slug == "all")
                return products;

            slug = slug.ToLower();

            // casos especiales del menú
            switch (slug)
            {
                case "new":   // productos nuevos
                    return products.Where(p => p.IsNew).OrderByDescending(p => p.CreatedAt).ToList();
                case "blackweek":  // ofertas Black Week
                    return products.Where(p => p.IsBlackWeek || p.HasDiscount).ToList();
                case "softs":   // colección Softs
                    return products.Where(p => p.IsSoftsCollection).ToList();
                case "classic":  // colección Classic
                    return products.Where(p => p.IsClassicCollection).ToList();
                case "activewear":  // ropa deportiva
                    return products.Where(p => p.SubCategorySlug == "joggers" || p.SubCategorySlug == "trackpants" || p.SubCategorySlug == "shorts").ToList();
                case "tracksuits":  // chándales
                    return products.Where(p => p.SubCategorySlug == "trackjackets" || p.SubCategorySlug == "trackpants").ToList();
            }

            // filtrar por slug de subcategoría
            return products.Where(p => p.SubCategorySlug == slug).ToList();
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var products = await GetAllProductsAsync();
            return products.FirstOrDefault(p => p.Id == productId);
        }

        public async Task<List<Product>> SearchProductsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Product>();

            var products = await GetAllProductsAsync();
            query = query.ToLower();

            return products.Where(p =>
                p.Name.ToLower().Contains(query) ||
                p.Description.ToLower().Contains(query) ||
                p.SubCategorySlug?.ToLower().Contains(query) == true ||
                p.Category?.Name?.ToLower().Contains(query) == true
            ).ToList();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await Task.FromResult(HoloCrewCategories.GetAllCategories());
        }

        public async Task<List<Product>> GetRelatedProductsAsync(int productId)
        {
            var product = await GetProductByIdAsync(productId);
            if (product == null) return new List<Product>();

            var products = await GetAllProductsAsync();

            // productos de la misma subcategoría (sin incluir el mismo)
            return products
                .Where(p => p.Id != productId && p.SubCategoryId == product.SubCategoryId)
                .Take(4)
                .ToList();
        }

        public async Task<List<Product>> GetBestSellersAsync()
        {
            var products = await GetAllProductsAsync();
            return products.OrderByDescending(p => p.ReviewCount).Take(10).ToList();  // los más valorados
        }

        public async Task<List<Product>> GetNewProductsAsync()
        {
            var products = await GetAllProductsAsync();
            return products.Where(p => p.IsNew).OrderByDescending(p => p.CreatedAt).Take(10).ToList();
        }

        public async Task<List<Product>> GetBlackWeekProductsAsync()
        {
            var products = await GetAllProductsAsync();
            return products.Where(p => p.IsBlackWeek || p.HasDiscount).ToList();
        }

        // carga todos los productos (con caché para no repetir)
        private async Task<List<Product>> GetAllProductsAsync()
        {
            if (_cachedProducts != null) return _cachedProducts;

            var repoProducts = await _productRepository.GetAllAsync();

            if (repoProducts != null && repoProducts.Any())
            {
                _cachedProducts = repoProducts;
                return repoProducts;
            }

            _cachedProducts = GetStreetwearCatalog();
            return _cachedProducts;
        }

        // ========== CATÁLOGO COMPLETO DE STREETWEAR HOLOCREW ==========
        private List<Product> GetStreetwearCatalog()
        {
            var products = new List<Product>();
            int id = 1;

            // T-SHIRTS (SubCategoryId = 20)
            products.AddRange(new[]
            {
                CreateProduct(id++, "CLASSIC LOGO TEE", "Essential cotton t-shirt with embroidered logo", 34.99m, 44.99m,
                    2, 20, "tshirts", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "VINTAGE WASH TEE", "Oversized t-shirt with vintage wash treatment", 39.99m, null,
                    2, 20, "tshirts", isNew: true),
                CreateProduct(id++, "GRAPHIC PRINT TEE", "Statement tee with exclusive HoloCrew artwork", 44.99m, 59.99m,
                    2, 20, "tshirts", isBlackWeek: true),
                CreateProduct(id++, "STRIPED LONG SLEEVE", "Premium striped long sleeve tee", 49.99m, null,
                    2, 20, "tshirts", isSofts: true),
                CreateProduct(id++, "OVERSIZED POCKET TEE", "Relaxed fit with chest pocket detail", 37.99m, null,
                    2, 20, "tshirts", isNew: true),
                CreateProduct(id++, "HEAVYWEIGHT TEE", "Premium 300gsm cotton, boxy fit", 54.99m, 69.99m,
                    2, 20, "tshirts", isFeatured: true, isClassic: true),
            });

            // HOODIES (SubCategoryId = 21)
            products.AddRange(new[]
            {
                CreateProduct(id++, "CLASSIC LOGO HOODIE", "Premium heavyweight hoodie with embroidered logo", 89.99m, 109.99m,
                    2, 21, "hoodies", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "OVERSIZED ZIP HOODIE", "Relaxed fit zip-up hoodie", 99.99m, null,
                    2, 21, "hoodies", isNew: true, isSofts: true),
                CreateProduct(id++, "VINTAGE WASHED HOODIE", "Stone-washed hoodie with distressed look", 94.99m, null,
                    2, 21, "hoodies", isClassic: true),
                CreateProduct(id++, "CROPPED HOODIE", "Women's cropped hoodie", 79.99m, 99.99m,
                    2, 21, "hoodies", gender: "Women", isBlackWeek: true),
                CreateProduct(id++, "HEAVYWEIGHT HOODIE", "400gsm premium fleece hoodie", 119.99m, null,
                    2, 21, "hoodies", isFeatured: true, isSofts: true),
                CreateProduct(id++, "SLEEVELESS HOODIE", "Cut-off sleeve hoodie for training", 69.99m, null,
                    2, 21, "hoodies"),
            });

            // TRACK JACKETS (SubCategoryId = 22)
            products.AddRange(new[]
            {
                CreateProduct(id++, "RETRO TRACK JACKET", "90s inspired track jacket with stripe detail", 109.99m, 139.99m,
                    2, 22, "trackjackets", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "VELOUR TRACK TOP", "Luxury velour track jacket", 129.99m, null,
                    2, 22, "trackjackets", isNew: true, isSofts: true),
                CreateProduct(id++, "TECH TRACK JACKET", "Lightweight technical fabric", 99.99m, null,
                    2, 22, "trackjackets"),
            });

            // JERSEYS (SubCategoryId = 23)
            products.AddRange(new[]
            {
                CreateProduct(id++, "SOCCER JERSEY", "Premium mesh soccer jersey", 74.99m, 89.99m,
                    2, 23, "jerseys", isBlackWeek: true),
                CreateProduct(id++, "BASKETBALL JERSEY", "Breathable basketball jersey", 69.99m, null,
                    2, 23, "jerseys", isNew: true),
                CreateProduct(id++, "RACING JERSEY", "Motorsport inspired jersey", 79.99m, null,
                    2, 23, "jerseys"),
            });

            // KNITWEAR (SubCategoryId = 24)
            products.AddRange(new[]
            {
                CreateProduct(id++, "CABLE KNIT SWEATER", "Classic cable knit in premium wool blend", 99.99m, 129.99m,
                    2, 24, "knitwear", isFeatured: true, isBlackWeek: true, isClassic: true),
                CreateProduct(id++, "RIBBED TURTLENECK", "Slim fit ribbed turtleneck", 84.99m, null,
                    2, 24, "knitwear", isNew: true),
                CreateProduct(id++, "OVERSIZED CARDIGAN", "Chunky knit open cardigan", 119.99m, null,
                    2, 24, "knitwear", isSofts: true),
            });

            // JACKETS (SubCategoryId = 25)
            products.AddRange(new[]
            {
                CreateProduct(id++, "PUFFER JACKET", "Premium down-filled puffer jacket", 179.99m, 229.99m,
                    2, 25, "jackets", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "BOMBER JACKET", "Classic MA-1 style bomber", 149.99m, null,
                    2, 25, "jackets", isClassic: true),
                CreateProduct(id++, "DENIM JACKET", "Washed denim trucker jacket", 129.99m, 159.99m,
                    2, 25, "jackets", isBlackWeek: true),
                CreateProduct(id++, "COACH JACKET", "Lightweight nylon coach jacket", 99.99m, null,
                    2, 25, "jackets", isNew: true),
                CreateProduct(id++, "LEATHER JACKET", "Premium faux leather biker jacket", 199.99m, null,
                    2, 25, "jackets", isFeatured: true),
            });

            // DENIM PANTS (SubCategoryId = 30)
            products.AddRange(new[]
            {
                CreateProduct(id++, "STRAIGHT FIT JEANS", "Classic straight leg denim", 89.99m, 109.99m,
                    3, 30, "denim", isFeatured: true, isBlackWeek: true, isClassic: true),
                CreateProduct(id++, "BAGGY JEANS", "Relaxed baggy fit jeans", 99.99m, null,
                    3, 30, "denim", isNew: true),
                CreateProduct(id++, "DISTRESSED JEANS", "Heavy distressed with rips", 109.99m, null,
                    3, 30, "denim"),
                CreateProduct(id++, "CARPENTER JEANS", "Utility style with hammer loop", 94.99m, null,
                    3, 30, "denim"),
            });

            // CARGO PANTS (SubCategoryId = 31)
            products.AddRange(new[]
            {
                CreateProduct(id++, "UTILITY CARGO PANTS", "6-pocket utility cargo pants", 99.99m, 129.99m,
                    3, 31, "cargo", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "WIDE LEG CARGO", "Relaxed wide leg cargo pants", 109.99m, null,
                    3, 31, "cargo", isNew: true),
                CreateProduct(id++, "RIPSTOP CARGO", "Military-grade ripstop fabric", 119.99m, null,
                    3, 31, "cargo"),
                CreateProduct(id++, "PARACHUTE PANTS", "Lightweight parachute cargo", 89.99m, 109.99m,
                    3, 31, "cargo", isBlackWeek: true),
            });

            // JOGGERS (SubCategoryId = 32)
            products.AddRange(new[]
            {
                CreateProduct(id++, "CLASSIC JOGGERS", "Essential cotton blend joggers", 69.99m, 89.99m,
                    3, 32, "joggers", isFeatured: true, isBlackWeek: true, isSofts: true),
                CreateProduct(id++, "TECH FLEECE JOGGERS", "Premium tech fleece fabric", 84.99m, null,
                    3, 32, "joggers", isNew: true),
                CreateProduct(id++, "HEAVYWEIGHT JOGGERS", "400gsm premium cotton joggers", 79.99m, null,
                    3, 32, "joggers", isSofts: true),
                CreateProduct(id++, "SLIM FIT JOGGERS", "Tapered slim fit joggers", 74.99m, null,
                    3, 32, "joggers"),
            });

            // TRACK PANTS (SubCategoryId = 33)
            products.AddRange(new[]
            {
                CreateProduct(id++, "RETRO TRACK PANTS", "90s inspired with side stripe", 89.99m, 109.99m,
                    3, 33, "trackpants", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "VELOUR TRACK PANTS", "Luxury velour matching pants", 99.99m, null,
                    3, 33, "trackpants", isSofts: true),
                CreateProduct(id++, "WIDE LEG TRACK PANTS", "Relaxed wide leg track pants", 84.99m, null,
                    3, 33, "trackpants", isNew: true),
            });

            // SHORTS (SubCategoryId = 35)
            products.AddRange(new[]
            {
                CreateProduct(id++, "MESH SHORTS", "Breathable mesh basketball shorts", 49.99m, 64.99m,
                    3, 35, "shorts", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "TERRY SHORTS", "Soft terry cloth shorts", 54.99m, null,
                    3, 35, "shorts", isSofts: true),
                CreateProduct(id++, "NYLON SHORTS", "Quick-dry nylon shorts", 44.99m, null,
                    3, 35, "shorts", isNew: true),
                CreateProduct(id++, "CARGO SHORTS", "Utility cargo shorts", 59.99m, null,
                    3, 35, "shorts"),
            });

            // SWIMSHORTS (SubCategoryId = 36)
            products.AddRange(new[]
            {
                CreateProduct(id++, "CLASSIC SWIM TRUNKS", "Quick-dry swim trunks with logo", 49.99m, 64.99m,
                    3, 36, "swimshorts", isBlackWeek: true),
                CreateProduct(id++, "LONG SWIM SHORTS", "Extended length swim shorts", 54.99m, null,
                    3, 36, "swimshorts"),
            });

            // ARMBO LOWS (SubCategoryId = 40)
            products.AddRange(new[]
            {
                CreateProduct(id++, "ARMBO LOW WHITE", "Classic low-top sneaker in white", 139.99m, 169.99m,
                    4, 40, "armbo", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "ARMBO LOW BLACK", "Classic low-top sneaker in black", 139.99m, null,
                    4, 40, "armbo", isFeatured: true),
                CreateProduct(id++, "ARMBO LOW CREAM", "Vintage cream colorway", 149.99m, null,
                    4, 40, "armbo", isNew: true),
            });

            // VORTEX (SubCategoryId = 41)
            products.AddRange(new[]
            {
                CreateProduct(id++, "VORTEX RUNNER", "Technical running-inspired sneaker", 159.99m, 189.99m,
                    4, 41, "vortex", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "VORTEX TRAIL", "All-terrain trail sneaker", 169.99m, null,
                    4, 41, "vortex", isNew: true),
            });

            // VENTURE (SubCategoryId = 42)
            products.AddRange(new[]
            {
                CreateProduct(id++, "VENTURE MID", "Mid-top basketball-inspired sneaker", 149.99m, 179.99m,
                    4, 42, "venture", isBlackWeek: true),
                CreateProduct(id++, "VENTURE HIGH", "High-top premium leather", 179.99m, null,
                    4, 42, "venture"),
            });

            // VITORIA (SubCategoryId = 43)
            products.AddRange(new[]
            {
                CreateProduct(id++, "VITORIA LOAFER", "Premium leather loafer", 199.99m, 249.99m,
                    4, 43, "vitoria", isFeatured: true, isBlackWeek: true, isClassic: true),
                CreateProduct(id++, "VITORIA DERBY", "Classic derby shoe", 219.99m, null,
                    4, 43, "vitoria"),
            });

            // V-SLIDES (SubCategoryId = 44)
            products.AddRange(new[]
            {
                CreateProduct(id++, "V-SLIDES CLASSIC", "Comfortable everyday slides", 39.99m, 49.99m,
                    4, 44, "vslides", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "V-SLIDES FOAM", "Extra cushion foam slides", 44.99m, null,
                    4, 44, "vslides", isNew: true),
            });

            // CAPS (SubCategoryId = 50)
            products.AddRange(new[]
            {
                CreateProduct(id++, "CLASSIC CAP", "6-panel structured cap with logo", 34.99m, 44.99m,
                    5, 50, "caps", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "DAD CAP", "Unstructured relaxed fit cap", 29.99m, null,
                    5, 50, "caps"),
                CreateProduct(id++, "TRUCKER CAP", "Mesh back trucker cap", 32.99m, null,
                    5, 50, "caps", isNew: true),
                CreateProduct(id++, "SNAPBACK CAP", "Flat brim snapback cap", 37.99m, null,
                    5, 50, "caps"),
            });

            // BAGS (SubCategoryId = 51)
            products.AddRange(new[]
            {
                CreateProduct(id++, "MESSENGER BAG", "Classic messenger bag with logo", 79.99m, 99.99m,
                    5, 51, "bags", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "BACKPACK", "Premium backpack with laptop sleeve", 99.99m, null,
                    5, 51, "bags"),
                CreateProduct(id++, "CROSSBODY BAG", "Compact crossbody bag", 59.99m, null,
                    5, 51, "bags", isNew: true),
                CreateProduct(id++, "TOTE BAG", "Oversized canvas tote bag", 49.99m, null,
                    5, 51, "bags"),
                CreateProduct(id++, "DUFFLE BAG", "Weekend duffle bag", 119.99m, 149.99m,
                    5, 51, "bags", isBlackWeek: true),
            });

            // BEANIES (SubCategoryId = 52)
            products.AddRange(new[]
            {
                CreateProduct(id++, "RIBBED BEANIE", "Classic ribbed knit beanie", 24.99m, 34.99m,
                    5, 52, "beanies", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "CUFFED BEANIE", "Double-fold cuffed beanie", 27.99m, null,
                    5, 52, "beanies"),
                CreateProduct(id++, "SLOUCHY BEANIE", "Relaxed oversized beanie", 29.99m, null,
                    5, 52, "beanies", isNew: true, isSofts: true),
            });

            // CARDHOLDER (SubCategoryId = 53)
            products.AddRange(new[]
            {
                CreateProduct(id++, "LEATHER CARDHOLDER", "Slim leather cardholder with 6 slots", 39.99m, 49.99m,
                    5, 53, "cardholder", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "CANVAS CARDHOLDER", "Durable canvas cardholder", 29.99m, null,
                    5, 53, "cardholder"),
            });

            // BELTS (SubCategoryId = 54)
            products.AddRange(new[]
            {
                CreateProduct(id++, "CANVAS WEB BELT", "Military-style canvas belt", 34.99m, 44.99m,
                    5, 54, "belts", isBlackWeek: true),
                CreateProduct(id++, "LEATHER BELT", "Premium leather belt with brass buckle", 49.99m, null,
                    5, 54, "belts", isFeatured: true, isClassic: true),
            });

            // RINGS (SubCategoryId = 55)
            products.AddRange(new[]
            {
                CreateProduct(id++, "SILVER BAND RING", "Sterling silver band ring", 44.99m, 54.99m,
                    5, 55, "rings", isBlackWeek: true),
                CreateProduct(id++, "SIGNET RING", "Classic signet ring with logo", 49.99m, null,
                    5, 55, "rings", isNew: true),
            });

            // RUGS (SubCategoryId = 56)
            products.AddRange(new[]
            {
                CreateProduct(id++, "LOGO RUG", "Premium floor rug with embroidered logo", 79.99m, 99.99m,
                    5, 56, "rugs", isFeatured: true, isBlackWeek: true),
                CreateProduct(id++, "MINI RUG", "Small accent rug", 49.99m, null,
                    5, 56, "rugs"),
            });

            return products;
        }

        // función auxiliar para crear productos de forma consistente
        private Product CreateProduct(
            int id, string name, string description,
            decimal price, decimal? originalPrice,
            int categoryId, int subCategoryId, string subCategorySlug,
            bool isFeatured = false, bool isNew = false, bool isBlackWeek = false,
            bool isSofts = false, bool isClassic = false, string gender = "Unisex")
        {
            string categoryName = categoryId switch
            {
                2 => "Tops",
                3 => "Bottoms",
                4 => "Footwear",
                5 => "Accessories",
                _ => "Shop All"
            };

            return new Product
            {
                Id = id,
                Name = name,
                Description = description,
                LongDescription = $"{description}. Premium quality, designed for comfort and style.",
                Price = price,
                OriginalPrice = originalPrice,
                Stock = new Random(id).Next(20, 200),
                MainImageUrl = $"/Resources/Images/{subCategorySlug}{id % 3 + 1}.jpg",
                ImageUrls = new List<string>
                {
                    $"/Resources/Images/{subCategorySlug}{id % 3 + 1}.jpg",
                    $"/Resources/Images/{subCategorySlug}{id % 3 + 1}_2.jpg",
                },
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                SubCategorySlug = subCategorySlug,
                Category = new Category { Id = categoryId, Name = categoryName },
                AverageRating = 4.0 + (id % 10) * 0.1,
                ReviewCount = 50 + (id * 17) % 500,
                IsFeatured = isFeatured,
                IsNew = isNew,
                IsBlackWeek = isBlackWeek,
                IsSoftsCollection = isSofts,
                IsClassicCollection = isClassic,
                Gender = gender,
                AvailableSizes = new List<string> { "XS", "S", "M", "L", "XL", "XXL" },
                AvailableColors = new List<string> { "Black", "White", "Gray", "Navy" },
                CreatedAt = DateTime.Now.AddDays(-new Random(id).Next(1, 120)),
                UpdatedAt = DateTime.Now
            };
        }
    }
}