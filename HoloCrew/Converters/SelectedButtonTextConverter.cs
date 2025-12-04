using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HoloCrew.Converters
{
    /// <summary>
    /// Convierte un bool a un color de texto para botones seleccionados
    /// true → Color del botón seleccionado (azul)
    /// false → Color del botón no seleccionado (gris)
    /// </summary>
    public class SelectedButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                // Si está seleccionado: azul
                // Si no está seleccionado: gris oscuro
                return isSelected
                    ? new SolidColorBrush(Color.FromRgb(25, 118, 210))  // #1976D2
                    : new SolidColorBrush(Color.FromRgb(117, 117, 117)); // #757575
            }

            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}