using HoloCrew.Models;

namespace HoloCrew.Services.Interfaces
{
    /// <summary>
    /// Servicio para gestión de productos
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Obtiene productos destacados
        /// </summary>
        Task<List<Product>> GetFeaturedProductsAsync();

        /// <summary>
        /// Obtiene productos por categoría
        /// </summary>
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        Task<Product> GetProductByIdAsync(int productId);

        /// <summary>
        /// Busca productos por texto
        /// </summary>
        Task<List<Product>> SearchProductsAsync(string query);

        /// <summary>
        /// Obtiene todas las categorías
        /// </summary>
        Task<List<Category>> GetCategoriesAsync();

        /// <summary>
        /// Obtiene productos relacionados a un producto
        /// </summary>
        Task<List<Product>> GetRelatedProductsAsync(int productId);

        /// <summary>
        /// Obtiene los más vendidos
        /// </summary>
        Task<List<Product>> GetBestSellersAsync();

        /// <summary>
        /// Obtiene productos nuevos
        /// </summary>
        Task<List<Product>> GetNewProductsAsync();
    }
}