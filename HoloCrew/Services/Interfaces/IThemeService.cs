namespace HoloCrew.Services.Interfaces
{
    /// <summary>
    /// Enumeración de temas disponibles
    /// </summary>
    public enum AppTheme
    {
        Light,
        Dark
    }

    /// <summary>
    /// Servicio para gestión de temas de la aplicación
    /// </summary>
    public interface IThemeService
    {
        /// <summary>
        /// Establece el tema de la aplicación
        /// </summary>
        void SetTheme(AppTheme theme);

        /// <summary>
        /// Obtiene el tema actual
        /// </summary>
        AppTheme GetCurrentTheme();

        /// <summary>
        /// Alterna entre tema claro y oscuro
        /// </summary>
        void ToggleTheme();

        /// <summary>
        /// Aplica el tema guardado en las preferencias
        /// </summary>
        void ApplySavedTheme();
    }
}