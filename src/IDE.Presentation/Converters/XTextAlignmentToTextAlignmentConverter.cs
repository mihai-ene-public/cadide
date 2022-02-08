using System;
using System.Windows.Data;
using System.Globalization;
using IDE.Core.Types.Media;
using System.Windows;

namespace IDE.Core.Converters
{
    public class XTextAlignmentToTextAlignmentConverter : IValueConverter
    {
        static XTextAlignmentToTextAlignmentConverter instance;
        public static XTextAlignmentToTextAlignmentConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new XTextAlignmentToTextAlignmentConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = (XTextAlignment)value;
            return (TextAlignment)t;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
