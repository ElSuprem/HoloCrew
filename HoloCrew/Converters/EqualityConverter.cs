using System;
using System.Globalization;
using System.Windows.Data;

// Compara dos valores y dice si son iguales. Se usa con MultiBinding
// para resaltar el elemento seleccionado en una lista.

namespace HoloCrew.Converters
{
    public class EqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            var value1 = values[0];
            var value2 = values[1];

            if (value1 == null && value2 == null)
                return true;

            if (value1 == null || value2 == null)
                return false;

            // compara dos valores y dice si son iguales
            // se usa con dos bindings: el item actual y el elemento seleccionado
            // sirve para marcar cuál está seleccionado en una lista (ejemplo: categoría activa)
            return value1.Equals(value2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}