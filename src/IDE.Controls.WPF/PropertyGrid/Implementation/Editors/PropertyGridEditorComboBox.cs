using System.Windows;
using System.Windows.Controls;

namespace IDE.Controls.WPF.PropertyGrid.Editors;

public class PropertyGridEditorComboBox : ComboBox
{
    static PropertyGridEditorComboBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorComboBox), new FrameworkPropertyMetadata(typeof(PropertyGridEditorComboBox)));
    }
}



