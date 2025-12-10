using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using System;
using System.IO;
using System.Text.Json;

namespace HoloCrew.Services
{
    /// <summary>
    /// Implementación del servicio de configuración
    /// Guarda en JSON en %AppData%/HoloCrew/settings.json
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsDirectory;
        private readonly string _settingsFilePath;

        public SettingsService()
        {
            // Directorio: %AppData%/HoloCrew/
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _settingsDirectory = Path.Combine(appData, "HoloCrew");
            _settingsFilePath = Path.Combine(_settingsDirectory, "settings.json");

            // Crear directorio si no existe
            if (!Directory.Exists(_settingsDirectory))
            {
                Directory.CreateDirectory(_settingsDirectory);
                System.Diagnostics.Debug.WriteLine($"📁 Directorio creado: {_settingsDirectory}");
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

                    System.Diagnostics.Debug.WriteLine($"✅ Settings cargados desde: {_settingsFilePath}");
                    return settings ?? new AppSettings();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("📄 No existe archivo de settings, usando valores por defecto");
                    return new AppSettings();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al cargar settings: {ex.Message}");
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
                    WriteIndented = true // JSON formateado bonito
                };

                var json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(_settingsFilePath, json);

                System.Diagnostics.Debug.WriteLine($"💾 Settings guardados en: {_settingsFilePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al guardar settings: {ex.Message}");
            }
        }

        public void ResetSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    File.Delete(_settingsFilePath);
                    System.Diagnostics.Debug.WriteLine("🗑️ Settings reseteados");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al resetear settings: {ex.Message}");
            }
        }
    }
}