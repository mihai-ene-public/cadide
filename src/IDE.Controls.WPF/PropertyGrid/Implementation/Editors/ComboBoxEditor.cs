using System.Collections;
using System.Windows.Controls;

namespace IDE.Controls.WPF.PropertyGrid.Editors;

public abstract class ComboBoxEditor : TypeEditor<ComboBox>
{
    protected override void SetValueDependencyProperty()
    {
        ValueProperty = ComboBox.SelectedItemProperty;
    }

    protected override ComboBox CreateEditor()
    {
        return new PropertyGridEditorComboBox();
    }

    protected override void ResolveValueBinding(PropertyItem propertyItem)
    {
        SetItemsSource(propertyItem);
        base.ResolveValueBinding(propertyItem);
    }

    protected abstract IEnumerable CreateItemsSource(PropertyItem propertyItem);

    private void SetItemsSource(PropertyItem propertyItem)
    {
        Editor.ItemsSource = CreateItemsSource(propertyItem);
    }
}



