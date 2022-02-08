using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using IDE.Core.Types.Media;

namespace IDE.Core.Converters
{
    /// <summary>
    /// Converts a color value to a brush.
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        static ColorToBrushConverter instance;
        public static ColorToBrushConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new ColorToBrushConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return new SolidColorBrush((Color)value);

            var c = value is Color ? (Color)value : ((XColor)value).ToColor();
            return new SolidColorBrush(c);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class XSweepDirectionToSweepDirectionConverter : IValueConverter
    {
        static XSweepDirectionToSweepDirectionConverter instance;
        public static XSweepDirectionToSweepDirectionConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new XSweepDirectionToSweepDirectionConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return new SolidColorBrush((Color)value);
            var c = (XSweepDirection)value;
            return (SweepDirection)c;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
