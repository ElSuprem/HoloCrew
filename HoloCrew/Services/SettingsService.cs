using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using System;
using System.IO;
using System.Text.Json;

// Servicio de configuración. Guarda los ajustes de la app en un archivo JSON.
// Ubicación: %AppData%/HoloCrew/settings.json
// Aquí se guarda modo oscuro, idioma, notificaciones, etc.

namespace HoloCrew.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsDirectory;
        private readonly string _settingsFilePath;

        public SettingsService()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _settingsDirectory = Path.Combine(appData, "HoloCrew");
            _settingsFilePath = Path.Combine(_settingsDirectory, "settings.json");

            if (!Directory.Exists(_settingsDirectory))
            {
                Directory.CreateDirectory(_settingsDirectory);
                System.Diagnostics.Debug.WriteLine($"Directorio creado: {_settingsDirectory}");
            }
        }

        public AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);

                    System.Diagnostics.Debug.WriteLine($"Settings cargados desde: {_settingsFilePath}");
                    return settings ?? new AppSettings();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No existe archivo de settings, usando valores por defecto");
                    return new AppSettings();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar settings: {ex.Message}");
                return new AppSettings();
            }
        }

        public void SaveSettings(AppSettings settings)
        {
            try
            {
                settings.LastUpdated = DateTime.Now;

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true  // el JSON se guarda con formato bonito (con saltos de línea)
                };

                var json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(_settingsFilePath, json);

                System.Diagnostics.Debug.WriteLine($"Settings guardados en: {_settingsFilePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar settings: {ex.Message}");
            }
        }

        public void ResetSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    File.Delete(_settingsFilePath);
                    System.Diagnostics.Debug.WriteLine("Settings reseteados");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al resetear settings: {ex.Message}");
            }
        }
    }
}