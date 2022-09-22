using System;
using System.Windows.Controls;
using System.Windows.Data;
using IDE.Controls.WPF.Docking.Layout;

namespace IDE.Controls.WPF.Docking.Converters;

[ValueConversion(typeof(AnchorSide), typeof(Orientation))]
public class AnchorSideToOrientationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        AnchorSide side = (AnchorSide)value;
        if (side == AnchorSide.Left ||
            side == AnchorSide.Right)
            return Orientation.Vertical;

        return Orientation.Horizontal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
