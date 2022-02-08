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
    public class BoolToBoldConverter : IValueConverter
    {
        static BoolToBoldConverter instance;
        public static BoolToBoldConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new BoolToBoldConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                var bold = (bool)value;
                return bold ? FontWeights.Bold: FontWeights.Normal;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
