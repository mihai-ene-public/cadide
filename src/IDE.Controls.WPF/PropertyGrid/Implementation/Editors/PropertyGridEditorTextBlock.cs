using System.Windows;
using System.Windows.Controls;

namespace IDE.Controls.WPF.PropertyGrid.Editors;

public class PropertyGridEditorTextBlock : TextBlock
{
    static PropertyGridEditorTextBlock()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorTextBlock), new FrameworkPropertyMetadata(typeof(PropertyGridEditorTextBlock)));
    }
}
