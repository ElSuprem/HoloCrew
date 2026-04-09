using System;
using System.Windows;
using System.Windows.Media;

// Gestor de temas para cambiar entre modo claro y oscuro.
// Funciona así:
// 1. Colors.xaml define colores (BlackPrimary, WhitePrimary, etc.)
// 2. Brushes.xaml define pinceles usando DynamicResource a esos colores
// 3. Los XAML usan DynamicResource a los pinceles
// 4. Este ThemeManager solo cambia los colores → los pinceles se actualizan solos
// Resultado: cambio de tema instantáneo sin recargar vistas.

namespace HoloCrew.Services
{
    public static class ThemeManager
    {
        private static bool _isDarkMode = false;
        public static bool IsDarkMode => _isDarkMode;

        // aplica el tema claro u oscuro a toda la app
        public static void ApplyTheme(bool isDarkMode)
        {
            try
            {
                _isDarkMode = isDarkMode;
                var app = Application.Current;
                if (app == null) return;

                if (isDarkMode)
                    ApplyDarkTheme(app);
                else
                    ApplyLightTheme(app);

                System.Diagnostics.Debug.WriteLine($"[Theme] Applied {(isDarkMode ? "DARK" : "LIGHT")} theme");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Theme] Error: {ex.Message}");
            }
        }

        // ========== TEMA OSCURO ==========
        private static void ApplyDarkTheme(Application app)
        {
            // colores base (invertidos)
            SetColor(app, "BlackPrimary", "#F0F0F0");   // texto oscuro → texto claro
            SetColor(app, "BlackSecondary", "#E0E0E0");
            SetColor(app, "BlackTertiary", "#CCCCCC");

            SetColor(app, "WhitePrimary", "#121212");   // fondo blanco → fondo oscuro
            SetColor(app, "WhiteSecondary", "#1A1A1A");
            SetColor(app, "WhiteTertiary", "#222222");

            // rojo de acento (un poco más suave en oscuro)
            SetColor(app, "RedAccent", "#FF2D2D");
            SetColor(app, "RedAccentDark", "#CC0000");
            SetColor(app, "RedAccentLight", "#FF5555");

            // grises ajustados para fondos oscuros
            SetColor(app, "Gray100", "#1E1E1E");
            SetColor(app, "Gray200", "#2A2A2A");
            SetColor(app, "Gray300", "#3A3A3A");
            SetColor(app, "Gray400", "#888888");
            SetColor(app, "Gray500", "#999999");
            SetColor(app, "Gray600", "#AAAAAA");
            SetColor(app, "Gray700", "#CCCCCC");
            SetColor(app, "Gray800", "#E0E0E0");
            SetColor(app, "Gray900", "#F0F0F0");

            // colores semánticos
            SetColor(app, "SuccessColor", "#34D399");
            SetColor(app, "WarningColor", "#FBBF24");
            SetColor(app, "ErrorColor", "#F87171");
            SetColor(app, "InfoColor", "#60A5FA");

            SetColor(app, "OverlayDark", "#B0000000");
            SetColor(app, "OverlayLight", "#20FFFFFF");

            SetColor(app, "HoverLight", "#2A2A2A");
            SetColor(app, "HoverDark", "#30FFFFFF");

            // alias semánticos
            SetBrush(app, "BackgroundPrimary", "#121212");
            SetBrush(app, "BackgroundSecondary", "#1A1A1A");
            SetBrush(app, "BackgroundDark", "#0A0A0A");

            SetBrush(app, "TextPrimary", "#F0F0F0");
            SetBrush(app, "TextSecondary", "#999999");
            SetBrush(app, "TextOnDark", "#FFFFFF");
            SetBrush(app, "TextMuted", "#666666");

            SetBrush(app, "BorderDefault", "#2A2A2A");
            SetBrush(app, "BorderBrush", "#2A2A2A");
            SetBrush(app, "AccentPrimary", "#FF2D2D");
            SetBrush(app, "AccentSecondary", "#F0F0F0");

            // alias de compatibilidad
            SetBrush(app, "PrimaryBrush", "#F0F0F0");
            SetBrush(app, "PrimaryDarkBrush", "#E0E0E0");
            SetBrush(app, "PrimaryLightBrush", "#CCCCCC");

            SetBrush(app, "AccentBrush", "#FF2D2D");
            SetBrush(app, "AccentLightBrush", "#FF5555");
            SetBrush(app, "AccentDarkBrush", "#CC0000");

            SetBrush(app, "BackgroundBrush", "#121212");
            SetBrush(app, "SurfaceBrush", "#1A1A1A");
            SetBrush(app, "CardBrush", "#1E1E1E");
            SetBrush(app, "DividerBrush", "#2A2A2A");

            SetBrush(app, "TextPrimaryBrush", "#F0F0F0");
            SetBrush(app, "TextSecondaryBrush", "#999999");
            SetBrush(app, "TextDisabledBrush", "#555555");
            SetBrush(app, "TextOnPrimaryBrush", "#0A0A0A");

            SetBrush(app, "SuccessBrush", "#34D399");
            SetBrush(app, "WarningBrush", "#FBBF24");
            SetBrush(app, "ErrorBrush", "#F87171");
            SetBrush(app, "InfoBrush", "#60A5FA");

            SetBrush(app, "BorderFocusBrush", "#F0F0F0");
            SetBrush(app, "BorderErrorBrush", "#F87171");

            SetBrush(app, "HoverBrush", "#2A2A2A");
            SetBrush(app, "PressedBrush", "#333333");

            SetBrush(app, "StarBrush", "#FBBF24");

            // pinceles nombrados individualmente
            UpdateNamedBrush(app, "BlackPrimaryBrush", "#F0F0F0");
            UpdateNamedBrush(app, "BlackSecondaryBrush", "#E0E0E0");
            UpdateNamedBrush(app, "BlackTertiaryBrush", "#CCCCCC");
            UpdateNamedBrush(app, "WhitePrimaryBrush", "#121212");
            UpdateNamedBrush(app, "WhiteSecondaryBrush", "#1A1A1A");
            UpdateNamedBrush(app, "WhiteTertiaryBrush", "#222222");
            UpdateNamedBrush(app, "RedAccentBrush", "#FF2D2D");
            UpdateNamedBrush(app, "RedAccentDarkBrush", "#CC0000");
            UpdateNamedBrush(app, "RedAccentLightBrush", "#FF5555");
            UpdateNamedBrush(app, "Gray50Brush", "#161616");
            UpdateNamedBrush(app, "Gray100Brush", "#1E1E1E");
            UpdateNamedBrush(app, "Gray200Brush", "#2A2A2A");
            UpdateNamedBrush(app, "Gray300Brush", "#3A3A3A");
            UpdateNamedBrush(app, "Gray400Brush", "#888888");
            UpdateNamedBrush(app, "Gray500Brush", "#999999");
            UpdateNamedBrush(app, "Gray600Brush", "#AAAAAA");
            UpdateNamedBrush(app, "Gray700Brush", "#CCCCCC");
            UpdateNamedBrush(app, "Gray800Brush", "#E0E0E0");
            UpdateNamedBrush(app, "Gray900Brush", "#F0F0F0");
            UpdateNamedBrush(app, "SuccessColorBrush", "#34D399");
            UpdateNamedBrush(app, "WarningColorBrush", "#FBBF24");
            UpdateNamedBrush(app, "ErrorColorBrush", "#F87171");
            UpdateNamedBrush(app, "InfoColorBrush", "#60A5FA");
            UpdateNamedBrush(app, "OverlayDarkBrush", "#B0000000");
            UpdateNamedBrush(app, "OverlayLightBrush", "#20FFFFFF");
            UpdateNamedBrush(app, "HoverLight", "#2A2A2A");
            UpdateNamedBrush(app, "HoverDark", "#30FFFFFF");
        }

