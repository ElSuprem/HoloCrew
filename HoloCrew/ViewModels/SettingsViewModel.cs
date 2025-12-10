using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
        private string _currentUserName = "Invitado";

        [ObservableProperty]
        private string _currentUserEmail = "No has iniciado sesión";

        // APARIENCIA
        [ObservableProperty]
        private bool _isDarkMode;

        [ObservableProperty]
        private string _selectedLanguage = "Español";

        public ObservableCollection<string> AvailableLanguages { get; } = new()
        {
            "Español",
            "English",
            "Français",
            "Deutsch",
            "Italiano"
        };

        // NOTIFICACIONES
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

        // PRIVACIDAD
        [ObservableProperty]
        private bool _dataCollectionEnabled = true;

        [ObservableProperty]
        private bool _personalizedAds = false;

        [ObservableProperty]
        private bool _shareDataWithPartners = false;

        public SettingsViewModel(
            IAuthenticationService authenticationService,
            INavigationService navigationService,
            ISettingsService settingsService)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;
            _settingsService = settingsService;

            Title = "Configuración";

            // Cargar configuración guardada
            LoadSettings();

            // Actualizar estado de autenticación
            UpdateAuthenticationState();

            // ⭐ Suscribirse a cambios de propiedades para guardar automáticamente
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != nameof(IsUserLoggedIn) &&
                    e.PropertyName != nameof(CurrentUserName) &&
                    e.PropertyName != nameof(CurrentUserEmail) &&
                    e.PropertyName != nameof(Title) &&
                    e.PropertyName != nameof(IsBusy))
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

            System.Diagnostics.Debug.WriteLine($"🌍 Idioma cambiado a: {language}");
        }

        [RelayCommand]
        private void ToggleNotifications()
        {
            SaveSettings();
            System.Diagnostics.Debug.WriteLine($"🔔 Notificaciones: {(NotificationsEnabled ? "Activadas" : "Desactivadas")}");
        }

        [RelayCommand]
        private void NavigateToProfile()
        {
            if (IsUserLoggedIn)
            {
                _navigationService.NavigateTo<ProfileViewModel>();
            }
            else
            {
                _navigationService.NavigateTo<LoginViewModel>();
            }
        }

        [RelayCommand]
        private void ChangePassword()
        {
            if (!IsUserLoggedIn)
            {
                _navigationService.NavigateTo<LoginViewModel>();
                return;
            }

            System.Diagnostics.Debug.WriteLine("🔒 Cambiar contraseña");
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            if (!IsUserLoggedIn)
            {
                return;
            }

            await _authenticationService.LogoutAsync();
            UpdateAuthenticationState();

            System.Diagnostics.Debug.WriteLine("👋 Sesión cerrada");
        }

        [RelayCommand]
        private void DeleteAccount()
        {
            if (!IsUserLoggedIn)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine("⚠️ Eliminar cuenta");
        }

        [RelayCommand]
        private void ClearCache()
        {
            System.Diagnostics.Debug.WriteLine("🗑️ Caché limpiada");
        }

        [RelayCommand]
        private void ExportData()
        {
            if (!IsUserLoggedIn)
            {
                _navigationService.NavigateTo<LoginViewModel>();
                return;
            }

            System.Diagnostics.Debug.WriteLine("📥 Exportar datos");
        }

        [RelayCommand]
        private void ViewPrivacyPolicy()
        {
            System.Diagnostics.Debug.WriteLine("📄 Ver política de privacidad");
        }

        [RelayCommand]
        private void ViewTermsOfService()
        {
            System.Diagnostics.Debug.WriteLine("📄 Ver términos de servicio");
        }

        [RelayCommand]
        private void ContactSupport()
        {
            System.Diagnostics.Debug.WriteLine("💬 Contactar soporte");
        }

        private async void UpdateAuthenticationState()
        {
            IsUserLoggedIn = await _authenticationService.IsAuthenticatedAsync();

            if (IsUserLoggedIn)
            {
                var user = _authenticationService.GetCurrentUser();
                CurrentUserName = user?.FullName ?? "Usuario";
                CurrentUserEmail = user?.Email ?? "";
            }
            else
            {
                CurrentUserName = "Invitado";
                CurrentUserEmail = "No has iniciado sesión";
            }
        }

        private void LoadSettings()
        {
            var settings = _settingsService.LoadSettings();

            // Cargar valores sin disparar PropertyChanged
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

            // Notificar cambios
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

            // ⭐ Aplicar tema cargado usando ThemeManager
            ThemeManager.ApplyTheme(IsDarkMode);

            System.Diagnostics.Debug.WriteLine("⚙️ Configuración cargada desde JSON");
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

            // ⭐ Aplicar tema cuando IsDarkMode cambia
            ThemeManager.ApplyTheme(IsDarkMode);
        }

        public override void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            UpdateAuthenticationState();
        }
    }
}