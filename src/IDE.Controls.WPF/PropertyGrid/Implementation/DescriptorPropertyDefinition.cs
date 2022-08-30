using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using IDE.Controls.WPF.Core.Extensions;
using IDE.Controls.WPF.PropertyGrid.Editors;

namespace IDE.Controls.WPF.PropertyGrid;

internal class DescriptorPropertyDefinition : DependencyObject
{
    #region Members

    private object _selectedObject;
    private PropertyDescriptor _propertyDescriptor;
    private DependencyPropertyDescriptor _dpDescriptor;

    private string _description;
    private string _displayName;
    private object _defaultValue;
    private int _displayOrder;
    private bool _isReadOnly;
    private IList<Type> _newItemTypes;
    #endregion

    #region Events

    public event EventHandler ContainerHelperInvalidated;

    #endregion

    #region Constructor

    internal DescriptorPropertyDefinition(PropertyDescriptor propertyDescriptor, object selectedObject, IPropertyContainer propertyContainer)
    {
        Init(propertyDescriptor, selectedObject);
    }

    #endregion

    #region Value Property (DP)

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DescriptorPropertyDefinition), new UIPropertyMetadata(null, OnValueChanged));
    public object Value
    {
        get
        {
            return GetValue(ValueProperty);
        }
        set
        {
            SetValue(ValueProperty, value);
        }
    }

    private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        ( (DescriptorPropertyDefinition)o ).OnValueChanged(e.OldValue, e.NewValue);
    }

    

    #endregion //Value Property


    #region Custom Properties

    internal PropertyDescriptor PropertyDescriptor
    {
        get
        {
            return _propertyDescriptor;
        }
    }

    private object SelectedObject
    {
        get
        {
            return _selectedObject;
        }
    }

    public string DisplayName
    {
        get
        {
            return _displayName;
        }
        internal set
        {
            _displayName = value;
        }
    }

    public object DefaultValue
    {
        get
        {
            return _defaultValue;
        }
        set
        {
            _defaultValue = value;
        }
    }



    public string Description
    {
        get
        {
            return _description;
        }
        internal set
        {
            _description = value;
        }
    }

    public int DisplayOrder
    {
        get
        {
            return _displayOrder;
        }
        internal set
        {
            _displayOrder = value;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return _isReadOnly;
        }
    }

    public IList<Type> NewItemTypes
    {
        get
        {
            return _newItemTypes;
        }
    }

    public string PropertyName
    {
        get
        {
            // A common property which is present in all selectedObjects will always have the same name.
            return PropertyDescriptor.Name;
        }
    }

    public Type PropertyType
    {
        get
        {
            return PropertyDescriptor.PropertyType;
        }
    }

    #endregion

    #region Override Methods

    internal PropertyContainerHelper CreateContainerHelper(IPropertyContainer parent)
    {
        return new PropertyContainerHelper(parent, Value);
    }

    internal void RaiseContainerHelperInvalidated()
    {
        if (ContainerHelperInvalidated != null)
        {
            ContainerHelperInvalidated(this, EventArgs.Empty);
        }
    }

    internal void OnValueChanged(object oldValue, object newValue)
    {
        CommandManager.InvalidateRequerySuggested();
        this.RaiseContainerHelperInvalidated();
    }

    protected BindingBase CreateValueBinding()
    {
        var selectedObject = SelectedObject;
        var propertyName = PropertyDescriptor.Name;

        //Bind the value property with the source object.
        var binding = new Binding(propertyName)
        {
            Source = GetValueInstance(selectedObject),
            Mode = PropertyDescriptor.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
            ValidatesOnDataErrors = true,
            ValidatesOnExceptions = true,
            ConverterCulture = CultureInfo.CurrentCulture
        };

        return binding;
    }

    protected bool ComputeIsReadOnly()
    {
        return PropertyDescriptor.IsReadOnly;
    }

    internal ITypeEditor CreateDefaultEditor(PropertyItem propertyItem)
    {
        return PropertyGridUtilities.CreateDefaultEditor(PropertyDescriptor.PropertyType, PropertyDescriptor.Converter, propertyItem);
    }

    protected bool ComputeCanResetValue()
    {
        return false;
    }

    protected string ComputeDescription()
    {
        return (string)ComputeDescriptionForItem(PropertyDescriptor);
    }

    protected int ComputeDisplayOrder()
    {
        return ComputeDisplayOrderForItem(PropertyDescriptor);
    }

    private string ComputeDisplayName()
    {
        var displayAttribute = PropertyDescriptor.GetAttribute<DisplayAttribute>();
        var displayName = ( displayAttribute != null ) ? displayAttribute.GetName() : PropertyDescriptor.DisplayName;
        if (displayName == null)
            displayName = PropertyDescriptor.DisplayName;

        var attribute = PropertyDescriptor.GetAttribute<ParenthesizePropertyNameAttribute>();
        if (( attribute != null ) && attribute.NeedParenthesis)
        {
            displayName = "(" + displayName + ")";
        }

        return displayName;
    }

    internal object ComputeDescriptionForItem(object item)
    {
        var pd = item as PropertyDescriptor;

        var descriptionAtt = pd.GetAttribute<DescriptionAttribute>();
        return ( descriptionAtt != null )
                ? descriptionAtt.Description
                : pd.Description;
    }

    internal int ComputeDisplayOrderForItem(object item)
    {
        var displayAttribute = PropertyDescriptor.GetAttribute<DisplayAttribute>();
        if (displayAttribute != null)
        {
            var order = displayAttribute.GetOrder();
            return order.GetValueOrDefault();
        }

        // Max Value. Properties with no order will be displayed last.
        return int.MaxValue;
    }

    internal int ComputeDisplayOrderInternal()
    {
        return ComputeDisplayOrder();
    }

    internal object GetValueInstance(object sourceObject)
    {
        ICustomTypeDescriptor customTypeDescriptor = sourceObject as ICustomTypeDescriptor;
        if (customTypeDescriptor != null)
            sourceObject = customTypeDescriptor.GetPropertyOwner(PropertyDescriptor);

        return sourceObject;
    }

    protected void ResetValue()
    {
        PropertyDescriptor.ResetValue(SelectedObject);

        var binding = BindingOperations.GetBindingExpressionBase(this, DescriptorPropertyDefinition.ValueProperty);
        if (binding != null)
        {
            binding.UpdateTarget();
        }
    }

    internal void UpdateValueFromSource()
    {
        var bindingExpr = BindingOperations.GetBindingExpressionBase(this, DescriptorPropertyDefinition.ValueProperty);
        if (bindingExpr != null)
        {
            bindingExpr.UpdateTarget();
        }
    }



    internal ITypeEditor CreateAttributeEditor()
    {
        var editorAttribute = GetAttribute<EditorAttribute>();
        if (editorAttribute != null)
        {
            Type type = null;

            type = Type.GetType(editorAttribute.EditorTypeName);

            // If the editor does not have any public parameterless constructor, forget it.
            if (typeof(ITypeEditor).IsAssignableFrom(type) && ( type.GetConstructor(new Type[0]) != null ))
            {
                var instance = Activator.CreateInstance(type) as ITypeEditor;
                Debug.Assert(instance != null, "Type was expected to be ITypeEditor with public constructor.");
                if (instance != null)
                    return instance;
            }
        }

        return null;
    }

    #endregion

    public void InitProperties()
    {
        // Do "IsReadOnly" and PropertyName first since the others may need that value.
        _isReadOnly = ComputeIsReadOnly();
        _description = ComputeDescription();
        _displayName = ComputeDisplayName();
        _displayOrder = ComputeDisplayOrder();

        BindingBase valueBinding = CreateValueBinding();
        BindingOperations.SetBinding(this, DescriptorPropertyDefinition.ValueProperty, valueBinding);
    }


    #region Private Methods

    private T GetAttribute<T>() where T : Attribute
    {
        return PropertyDescriptor.GetAttribute<T>();
    }

    private void Init(PropertyDescriptor propertyDescriptor, object selectedObject)
    {
        if (propertyDescriptor == null)
            throw new ArgumentNullException(nameof(propertyDescriptor));

        if (selectedObject == null)
            throw new ArgumentNullException(nameof(selectedObject));

        _propertyDescriptor = propertyDescriptor;
        _selectedObject = selectedObject;
        _dpDescriptor = DependencyPropertyDescriptor.FromProperty(propertyDescriptor);
    }

    #endregion //Private Methods

}
