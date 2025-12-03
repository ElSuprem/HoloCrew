using HoloCrew.Models;

namespace HoloCrew.Repositories.Interfaces
{
    /// <summary>
    /// Repositorio para acceso a datos de pedidos
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Crea un nuevo pedido
        /// </summary>
        Task<Order> CreateAsync(Order order);

        /// <summary>
        /// Obtiene todos los pedidos de un usuario
        /// </summary>
        Task<List<Order>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Obtiene un pedido por su ID
        /// </summary>
        Task<Order> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene un pedido por su número de orden
        /// </summary>
        Task<Order> GetByOrderNumberAsync(string orderNumber);

        /// <summary>
        /// Actualiza un pedido existente
        /// </summary>
        Task<Order> UpdateAsync(Order order);

        /// <summary>
        /// Obtiene todos los pedidos (admin)
        /// </summary>
        Task<List<Order>> GetAllAsync();

        /// <summary>
        /// Obtiene pedidos por estado
        /// </summary>
        Task<List<Order>> GetByStatusAsync(OrderStatus status);

        /// <summary>
        /// Obtiene pedidos en un rango de fechas
        /// </summary>
        Task<List<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Elimina un pedido (soft delete)
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}