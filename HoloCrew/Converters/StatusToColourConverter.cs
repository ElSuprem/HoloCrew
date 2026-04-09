using HoloCrew.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

// Convierte cada estado de pedido (OrderStatus) en un color distinto.
// Los estados vienen del modelo en HoloCrew.Models.

namespace HoloCrew.Converters
{
    public class StatusToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                // cada estado del pedido tiene un color distinto para mostrar en la interfaz
                // los estados vienen del modelo OrderStatus (está en HoloCrew.Models)
                return status switch
                {
                    OrderStatus.Pending => CreateBrush("#FFA500"),      // Naranja - esperando
                    OrderStatus.Confirmed => CreateBrush("#1976D2"),    // Azul - confirmado
                    OrderStatus.Processing => CreateBrush("#1976D2"),   // Azul - preparando
                    OrderStatus.Shipped => CreateBrush("#2196F3"),      // Azul claro - enviado
                    OrderStatus.Delivered => CreateBrush("#4CAF50"),    // Verde - entregado
                    OrderStatus.Cancelled => CreateBrush("#F44336"),    // Rojo - cancelado
                    OrderStatus.Refunded => CreateBrush("#FF9800"),     // Naranja oscuro - reembolsado
                    _ => CreateBrush("#757575")                          // Gris - estado desconocido
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        // convierte un texto como "#FFA500" en un pincel de color (brush)
        // si falla la conversión, devuelve gris por si acaso
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
                // si el color no es válido, usa gris
            }

            return new SolidColorBrush(Colors.Gray);
        }
    }
}