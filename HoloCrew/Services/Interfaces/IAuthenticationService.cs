using HoloCrew.Models;
using System;
using System.Threading.Tasks;

// Servicio para manejar login, registro, cierre de sesión, cambio de contraseña, etc.
// Implementado con Supabase Auth.
// Lanza el evento AuthStateChanged cuando el estado de autenticación cambia
// (login, registro, logout) para que la UI pueda actualizarse.

namespace HoloCrew.Services.Interfaces
{
    public interface IAuthenticationService
    {
        event EventHandler? AuthStateChanged;  // se dispara al cambiar el estado de auth
        Task<User?> LoginAsync(string email, string password);
        Task<User?> RegisterAsync(User user, string password);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        User? GetCurrentUser();
        Task RefreshCurrentUserAsync();   // recarga el perfil desde Supabase y actualiza la caché
        Task<bool> ValidateTokenAsync();
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
        Task<bool> RequestPasswordResetAsync(string email);
        Task<bool> UpdateProfileAsync(string firstName, string lastName, string phone);
        Task<bool> UpdateAvatarUrlAsync(string avatarUrl);
        Task<User?> LoginWithGoogleAsync();
    }
}