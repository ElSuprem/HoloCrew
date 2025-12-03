using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;

namespace HoloCrew.Services
{
    /// <summary>
    /// Implementación del servicio de productos
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
            // Obtener productos destacados del repositorio
            var allProducts = await _productRepository.GetAllAsync();
            return allProducts.Where(p => p.IsFeatured).ToList();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            if (categoryId == 0)
            {
                // 0 = todas las categorías
                return await _productRepository.GetAllAsync();
            }

            return await _productRepository.GetByCategoryAsync(categoryId);
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }

        public async Task<List<Product>> SearchProductsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<Product>();
            }

            return await _productRepository.SearchAsync(query);
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            // TODO: Implementar con CategoryRepository cuando se cree
            // Por ahora retornar datos mock
            return await Task.FromResult(new List<Category>
            {
                new Category { Id = 1, Name = "Electrónica", IconUrl = "/Resources/Icons/electronics.png" },
                new Category { Id = 2, Name = "Ropa", IconUrl = "/Resources/Icons/clothing.png" },
                new Category { Id = 3, Name = "Hogar", IconUrl = "/Resources/Icons/home.png" },
                new Category { Id = 4, Name = "Deportes", IconUrl = "/Resources/Icons/sports.png" },
                new Category { Id = 5, Name = "Libros", IconUrl = "/Resources/Icons/books.png" }
            });
        }

        public async Task<List<Product>> GetRelatedProductsAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                return new List<Product>();
            }

            // Obtener productos de la misma categoría
            var relatedProducts = await _productRepository.GetByCategoryAsync(product.CategoryId);

            // Excluir el producto actual y tomar solo 4
            return relatedProducts
                .Where(p => p.Id != productId)
                .Take(4)
                .ToList();
        }

        public async Task<List<Product>> GetBestSellersAsync()
        {
            var allProducts = await _productRepository.GetAllAsync();

            // Ordenar por ReviewCount (simulando ventas)
            return allProducts
                .OrderByDescending(p => p.ReviewCount)
                .Take(10)
                .ToList();
        }

        public async Task<List<Product>> GetNewProductsAsync()
        {
            var allProducts = await _productRepository.GetAllAsync();

            return allProducts
                .Where(p => p.IsNew)
                .OrderByDescending(p => p.CreatedAt)
                .Take(10)
                .ToList();
        }
    }
}