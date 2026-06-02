using CommunityToolkit.Mvvm.Input;
using HoloCrew.Helpers;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Windows.Input;

// ViewModel de la página de inicio de sesión.
// Valida email y contraseña antes de enviar, muestra errores en tiempo real.
// Se conecta con AuthenticationService y NavigationService.

namespace HoloCrew.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authService;
        private readonly INavigationService _navigationService;

        // Properties
        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                ValidateEmail();
                ((RelayCommand)LoginCommand).NotifyCanExecuteChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ValidatePassword();
                ((RelayCommand)LoginCommand).NotifyCanExecuteChanged();
            }
        }

        private bool _rememberMe;
        public bool RememberMe
        {
            get => _rememberMe;
            set => SetProperty(ref _rememberMe, value);
        }

        // errores de validación
        private bool _hasEmailError;
        public bool HasEmailError
        {
            get => _hasEmailError;
            set => SetProperty(ref _hasEmailError, value);
        }

        private string _emailErrorMessage;
        public string EmailErrorMessage
        {
            get => _emailErrorMessage;
            set => SetProperty(ref _emailErrorMessage, value);
        }

        private bool _hasPasswordError;
        public bool HasPasswordError
        {
            get => _hasPasswordError;
            set => SetProperty(ref _hasPasswordError, value);
        }

        private string _passwordErrorMessage;
        public string PasswordErrorMessage
        {
            get => _passwordErrorMessage;
            set => SetProperty(ref _passwordErrorMessage, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(HasError));   // recalcula la visibilidad del recuadro rojo
            }
        }

        // True solo cuando hay un mensaje de error real (no vacío).
        // El recuadro rojo se ata a esto, así no parpadea al poner ErrorMessage = "".
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Commands
        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        public LoginViewModel(
            IAuthenticationService authService,
            INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;

            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            ForgotPasswordCommand = new RelayCommand(ExecuteForgotPassword);
            NavigateToRegisterCommand = new RelayCommand(ExecuteNavigateToRegister);
        }

        // valida el formato del email
        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                HasEmailError = false;
                EmailErrorMessage = string.Empty;
                return;
            }

            if (!ValidationHelper.IsValidEmail(Email))
            {
                HasEmailError = true;
                EmailErrorMessage = "Please enter a valid email";
            }
            else
            {
                HasEmailError = false;
                EmailErrorMessage = string.Empty;
            }
        }

        // valida la longitud de la contraseña
        private void ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                HasPasswordError = false;
                PasswordErrorMessage = string.Empty;
                return;
            }

            if (!ValidationHelper.IsValidPassword(Password))
            {
                HasPasswordError = true;
                PasswordErrorMessage = "Password must be at least 8 characters";
            }
            else
            {
                HasPasswordError = false;
                PasswordErrorMessage = string.Empty;
            }
        }

        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !HasEmailError &&
                   !HasPasswordError &&
                   !IsLoading;
        }

        private async void ExecuteLogin()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                ValidateEmail();
                ValidatePassword();

                if (HasEmailError || HasPasswordError)
                {
                    return;
                }

                var user = await _authService.LoginAsync(Email, Password);

                if (user != null)
                {
                    _navigationService.NavigateTo<HomeViewModel>();
                }
                else
                {
                    ErrorMessage = "Login failed. Please check your credentials.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An unexpected error occurred. Please try again.";
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ExecuteForgotPassword()
        {
            // si existe ForgotPasswordViewModel, descomentar la línea de abajo
            // _navigationService.NavigateTo<ForgotPasswordViewModel>();
            ErrorMessage = "Password recovery feature coming soon.";
        }

        private void ExecuteNavigateToRegister()
        {
            _navigationService.NavigateTo<RegisterViewModel>();
        }
    }
}