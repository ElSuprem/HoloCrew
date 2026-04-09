using HoloCrew.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

// Repositorio para acceder a los datos de usuarios.
// Operaciones: buscar por id o email, crear, actualizar, borrar, comprobar si existe el email, etc.

namespace HoloCrew.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<bool> EmailExistsAsync(string email);                 // para no duplicar emails al registrarse
        Task<List<User>> GetAllAsync();                            // solo para admin
        Task UpdateLastLoginAsync(int userId);                     // guarda cuándo inició sesión
        Task<User> ValidateCredentialsAsync(string email, string password); // comprueba email y contraseña
    }
}