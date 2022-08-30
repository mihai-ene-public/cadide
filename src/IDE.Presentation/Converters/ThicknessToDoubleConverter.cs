using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IDE.Core.Converters
{
    public class ThicknessToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double thickness = 1.0;

            if (value != null)
                thickness = ( (Thickness)value ).Top;

            return thickness;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double thickness = 1.0;

            if (value != null)
                thickness = (double)value;

            return new Thickness(thickness);
        }
    }
}