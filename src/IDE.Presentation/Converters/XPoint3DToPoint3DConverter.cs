using System;
using System.Windows.Data;
using System.Globalization;
using IDE.Core.Types.Media3D;
using System.Windows.Media.Media3D;

namespace IDE.Core.Converters
{
    public class XPoint3DToPoint3DConverter : IValueConverter
    {
        static XPoint3DToPoint3DConverter instance;
        public static XPoint3DToPoint3DConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new XPoint3DToPoint3DConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (XPoint3D)value;
            return new Point3D(c.X, c.Y, c.Z);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (Point3D)value;
            return new XPoint3D(c.X, c.Y, c.Z);
        }
    }
}
