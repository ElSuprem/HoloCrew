using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

// Si el valor es nulo se esconde, si tiene algo se ve.
// Sirve para mostrar detalles solo cuando hay datos seleccionados.

namespace HoloCrew.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // si el valor es nulo se esconde, si tiene algo se ve
            // sirve para mostrar cosas solo cuando hay datos (ejemplo: detalles de un producto seleccionado)
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}