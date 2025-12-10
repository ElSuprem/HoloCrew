using HoloCrew.Models;

namespace HoloCrew.Services.Interfaces
{
    /// <summary>
    /// Servicio para gestionar la configuración de la aplicación
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Carga la configuración desde el archivo JSON
        /// </summary>
        AppSettings LoadSettings();

        /// <summary>
        /// Guarda la configuración en el archivo JSON
        /// </summary>
        void SaveSettings(AppSettings settings);

        /// <summary>
        /// Restablece la configuración a valores por defecto
        /// </summary>
        void ResetSettings();
    }
}