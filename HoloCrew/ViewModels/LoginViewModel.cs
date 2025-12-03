using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;

namespace HoloCrew.ViewModels
{
    /// <summary>
    /// ViewModel para el inicio de sesión
    /// </summary>
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private bool _rememberMe;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _isLoggingIn;

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
            // Validación básica
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Por favor, complete todos los campos.";
                return;
            }

            if (IsLoggingIn) return;

            try
            {
                IsLoggingIn = true;
                ErrorMessage = null;

                var user = await _authenticationService.LoginAsync(Email, Password);

                if (user != null)
                {
                    // Login exitoso
                    // TODO: Actualizar estado global de usuario logueado
                    _navigationService.NavigateTo<HomeViewModel>();
                }
                else
                {
                    ErrorMessage = "Credenciales incorrectas. Intente nuevamente.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al iniciar sesión. Por favor, intente más tarde.";
            }
            finally
            {
                IsLoggingIn = false;
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
            ErrorMessage = "Funcionalidad de recuperación de contraseña próximamente.";
        }

        [RelayCommand]
        private void LoginWithGoogle()
        {
            // TODO: Implementar login con Google
            ErrorMessage = "Login con Google próximamente.";
        }

        [RelayCommand]
        private void LoginWithFacebook()
        {
            // TODO: Implementar login con Facebook
            ErrorMessage = "Login con Facebook próximamente.";
        }
    }
}