using IDE.Core.Units;
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
    public class TextToLengthUnitsConverter : IValueConverter
    {

        static TextToLengthUnitsConverter instance;

        public static TextToLengthUnitsConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new TextToLengthUnitsConverter();
                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var unit = value as AbstractUnit;
            if (unit == null)
                return Binding.DoNothing;
            return unit.CurrentValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //todo: we could use regex to match what we need
            //@"-?\d+(?:\.\d+)?"
            // /^-?\d(.\d+)?[^.]/
            var vstr = value as string;
            if (string.IsNullOrEmpty(vstr))
                return Binding.DoNothing;
            vstr = vstr.ToLower();

            double val;
            if (vstr.Contains("mm"))
            {
                vstr = vstr.Replace("mm", "").Trim();
                if (double.TryParse(vstr, out val))
                    return new MillimeterUnit(val);
            }

            else if (vstr.Contains("mil"))
            {
                vstr = vstr.Replace("mil", "").Trim();
                if (double.TryParse(vstr, out val))
                    return new MilUnit(val);
            }
            else
            {
                vstr = vstr.Trim();
                if (vstr.EndsWith("."))
                    return Binding.DoNothing;

                if (double.TryParse(vstr, out val))
                    return new AdimensionalUnit(val);
            }

            return Binding.DoNothing;
        }

        //public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        //{
        //    try
        //    {
        //        return values[0].ToString();
        //    }
        //    catch
        //    {
        //        return Binding.DoNothing;
        //    }
        //}

        //public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        //{
        //    var model = parameter as LengthUnitsViewModel;
        //    if (model == null)
        //        return null;
        //    var vString = value as string;
        //    return null;
        //    model.CurrentValue
        //}
    }
}
