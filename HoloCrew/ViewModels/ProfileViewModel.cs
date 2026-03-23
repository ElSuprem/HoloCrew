using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Constants;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HoloCrew.ViewModels
{
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

        public ProfileViewModel(
            IAuthenticationService authenticationService,
            INavigationService navigationService)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;

            Title = AppConstants.Profile.Title;
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
                SetSuccess();
            }
            else
            {
                SetEmpty();
            }
        }

        [RelayCommand]
        private void EditProfile()
        {
            IsEditing = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        [RelayCommand]
        private async Task SaveProfileAsync()
        {
            if (User == null) return;

            await ExecuteAsync(async () =>
            {
                // TODO: Call service to update profile
                await Task.Delay(500); // Simulate API call

                SuccessMessage = AppConstants.Success.ProfileUpdated;
                IsEditing = false;
                SetSuccess();
            }, isRefresh: true);
        }

        [RelayCommand]
        private void CancelEdit()
        {
            IsEditing = false;
            LoadUserProfile();
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmNewPassword))
            {
                ErrorMessage = AppConstants.Errors.RequiredField;
                return;
            }

            if (NewPassword != ConfirmNewPassword)
            {
                ErrorMessage = AppConstants.Errors.PasswordMismatch;
                return;
            }

            if (NewPassword.Length < 6)
            {
                ErrorMessage = AppConstants.Errors.PasswordTooShort;
                return;
            }

            await ExecuteAsync(async () =>
            {
                // TODO: Call service to change password
                await Task.Delay(500);

                SuccessMessage = AppConstants.Success.PasswordChanged;
                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmNewPassword = string.Empty;
                SetSuccess();
            }, isRefresh: true);
        }

        [RelayCommand]
        private void AddAddress()
        {
            var newAddress = new Address { Label = "New Address" };
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
            await ExecuteAsync(async () =>
            {
                // TODO: Save preferences
                await Task.Delay(500);

                SuccessMessage = AppConstants.Success.PreferencesSaved;
                SetSuccess();
            }, isRefresh: true);
        }

        [RelayCommand]
        private void ViewOrderHistory()
        {
            _navigationService.NavigateTo<OrderHistoryViewModel>();
        }

        // Sidebar navigation commands
        [RelayCommand]
        private void NavigateToPersonalInfo()
        {
            // Already showing personal info - no action needed
        }

        [RelayCommand]
        private void NavigateToAddresses()
        {
            // TODO: Fase 2 - section switching for saved addresses
        }

        [RelayCommand]
        private void NavigateToPaymentMethods()
        {
            // TODO: Fase 2 - section switching for payment methods
        }

        [RelayCommand]
        private void NavigateToOrderHistory()
        {
            _navigationService.NavigateTo<OrderHistoryViewModel>();
        }

        [RelayCommand]
        private void NavigateToSettings()
        {
            _navigationService.NavigateTo<SettingsViewModel>();
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            await _authenticationService.LogoutAsync();
            _navigationService.NavigateTo<HomeViewModel>();
        }

        [RelayCommand]
        private async Task SaveChangesAsync()
        {
            await SaveProfileAsync();
        }
    }
}