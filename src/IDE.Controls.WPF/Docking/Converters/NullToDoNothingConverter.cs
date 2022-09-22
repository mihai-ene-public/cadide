using System;
using System.Windows.Data;

namespace IDE.Controls.WPF.Docking.Converters;

public class NullToDoNothingConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value == null)
            return Binding.DoNothing;

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
