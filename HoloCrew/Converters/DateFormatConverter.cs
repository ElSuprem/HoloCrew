using System;
using System.Globalization;
using System.Windows.Data;

// Formatea fechas a texto legible: "Hoy a las 15:30", "Ayer a las 20:00",
// "lunes a las 10:00", o "dd/MM/yyyy HH:mm" si es más vieja de una semana.

namespace HoloCrew.Converters
{
    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // si al usarlo le pasas un formato (ejemplo: "dd/MM/yyyy"), usa ese
                if (parameter is string format)
                {
                    return dateTime.ToString(format);
                }

                // formato por defecto: muestra "Hoy", "Ayer", o el día de la semana según cuándo sea
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
                    // para fechas de menos de una semana, muestra el nombre del día (ej: "lunes a las 15:30")
                    return dateTime.ToString("dddd 'a las' HH:mm", new CultureInfo("es-ES"));
                }
                else
                {
                    // para fechas más viejas, formato normal
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