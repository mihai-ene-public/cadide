using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using IDE.Core.Types.Media;

namespace IDE.Core.Converters
{
    public class XColorToColorConverter : IValueConverter
    {
        static XColorToColorConverter instance;
        public static XColorToColorConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new XColorToColorConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is XColor c)
            {
                //var c = (XColor)value;
                return c.ToColor();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color c)
            {
                //var c = (Color)value;
                return c.ToXColor();
            }
            return null;
        }
    }
}
