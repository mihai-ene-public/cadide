using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using HelixToolkit.Wpf;
using IDE.Core.Types.Media;
using IDE.Presentation.Extensions;

namespace IDE.Core.Converters
{
    public class ColorToMaterialConverter : IValueConverter
    {
        static ColorToMaterialConverter instance;
        public static ColorToMaterialConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new ColorToMaterialConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (XColor)value;
            return MaterialHelper.CreateMaterial(c.ToColor());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
