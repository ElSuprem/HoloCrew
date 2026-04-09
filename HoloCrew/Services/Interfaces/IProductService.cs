using HoloCrew.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

// Servicio para gestionar productos: listar por categoría, buscar, destacados, más vendidos, etc.
// Se conecta con los modelos Product y Category.

namespace HoloCrew.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetFeaturedProductsAsync();           // productos destacados
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);  // por id de categoría
        Task<List<Product>> GetProductsBySlugAsync(string slug);  // por slug de subcategoría (desde el mega menú)
        Task<Product> GetProductByIdAsync(int productId);         // un producto por su id
        Task<List<Product>> SearchProductsAsync(string query);    // buscar por texto
        Task<List<Category>> GetCategoriesAsync();                // todas las categorías (con subcategorías)
        Task<List<Product>> GetRelatedProductsAsync(int productId); // productos similares
        Task<List<Product>> GetBestSellersAsync();                // los más vendidos
        Task<List<Product>> GetNewProductsAsync();                // productos nuevos
        Task<List<Product>> GetBlackWeekProductsAsync();          // productos en oferta de Black Week
    }
}