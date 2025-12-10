using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Threading.Tasks;

namespace HoloCrew.ViewModels
{
    public partial class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _acceptTerms;

        public RegisterViewModel(
            IAuthenticationService authenticationService,
            INavigationService navigationService)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;

            Title = "Crear Cuenta";
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            // Limpiar error previo
            ErrorMessage = string.Empty;

            // Validar campos
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Por favor, ingresa tu nombre completo.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Por favor, ingresa tu email.";
                return;
            }

            if (!IsValidEmail(Email))
            {
                ErrorMessage = "El formato del email no es válido.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Por favor, ingresa una contraseña.";
                return;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "La contraseña debe tener al menos 6 caracteres.";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Las contraseñas no coinciden.";
                return;
            }

            if (!AcceptTerms)
            {
                ErrorMessage = "Debes aceptar los términos y condiciones.";
                return;
            }

            try
            {
                IsBusy = true;

                // Crear usuario
                var user = new User
                {
                    Email = Email,
                    FullName = FullName,
                    CreatedAt = DateTime.Now
                };

                // ⭐ ARREGLADO: AuthenticationService devuelve User, no bool
                var registeredUser = await _authenticationService.RegisterAsync(user, Password);
                var success = registeredUser != null;

                if (success)
                {
                    // Registro exitoso
                    System.Diagnostics.Debug.WriteLine($"✅ Registro exitoso: {Email}");

                    // Auto-login después del registro
                    await _authenticationService.LoginAsync(Email, Password);

                    // Navegar a Home
                    _navigationService.NavigateTo<HomeViewModel>();
                }
                else
                {
                    // El email ya existe
                    ErrorMessage = "Este email ya está registrado. Intenta iniciar sesión.";
                    System.Diagnostics.Debug.WriteLine($"❌ Registro fallido: Email duplicado {Email}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al crear la cuenta. Por favor, inténtalo más tarde.";
                System.Diagnostics.Debug.WriteLine($"❌ Error en registro: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void NavigateToLogin()
        {
            _navigationService.NavigateTo<LoginViewModel>();
        }

        [RelayCommand]
        private void RegisterWithGoogle()
        {
            // TODO: Implementar OAuth con Google
            ErrorMessage = "Registro con Google próximamente disponible.";
        }

        [RelayCommand]
        private void RegisterWithFacebook()
        {
            // TODO: Implementar OAuth con Facebook
            ErrorMessage = "Registro con Facebook próximamente disponible.";
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
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }
    }
}