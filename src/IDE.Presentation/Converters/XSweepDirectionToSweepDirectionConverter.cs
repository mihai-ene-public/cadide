using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using IDE.Core.Types.Media;

namespace IDE.Core.Converters
{
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
