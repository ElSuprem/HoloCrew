using System;
using System.Windows;
using System.Windows.Media;

namespace HoloCrew.Services
{
    /// <summary>
    /// Gestor de temas - Cambia entre modo claro y oscuro
    /// </summary>
    public static class ThemeManager
    {
        /// <summary>
        /// Aplica el tema claro u oscuro a la aplicación
        /// </summary>
        public static void ApplyTheme(bool isDarkMode)
        {
            try
            {
                var app = Application.Current;

                if (isDarkMode)
                {
                    ApplyDarkTheme(app);
                    System.Diagnostics.Debug.WriteLine("🎨 Tema OSCURO aplicado");
                }
                else
                {
                    ApplyLightTheme(app);
                    System.Diagnostics.Debug.WriteLine("🎨 Tema CLARO aplicado");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al aplicar tema: {ex.Message}");
            }
        }

        private static void ApplyDarkTheme(Application app)
        {
            // Cambiar colores principales
            UpdateColor(app, "BlackPrimary", "#FFFFFF");
            UpdateColor(app, "WhitePrimary", "#1A1A1A");
            UpdateColor(app, "WhiteSecondary", "#121212");
            UpdateColor(app, "WhiteTertiary", "#0A0A0A");

            // Background
            UpdateColor(app, "Gray100", "#2A2A2A");
            UpdateColor(app, "Gray200", "#3A3A3A");
            UpdateColor(app, "Gray300", "#4A4A4A");

            // Accent
            UpdateColor(app, "RedAccent", "#CF6679");

            // Text colors (invertidos)
            UpdateColor(app, "TextPrimaryColor", "#FFFFFF");
            UpdateColor(app, "TextSecondaryColor", "#B3B3B3");

            // Border
            UpdateColor(app, "BorderColor", "#3A3A3A");
        }

        private static void ApplyLightTheme(Application app)
        {
            // Restaurar colores originales
            UpdateColor(app, "BlackPrimary", "#0A0A0A");
            UpdateColor(app, "WhitePrimary", "#FFFFFF");
            UpdateColor(app, "WhiteSecondary", "#F5F5F5");
            UpdateColor(app, "WhiteTertiary", "#E8E8E8");

            // Background
            UpdateColor(app, "Gray100", "#F7F7F7");
            UpdateColor(app, "Gray200", "#E8E8E8");
            UpdateColor(app, "Gray300", "#D1D1D1");

            // Accent
            UpdateColor(app, "RedAccent", "#FF0000");

            // Text colors
            UpdateColor(app, "TextPrimaryColor", "#000000");
            UpdateColor(app, "TextSecondaryColor", "#666666");

            // Border
            UpdateColor(app, "BorderColor", "#E0E0E0");
        }

        private static void UpdateColor(Application app, string resourceKey, string hexColor)
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hexColor);

                // Actualizar el Color
                if (app.Resources.Contains(resourceKey))
                {
                    app.Resources[resourceKey] = color;
                }

                // Actualizar el Brush correspondiente
                var brushKey = resourceKey + "Brush";
                if (app.Resources.Contains(brushKey))
                {
                    app.Resources[brushKey] = new SolidColorBrush(color);
                }

                // Casos especiales para colores de texto
                if (resourceKey == "BlackPrimary")
                {
                    UpdateBrush(app, "TextPrimary", hexColor);
                }
                else if (resourceKey == "Gray600")
                {
                    UpdateBrush(app, "TextSecondary", hexColor);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ No se pudo actualizar {resourceKey}: {ex.Message}");
            }
        }

        private static void UpdateBrush(Application app, string resourceKey, string hexColor)
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hexColor);

                if (app.Resources.Contains(resourceKey))
                {
                    app.Resources[resourceKey] = new SolidColorBrush(color);
                }
            }
            catch { }
        }
    }
}