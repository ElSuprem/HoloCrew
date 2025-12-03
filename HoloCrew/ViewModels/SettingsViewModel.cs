using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;

namespace HoloCrew.ViewModels
{
    /// <summary>
    /// ViewModel para la configuración de la aplicación
    /// </summary>
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

        public List<string> AvailableLanguages { get; } = new List<string>
        {
            "Español",
            "English",
            "Français",
            "Deutsch",
            "Italiano"
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
            // Cargar configuraciones guardadas
            IsDarkMode = _themeService.GetCurrentTheme() == AppTheme.Dark;

            // TODO: Cargar otras configuraciones desde settings

            // Calcular tamaño de caché
            CalculateCacheSize();
        }

        [RelayCommand]
        private void ToggleTheme()
        {
            _themeService.ToggleTheme();
            IsDarkMode = _themeService.GetCurrentTheme() == AppTheme.Dark;
        }

        [RelayCommand]
        private void ChangeLanguage(string language)
        {
            if (string.IsNullOrEmpty(language)) return;

            SelectedLanguage = language;
            // TODO: Implementar cambio de idioma
            // ResourceManager.ChangeLanguage(language);
        }

        [RelayCommand]
        private void ToggleNotifications()
        {
            // TODO: Guardar preferencia de notificaciones
        }

        [RelayCommand]
        private void ToggleSound()
        {
            // TODO: Guardar preferencia de sonido
        }

        [RelayCommand]
        private async Task ClearCacheAsync()
        {
            try
            {
                IsBusy = true;

                // TODO: Implementar limpieza de caché
                // await _cacheManager.ClearAllAsync();

                CacheSize = "0 MB";
                // TODO: Mostrar notificación de éxito
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
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

                // TODO: Implementar verificación de actualizaciones
                await Task.Delay(2000); // Simular verificación

                // TODO: Mostrar resultado
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void ViewAbout()
        {
            // TODO: Mostrar ventana "Acerca de"
        }

        [RelayCommand]
        private void ViewPrivacyPolicy()
        {
            // TODO: Abrir política de privacidad
        }

        [RelayCommand]
        private void ViewTermsOfService()
        {
            // TODO: Abrir términos de servicio
        }

        [RelayCommand]
        private void ContactSupport()
        {
            // TODO: Abrir formulario de contacto
        }

        [RelayCommand]
        private void ResetToDefaults()
        {
            // Restaurar valores por defecto
            IsDarkMode = false;
            _themeService.SetTheme(AppTheme.Light);
            SelectedLanguage = "Español";
            NotificationsEnabled = true;
            SoundEnabled = true;
            AutoUpdate = true;

            // TODO: Mostrar confirmación
        }

        private void CalculateCacheSize()
        {
            // TODO: Implementar cálculo real de caché
            CacheSize = "12.5 MB";
        }
    }
}