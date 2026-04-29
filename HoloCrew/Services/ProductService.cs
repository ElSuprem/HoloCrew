using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;
using HoloCrew.Infraestructure.Supabase.Models;
using PgConstants = Supabase.Postgrest.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Servicio de productos conectado a Supabase a través del ProductRepository.
// Cachea la lista completa la primera vez que se pide, para que los filtros
// posteriores sean instantáneos sin volver a llamar a la red.

namespace HoloCrew.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly Supabase.Client _supabase;

        // Caché en memoria
        private List<Product>? _cachedProducts;
        private List<CategoryDto>? _cachedCategories;

        public ProductService(IProductRepository productRepository, Supabase.Client supabase)
        {
            _productRepository = productRepository;
            _supabase = supabase;
        }

        public async Task<List<Product>> GetFeaturedProductsAsync()
        {
            var products = await GetAllProductsAsync();
            return products.Where(p => p.IsFeatured).Take(8).ToList();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            if (_cachedProducts != null)
                return _cachedProducts;

            _cachedProducts = await _productRepository.GetAllAsync();
            return _cachedProducts;
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            if (categoryId == 0)
                return await GetAllProductsAsync();

            var categories = await GetAllCategoriesAsync();
            var descendantIds = GetDescendantIds(categoryId, categories);

            var products = await GetAllProductsAsync();
            return products.Where(p => descendantIds.Contains(p.CategoryId)).ToList();
        }

        // Obtiene productos por slug de subcategoría (usado desde el mega menú)
        public async Task<List<Product>> GetProductsBySlugAsync(string slug)
        {
            System.Diagnostics.Debug.WriteLine($"[ProductService] Recibo slug: '{slug}'");

            var products = await GetAllProductsAsync();

            if (string.IsNullOrEmpty(slug) || slug == "all")
                return products;

            slug = slug.ToLower().Trim();

            // ALIASES: el menú envía slugs cortos, los traducimos a los reales de la BD
            slug = slug switch
            {
                "tshirts" => "t-shirts",
                "denim" => "denim-pants",
                "cargo" => "cargo-pants",
                "trackpants" => "track-pants",
                "armbo" => "armbo-lows",
                "slides" => "v-slides",
                "sneakers" => "sneakers",   // por si acaso, ya lo es
                _ => slug
            };

            // CASOS ESPECIALES (colecciones que cruzan varias categorías, no son slugs de tabla)
            switch (slug)
            {
                case "new":
                case "new-arrivals":
                    return products.Where(p => p.IsNew)
                                   .OrderByDescending(p => p.CreatedAt)
                                   .ToList();

                case "blackweek":
                case "black-week":
                    return products.Where(p => p.IsBlackWeek || p.HasDiscount).ToList();

                case "softs":
                case "softs-collection":
                    return products.Where(p => p.IsSoftsCollection).ToList();

                case "classic":
                case "classic-collection":
                    return products.Where(p => p.IsClassicCollection).ToList();

                case "activewear":
                    return products.Where(p =>
                        p.CategoryId == 23 ||  // joggers
                        p.CategoryId == 27 ||  // track-pants
                        p.CategoryId == 22 ||  // shorts
                        p.CategoryId == 28     // swimshorts
                    ).ToList();

                case "tracksuits":
                    return products.Where(p =>
                        p.CategoryId == 52 ||  // trackjackets
                        p.CategoryId == 27     // track-pants
                    ).ToList();
            }

            // BÚSQUEDA POR SLUG REAL DE CATEGORÍA
            var categories = await GetAllCategoriesAsync();
            var match = categories.FirstOrDefault(c =>
                c.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

            if (match == null)
            {
                System.Diagnostics.Debug.WriteLine($"[ProductService] Slug NO encontrado en BD: '{slug}'");
                return new List<Product>();
            }

            var descendantIds = GetDescendantIds(match.Id, categories);
            var result = products.Where(p => descendantIds.Contains(p.CategoryId)).ToList();

            System.Diagnostics.Debug.WriteLine(
                $"[ProductService] Slug '{slug}' → categoría id={match.Id}, descendientes=[{string.Join(",", descendantIds)}], productos encontrados={result.Count}");

            return result;
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }

        public async Task<List<Product>> SearchProductsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Product>();

            var products = await GetAllProductsAsync();
            var q = query.ToLower();

            return products.Where(p =>
                p.Name.ToLower().Contains(q) ||
                (p.Description?.ToLower().Contains(q) ?? false) ||
                (p.LongDescription?.ToLower().Contains(q) ?? false)
            ).ToList();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var dtos = await GetAllCategoriesAsync();
            return dtos.Select(d => d.ToCategory()).ToList();
        }

        public async Task<List<Product>> GetRelatedProductsAsync(int productId)
        {
            var products = await GetAllProductsAsync();
            var product = products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
                return new List<Product>();

            return products
                .Where(p => p.Id != productId && p.CategoryId == product.CategoryId)
                .Take(4)
                .ToList();
        }

        public async Task<List<Product>> GetBestSellersAsync()
        {
            var products = await GetAllProductsAsync();
            return products
                .Where(p => p.IsFeatured || p.ReviewCount > 0)
                .OrderByDescending(p => p.ReviewCount)
                .Take(10)
                .ToList();
        }

        public async Task<List<Product>> GetNewProductsAsync()
        {
            var products = await GetAllProductsAsync();
            return products
                .Where(p => p.IsNew)
                .OrderByDescending(p => p.CreatedAt)
                .Take(10)
                .ToList();
        }

        public async Task<List<Product>> GetBlackWeekProductsAsync()
        {
            var products = await GetAllProductsAsync();
            return products.Where(p => p.IsBlackWeek || p.HasDiscount).ToList();
        }


        // ==================== HELPERS ====================

        private async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            if (_cachedCategories != null)
                return _cachedCategories;

            var response = await _supabase
                .From<CategoryDto>()
                .Where(c => c.IsActive == true)
                .Get();

            _cachedCategories = response.Models;
            return _cachedCategories;
        }

        private List<int> GetDescendantIds(int rootId, List<CategoryDto> all)
        {
            var result = new List<int> { rootId };
            foreach (var child in all.Where(c => c.ParentId == rootId))
            {
                result.AddRange(GetDescendantIds(child.Id, all));
            }
            return result;
        }
    }
}