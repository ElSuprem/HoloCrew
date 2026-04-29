using HoloCrew.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

// Repositorio para acceder a los datos de usuarios.
// Con Supabase Auth la mayoría de operaciones (login, registro) las hace
// AuthenticationService directamente. Este repositorio queda como
// helper para casos puntuales (búsquedas, listado, etc.).

namespace HoloCrew.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> CreateAsync(User user);
        Task<User?> UpdateAsync(User user);
        Task<bool> DeleteAsync(string id);
        Task<bool> EmailExistsAsync(string email);
        Task<List<User>> GetAllAsync();
        Task UpdateLastLoginAsync(string userId);
        Task<User?> ValidateCredentialsAsync(string email, string password);
    }
}