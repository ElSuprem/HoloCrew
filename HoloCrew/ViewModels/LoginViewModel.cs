using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Threading.Tasks;

namespace HoloCrew.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _rememberMe;

        public LoginViewModel(
            IAuthenticationService authenticationService,
            INavigationService navigationService)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;

            Title = "Iniciar Sesión";
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            // Limpiar error previo
            ErrorMessage = string.Empty;

            // Validar campos
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Por favor, ingresa tu email.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Por favor, ingresa tu contraseña.";
                return;
            }

            if (!IsValidEmail(Email))
            {
                ErrorMessage = "El formato del email no es válido.";
                return;
            }

            try
            {
                IsBusy = true;

                // ⭐ ARREGLADO: AuthenticationService devuelve User, no bool
                var user = await _authenticationService.LoginAsync(Email, Password);
                var success = user != null;

                if (success)
                {
                    // Login exitoso
                    System.Diagnostics.Debug.WriteLine($"✅ Login exitoso: {Email}");

                    // Navegar a Home
                    _navigationService.NavigateTo<HomeViewModel>();
                }
                else
                {
                    // Credenciales incorrectas
                    ErrorMessage = "Email o contraseña incorrectos. Inténtalo de nuevo.";
                    System.Diagnostics.Debug.WriteLine($"❌ Login fallido: {Email}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al iniciar sesión. Por favor, inténtalo más tarde.";
                System.Diagnostics.Debug.WriteLine($"❌ Error en login: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void NavigateToRegister()
        {
            _navigationService.NavigateTo<RegisterViewModel>();
        }

        [RelayCommand]
        private void ForgotPassword()
        {
            // TODO: Implementar recuperación de contraseña
            ErrorMessage = "Funcionalidad en desarrollo. Contacta con soporte.";
        }

        [RelayCommand]
        private void LoginWithGoogle()
        {
            // TODO: Implementar OAuth con Google
            ErrorMessage = "Login con Google próximamente disponible.";
        }

        [RelayCommand]
        private void LoginWithFacebook()
        {
            // TODO: Implementar OAuth con Facebook
            ErrorMessage = "Login con Facebook próximamente disponible.";
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Limpiar campos al navegar
        public override void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            ErrorMessage = string.Empty;
            Password = string.Empty; // Limpiar contraseña por seguridad
        }
    }
}