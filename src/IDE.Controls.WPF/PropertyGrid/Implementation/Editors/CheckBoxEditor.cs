using System.Windows;
using System.Windows.Controls;

namespace IDE.Controls.WPF.PropertyGrid.Editors;

public class CheckBoxEditor : TypeEditor<CheckBox>
{
    protected override CheckBox CreateEditor()
    {
        return new PropertyGridEditorCheckBox();
    }

    protected override void SetControlProperties(PropertyItem propertyItem)
    {
        Editor.Margin = new Thickness(5, 0, 0, 0);
    }

    protected override void SetValueDependencyProperty()
    {
        ValueProperty = CheckBox.IsCheckedProperty;
    }
}
