using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace IDE.Core.Converters
{
    public class BoolToTextWrappingConverter : IValueConverter
    {

        static BoolToTextWrappingConverter instance;

        public static BoolToTextWrappingConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new BoolToTextWrappingConverter();
                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = false;
            if (value is bool)
            {
                flag = (bool)value;
            }
            else if (value is bool?)
            {
                var nullable = (bool?)value;
                flag = nullable.GetValueOrDefault();
            }

            return flag ? TextWrapping.Wrap : TextWrapping.NoWrap;

            //return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
