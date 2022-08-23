using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using IDE.Controls.WPF.PropertyGrid.Editors;

namespace IDE.Controls.WPF.PropertyGrid;

internal class PropertyGridUtilities
{
    internal static ITypeEditor CreateDefaultEditor(Type propertyType, TypeConverter typeConverter, PropertyItem propertyItem)
    {
        ITypeEditor editor = null;

        var context = new EditorTypeDescriptorContext(propertyItem.Instance, propertyItem.PropertyDescriptor);
        if (( typeConverter != null )
          && typeConverter.GetStandardValuesSupported(context)
          && typeConverter.GetStandardValuesExclusive(context)
          && ( propertyType != typeof(bool) ) && ( propertyType != typeof(bool?) ))  //Bool type always have a BooleanConverter with standardValues : True/False.
        {
            var items = typeConverter.GetStandardValues(context);
            editor = new SourceComboBoxEditor(items, typeConverter);
        }
        else if (propertyType == typeof(string))
            editor = new TextBoxEditor();
        else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            editor = new CheckBoxEditor();
        else if (( propertyType == typeof(Color) ) || ( propertyType == typeof(Color?) ))
            editor = new ColorEditor();
        else if (propertyType.IsEnum)
            editor = new EnumComboBoxEditor();
        else if (propertyType == typeof(FontFamily) || propertyType == typeof(FontWeight) || propertyType == typeof(FontStyle) || propertyType == typeof(FontStretch))
            editor = new FontComboBoxEditor();
        else if (propertyType == typeof(object))
            // If any type of object is possible in the property, default to the TextBoxEditor.
            // Useful in some case (e.g., Button.Content).
            // Can be reconsidered but was the legacy behavior on the PropertyGrid.
            editor = new TextBoxEditor();
        else
        {
            // If the type is not supported, check if there is a converter that supports
            // string conversion to the object type. Use TextBox in theses cases.
            // Otherwise, return a TextBlock editor since no valid editor exists.
            editor = ( typeConverter != null && typeConverter.CanConvertFrom(typeof(string)) )
              ? (ITypeEditor)new TextBoxEditor()
              : (ITypeEditor)new TextBlockEditor();
        }
        return editor;
    }

    #region Private class

    private class EditorTypeDescriptorContext : ITypeDescriptorContext
    {
        object _instance;
        PropertyDescriptor _propertyDescriptor;

        internal EditorTypeDescriptorContext(object instance, PropertyDescriptor pd)
        {
            _instance = instance;
            _propertyDescriptor = pd;
        }

        IContainer ITypeDescriptorContext.Container => null;

        object ITypeDescriptorContext.Instance => _instance;

        PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor => _propertyDescriptor;

        void ITypeDescriptorContext.OnComponentChanged()
        {
        }

        bool ITypeDescriptorContext.OnComponentChanging() => false;

        object IServiceProvider.GetService(Type serviceType) => null;
    }

    #endregion
}


