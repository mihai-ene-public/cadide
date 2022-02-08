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
    public class ExpansionToThicknessConverter : IValueConverter
    {
        static ExpansionToThicknessConverter instance;
        public static ExpansionToThicknessConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new ExpansionToThicknessConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Thickness(-(double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
