using System;
using System.Globalization;
using System.Windows.Data;

namespace HoloCrew.Converters
{
    /// <summary>
    /// Formatea fechas en formato legible
    /// </summary>
    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // Si el parámetro especifica un formato, usarlo
                if (parameter is string format)
                {
                    return dateTime.ToString(format);
                }

                // Formato por defecto
                var today = DateTime.Today;
                var yesterday = today.AddDays(-1);

                if (dateTime.Date == today)
                {
                    return $"Hoy a las {dateTime:HH:mm}";
                }
                else if (dateTime.Date == yesterday)
                {
                    return $"Ayer a las {dateTime:HH:mm}";
                }
                else if (dateTime.Date > today.AddDays(-7))
                {
                    return dateTime.ToString("dddd 'a las' HH:mm", new CultureInfo("es-ES"));
                }
                else
                {
                    return dateTime.ToString("dd/MM/yyyy HH:mm");
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}