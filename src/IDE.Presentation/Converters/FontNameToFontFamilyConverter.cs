using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace IDE.Core.Converters
{
    public class FontNameToFontFamilyConverter : IValueConverter
    {
        static FontNameToFontFamilyConverter instance;
        public static FontNameToFontFamilyConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new FontNameToFontFamilyConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var f = (string)value;
            return new FontFamily(f);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
