using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

// Convierte bool a color de texto. Si true = azul, si false = gris.
// Se usa en botones de navegación o pestañas para marcar cuál está activo.

namespace HoloCrew.Converters
{
    public class SelectedButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                // si está seleccionado: texto azul
                // si no está seleccionado: texto gris
                // esto se usa en botones de navegación o pestañas para resaltar cuál está activo
                return isSelected
                    ? new SolidColorBrush(Color.FromRgb(25, 118, 210))  // azul (#1976D2)
                    : new SolidColorBrush(Color.FromRgb(117, 117, 117)); // gris (#757575)
            }

            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}