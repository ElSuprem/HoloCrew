using HoloCrew.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HoloCrew.Converters
{
    /// <summary>
    /// Convierte OrderStatus a color
    /// ⭐ CORREGIDO: Manejo seguro de ColorConverter.ConvertFromString
    /// </summary>
    public class StatusToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                return status switch
                {
                    OrderStatus.Pending => CreateBrush("#FFA500"),      // Naranja
                    OrderStatus.Confirmed => CreateBrush("#1976D2"),    // Azul
                    OrderStatus.Processing => CreateBrush("#1976D2"),   // Azul
                    OrderStatus.Shipped => CreateBrush("#2196F3"),      // Azul claro
                    OrderStatus.Delivered => CreateBrush("#4CAF50"),    // Verde
                    OrderStatus.Cancelled => CreateBrush("#F44336"),    // Rojo
                    OrderStatus.Refunded => CreateBrush("#FF9800"),     // Naranja oscuro
                    _ => CreateBrush("#757575")                          // Gris
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Crea un SolidColorBrush de forma segura desde un código hex
        /// </summary>
        private static SolidColorBrush CreateBrush(string hexColor)
        {
            try
            {
                var colorObj = ColorConverter.ConvertFromString(hexColor);
                if (colorObj != null)
                {
                    return new SolidColorBrush((Color)colorObj);
                }
            }
            catch
            {
                // Si falla la conversión, usar gris por defecto
            }

            return new SolidColorBrush(Colors.Gray);
        }
    }
}