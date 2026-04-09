using HoloCrew.Models;
using Microsoft.EntityFrameworkCore;

// Contexto de base de datos para Entity Framework Core.
// Solo se usa si tienes base de datos real. Los repositorios mock funcionan sin esto.
// Define todas las tablas (Users, Products, Orders, etc.) y sus configuraciones.

namespace HoloCrew.Infraestructure.Database
{
    public class LocalDbContext : DbContext
    {
        public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options)
        {
        }

        // Tablas de la base de datos
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Usuario: el email es único y obligatorio
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            });

            // Producto: el precio se guarda con dos decimales
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18,2)");
            });

            // Categoría: el nombre es único
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Pedido: el número de pedido es único, los precios con dos decimales
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ShippingCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Discount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");

                // un usuario puede tener muchos pedidos
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Item del carrito
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                // cada item apunta a un producto, si se borra el producto no se borra el item del carrito
                entity.HasOne(c => c.Product)
                      .WithMany()
                      .HasForeignKey(c => c.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Reseña de producto
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Rating).IsRequired();
                entity.Property(e => e.Comment).HasMaxLength(1000);

                // si se borra un producto, se borran sus reseñas
                entity.HasOne<Product>()
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                // si se borra un usuario, se borran sus reseñas
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Dirección de envío o facturación
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.AddressLine1).IsRequired().HasMaxLength(200);
                entity.Property(e => e.City).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PostalCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
            });

            // Método de pago guardado (solo se guardan los últimos 4 dígitos, no el número completo)
            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CardholderName).HasMaxLength(200);
                entity.Property(e => e.CardNumberMasked).HasMaxLength(20);
            });

            // Notificación del usuario
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);

                // si se borra un usuario, se borran sus notificaciones
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}