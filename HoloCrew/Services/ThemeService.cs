using HoloCrew.Services;
using HoloCrew.Services.Interfaces;
using System;
using System.Windows;

namespace HoloCrew.Services
{
    /// <summary>
    /// Implementación del servicio de temas
    /// </summary>
    public class ThemeService : IThemeService
    {
        private AppTheme _currentTheme = AppTheme.Light;

        public void SetTheme(AppTheme theme)
        {
            _currentTheme = theme;

            // Cambiar los recursos de la aplicación
            var themeDictionary = theme == AppTheme.Dark
                ? new ResourceDictionary { Source = new Uri("Resources/Themes/Dark.xaml", UriKind.Relative) }
                : new ResourceDictionary { Source = new Uri("Resources/Themes/Light.xaml", UriKind.Relative) };

            try
            {
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(themeDictionary);

                // Guardar preferencia
                SaveThemePreference(theme);
            }
            catch (Exception)
            {
                // Si falla (ej: archivos de tema no existen), continuar sin error
                System.Diagnostics.Debug.WriteLine($"No se pudo cargar el tema: {theme}");
            }
        }

        public AppTheme GetCurrentTheme()
        {
            return _currentTheme;
        }

        public void ToggleTheme()
        {
            var newTheme = _currentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
            SetTheme(newTheme);
        }

        public void ApplySavedTheme()
        {
            // Cargar tema guardado desde configuración
            var savedTheme = LoadThemePreference();
            SetTheme(savedTheme);
        }

        private void SaveThemePreference(AppTheme theme)
        {
            // TODO: Guardar en configuración persistente
            // Properties.Settings.Default.Theme = theme.ToString();
            // Properties.Settings.Default.Save();
        }

        private AppTheme LoadThemePreference()
        {
            // TODO: Cargar desde configuración persistente
            // var savedTheme = Properties.Settings.Default.Theme;
            // return Enum.TryParse<AppTheme>(savedTheme, out var theme) ? theme : AppTheme.Light;

            return AppTheme.Light;
        }
    }
}