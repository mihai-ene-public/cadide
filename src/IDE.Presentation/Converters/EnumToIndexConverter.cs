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
  public  class EnumToIndexConverter : IValueConverter
    {
        static EnumToIndexConverter instance;
        public static EnumToIndexConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new EnumToIndexConverter();
                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (int)value;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var intValue= (int)value;
                return Enum.ToObject(targetType, intValue);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
