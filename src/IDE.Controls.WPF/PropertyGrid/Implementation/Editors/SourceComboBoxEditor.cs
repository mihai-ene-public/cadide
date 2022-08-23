using System.Collections;
using System.ComponentModel;
using System.Windows.Data;

namespace IDE.Controls.WPF.PropertyGrid.Editors;

public class SourceComboBoxEditor : ComboBoxEditor
{
    ICollection _collection;
    TypeConverter _typeConverter;

    public SourceComboBoxEditor(ICollection collection, TypeConverter typeConverter)
    {
        _collection = collection;
        _typeConverter = typeConverter;
    }

    protected override IEnumerable CreateItemsSource(PropertyItem propertyItem)
    {
        return _collection;
    }

    protected override IValueConverter CreateValueConverter()
    {
        //When using a stringConverter, we need to convert the value
        if (( _typeConverter != null ) && ( _typeConverter is StringConverter ))
            return new SourceComboBoxEditorConverter(_typeConverter);
        return null;
    }
}
