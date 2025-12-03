using HoloCrew.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HoloCrew.Converters
{
    /// <summary>
    /// Convierte OrderStatus a color
    /// </summary>
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                return status switch
                {
                    OrderStatus.Pending => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500")),      // Naranja
                    OrderStatus.Confirmed => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1976D2")),    // Azul
                    OrderStatus.Processing => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1976D2")),   // Azul
                    OrderStatus.Shipped => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3")),      // Azul claro
                    OrderStatus.Delivered => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")),    // Verde
                    OrderStatus.Cancelled => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336")),    // Rojo
                    OrderStatus.Refunded => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9800")),     // Naranja oscuro
                    _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575"))                          // Gris
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}