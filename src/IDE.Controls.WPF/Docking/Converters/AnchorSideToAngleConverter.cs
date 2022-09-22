using System;
using System.Windows.Data;
using IDE.Controls.WPF.Docking.Layout;

namespace IDE.Controls.WPF.Docking.Converters;

[ValueConversion(typeof(AnchorSide), typeof(double))]
public class AnchorSideToAngleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        AnchorSide side = (AnchorSide)value;
        if (side == AnchorSide.Left ||
            side == AnchorSide.Right)
            return 90.0;

        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
