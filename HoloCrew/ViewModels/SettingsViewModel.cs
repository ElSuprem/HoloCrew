using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace HoloCrew.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly IThemeService _themeService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private bool _isDarkMode;

        [ObservableProperty]
        private string _selectedLanguage = "Español";

        [ObservableProperty]
        private bool _notificationsEnabled = true;

        [ObservableProperty]
        private bool _soundEnabled = true;

        [ObservableProperty]
        private string _cacheSize = "0 MB";

        [ObservableProperty]
        private string _appVersion = "1.0.0";

        [ObservableProperty]
        private bool _autoUpdate = true;

        // ⭐ PROPIEDADES AGREGADAS PARA BINDINGS
        [ObservableProperty]
        private string _selectedCurrency = "EUR";

        [ObservableProperty]
        private string _selectedTimeZone = "Europe/Madrid";

        [ObservableProperty]
        private bool _isLightThemeSelected = true;

        [ObservableProperty]
        private bool _isDarkThemeSelected = false;

        [ObservableProperty]
        private int _selectedFontSize = 14;

        [ObservableProperty]
        private bool _emailNotificationsEnabled = true;

        [ObservableProperty]
        private bool _pushNotificationsEnabled = true;

        [ObservableProperty]
        private bool _smsNotificationsEnabled = false;

        [ObservableProperty]
        private bool _shareUsageData = true;

        [ObservableProperty]
        private bool _saveSearchHistory = true;

        [ObservableProperty]
        private bool _showPublicProfile = true;

        public List<string> AvailableLanguages { get; } = new List<string>
        {
            "Español",
            "English",
            "Français",
            "Deutsch",
            "Italiano"
        };

        // ⭐ LISTAS AGREGADAS
        public List<string> Languages => AvailableLanguages;

        public List<string> Currencies { get; } = new List<string>
        {
            "EUR",
            "USD",
            "GBP",
            "JPY"
        };

        public List<string> TimeZones { get; } = new List<string>
        {
            "Europe/Madrid",
            "America/New_York",
            "America/Los_Angeles",
            "Asia/Tokyo"
        };

        public List<int> FontSizes { get; } = new List<int>
        {
            12, 14, 16, 18, 20
        };

        public SettingsViewModel(
            IThemeService themeService,
            INavigationService navigationService)
        {
            _themeService = themeService;
            _navigationService = navigationService;

            Title = "Configuración";

            LoadSettings();
        }

        private void LoadSettings()
        {
            IsDarkMode = _themeService.GetCurrentTheme() == AppTheme.Dark;
            IsLightThemeSelected = !IsDarkMode;
            IsDarkThemeSelected = IsDarkMode;

            CalculateCacheSize();
        }

        [RelayCommand]
        private void ToggleTheme()
        {
            _themeService.ToggleTheme();
            IsDarkMode = _themeService.GetCurrentTheme() == AppTheme.Dark;
            IsLightThemeSelected = !IsDarkMode;
            IsDarkThemeSelected = IsDarkMode;
        }

        [RelayCommand]
        private void ChangeLanguage(string language)
        {
            if (string.IsNullOrEmpty(language)) return;
            SelectedLanguage = language;
        }

        [RelayCommand]
        private void ToggleNotifications()
        {
            // Guardar preferencia
        }

        [RelayCommand]
        private void ToggleSound()
        {
            // Guardar preferencia
        }

        [RelayCommand]
        private async Task ClearCacheAsync()
        {
            try
            {
                IsBusy = true;
                await Task.Delay(1000);
                CacheSize = "0 MB";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CheckForUpdatesAsync()
        {
            try
            {
                IsBusy = true;
                await Task.Delay(2000);
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ⭐ COMANDOS AGREGADOS
        [RelayCommand]
        private void SavePreferences()
        {
            // Guardar todas las preferencias
        }

        [RelayCommand]
        private async Task DownloadDataAsync()
        {
            try
            {
                IsBusy = true;
                await Task.Delay(2000);
                // Descargar datos del usuario
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteAccountAsync()
        {
            // Confirmación y eliminación de cuenta
            await Task.CompletedTask;
        }

        [RelayCommand]
        private void ViewAbout()
        {
            // Mostrar ventana "Acerca de"
        }

        [RelayCommand]
        private void ViewPrivacyPolicy()
        {
            // Abrir política de privacidad
        }

        [RelayCommand]
        private void ViewTermsOfService()
        {
            // Abrir términos de servicio
        }

        [RelayCommand]
        private void ContactSupport()
        {
            // Abrir formulario de contacto
        }

        [RelayCommand]
        private void ResetToDefaults()
        {
            IsDarkMode = false;
            _themeService.SetTheme(AppTheme.Light);
            SelectedLanguage = "Español";
            NotificationsEnabled = true;
            SoundEnabled = true;
            AutoUpdate = true;
            IsLightThemeSelected = true;
            IsDarkThemeSelected = false;
        }

        private void CalculateCacheSize()
        {
            CacheSize = "12.5 MB";
        }
    }
}
