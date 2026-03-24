using HoloCrew.Services.Interfaces;
using System;

namespace HoloCrew.Services
{
    /// <summary>
    /// Servicio de temas — Delega a ThemeManager para el cambio real.
    /// Ya NO intenta cargar Dark.xaml/Light.xaml (no existen).
    /// </summary>
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
            // Apply via ThemeManager — no ResourceDictionary file loading
            ThemeManager.ApplyTheme(savedTheme == AppTheme.Dark);
        }

        private void SaveThemePreference(AppTheme theme)
        {
            // TODO Fase 2: Guardar en configuración persistente (Supabase o local settings)
            System.Diagnostics.Debug.WriteLine($"[ThemeService] Saved preference: {theme}");
        }

        private AppTheme LoadThemePreference()
        {
            // TODO Fase 2: Cargar desde configuración persistente
            return AppTheme.Light;
        }
    }
}