using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

// Convierte un bool a Visibility. Si true se ve, si false se esconde.
// Acepta parámetro "Invert" para dar la vuelta a la lógica.

namespace HoloCrew.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // si ponemos "Invert" al usarlo, da la vuelta al resultado
            bool invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) ?? false;

            if (value is bool boolValue)
            {
                bool result = invert ? !boolValue : boolValue;
                return result ? Visibility.Visible : Visibility.Collapsed;
            }

            // si no es true/false, mejor esconderlo
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) ?? false;

            if (value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;
                return invert ? !result : result;
            }

            return false;
        }
    }
}