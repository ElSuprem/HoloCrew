using System;
using System.Globalization;
using System.Windows.Data;

namespace HoloCrew.Converters
{
    // Convierte un ancho (ActualWidth) en una altura proporcional, para que las
    // tarjetas mantengan su proporción al redimensionar la ventana y la imagen
    // se recorte siempre igual (que no cambie al maximizar).
    // ConverterParameter = ratio alto/ancho (ej: 1.2 = más alto que ancho).
    public class AspectRatioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width && !double.IsNaN(width) && width > 0 &&
                double.TryParse(parameter?.ToString(), NumberStyles.Any,
                                CultureInfo.InvariantCulture, out var ratio))
            {
                return width * ratio;
            }
            return 0d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}