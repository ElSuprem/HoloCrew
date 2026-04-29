using HoloCrew.Infraestructure.Supabase.Models;
using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using PgConstants = Supabase.Postgrest.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Repositorio de productos conectado a Supabase.
// Lee de las tablas: products, product_images, product_sizes, product_colors.
// Convierte los DTOs a modelos Product que consumen los ViewModels.

namespace HoloCrew.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly Supabase.Client _supabase;

        public ProductRepository(Supabase.Client supabase)
        {
            _supabase = supabase;
        }


        public async Task<List<Product>> GetAllAsync()
        {
            // Solo productos activos
            var productsResponse = await _supabase
                .From<ProductDto>()
                .Where(p => p.IsActive == true)
                .Order("created_at", PgConstants.Ordering.Descending)
                .Get();

            var products = productsResponse.Models;
            if (products.Count == 0)
                return new List<Product>();

            // Cargamos en paralelo imágenes, tallas y colores de todos los productos
            // para evitar N+1 queries.
            var productIds = products.Select(p => p.Id).ToList();

            var imagesTask = _supabase.From<ProductImageDto>()
                .Filter("product_id", PgConstants.Operator.In, productIds)
                .Get();

            var sizesTask = _supabase.From<ProductSizeDto>()
                .Filter("product_id", PgConstants.Operator.In, productIds)
                .Get();

            var colorsTask = _supabase.From<ProductColorDto>()
                .Filter("product_id", PgConstants.Operator.In, productIds)
                .Get();

            await Task.WhenAll(imagesTask, sizesTask, colorsTask);

            var images = imagesTask.Result.Models;
            var sizes = sizesTask.Result.Models;
            var colors = colorsTask.Result.Models;

            return products
                .Select(p => p.ToProduct(images, sizes, colors))
                .ToList();
        }


        public async Task<Product?> GetByIdAsync(int id)
        {
            var productResponse = await _supabase
                .From<ProductDto>()
                .Where(p => p.Id == id)
                .Single();

            if (productResponse == null)
                return null;

            // Cargamos imágenes, tallas y colores de este producto en paralelo
            var imagesTask = _supabase.From<ProductImageDto>()
                .Where(i => i.ProductId == id)
                .Order("sort_order", PgConstants.Ordering.Ascending)
                .Get();

            var sizesTask = _supabase.From<ProductSizeDto>()
                .Where(s => s.ProductId == id)
                .Get();

            var colorsTask = _supabase.From<ProductColorDto>()
                .Where(c => c.ProductId == id)
                .Get();

            await Task.WhenAll(imagesTask, sizesTask, colorsTask);

            return productResponse.ToProduct(
                imagesTask.Result.Models,
                sizesTask.Result.Models,
                colorsTask.Result.Models);
        }


        public async Task<List<Product>> GetByCategoryAsync(int categoryId)
        {
            // Primero buscamos la categoría y todas sus descendientes (usando ltree de Postgres).
            // Si pasan id=1 (Tops), nos devuelve también 10, 11, 12, 13 (T-Shirts, Shirts, Hoodies, Sweatshirts).
            var allCategories = await _supabase
                .From<CategoryDto>()
                .Where(c => c.IsActive == true)
                .Get();

            // Buscamos la categoría pedida
            var rootCategory = allCategories.Models.FirstOrDefault(c => c.Id == categoryId);
            if (rootCategory == null)
                return new List<Product>();

            // Recopilamos todos los IDs: el de la categoría y los de sus hijas (recursivamente)
            var categoryIds = GetDescendantIds(rootCategory.Id, allCategories.Models);

            // Ahora pedimos los productos cuya categoría esté en esa lista
            var productsResponse = await _supabase
                .From<ProductDto>()
                .Where(p => p.IsActive == true)
                .Filter("category_id", PgConstants.Operator.In, categoryIds)
                .Get();

            return await EnrichProductsAsync(productsResponse.Models);
        }


        // Helper: dado un id de categoría, devuelve [su_id] + ids de todos sus descendientes.
        private List<int> GetDescendantIds(int rootId, List<CategoryDto> allCategories)
        {
            var result = new List<int> { rootId };
            var children = allCategories.Where(c => c.ParentId == rootId).ToList();
            foreach (var child in children)
            {
                result.AddRange(GetDescendantIds(child.Id, allCategories));
            }
            return result;
        }


        public async Task<List<Product>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Product>();

            // Búsqueda por coincidencia parcial en el nombre (case insensitive).
            // Más adelante podríamos cambiar esto para usar el search_vector
            // y aprovechar el full-text search de PostgreSQL.
            var productsResponse = await _supabase
                .From<ProductDto>()
                .Where(p => p.IsActive == true)
                .Filter("name", PgConstants.Operator.ILike, $"%{query}%")
                .Get();

            return await EnrichProductsAsync(productsResponse.Models);
        }


        public async Task<List<Product>> GetFeaturedAsync()
        {
            var productsResponse = await _supabase
                .From<ProductDto>()
                .Where(p => p.IsActive == true)
                .Where(p => p.IsFeatured == true)
                .Get();

            return await EnrichProductsAsync(productsResponse.Models);
        }


        public async Task<List<Product>> GetNewProductsAsync()
        {
            var productsResponse = await _supabase
                .From<ProductDto>()
                .Where(p => p.IsActive == true)
                .Where(p => p.IsNew == true)
                .Order("created_at", PgConstants.Ordering.Descending)
                .Get();

            return await EnrichProductsAsync(productsResponse.Models);
        }


        // Estos tres métodos los tienes en la interfaz pero la creación/edición/borrado
        // de productos NO la hace el cliente desde el WPF (RLS lo bloquea), sino
        // un admin desde el dashboard de Supabase. Los dejamos preparados pero
        // lanzando NotImplementedException de momento.

        public Task<Product> CreateAsync(Product product)
        {
            throw new NotImplementedException(
                "La creación de productos se hace desde el dashboard de Supabase, no desde el cliente.");
        }

        public Task<Product> UpdateAsync(Product product)
        {
            throw new NotImplementedException(
                "La actualización de productos se hace desde el dashboard de Supabase, no desde el cliente.");
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException(
                "El borrado de productos se hace desde el dashboard de Supabase, no desde el cliente.");
        }


        // Helper privado: dada una lista de ProductDto, carga sus imágenes/tallas/colores
        // y devuelve la lista de Product completa.
        private async Task<List<Product>> EnrichProductsAsync(List<ProductDto> products)
        {
            if (products.Count == 0)
                return new List<Product>();

            var productIds = products.Select(p => p.Id).ToList();

            var imagesTask = _supabase.From<ProductImageDto>()
                .Filter("product_id", PgConstants.Operator.In, productIds)
                .Get();

            var sizesTask = _supabase.From<ProductSizeDto>()
                .Filter("product_id", PgConstants.Operator.In, productIds)
                .Get();

            var colorsTask = _supabase.From<ProductColorDto>()
                .Filter("product_id", PgConstants.Operator.In, productIds)
                .Get();

            await Task.WhenAll(imagesTask, sizesTask, colorsTask);

            return products
                .Select(p => p.ToProduct(
                    imagesTask.Result.Models,
                    sizesTask.Result.Models,
                    colorsTask.Result.Models))
                .ToList();
        }
    }
}