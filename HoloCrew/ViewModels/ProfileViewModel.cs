using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Constants;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

// ViewModel de la página de perfil del usuario.
// Carga datos reales desde Supabase, permite editarlos, gestiona avatar
// y muestra los pedidos recientes del usuario.

namespace HoloCrew.ViewModels
{
    public partial class ProfileViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;
        private readonly IOrderService _orderService;
        private readonly IAvatarService _avatarService;

        [ObservableProperty]
        private User _user;

        [ObservableProperty]
        private string _userInitials = string.Empty;

        [ObservableProperty]
        private string _userFullName = string.Empty;

        [ObservableProperty]
        private string _userEmail = string.Empty;

        [ObservableProperty]
        private string _avatarUrl = string.Empty;

        [ObservableProperty]
        private bool _hasAvatar;

        [ObservableProperty]
        private string _firstName = string.Empty;

        [ObservableProperty]
        private string _lastName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _phone = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Order> _recentOrders = new();

        [ObservableProperty]
        private string _currentPassword = string.Empty;

        [ObservableProperty]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        private string _confirmNewPassword = string.Empty;

        [ObservableProperty]
        private string _successMessage = string.Empty;

        [ObservableProperty]
        private bool _isEditing;

        public ProfileViewModel(
            IAuthenticationService authenticationService,
            INavigationService navigationService,
            IOrderService orderService,
            IAvatarService avatarService)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;
            _orderService = orderService;
            _avatarService = avatarService;

            Title = AppConstants.Profile.Title;
        }


        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadProfileAsync();
            await LoadRecentOrdersAsync();
        }


        private async Task LoadProfileAsync()
        {
            await ExecuteAsync(async () =>
            {
                User = _authenticationService.GetCurrentUser();

                if (User == null)
                {
                    SetEmpty();
                    return;
                }

                var (first, last) = SplitFullName(User.FullName);

                FirstName = first;
                LastName = last;
                Email = User.Email ?? string.Empty;
                Phone = User.PhoneNumber ?? string.Empty;

                UserFullName = User.FullName ?? "User";
                UserEmail = User.Email ?? string.Empty;
                UserInitials = GetInitials(first, last);

                AvatarUrl = User.ProfileImageUrl ?? string.Empty;
                HasAvatar = !string.IsNullOrEmpty(AvatarUrl);

                SetSuccess();
                await Task.CompletedTask;
            });
        }


        private async Task LoadRecentOrdersAsync()
        {
            try
            {
                if (User == null || string.IsNullOrEmpty(User.Id))
                    return;

                var orders = await _orderService.GetUserOrdersAsync(User.Id);
                var recent = orders.Take(5).ToList();
                RecentOrders = new ObservableCollection<Order>(recent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Profile] LoadRecentOrders error: {ex.Message}");
            }
        }


        // Abre un diálogo de selección de archivo, copia la imagen al almacén local
        // y actualiza la URL del avatar en BD.
        [RelayCommand]
        private async Task ChangeAvatarAsync()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Select your avatar",
                    Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
                    Multiselect = false
                };

                var result = dialog.ShowDialog();
                if (result != true || string.IsNullOrEmpty(dialog.FileName))
                    return;

                // Subir la imagen (copia local mockup)
                var newAvatarUrl = await _avatarService.UploadAvatarAsync(dialog.FileName);

                if (string.IsNullOrEmpty(newAvatarUrl))
                {
                    ErrorMessage = "Could not save the image. Try another file.";
                    return;
                }

                // Persistir la URL en BD
                var ok = await _authenticationService.UpdateAvatarUrlAsync(newAvatarUrl);

                if (ok)
                {
                    // Limpiamos primero para forzar a WPF a soltar el binding antiguo,
                    // y luego asignamos la URL nueva. Sin esto, si la URL nueva fuera idéntica
                    // a la anterior (mismo path + cache-busting), WPF podría no refrescar.
                    AvatarUrl = string.Empty;
                    HasAvatar = false;

                    AvatarUrl = newAvatarUrl;
                    HasAvatar = true;
                    SuccessMessage = "Avatar updated successfully";
                    SetSuccess();
                }
                else
                {
                    ErrorMessage = "Could not update avatar in database";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error changing avatar";
                System.Diagnostics.Debug.WriteLine($"[Profile] ChangeAvatar error: {ex.Message}");
            }
        }


        [RelayCommand]
        private async Task SaveChangesAsync()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                ErrorMessage = "First name cannot be empty.";
                return;
            }

            await ExecuteAsync(async () =>
            {
                var ok = await _authenticationService.UpdateProfileAsync(FirstName, LastName, Phone);

                if (ok)
                {
                    SuccessMessage = AppConstants.Success.ProfileUpdated;

                    UserFullName = $"{FirstName} {LastName}".Trim();
                    UserInitials = GetInitials(FirstName, LastName);

                    SetSuccess();
                }
                else
                {
                    ErrorMessage = "Could not save changes.";
                }
            }, isRefresh: true);
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

            if (NewPassword.Length < 8)
            {
                ErrorMessage = AppConstants.Errors.PasswordTooShort;
                return;
            }

            await ExecuteAsync(async () =>
            {
                var ok = await _authenticationService.ChangePasswordAsync(CurrentPassword, NewPassword);

                if (ok)
                {
                    SuccessMessage = AppConstants.Success.PasswordChanged;
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmNewPassword = string.Empty;
                    SetSuccess();
                }
                else
                {
                    ErrorMessage = "Could not change password.";
                }
            }, isRefresh: true);
        }


        [RelayCommand]
        private async Task LogoutAsync()
        {
            await _authenticationService.LogoutAsync();
            _navigationService.NavigateTo<HomeViewModel>();
        }


        [RelayCommand]
        private void NavigateToPersonalInfo() { }

        [RelayCommand]
        private void NavigateToAddresses() { }

        [RelayCommand]
        private void NavigateToPaymentMethods() { }

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
        private void ViewOrderDetail(Order order)
        {
            if (order == null) return;
            _navigationService.NavigateTo<OrderDetailViewModel>(order.Id);
        }


        // ==================== HELPERS ====================

        private (string first, string last) SplitFullName(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return (string.Empty, string.Empty);

            var trimmed = fullName.Trim();
            var firstSpace = trimmed.IndexOf(' ');

            if (firstSpace < 0)
                return (trimmed, string.Empty);

            return (trimmed.Substring(0, firstSpace), trimmed.Substring(firstSpace + 1));
        }

        private string GetInitials(string first, string last)
        {
            string firstChar = !string.IsNullOrEmpty(first) ? first[0].ToString().ToUpper() : "";
            string lastChar = !string.IsNullOrEmpty(last) ? last[0].ToString().ToUpper() : "";
            var initials = (firstChar + lastChar).Trim();
            return string.IsNullOrEmpty(initials) ? "?" : initials;
        }
    }
}