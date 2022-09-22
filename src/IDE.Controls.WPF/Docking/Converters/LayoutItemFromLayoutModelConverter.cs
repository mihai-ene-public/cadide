using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using IDE.Controls.WPF.Docking.Layout;

namespace IDE.Controls.WPF.Docking.Converters;
public class LayoutItemFromLayoutModelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        var layoutModel = value as LayoutContent;
        if (layoutModel == null)
            return null;
        if (layoutModel.Root == null)
            return null;
        if (layoutModel.Root.Manager == null)
            return null;

        var layoutItemModel = layoutModel.Root.Manager.GetLayoutItemFromModel(layoutModel);
        if (layoutItemModel == null)
            return Binding.DoNothing;

        return layoutItemModel;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
