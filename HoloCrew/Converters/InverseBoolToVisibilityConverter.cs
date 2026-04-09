using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

// Versión invertida del BoolToVisibility. Si true se esconde, si false se ve.
// Útil para mostrar algo solo cuando NO está cargando (ej: !IsLoading).

namespace HoloCrew.Converters
{
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // si es true se esconde, si es false se ve (al revés del converter normal)
                // útil para cuando tienes una propiedad "IsLoading" y quieres mostrar algo solo cuando NO está cargando
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Collapsed;
            }
            return true;
        }
    }
}