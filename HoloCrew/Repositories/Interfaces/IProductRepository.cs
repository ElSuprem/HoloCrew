using HoloCrew.Models;

// Repositorio para acceder a los datos de productos.
// Aquí se definen las operaciones: listar, buscar, crear, actualizar, borrar.

namespace HoloCrew.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();                 // todos los productos
        Task<Product> GetByIdAsync(int id);                // un producto por su id
        Task<List<Product>> GetByCategoryAsync(int categoryId); // productos de una categoría
        Task<List<Product>> SearchAsync(string query);     // buscar por texto
        Task<Product> CreateAsync(Product product);        // crear nuevo producto
        Task<Product> UpdateAsync(Product product);        // actualizar producto existente
        Task<bool> DeleteAsync(int id);                    // borrar producto
        Task<List<Product>> GetFeaturedAsync();            // productos destacados
        Task<List<Product>> GetNewProductsAsync();         // productos nuevos
    }
}