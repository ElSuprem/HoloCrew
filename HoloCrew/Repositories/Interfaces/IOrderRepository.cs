using HoloCrew.Models;

// Repositorio para manejar pedidos (guardar, buscar, actualizar, etc.)
// Se conecta con el modelo Order y OrderStatus.

namespace HoloCrew.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order);                           // crear nuevo pedido
        Task<List<Order>> GetByUserIdAsync(int userId);                 // pedidos de un usuario
        Task<Order> GetByIdAsync(int id);                               // buscar por id
        Task<Order> GetByOrderNumberAsync(string orderNumber);         // buscar por número de pedido
        Task<Order> UpdateAsync(Order order);                           // actualizar un pedido
        Task<List<Order>> GetAllAsync();                                // todos los pedidos (para admin)
        Task<List<Order>> GetByStatusAsync(OrderStatus status);         // pedidos por estado (pendiente, enviado, etc.)
        Task<List<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate); // pedidos entre dos fechas
        Task<bool> DeleteAsync(int id);                                 // borrar pedido (borrado suave, no se elimina del todo)
    }
}