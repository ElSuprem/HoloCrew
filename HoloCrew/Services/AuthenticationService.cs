using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services.Interfaces;

// Implementación del servicio de autenticación.
// Usa IUserRepository para acceder a los datos de usuario.
// NOTA: Las contraseñas se manejan sin encriptar en los datos mock (en producción habría que usar hash).

namespace HoloCrew.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private User _currentUser;
        private string _authToken;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);

                if (user == null)
                {
                    return null;
                }

                // en producción habría que comparar la contraseña encriptada con BCrypt o similar
                _currentUser = user;
                _authToken = GenerateToken(user);

                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(user.Email);

                if (existingUser != null)
                {
                    return null; // el email ya está registrado
                }

                var createdUser = await _userRepository.CreateAsync(user);
                return createdUser;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Task LogoutAsync()
        {
            _currentUser = null;
            _authToken = null;
            return Task.CompletedTask;
        }

        public Task<bool> IsAuthenticatedAsync()
        {
            return Task.FromResult(_currentUser != null && !string.IsNullOrEmpty(_authToken));
        }

        public User GetCurrentUser()
        {
            return _currentUser;
        }

        public async Task<bool> ValidateTokenAsync()
        {
            if (string.IsNullOrEmpty(_authToken))
            {
                return false;
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            if (_currentUser == null)
            {
                return false;
            }

            try
            {
                // en producción habría que verificar la contraseña actual y hashear la nueva
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);

                if (user == null)
                {
                    return false;
                }

                // en producción se enviaría un email con un enlace para resetear la contraseña
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // genera un token simple para la sesión (en producción sería JWT)
        private string GenerateToken(User user)
        {
            return $"token_{user.Id}_{DateTime.Now.Ticks}";
        }
    }
}