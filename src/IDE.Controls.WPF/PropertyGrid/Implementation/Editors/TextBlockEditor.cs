using System.Windows;
using System.Windows.Controls;

namespace IDE.Controls.WPF.PropertyGrid.Editors;

public class TextBlockEditor : TypeEditor<TextBlock>
{
    protected override TextBlock CreateEditor()
    {
        return new PropertyGridEditorTextBlock();
    }

    protected override void SetValueDependencyProperty()
    {
        ValueProperty = TextBlock.TextProperty;
    }

    protected override void SetControlProperties(PropertyItem propertyItem)
    {
        Editor.Margin = new System.Windows.Thickness(5, 0, 0, 0);
        Editor.TextTrimming = TextTrimming.CharacterEllipsis;
    }
}