        // ========== TEMA CLARO (valores originales) ==========
        private static void ApplyLightTheme(Application app)
        {
            SetColor(app, "BlackPrimary", "#0A0A0A");
            SetColor(app, "BlackSecondary", "#1A1A1A");
            SetColor(app, "BlackTertiary", "#2A2A2A");

            SetColor(app, "WhitePrimary", "#FFFFFF");
            SetColor(app, "WhiteSecondary", "#F5F5F5");
            SetColor(app, "WhiteTertiary", "#E8E8E8");

            SetColor(app, "RedAccent", "#FF0000");
            SetColor(app, "RedAccentDark", "#CC0000");
            SetColor(app, "RedAccentLight", "#FF3333");

            SetColor(app, "Gray100", "#F7F7F7");
            SetColor(app, "Gray200", "#E8E8E8");
            SetColor(app, "Gray300", "#D1D1D1");
            SetColor(app, "Gray400", "#B0B0B0");
            SetColor(app, "Gray500", "#8A8A8A");
            SetColor(app, "Gray600", "#6B6B6B");
            SetColor(app, "Gray700", "#4A4A4A");
            SetColor(app, "Gray800", "#2E2E2E");
            SetColor(app, "Gray900", "#1F1F1F");

            SetColor(app, "SuccessColor", "#10B981");
            SetColor(app, "WarningColor", "#F59E0B");
            SetColor(app, "ErrorColor", "#EF4444");
            SetColor(app, "InfoColor", "#3B82F6");

            SetColor(app, "OverlayDark", "#80000000");
            SetColor(app, "OverlayLight", "#40FFFFFF");

            SetColor(app, "HoverLight", "#F0F0F0");
            SetColor(app, "HoverDark", "#20FFFFFF");

            SetBrush(app, "BackgroundPrimary", "#FFFFFF");
            SetBrush(app, "BackgroundSecondary", "#F5F5F5");
            SetBrush(app, "BackgroundDark", "#0A0A0A");

            SetBrush(app, "TextPrimary", "#0A0A0A");
            SetBrush(app, "TextSecondary", "#6B6B6B");
            SetBrush(app, "TextOnDark", "#FFFFFF");
            SetBrush(app, "TextMuted", "#B0B0B0");

            SetBrush(app, "BorderDefault", "#E8E8E8");
            SetBrush(app, "BorderBrush", "#E8E8E8");
            SetBrush(app, "AccentPrimary", "#E31E24");
            SetBrush(app, "AccentSecondary", "#0A0A0A");

            SetBrush(app, "PrimaryBrush", "#0A0A0A");
            SetBrush(app, "PrimaryDarkBrush", "#1A1A1A");
            SetBrush(app, "PrimaryLightBrush", "#2A2A2A");

            SetBrush(app, "AccentBrush", "#E31E24");
            SetBrush(app, "AccentLightBrush", "#FF3333");
            SetBrush(app, "AccentDarkBrush", "#CC0000");

            SetBrush(app, "BackgroundBrush", "#FFFFFF");
            SetBrush(app, "SurfaceBrush", "#FFFFFF");
            SetBrush(app, "CardBrush", "#FFFFFF");
            SetBrush(app, "DividerBrush", "#E8E8E8");

            SetBrush(app, "TextPrimaryBrush", "#0A0A0A");
            SetBrush(app, "TextSecondaryBrush", "#6B6B6B");
            SetBrush(app, "TextDisabledBrush", "#B0B0B0");
            SetBrush(app, "TextOnPrimaryBrush", "#FFFFFF");

            SetBrush(app, "SuccessBrush", "#10B981");
            SetBrush(app, "WarningBrush", "#F59E0B");
            SetBrush(app, "ErrorBrush", "#EF4444");
            SetBrush(app, "InfoBrush", "#3B82F6");

            SetBrush(app, "BorderFocusBrush", "#0A0A0A");
            SetBrush(app, "BorderErrorBrush", "#EF4444");

            SetBrush(app, "HoverBrush", "#F0F0F0");
            SetBrush(app, "PressedBrush", "#E8E8E8");

            SetBrush(app, "StarBrush", "#F59E0B");

            UpdateNamedBrush(app, "BlackPrimaryBrush", "#0A0A0A");
            UpdateNamedBrush(app, "BlackSecondaryBrush", "#1A1A1A");
            UpdateNamedBrush(app, "BlackTertiaryBrush", "#2A2A2A");
            UpdateNamedBrush(app, "WhitePrimaryBrush", "#FFFFFF");
            UpdateNamedBrush(app, "WhiteSecondaryBrush", "#F5F5F5");
            UpdateNamedBrush(app, "WhiteTertiaryBrush", "#E8E8E8");
            UpdateNamedBrush(app, "RedAccentBrush", "#E31E24");
            UpdateNamedBrush(app, "RedAccentDarkBrush", "#CC0000");
            UpdateNamedBrush(app, "RedAccentLightBrush", "#FF3333");
            UpdateNamedBrush(app, "Gray50Brush", "#FAFAFA");
            UpdateNamedBrush(app, "Gray100Brush", "#F7F7F7");
            UpdateNamedBrush(app, "Gray200Brush", "#E8E8E8");
            UpdateNamedBrush(app, "Gray300Brush", "#D1D1D1");
            UpdateNamedBrush(app, "Gray400Brush", "#B0B0B0");
            UpdateNamedBrush(app, "Gray500Brush", "#8A8A8A");
            UpdateNamedBrush(app, "Gray600Brush", "#6B6B6B");
            UpdateNamedBrush(app, "Gray700Brush", "#4A4A4A");
            UpdateNamedBrush(app, "Gray800Brush", "#2E2E2E");
            UpdateNamedBrush(app, "Gray900Brush", "#1F1F1F");
            UpdateNamedBrush(app, "SuccessColorBrush", "#10B981");
            UpdateNamedBrush(app, "WarningColorBrush", "#F59E0B");
            UpdateNamedBrush(app, "ErrorColorBrush", "#EF4444");
            UpdateNamedBrush(app, "InfoColorBrush", "#3B82F6");
            UpdateNamedBrush(app, "OverlayDarkBrush", "#80000000");
            UpdateNamedBrush(app, "OverlayLightBrush", "#40FFFFFF");
            UpdateNamedBrush(app, "HoverLight", "#F0F0F0");
            UpdateNamedBrush(app, "HoverDark", "#20FFFFFF");
        }

        // ========== FUNCIONES AUXILIARES ==========

        // cambia un color en los recursos de la app
        private static void SetColor(Application app, string key, string hex)
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hex);
                app.Resources[key] = color;
            }
            catch { }
        }

        // cambia un pincel sólido en los recursos de la app
        private static void SetBrush(Application app, string key, string hex)
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hex);
                app.Resources[key] = new SolidColorBrush(color);
            }
            catch { }
        }

        // actualiza un pincel nombrado (ej: BlackPrimaryBrush)
        private static void UpdateNamedBrush(Application app, string key, string hex)
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hex);
                app.Resources[key] = new SolidColorBrush(color);
            }
            catch { }
        }
    }
}