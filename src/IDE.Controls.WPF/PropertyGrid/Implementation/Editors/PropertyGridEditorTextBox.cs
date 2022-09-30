using System.Windows;

namespace IDE.Controls.WPF.PropertyGrid.Editors;

public class PropertyGridEditorTextBox : AdvancedTextBox
{
    static PropertyGridEditorTextBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorTextBox), new FrameworkPropertyMetadata(typeof(PropertyGridEditorTextBox)));
    }
}
