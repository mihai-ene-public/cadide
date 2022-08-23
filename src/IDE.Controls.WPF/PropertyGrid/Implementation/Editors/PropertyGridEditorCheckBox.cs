using System.Windows;
using System.Windows.Controls;

namespace IDE.Controls.WPF.PropertyGrid.Editors;

public class PropertyGridEditorCheckBox : CheckBox
{
    static PropertyGridEditorCheckBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorCheckBox), new FrameworkPropertyMetadata(typeof(PropertyGridEditorCheckBox)));
    }
}