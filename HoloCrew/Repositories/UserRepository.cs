using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Repositorio de usuarios.
// Con Supabase, la mayoría de las operaciones de auth las hace AuthenticationService.
// Este repositorio mantiene una caché en memoria de usuarios consultados,
// para casos donde los ViewModels lo inyecten directamente.
// Las operaciones reales contra BD se delegan a AuthenticationService o a
// queries directas con Supabase.Client cuando sea necesario.

namespace HoloCrew.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static readonly List<User> _cachedUsers = new();

        public Task<User?> GetByIdAsync(string id)
        {
            var user = _cachedUsers.FirstOrDefault(u => u.Id == id);
            return Task.FromResult<User?>(user);
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            var user = _cachedUsers.FirstOrDefault(u =>
                u.Email != null &&
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult<User?>(user);
        }

        public Task<User?> CreateAsync(User user)
        {
            // Las creaciones reales pasan por AuthenticationService.RegisterAsync
            // (que llama a Supabase Auth). Aquí solo cacheamos.
            if (string.IsNullOrEmpty(user.Id))
                user.Id = Guid.NewGuid().ToString();
            user.CreatedAt = DateTime.Now;
            _cachedUsers.Add(user);
            return Task.FromResult<User?>(user);
        }

        public Task<User?> UpdateAsync(User user)
        {
            var existing = _cachedUsers.FirstOrDefault(u => u.Id == user.Id);
            if (existing != null)
            {
                var index = _cachedUsers.IndexOf(existing);
                _cachedUsers[index] = user;
                return Task.FromResult<User?>(user);
            }
            return Task.FromResult<User?>(null);
        }

        public Task<bool> DeleteAsync(string id)
        {
            var user = _cachedUsers.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _cachedUsers.Remove(user);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            var exists = _cachedUsers.Any(u =>
                u.Email != null &&
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(exists);
        }

        public Task<List<User>> GetAllAsync()
        {
            return Task.FromResult(_cachedUsers.ToList());
        }

        public Task UpdateLastLoginAsync(string userId)
        {
            var user = _cachedUsers.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.LastLoginAt = DateTime.Now;
            }
            return Task.CompletedTask;
        }

        public Task<User?> ValidateCredentialsAsync(string email, string password)
        {
            // Las validaciones reales pasan por AuthenticationService.LoginAsync.
            return Task.FromResult<User?>(null);
        }
    }
}