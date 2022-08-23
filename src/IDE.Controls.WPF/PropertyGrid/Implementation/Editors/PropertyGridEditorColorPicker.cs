using System.Windows;
using Xceed.Wpf.Toolkit;

namespace IDE.Controls.WPF.PropertyGrid.Editors;

public class PropertyGridEditorColorPicker : ColorPicker
{
    static PropertyGridEditorColorPicker()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorColorPicker), new FrameworkPropertyMetadata(typeof(PropertyGridEditorColorPicker)));
    }
}
