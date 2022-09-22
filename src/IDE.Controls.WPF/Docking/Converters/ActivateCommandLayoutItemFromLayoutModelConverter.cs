﻿using System;
using System.Windows.Data;
using IDE.Controls.WPF.Docking.Layout;

namespace IDE.Controls.WPF.Docking.Converters;

public class ActivateCommandLayoutItemFromLayoutModelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        //when this converter is called layout could be constructing so many properties here are potentially not valid
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

        return layoutItemModel.ActivateCommand;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
