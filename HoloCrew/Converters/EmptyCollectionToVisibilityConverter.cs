using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

// Si una lista tiene elementos se ve, si está vacía se esconde.
// Útil para mostrar "No hay resultados" cuando la lista está vacía.

namespace HoloCrew.Converters
{
    public class EmptyCollectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable collection)
            {
                // si la lista tiene algo, se ve; si está vacía, se esconde
                // esto sirve para mostrar un mensaje de "no hay resultados" cuando la lista está vacía
                return collection.Cast<object>().Any() ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}