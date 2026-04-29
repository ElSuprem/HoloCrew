using HoloCrew.Infraestructure.Supabase.Models;
using HoloCrew.Services.Interfaces;
using Supabase.Gotrue;
using System;
using System.Threading.Tasks;
using ModelsUser = HoloCrew.Models.User;
using SupaUserAttrs = Supabase.Gotrue.UserAttributes;

// Implementación del servicio de autenticación usando Supabase Auth.
// Gestiona login, registro, sesión, cambio de contraseña y reset.
// Dispara AuthStateChanged tras login, registro y logout para que la UI reaccione.

namespace HoloCrew.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly Supabase.Client _supabase;
        private ModelsUser? _currentUserCache;

        public event EventHandler? AuthStateChanged;

        public AuthenticationService(Supabase.Client supabase)
        {
            _supabase = supabase;
        }


        // ==================== LOGIN ====================

        public async Task<ModelsUser?> LoginAsync(string email, string password)
        {
            try
            {
                var session = await _supabase.Auth.SignIn(email, password);

                if (session?.User == null)
                    return null;

                // Cargar perfil para devolver el User completo
                var profile = await GetProfileByIdAsync(session.User.Id);
                _currentUserCache = profile?.ToUser();

                AuthStateChanged?.Invoke(this, EventArgs.Empty);
                return _currentUserCache;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] Login error: {ex.Message}");
                return null;
            }
        }


        // ==================== REGISTRO ====================

        public async Task<ModelsUser?> RegisterAsync(ModelsUser user, string password)
        {
            try
            {
                // Separar FullName en first_name + last_name
                var (firstName, lastName) = SplitFullName(user.FullName);

                // Pasar metadatos para que el trigger handle_new_user los recoja
                // y rellene la tabla profiles automáticamente.
                var options = new SignUpOptions
                {
                    Data = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "first_name", firstName },
                        { "last_name", lastName },
                        { "phone", user.PhoneNumber ?? string.Empty }
                    }
                };

                var session = await _supabase.Auth.SignUp(email: user.Email, password: password, options: options);

                if (session?.User == null)
                    return null;

                // Construir un User básico con los datos que ya tenemos
                // (el trigger handle_new_user creará el perfil completo en segundo plano)
                var basicUser = new ModelsUser
                {
                    Id = session.User.Id ?? string.Empty,
                    Email = session.User.Email ?? user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    CreatedAt = DateTime.Now
                };

                _currentUserCache = basicUser;

                // Intentar cargar el perfil completo en background (sin bloquear)
                // Damos un pequeño margen para que el trigger termine
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500); // espera medio segundo para que el trigger ejecute
                    var profile = await GetProfileByIdAsync(basicUser.Id);
                    if (profile != null)
                        _currentUserCache = profile.ToUser();
                });

                AuthStateChanged?.Invoke(this, EventArgs.Empty);
                return basicUser;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] Register error: {ex.Message}");
                return null;
            }
        }


        // ==================== LOGOUT ====================

        public async Task LogoutAsync()
        {
            try
            {
                await _supabase.Auth.SignOut();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] Logout error: {ex.Message}");
            }
            finally
            {
                _currentUserCache = null;
                AuthStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        // ==================== ESTADO DE SESIÓN ====================

        public Task<bool> IsAuthenticatedAsync()
        {
            var session = _supabase.Auth.CurrentSession;
            var isAuth = session != null && !string.IsNullOrEmpty(session.AccessToken);
            return Task.FromResult(isAuth);
        }

        public ModelsUser? GetCurrentUser()
        {
            // Si tenemos el user en cache, devolverlo
            if (_currentUserCache != null)
                return _currentUserCache;

            // Si hay sesión activa pero no hay cache, devolver datos básicos
            // (el perfil completo se carga la próxima vez que sea necesario)
            var supabaseUser = _supabase.Auth.CurrentUser;
            if (supabaseUser != null)
            {
                _currentUserCache = new ModelsUser
                {
                    Id = supabaseUser.Id ?? string.Empty,
                    Email = supabaseUser.Email ?? string.Empty,
                    FullName = supabaseUser.Email ?? "User"
                };
                // Cargar perfil completo en background (sin bloquear)
                _ = LoadProfileAsync(supabaseUser.Id);
                return _currentUserCache;
            }

            return null;
        }

        public Task<bool> ValidateTokenAsync()
        {
            var session = _supabase.Auth.CurrentSession;
            return Task.FromResult(session != null && !string.IsNullOrEmpty(session.AccessToken));
        }


        // ==================== CONTRASEÑA ====================

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            try
            {
                if (_supabase.Auth.CurrentUser == null)
                    return false;

                var attrs = new SupaUserAttrs { Password = newPassword };
                var updated = await _supabase.Auth.Update(attrs);
                return updated != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] ChangePassword error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            try
            {
                await _supabase.Auth.ResetPasswordForEmail(email);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] PasswordReset error: {ex.Message}");
                return false;
            }
        }


        // ==================== PERFIL ====================

        public async Task<bool> UpdateProfileAsync(string firstName, string lastName, string phone)
        {
            try
            {
                var supabaseUser = _supabase.Auth.CurrentUser;
                if (supabaseUser == null || string.IsNullOrEmpty(supabaseUser.Id))
                    return false;

                if (!Guid.TryParse(supabaseUser.Id, out var userGuid))
                    return false;

                // Actualizar la fila en public.profiles
                await _supabase
                    .From<ProfileDto>()
                    .Where(p => p.Id == userGuid)
                    .Set(p => p.FirstName, firstName ?? string.Empty)
                    .Set(p => p.LastName, lastName ?? string.Empty)
                    .Set(p => p.Phone, phone ?? string.Empty)
                    .Update();

                // Refrescar la cache local del User
                var profile = await GetProfileByIdAsync(supabaseUser.Id);
                if (profile != null)
                    _currentUserCache = profile.ToUser();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] UpdateProfile error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateAvatarUrlAsync(string avatarUrl)
        {
            try
            {
                var supabaseUser = _supabase.Auth.CurrentUser;
                if (supabaseUser == null || string.IsNullOrEmpty(supabaseUser.Id))
                    return false;

                if (!Guid.TryParse(supabaseUser.Id, out var userGuid))
                    return false;

                await _supabase
                    .From<ProfileDto>()
                    .Where(p => p.Id == userGuid)
                    .Set(p => p.AvatarUrl, avatarUrl ?? string.Empty)
                    .Update();

                // Refrescar la cache local del User
                if (_currentUserCache != null)
                    _currentUserCache.ProfileImageUrl = avatarUrl ?? string.Empty;

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] UpdateAvatarUrl error: {ex.Message}");
                return false;
            }
        }


        // ==================== HELPERS ====================

        // Carga el perfil completo desde public.profiles
        private async Task<ProfileDto?> GetProfileByIdAsync(string userId)
        {
            try
            {
                if (!Guid.TryParse(userId, out var guid))
                    return null;

                var response = await _supabase
                    .From<ProfileDto>()
                    .Where(p => p.Id == guid)
                    .Single();

                return response;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] GetProfile error: {ex.Message}");
                return null;
            }
        }

        // Carga el perfil de forma asíncrona y actualiza la cache
        private async Task LoadProfileAsync(string userId)
        {
            var profile = await GetProfileByIdAsync(userId);
            if (profile != null)
                _currentUserCache = profile.ToUser();
        }

        // Separa "Juan García López" en ("Juan", "García López")
        private (string firstName, string lastName) SplitFullName(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return (string.Empty, string.Empty);

            var trimmed = fullName.Trim();
            var firstSpace = trimmed.IndexOf(' ');

            if (firstSpace < 0)
                return (trimmed, string.Empty);

            return (trimmed.Substring(0, firstSpace), trimmed.Substring(firstSpace + 1));
        }
    }
}