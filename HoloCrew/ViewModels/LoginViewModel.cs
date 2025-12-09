using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
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
        private bool _rememberMe;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isLoading = false;

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
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Por favor, completa todos los campos";
                    return;
                }

                // ⭐ ARREGLADO: LoginAsync devuelve User, no bool
                var user = await _authenticationService.LoginAsync(Email, Password);

                if (user != null)
                {
                    _navigationService.NavigateTo<HomeViewModel>();
                }
                else
                {
                    ErrorMessage = "Email o contraseña incorrectos";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al iniciar sesión. Intenta nuevamente.";
            }
            finally
            {
                IsLoading = false;
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
            // Implementar recuperación de contraseña
        }
    }
}
