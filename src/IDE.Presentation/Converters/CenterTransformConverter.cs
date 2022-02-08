using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IDE.Core.Converters
{
    public class CenterTransformConverter : IValueConverter
    {

        static CenterTransformConverter instance;
        public static CenterTransformConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new CenterTransformConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var length = 0.0d;
            var amount = 0.5d;
            try
            {
                length = (double)value;
                var pStr = parameter as string;
                var temp = 0.0d;
                if (!string.IsNullOrEmpty(pStr) && double.TryParse(pStr, out temp))
                    amount = temp;

                return MilimetersToDpiHelper.ConvertToDpi(-length * amount);
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
