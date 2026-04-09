using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

// ViewModel de la página de configuración (Settings).
// Gestiona apariencia (modo oscuro, idioma), notificaciones, privacidad,
// y acciones como cambiar contraseña, cerrar sesión, eliminar cuenta, etc.
// Se conecta con AuthenticationService, NavigationService y SettingsService.
// Los cambios en las propiedades se guardan automáticamente en JSON.

namespace HoloCrew.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private bool _isUserLoggedIn;

        [ObservableProperty]
        private string _currentUserName = "Guest";

        [ObservableProperty]
        private string _currentUserEmail = "Not signed in";

        // apariencia
        [ObservableProperty]
        private bool _isDarkMode;

        [ObservableProperty]
        private string _selectedLanguage = "Spanish";

        public ObservableCollection<string> AvailableLanguages { get; } = new()
        {
            "Spanish",
            "English",
            "French",
            "German",
            "Italian"
        };

        // notificaciones
        [ObservableProperty]
        private bool _notificationsEnabled = true;

        [ObservableProperty]
        private bool _emailNotifications = true;

        [ObservableProperty]
        private bool _pushNotifications = true;

        [ObservableProperty]
        private bool _orderUpdates = true;

        [ObservableProperty]
        private bool _promotionalEmails = false;

        // privacidad
        [ObservableProperty]
        private bool _dataCollectionEnabled = true;

        [ObservableProperty]
        private bool _personalizedAds = false;

        [ObservableProperty]
        private bool _shareDataWithPartners = false;

        // modales (popups)
        [ObservableProperty]
        private bool _showPrivacyPolicy = false;

        [ObservableProperty]
        private bool _showTermsOfService = false;

        [ObservableProperty]
        private bool _showContactSupport = false;

        public event EventHandler? ScrollToTopRequested;

        public SettingsViewModel(
            IAuthenticationService authenticationService,
            INavigationService navigationService,
            ISettingsService settingsService)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;
            _settingsService = settingsService;

            Title = "Settings";

            LoadSettings();
            UpdateAuthenticationState();

            // cuando cambia una propiedad que no es de autenticación, se guarda automáticamente
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != nameof(IsUserLoggedIn) &&
                    e.PropertyName != nameof(CurrentUserName) &&
                    e.PropertyName != nameof(CurrentUserEmail) &&
                    e.PropertyName != nameof(Title) &&
                    e.PropertyName != nameof(IsBusy) &&
                    e.PropertyName != nameof(ShowPrivacyPolicy) &&
                    e.PropertyName != nameof(ShowTermsOfService) &&
                    e.PropertyName != nameof(ShowContactSupport))
                {
                    SaveSettings();
                }
            };
        }

        [RelayCommand]
        private void ChangeLanguage(string language)
        {
            SelectedLanguage = language;
            SaveSettings();
        }

        [RelayCommand]
        private void ToggleNotifications()
        {
            SaveSettings();
        }

        [RelayCommand]
        private void NavigateToProfile()
        {
            if (IsUserLoggedIn)
                _navigationService.NavigateTo<ProfileViewModel>();
            else
                _navigationService.NavigateTo<LoginViewModel>();
        }

        [RelayCommand]
        private void ChangePassword()
        {
            if (!IsUserLoggedIn)
            {
                _navigationService.NavigateTo<LoginViewModel>();
                return;
            }
            System.Diagnostics.Debug.WriteLine("Change password");
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            if (!IsUserLoggedIn) return;

            await _authenticationService.LogoutAsync();
            UpdateAuthenticationState();
        }

        [RelayCommand]
        private void DeleteAccount()
        {
            if (!IsUserLoggedIn) return;
            System.Diagnostics.Debug.WriteLine("Delete account");
        }

        [RelayCommand]
        private void ExportData()
        {
            if (!IsUserLoggedIn)
            {
                _navigationService.NavigateTo<LoginViewModel>();
                return;
            }
            System.Diagnostics.Debug.WriteLine("Export data");
        }

        // política de privacidad
        [RelayCommand]
        private void ViewPrivacyPolicy()
        {
            ScrollToTopRequested?.Invoke(this, EventArgs.Empty);
            ShowPrivacyPolicy = true;
        }

        [RelayCommand]
        private void ClosePrivacyPolicy()
        {
            ShowPrivacyPolicy = false;
        }

        // términos de servicio
        [RelayCommand]
        private void ViewTermsOfService()
        {
            ScrollToTopRequested?.Invoke(this, EventArgs.Empty);
            ShowTermsOfService = true;
        }

        [RelayCommand]
        private void CloseTermsOfService()
        {
            ShowTermsOfService = false;
        }

        // contacto con soporte
        [RelayCommand]
        private void ContactSupport()
        {
            ScrollToTopRequested?.Invoke(this, EventArgs.Empty);
            ShowContactSupport = true;
        }

        [RelayCommand]
        private void CloseContactSupport()
        {
            ShowContactSupport = false;
        }

        private async void UpdateAuthenticationState()
        {
            IsUserLoggedIn = await _authenticationService.IsAuthenticatedAsync();

            if (IsUserLoggedIn)
            {
                var user = _authenticationService.GetCurrentUser();
                CurrentUserName = user?.FullName ?? "User";
                CurrentUserEmail = user?.Email ?? "";
            }
            else
            {
                CurrentUserName = "Guest";
                CurrentUserEmail = "Not signed in";
            }
        }

        private void LoadSettings()
        {
            var settings = _settingsService.LoadSettings();

            _isDarkMode = settings.IsDarkMode;
            _selectedLanguage = settings.SelectedLanguage;
            _notificationsEnabled = settings.NotificationsEnabled;
            _emailNotifications = settings.EmailNotifications;
            _pushNotifications = settings.PushNotifications;
            _orderUpdates = settings.OrderUpdates;
            _promotionalEmails = settings.PromotionalEmails;
            _dataCollectionEnabled = settings.DataCollectionEnabled;
            _personalizedAds = settings.PersonalizedAds;
            _shareDataWithPartners = settings.ShareDataWithPartners;

            OnPropertyChanged(nameof(IsDarkMode));
            OnPropertyChanged(nameof(SelectedLanguage));
            OnPropertyChanged(nameof(NotificationsEnabled));
            OnPropertyChanged(nameof(EmailNotifications));
            OnPropertyChanged(nameof(PushNotifications));
            OnPropertyChanged(nameof(OrderUpdates));
            OnPropertyChanged(nameof(PromotionalEmails));
            OnPropertyChanged(nameof(DataCollectionEnabled));
            OnPropertyChanged(nameof(PersonalizedAds));
            OnPropertyChanged(nameof(ShareDataWithPartners));

            ThemeManager.ApplyTheme(IsDarkMode);
        }

        private void SaveSettings()
        {
            var settings = new AppSettings
            {
                IsDarkMode = IsDarkMode,
                SelectedLanguage = SelectedLanguage,
                NotificationsEnabled = NotificationsEnabled,
                EmailNotifications = EmailNotifications,
                PushNotifications = PushNotifications,
                OrderUpdates = OrderUpdates,
                PromotionalEmails = PromotionalEmails,
                DataCollectionEnabled = DataCollectionEnabled,
                PersonalizedAds = PersonalizedAds,
                ShareDataWithPartners = ShareDataWithPartners
            };

            _settingsService.SaveSettings(settings);
            ThemeManager.ApplyTheme(IsDarkMode);
        }

        public override void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            UpdateAuthenticationState();
        }
    }
}