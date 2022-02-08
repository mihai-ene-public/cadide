namespace IDE.Core.Converters
{
    using IDE.Core.Interfaces;
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class IGeometryToGeometryConverter : IValueConverter
    {

        static IGeometryToGeometryConverter instance;
        public static IGeometryToGeometryConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new IGeometryToGeometryConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var g = value as IGeometry;
            if(g!=null)
            {
                return g.Geometry;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
