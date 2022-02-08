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
    public class DataTypeToVisibilityConverter : IValueConverter
    {
        static DataTypeToVisibilityConverter instance;
        public static DataTypeToVisibilityConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new DataTypeToVisibilityConverter();
                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return Binding.DoNothing;
                return value.GetType() == (Type)parameter ? Visibility.Visible : Visibility.Collapsed;
            }
            catch
            {
                return Binding.DoNothing;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
