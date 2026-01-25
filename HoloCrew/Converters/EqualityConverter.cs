using System;
using System.Globalization;
using System.Windows.Data;

namespace HoloCrew.Converters
{
    /// <summary>
    /// Compara dos valores y devuelve true si son iguales.
    /// Útil para resaltar elementos seleccionados en listas.
    /// 
    /// Uso con MultiBinding:
    /// <MultiBinding Converter="{StaticResource EqualityConverter}">
    ///     <Binding Path="." />  <!-- Valor actual del item -->
    ///     <Binding Path="DataContext.SelectedCategory" RelativeSource="..." />  <!-- Valor seleccionado -->
    /// </MultiBinding>
    /// </summary>
    public class EqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            var value1 = values[0];
            var value2 = values[1];

            if (value1 == null && value2 == null)
                return true;

            if (value1 == null || value2 == null)
                return false;

            return value1.Equals(value2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
