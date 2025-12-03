using HoloCrew.Models;

namespace HoloCrew.Repositories.Interfaces
{
    /// <summary>
    /// Repositorio para acceso a datos de productos
    /// Capa de acceso a datos (puede ser API, base de datos local, etc.)
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        Task<List<Product>> GetAllAsync();

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        Task<Product> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene productos por categoría
        /// </summary>
        Task<List<Product>> GetByCategoryAsync(int categoryId);

        /// <summary>
        /// Busca productos por texto
        /// </summary>
        Task<List<Product>> SearchAsync(string query);

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        Task<Product> CreateAsync(Product product);

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        Task<Product> UpdateAsync(Product product);

        /// <summary>
        /// Elimina un producto
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Obtiene productos destacados
        /// </summary>
        Task<List<Product>> GetFeaturedAsync();

        /// <summary>
        /// Obtiene productos nuevos
        /// </summary>
        Task<List<Product>> GetNewProductsAsync();
    }
}