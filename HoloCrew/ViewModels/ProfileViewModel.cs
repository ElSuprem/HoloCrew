using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;

namespace HoloCrew.ViewModels
{
    /// <summary>
    /// ViewModel para el perfil del usuario
    /// Permite ver y editar información personal
    /// </summary>
    public partial class ProfileViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private User _user;

        [ObservableProperty]
        private bool _isEditing;

        [ObservableProperty]
        private ObservableCollection<Address> _savedAddresses = new();

        [ObservableProperty]
        private ObservableCollection<PaymentMethod> _savedPaymentMethods = new();

        [ObservableProperty]
        private string _currentPassword;

        [ObservableProperty]
        private string _newPassword;

        [ObservableProperty]
        private string _confirmNewPassword;

        [ObservableProperty]
        private NotificationSettings _notificationPreferences;

        [ObservableProperty]
        private string _successMessage;

        [ObservableProperty]
        private string _errorMessage;

        public ProfileViewModel(
            IAuthenticationService authenticationService,
            INavigationService navigationService)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;

            Title = "Mi Perfil";
        }

        public override void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            User = _authenticationService.GetCurrentUser();

            if (User != null)
            {
                SavedAddresses = new ObservableCollection<Address>(User.Addresses ?? new List<Address>());
                SavedPaymentMethods = new ObservableCollection<PaymentMethod>(User.PaymentMethods ?? new List<PaymentMethod>());
                NotificationPreferences = User.NotificationPreferences ?? new NotificationSettings();
            }
        }

        [RelayCommand]
        private void EditProfile()
        {
            IsEditing = true;
            ErrorMessage = null;
            SuccessMessage = null;
        }

        [RelayCommand]
        private async Task SaveProfileAsync()
        {
            if (User == null) return;

            try
            {
                IsBusy = true;
                ErrorMessage = null;

                // TODO: Llamar al servicio para actualizar el perfil
                // await _userService.UpdateProfileAsync(User);

                SuccessMessage = "Perfil actualizado correctamente.";
                IsEditing = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al guardar el perfil. Intente nuevamente.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void CancelEdit()
        {
            IsEditing = false;
            LoadUserProfile(); // Recargar datos originales
            ErrorMessage = null;
            SuccessMessage = null;
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            // Validar campos
            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmNewPassword))
            {
                ErrorMessage = "Complete todos los campos de contraseña.";
                return;
            }

            if (NewPassword != ConfirmNewPassword)
            {
                ErrorMessage = "Las contraseñas nuevas no coinciden.";
                return;
            }

            if (NewPassword.Length < 6)
            {
                ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres.";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = null;

                // TODO: Implementar cambio de contraseña
                // await _authenticationService.ChangePasswordAsync(CurrentPassword, NewPassword);

                SuccessMessage = "Contraseña actualizada correctamente.";

                // Limpiar campos
                CurrentPassword = null;
                NewPassword = null;
                ConfirmNewPassword = null;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al cambiar la contraseña. Verifique su contraseña actual.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void AddAddress()
        {
            var newAddress = new Address { Label = "Nueva dirección" };
            SavedAddresses.Add(newAddress);
        }

        [RelayCommand]
        private void RemoveAddress(Address address)
        {
            if (address != null)
            {
                SavedAddresses.Remove(address);
            }
        }

        [RelayCommand]
        private void AddPaymentMethod()
        {
            var newPayment = new PaymentMethod { Type = PaymentType.CreditCard };
            SavedPaymentMethods.Add(newPayment);
        }

        [RelayCommand]
        private void RemovePaymentMethod(PaymentMethod paymentMethod)
        {
            if (paymentMethod != null)
            {
                SavedPaymentMethods.Remove(paymentMethod);
            }
        }

        [RelayCommand]
        private async Task UpdateNotificationPreferencesAsync()
        {
            try
            {
                IsBusy = true;

                // TODO: Guardar preferencias
                // await _userService.UpdateNotificationPreferencesAsync(NotificationPreferences);

                SuccessMessage = "Preferencias actualizadas correctamente.";
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al actualizar preferencias.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void ViewOrderHistory()
        {
            _navigationService.NavigateTo<OrderHistoryViewModel>();
        }
    }
}