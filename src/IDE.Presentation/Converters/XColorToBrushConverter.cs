using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using IDE.Core.Types.Media;

namespace IDE.Core.Converters
{
    ///// <summary>
    ///// Converts a color value to a brush.
    ///// </summary>
    //public class XColorToBrushConverter : IValueConverter
    //{
    //    static XColorToBrushConverter instance;
    //    public static XColorToBrushConverter Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new XColorToBrushConverter();

    //            return instance;
    //        }
    //    }

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        var c = (XColor)value;
    //        return new SolidColorBrush(c.ToColor());
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
