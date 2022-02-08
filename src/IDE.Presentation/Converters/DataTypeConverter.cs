using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IDE.Core.Converters
{
    public class DataTypeConverter : IValueConverter
    {
        static DataTypeConverter instance;
        public static DataTypeConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new DataTypeConverter();
                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
