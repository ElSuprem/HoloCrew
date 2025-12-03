using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.Generic;
using System.Windows.Controls;

namespace HoloCrew.ViewModels
{
    /// <summary>
    /// ViewModel para el registro de nuevos usuarios
    /// </summary>
    public partial class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _fullName;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _confirmPassword;

        [ObservableProperty]
        private string _phoneNumber;

        [ObservableProperty]
        private bool _acceptTerms;

        [ObservableProperty]
        private Dictionary<string, string> _validationErrors = new();

        [ObservableProperty]
        private bool _isRegistering;

        [ObservableProperty]
        private string _generalError;

        public RegisterViewModel(
            IAuthenticationService authenticationService,
            INavigationService navigationService)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;

            Title = "Registro";
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (!ValidateForm())
            {
                return;
            }

            if (IsRegistering) return;

            try
            {
                IsRegistering = true;
                GeneralError = null;

                var user = new User
                {
                    FullName = FullName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    CreatedAt = DateTime.Now
                };

                var registeredUser = await _authenticationService.RegisterAsync(user, Password);

                if (registeredUser != null)
                {
                    // Registro exitoso, hacer login automático
                    await _authenticationService.LoginAsync(Email, Password);
                    _navigationService.NavigateTo<HomeViewModel>();
                }
                else
                {
                    GeneralError = "Error al registrar el usuario. El email podría estar ya registrado.";
                }
            }
            catch (Exception ex)
            {
                GeneralError = "Error al registrar. Por favor, intente más tarde.";
            }
            finally
            {
                IsRegistering = false;
            }
        }

        [RelayCommand]
        private void NavigateToLogin()
        {
            _navigationService.NavigateTo<LoginViewModel>();
        }

        private bool ValidateForm()
        {
            ValidationErrors.Clear();

            // Validar nombre completo
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ValidationErrors["FullName"] = "El nombre es requerido.";
            }

            // Validar email
            if (string.IsNullOrWhiteSpace(Email))
            {
                ValidationErrors["Email"] = "El email es requerido.";
            }
            else if (!IsValidEmail(Email))
            {
                ValidationErrors["Email"] = "El email no es válido.";
            }

            // Validar contraseña
            if (string.IsNullOrWhiteSpace(Password))
            {
                ValidationErrors["Password"] = "La contraseña es requerida.";
            }
            else if (Password.Length < 6)
            {
                ValidationErrors["Password"] = "La contraseña debe tener al menos 6 caracteres.";
            }

            // Validar confirmación de contraseña
            if (Password != ConfirmPassword)
            {
                ValidationErrors["ConfirmPassword"] = "Las contraseñas no coinciden.";
            }

            // Validar términos y condiciones
            if (!AcceptTerms)
            {
                ValidationErrors["AcceptTerms"] = "Debe aceptar los términos y condiciones.";
            }

            OnPropertyChanged(nameof(ValidationErrors));
            return ValidationErrors.Count == 0;
        }

        private bool IsValidEmail(string email)
        {
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
    }
}