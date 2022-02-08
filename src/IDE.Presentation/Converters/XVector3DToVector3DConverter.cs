using System;
using System.Windows.Data;
using System.Globalization;
using IDE.Core.Types.Media3D;
using System.Windows.Media.Media3D;

namespace IDE.Core.Converters
{
    public class XVector3DToVector3DConverter : IValueConverter
    {
        static XVector3DToVector3DConverter instance;
        public static XVector3DToVector3DConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new XVector3DToVector3DConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (XVector3D)value;
            return new Vector3D(c.X, c.Y, c.Z);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (Vector3D)value;
            return new XVector3D(c.X, c.Y, c.Z);
        }
    }
}
