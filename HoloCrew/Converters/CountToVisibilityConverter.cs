using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

// Convierte un número a Visibility. Si es mayor que 0 se ve, si es 0 se esconde.
// Sirve para badges de contador (carrito, notificaciones, etc.).

namespace HoloCrew.Converters
{
    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                // si el número es mayor que 0 se ve, si es 0 se esconde (para badges de contador)
                return count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}