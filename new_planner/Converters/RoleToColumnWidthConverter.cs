using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace new_planner.Converters
{
    public class RoleToColumnWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool show && show)
            {
                return 120.0; // Возвращаем double, а не DataGridLength
            }
            return 0.0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}