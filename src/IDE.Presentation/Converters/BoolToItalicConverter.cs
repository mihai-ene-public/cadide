using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using System.Windows;

namespace IDE.Core.Converters
{
    public class BoolToItalicConverter : IValueConverter
    {
        static BoolToItalicConverter instance;
        public static BoolToItalicConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new BoolToItalicConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool)
            {
                var italic = (bool)value;
                return italic ? FontStyles.Italic : FontStyles.Normal;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
