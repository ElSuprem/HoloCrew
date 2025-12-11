using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace HoloCrew.Converters
{
    /// <summary>
    /// Convierte una colección vacía a Visibility
    /// Colección vacía = Collapsed, con elementos = Visible
    /// ⭐ CORREGIDO: Sin using duplicado
    /// </summary>
    public class EmptyCollectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable collection)
            {
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