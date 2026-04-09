using System;
using System.Globalization;
using System.Windows.Data;

// Convierte un decimal a texto con símbolo euro y dos decimales (ej: 29.99 -> "€29,99").
// También puede convertir de vuelta de texto a número.

namespace HoloCrew.Converters
{
    public class PriceFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal price)
            {
                // convierte un número decimal en texto con el símbolo euro y dos decimales (ej: 29.99 -> "€29,99")
                return $"€{price:N2}";
            }
            return "€0,00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                // quita el símbolo euro y convierte el texto de vuelta a número decimal
                strValue = strValue.Replace("€", "").Trim();
                if (decimal.TryParse(strValue, out var price))
                {
                    return price;
                }
            }
            return 0m;
        }
    }
}