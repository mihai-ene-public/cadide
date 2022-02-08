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
    public class EnumToVisibilityConverter : IValueConverter
    {
        static EnumToVisibilityConverter instance;
        static EnumToVisibilityConverter instanceReverse;

        public static EnumToVisibilityConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new EnumToVisibilityConverter();
                return instance;
            }
        }

        public static EnumToVisibilityConverter InstanceReverse
        {
            get
            {
                if (instanceReverse == null)
                    instanceReverse = new EnumToVisibilityConverter() { Reverse = true };
                return instanceReverse;
            }
        }

        public bool Reverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null || parameter == null)
                return DependencyProperty.UnsetValue;

            var parameterString = parameter.ToString().ToLower();
            var valueString = value.ToString().ToLower();

            var b = parameterString == valueString;
            if (Reverse)
                b = !b;

            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
