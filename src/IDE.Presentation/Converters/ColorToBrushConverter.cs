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
            var color = Colors.Transparent;
            if (value is Color)
                color = (Color)value;
            if (value is XColor xColor)
                color = xColor.ToColor();
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
