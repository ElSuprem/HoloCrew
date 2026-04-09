using HoloCrew.Models;

// Servicio para manejar login, registro, cierre de sesión, cambio de contraseña, etc.
// Se conecta con el modelo User.

namespace HoloCrew.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User> LoginAsync(string email, string password);           // inicia sesión
        Task<User> RegisterAsync(User user, string password);          // crea cuenta nueva
        Task LogoutAsync();                                             // cierra sesión
        Task<bool> IsAuthenticatedAsync();                              // comprueba si hay alguien logueado
        User GetCurrentUser();                                          // devuelve el usuario logueado
        Task<bool> ValidateTokenAsync();                                // comprueba si el token de sesión es válido
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword); // cambia la contraseña
        Task<bool> RequestPasswordResetAsync(string email);             // envía email para recuperar contraseña
    }
}