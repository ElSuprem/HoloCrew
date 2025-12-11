using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HoloCrew.Converters
{
    /// <summary>
    /// Convierte bool a Visibility
    /// true = Visible, false = Collapsed
    /// 
    /// Uso:
    /// <TextBlock Visibility="{Binding IsVisible, Converter={StaticResource BoolToVisibilityConverter}}"/>
    /// 
    /// Con parámetro "Invert" para invertir lógica:
    /// <TextBlock Visibility="{Binding IsHidden, Converter={StaticResource BoolToVisibilityConverter}, ConverterParameter=Invert}"/>
    /// true = Collapsed, false = Visible
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Lógica invertida si parameter es "Invert"
            bool invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) ?? false;

            if (value is bool boolValue)
            {
                // Si invert=true, invertimos la lógica
                bool result = invert ? !boolValue : boolValue;
                return result ? Visibility.Visible : Visibility.Collapsed;
            }

            // Si no es bool, devolver Collapsed por defecto
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) ?? false;

            if (value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;
                return invert ? !result : result;
            }

            return false;
        }
    }
}