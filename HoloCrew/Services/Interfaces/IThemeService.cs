// Servicio para cambiar entre tema claro y oscuro.
// Guarda la preferencia del usuario y aplica los colores correspondientes.

namespace HoloCrew.Services.Interfaces
{
    public enum AppTheme
    {
        Light,
        Dark
    }

    public interface IThemeService
    {
        void SetTheme(AppTheme theme);           // cambiar a un tema concreto
        AppTheme GetCurrentTheme();              // saber qué tema está activo ahora
        void ToggleTheme();                      // cambiar de claro a oscuro o viceversa
        void ApplySavedTheme();                  // cargar el tema que el usuario tenía guardado
    }
}