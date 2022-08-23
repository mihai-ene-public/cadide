using Xceed.Wpf.Toolkit;
using System.Windows;

namespace IDE.Controls.WPF.PropertyGrid.Editors;

public class PropertyGridEditorTextBox : WatermarkTextBox
{
    static PropertyGridEditorTextBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorTextBox), new FrameworkPropertyMetadata(typeof(PropertyGridEditorTextBox)));
    }
}
