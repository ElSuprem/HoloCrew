using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Constants;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Threading.Tasks;

// ViewModel de la página de registro de usuario.
// Valida los campos (nombre, email, contraseña, términos) y crea una cuenta nueva.
// Se conecta con AuthenticationService y NavigationService.

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
        private bool _acceptTerms;

        public RegisterViewModel(
            IAuthenticationService authenticationService,
            INavigationService navigationService)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;

            Title = AppConstants.Auth.Register;
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = AppConstants.Errors.RequiredField;
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = AppConstants.Errors.RequiredField;
                return;
            }

            if (!IsValidEmail(Email))
            {
                ErrorMessage = AppConstants.Errors.InvalidEmail;
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = AppConstants.Errors.RequiredField;
                return;
            }

            if (Password.Length < 8)
            {
                ErrorMessage = AppConstants.Errors.PasswordTooShort;
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = AppConstants.Errors.PasswordMismatch;
                return;
            }

            if (!AcceptTerms)
            {
                ErrorMessage = "You must accept the terms and conditions.";
                return;
            }

            await ExecuteAsync(async () =>
            {
                var user = new User
                {
                    Email = Email,
                    FullName = FullName,
                    CreatedAt = DateTime.Now
                };

                var registeredUser = await _authenticationService.RegisterAsync(user, Password);

                if (registeredUser != null)
                {
                    await _authenticationService.LoginAsync(Email, Password);
                    _navigationService.NavigateTo<HomeViewModel>();
                }
                else
                {
                    SetError(AppConstants.Errors.EmailInUse);
                }
            });
        }

        [RelayCommand]
        private void NavigateToLogin()
        {
            _navigationService.NavigateTo<LoginViewModel>();
        }

        [RelayCommand]
        private void RegisterWithGoogle()
        {
            ErrorMessage = "Google registration coming soon.";
        }

        [RelayCommand]
        private void RegisterWithFacebook()
        {
            ErrorMessage = "Facebook registration coming soon.";
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

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

        public override void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            ErrorMessage = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }
    }
}