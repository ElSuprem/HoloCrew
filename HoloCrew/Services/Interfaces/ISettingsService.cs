using HoloCrew.Models;

// Servicio para guardar y cargar la configuración de la app (modo oscuro, idioma, notificaciones, etc.).
// Los datos se guardan en un archivo JSON en %AppData%/HoloCrew/settings.json.

namespace HoloCrew.Services.Interfaces
{
    public interface ISettingsService
    {
        AppSettings LoadSettings();        // cargar configuración desde el archivo
        void SaveSettings(AppSettings settings);  // guardar configuración en el archivo
        void ResetSettings();              // volver a los valores por defecto
    }
}