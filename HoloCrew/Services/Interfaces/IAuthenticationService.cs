using HoloCrew.Models;

namespace HoloCrew.Services.Interfaces
{
    /// <summary>
    /// Servicio de autenticación y autorización
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Inicia sesión con email y contraseña
        /// </summary>
        Task<User> LoginAsync(string email, string password);

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        Task<User> RegisterAsync(User user, string password);

        /// <summary>
        /// Cierra la sesión actual
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Verifica si hay un usuario autenticado
        /// </summary>
        Task<bool> IsAuthenticatedAsync();

        /// <summary>
        /// Obtiene el usuario actual
        /// </summary>
        User GetCurrentUser();

        /// <summary>
        /// Valida el token de sesión
        /// </summary>
        Task<bool> ValidateTokenAsync();

        /// <summary>
        /// Cambia la contraseña del usuario actual
        /// </summary>
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);

        /// <summary>
        /// Solicita recuperación de contraseña
        /// </summary>
        Task<bool> RequestPasswordResetAsync(string email);
    }
}