using HoloCrew.Services.Interfaces;
using System;

// Servicio de temas. Delega el cambio real a ThemeManager.
// No intenta cargar archivos Dark.xaml / Light.xaml porque no existen.

namespace HoloCrew.Services
{
    public class ThemeService : IThemeService
    {
        private AppTheme _currentTheme = AppTheme.Light;

        public void SetTheme(AppTheme theme)
        {
            _currentTheme = theme;
            ThemeManager.ApplyTheme(theme == AppTheme.Dark);
            SaveThemePreference(theme);
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
            var savedTheme = LoadThemePreference();
            _currentTheme = savedTheme;
            ThemeManager.ApplyTheme(savedTheme == AppTheme.Dark);
        }

        // guarda la preferencia del tema (pendiente de implementar con base de datos o settings)
        private void SaveThemePreference(AppTheme theme)
        {
            System.Diagnostics.Debug.WriteLine($"[ThemeService] Saved preference: {theme}");
        }

        // carga la preferencia del tema guardada (pendiente de implementar)
        private AppTheme LoadThemePreference()
        {
            return AppTheme.Light;  // por defecto, tema claro
        }
    }
}