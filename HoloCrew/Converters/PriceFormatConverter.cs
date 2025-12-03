using System;
using System.Globalization;
using System.Windows.Data;

namespace HoloCrew.Converters
{
    /// <summary>
    /// Formatea precios con símbolo de euro
    /// Ejemplo: 29.99 -> "€29,99"
    /// </summary>
    public class PriceFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal price)
            {
                return $"€{price:N2}";
            }
            return "€0,00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
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