using HoloCrew.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HoloCrew.Repositories.Interfaces
{
    /// <summary>
    /// Repositorio para acceso a datos de usuarios
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        Task<User> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene un usuario por su email
        /// </summary>
        Task<User> GetByEmailAsync(string email);

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        Task<User> CreateAsync(User user);

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        Task<User> UpdateAsync(User user);

        /// <summary>
        /// Elimina un usuario
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Verifica si un email ya está registrado
        /// </summary>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Obtiene todos los usuarios (admin)
        /// </summary>
        Task<List<User>> GetAllAsync();

        /// <summary>
        /// Actualiza la última fecha de login
        /// </summary>
        Task UpdateLastLoginAsync(int userId);

        /// <summary>
        /// Valida las credenciales de un usuario (email + password)
        /// </summary>
        Task<User> ValidateCredentialsAsync(string email, string password);
    }
}