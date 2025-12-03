using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HoloCrew.Converters
{
    /// <summary>
    /// Convierte null a Visibility
    /// null = Collapsed, no null = Visible
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}