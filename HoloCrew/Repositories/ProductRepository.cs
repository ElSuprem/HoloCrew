using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloCrew.Repositories
{
    /// <summary>
    /// Implementación del repositorio de productos
    /// NOTA: Esta implementación usa datos MOCK en memoria
    /// En producción, conectaría con una API real o base de datos
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private static List<Product> _products;
        private static int _nextId = 1;

        public ProductRepository()
        {
            // Inicializar datos mock solo una vez
            if (_products == null)
            {
                InitializeMockData();
            }
        }

        public Task<List<Product>> GetAllAsync()
        {
            return Task.FromResult(_products.ToList());
        }

        public Task<Product> GetByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<List<Product>> GetByCategoryAsync(int categoryId)
        {
            var products = _products
                .Where(p => p.CategoryId == categoryId)
                .ToList();
            return Task.FromResult(products);
        }

        public Task<List<Product>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Task.FromResult(new List<Product>());
            }

            var lowerQuery = query.ToLower();
            var results = _products
                .Where(p => p.Name.ToLower().Contains(lowerQuery) ||
                           p.Description.ToLower().Contains(lowerQuery))
                .ToList();

            return Task.FromResult(results);
        }

        public Task<Product> CreateAsync(Product product)
        {
            product.Id = _nextId++;
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            _products.Add(product);
            return Task.FromResult(product);
        }

        public Task<Product> UpdateAsync(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing != null)
            {
                var index = _products.IndexOf(existing);
                product.UpdatedAt = DateTime.Now;
                _products[index] = product;
                return Task.FromResult(product);
            }
            return Task.FromResult<Product>(null);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<List<Product>> GetFeaturedAsync()
        {
            var featured = _products
                .Where(p => p.IsFeatured)
                .ToList();
            return Task.FromResult(featured);
        }

        public Task<List<Product>> GetNewProductsAsync()
        {
            var newProducts = _products
                .Where(p => p.IsNew)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
            return Task.FromResult(newProducts);
        }

        private void InitializeMockData()
        {
            _products = new List<Product>
            {
                // Electrónica
                new Product
                {
                    Id = _nextId++,
                    Name = "Laptop HP Pavilion",
                    Description = "Laptop potente para trabajo y entretenimiento",
                    LongDescription = "Laptop HP Pavilion con procesador Intel Core i5, 8GB RAM, 256GB SSD. Perfecta para productividad y multimedia.",
                    Price = 699.99m,
                    OriginalPrice = 849.99m,
                    Stock = 15,
                    MainImageUrl = "/Resources/Images/Products/laptop1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/laptop1.jpg", "/Resources/Images/Products/laptop1-2.jpg" },
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Electrónica" },
                    AverageRating = 4.5,
                    ReviewCount = 127,
                    IsFeatured = true,
                    IsNew = false,
                    CreatedAt = DateTime.Now.AddMonths(-3),
                    UpdatedAt = DateTime.Now.AddMonths(-3)
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "Auriculares Sony WH-1000XM4",
                    Description = "Auriculares con cancelación de ruido",
                    LongDescription = "Auriculares premium con cancelación de ruido líder en la industria, hasta 30 horas de batería.",
                    Price = 279.99m,
                    OriginalPrice = 349.99m,
                    Stock = 32,
                    MainImageUrl = "/Resources/Images/Products/headphones1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/headphones1.jpg" },
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Electrónica" },
                    AverageRating = 4.8,
                    ReviewCount = 203,
                    IsFeatured = true,
                    IsNew = false,
                    CreatedAt = DateTime.Now.AddMonths(-5),
                    UpdatedAt = DateTime.Now.AddMonths(-5)
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "iPhone 15 Pro",
                    Description = "El último smartphone de Apple",
                    LongDescription = "iPhone 15 Pro con chip A17 Pro, cámara de 48MP, titanio aeroespacial.",
                    Price = 1199.99m,
                    Stock = 8,
                    MainImageUrl = "/Resources/Images/Products/iphone15.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/iphone15.jpg" },
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Electrónica" },
                    AverageRating = 4.9,
                    ReviewCount = 456,
                    IsFeatured = true,
                    IsNew = true,
                    CreatedAt = DateTime.Now.AddDays(-15),
                    UpdatedAt = DateTime.Now.AddDays(-15)
                },

                // Ropa
                new Product
                {
                    Id = _nextId++,
                    Name = "Camiseta Nike Dri-FIT",
                    Description = "Camiseta deportiva transpirable",
                    LongDescription = "Camiseta deportiva Nike con tecnología Dri-FIT para mantenerte seco y cómodo.",
                    Price = 29.99m,
                    OriginalPrice = 39.99m,
                    Stock = 50,
                    MainImageUrl = "/Resources/Images/Products/tshirt1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/tshirt1.jpg" },
                    CategoryId = 2,
                    Category = new Category { Id = 2, Name = "Ropa" },
                    AverageRating = 4.3,
                    ReviewCount = 89,
                    IsFeatured = false,
                    IsNew = false,
                    CreatedAt = DateTime.Now.AddMonths(-6),
                    UpdatedAt = DateTime.Now.AddMonths(-6)
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "Zapatillas Adidas Ultraboost",
                    Description = "Zapatillas running premium",
                    LongDescription = "Zapatillas Adidas Ultraboost con tecnología BOOST para máximo retorno de energía.",
                    Price = 179.99m,
                    OriginalPrice = 220.00m,
                    Stock = 25,
                    MainImageUrl = "/Resources/Images/Products/shoes1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/shoes1.jpg" },
                    CategoryId = 2,
                    Category = new Category { Id = 2, Name = "Ropa" },
                    AverageRating = 4.7,
                    ReviewCount = 312,
                    IsFeatured = true,
                    IsNew = false,
                    CreatedAt = DateTime.Now.AddMonths(-4),
                    UpdatedAt = DateTime.Now.AddMonths(-4)
                },

                // Hogar
                new Product
                {
                    Id = _nextId++,
                    Name = "Aspiradora Roomba i7+",
                    Description = "Robot aspirador inteligente",
                    LongDescription = "Roomba i7+ con vaciado automático, mapeo inteligente y control por app.",
                    Price = 599.99m,
                    Stock = 12,
                    MainImageUrl = "/Resources/Images/Products/roomba.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/roomba.jpg" },
                    CategoryId = 3,
                    Category = new Category { Id = 3, Name = "Hogar" },
                    AverageRating = 4.6,
                    ReviewCount = 178,
                    IsFeatured = true,
                    IsNew = false,
                    CreatedAt = DateTime.Now.AddMonths(-7),
                    UpdatedAt = DateTime.Now.AddMonths(-7)
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "Cafetera Nespresso",
                    Description = "Cafetera de cápsulas premium",
                    LongDescription = "Cafetera Nespresso Vertuo con sistema de cápsulas y preparación en un toque.",
                    Price = 149.99m,
                    OriginalPrice = 199.99m,
                    Stock = 30,
                    MainImageUrl = "/Resources/Images/Products/nespresso.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/nespresso.jpg" },
                    CategoryId = 3,
                    Category = new Category { Id = 3, Name = "Hogar" },
                    AverageRating = 4.4,
                    ReviewCount = 95,
                    IsFeatured = false,
                    IsNew = false,
                    CreatedAt = DateTime.Now.AddMonths(-8),
                    UpdatedAt = DateTime.Now.AddMonths(-8)
                },

                // Deportes
                new Product
                {
                    Id = _nextId++,
                    Name = "Bicicleta de Montaña Trek",
                    Description = "Bicicleta todo terreno profesional",
                    LongDescription = "Trek Mountain Bike con suspensión completa, 27 velocidades, cuadro de aluminio.",
                    Price = 899.99m,
                    Stock = 6,
                    MainImageUrl = "/Resources/Images/Products/bike.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/bike.jpg" },
                    CategoryId = 4,
                    Category = new Category { Id = 4, Name = "Deportes" },
                    AverageRating = 4.8,
                    ReviewCount = 67,
                    IsFeatured = true,
                    IsNew = true,
                    CreatedAt = DateTime.Now.AddDays(-20),
                    UpdatedAt = DateTime.Now.AddDays(-20)
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "Mancuernas Ajustables 20kg",
                    Description = "Set de mancuernas para gimnasio en casa",
                    LongDescription = "Set de mancuernas ajustables de 5 a 20kg por mancuerna, compactas y fáciles de usar.",
                    Price = 89.99m,
                    Stock = 18,
                    MainImageUrl = "/Resources/Images/Products/dumbbells.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/dumbbells.jpg" },
                    CategoryId = 4,
                    Category = new Category { Id = 4, Name = "Deportes" },
                    AverageRating = 4.5,
                    ReviewCount = 134,
                    IsFeatured = false,
                    IsNew = false,
                    CreatedAt = DateTime.Now.AddMonths(-2),
                    UpdatedAt = DateTime.Now.AddMonths(-2)
                },

                // Libros
                new Product
                {
                    Id = _nextId++,
                    Name = "Cien Años de Soledad",
                    Description = "Clásico de Gabriel García Márquez",
                    LongDescription = "Obra maestra del realismo mágico, una de las novelas más importantes del siglo XX.",
                    Price = 14.99m,
                    Stock = 40,
                    MainImageUrl = "/Resources/Images/Products/book1.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/book1.jpg" },
                    CategoryId = 5,
                    Category = new Category { Id = 5, Name = "Libros" },
                    AverageRating = 4.9,
                    ReviewCount = 523,
                    IsFeatured = false,
                    IsNew = false,
                    CreatedAt = DateTime.Now.AddYears(-1),
                    UpdatedAt = DateTime.Now.AddYears(-1)
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "El Código Limpio",
                    Description = "Manual de programación ágil",
                    LongDescription = "Clean Code de Robert C. Martin, guía esencial para escribir código mantenible.",
                    Price = 34.99m,
                    Stock = 22,
                    MainImageUrl = "/Resources/Images/Products/cleancode.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/cleancode.jpg" },
                    CategoryId = 5,
                    Category = new Category { Id = 5, Name = "Libros" },
                    AverageRating = 4.7,
                    ReviewCount = 287,
                    IsFeatured = true,
                    IsNew = false,
                    CreatedAt = DateTime.Now.AddMonths(-10),
                    UpdatedAt = DateTime.Now.AddMonths(-10)
                },

                // Más productos...
                new Product
                {
                    Id = _nextId++,
                    Name = "Smart TV Samsung 55\"",
                    Description = "Televisor 4K UHD con HDR",
                    LongDescription = "Smart TV Samsung 55 pulgadas con resolución 4K, HDR10+, Tizen OS y asistente de voz.",
                    Price = 549.99m,
                    OriginalPrice = 699.99m,
                    Stock = 10,
                    MainImageUrl = "/Resources/Images/Products/tv.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/tv.jpg" },
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Electrónica" },
                    AverageRating = 4.6,
                    ReviewCount = 234,
                    IsFeatured = true,
                    IsNew = false,
                    CreatedAt = DateTime.Now.AddMonths(-4),
                    UpdatedAt = DateTime.Now.AddMonths(-4)
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "Reloj Inteligente Apple Watch Series 9",
                    Description = "Smartwatch con monitoreo de salud",
                    LongDescription = "Apple Watch Series 9 con pantalla siempre activa, GPS, monitoreo cardíaco y ECG.",
                    Price = 429.99m,
                    Stock = 16,
                    MainImageUrl = "/Resources/Images/Products/applewatch.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/applewatch.jpg" },
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Electrónica" },
                    AverageRating = 4.8,
                    ReviewCount = 389,
                    IsFeatured = true,
                    IsNew = true,
                    CreatedAt = DateTime.Now.AddDays(-10),
                    UpdatedAt = DateTime.Now.AddDays(-10)
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "Chaqueta Impermeable North Face",
                    Description = "Chaqueta outdoor profesional",
                    LongDescription = "Chaqueta North Face con tecnología Gore-Tex, perfecta para senderismo y aventuras.",
                    Price = 199.99m,
                    Stock = 28,
                    MainImageUrl = "/Resources/Images/Products/jacket.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/jacket.jpg" },
                    CategoryId = 2,
                    Category = new Category { Id = 2, Name = "Ropa" },
                    AverageRating = 4.7,
                    ReviewCount = 156,
                    IsFeatured = false,
                    IsNew = false,
                    CreatedAt = DateTime.Now.AddMonths(-5),
                    UpdatedAt = DateTime.Now.AddMonths(-5)
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "Kindle Paperwhite",
                    Description = "Lector de libros electrónicos",
                    LongDescription = "Kindle Paperwhite con pantalla de 6.8 pulgadas, resistente al agua, luz ajustable.",
                    Price = 139.99m,
                    Stock = 35,
                    MainImageUrl = "/Resources/Images/Products/kindle.jpg",
                    ImageUrls = new List<string> { "/Resources/Images/Products/kindle.jpg" },
                    CategoryId = 5,
                    Category = new Category { Id = 5, Name = "Libros" },
                    AverageRating = 4.7,
                    ReviewCount = 421,
                    IsFeatured = true,
                    IsNew = false,
                    CreatedAt = DateTime.Now.AddMonths(-3),
                    UpdatedAt = DateTime.Now.AddMonths(-3)
                }
            };
        }
    }
}