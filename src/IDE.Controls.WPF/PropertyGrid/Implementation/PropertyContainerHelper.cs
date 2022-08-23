using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using IDE.Controls.WPF.Core.Extensions;
using IDE.Controls.WPF.PropertyGrid.Editors;

namespace IDE.Controls.WPF.PropertyGrid;

internal class PropertyContainerHelper
{
    public PropertyContainerHelper(IPropertyContainer propertyContainer, object selectedObject)
    {
        if (propertyContainer == null)
            throw new ArgumentNullException(nameof(propertyContainer));

        _propertyContainer = propertyContainer;

        propertyContainer.PropertyChanged += new PropertyChangedEventHandler(OnPropertyContainerPropertyChanged);

        _propertyItemCollection = new PropertyItemCollection(new ObservableCollection<PropertyItem>());

        _selectedObject = selectedObject;
    }

    private readonly IPropertyContainer _propertyContainer;
    private object _selectedObject;

    private bool _isPreparingItemFlag = false;
    private PropertyItemCollection _propertyItemCollection;

    #region IsGenerated attached property

    internal static readonly DependencyProperty IsGeneratedProperty = DependencyProperty.RegisterAttached(
      "IsGenerated",
      typeof(bool),
      typeof(PropertyContainerHelper),
      new PropertyMetadata(false));

    internal static bool GetIsGenerated(DependencyObject obj)
    {
        return (bool)obj.GetValue(PropertyContainerHelper.IsGeneratedProperty);
    }

    internal static void SetIsGenerated(DependencyObject obj, bool value)
    {
        obj.SetValue(PropertyContainerHelper.IsGeneratedProperty, value);
    }

    #endregion IsGenerated attached property


    public IList Properties
    {
        get { return _propertyItemCollection; }
    }

    internal ItemsControl ChildrenItemsControl { get; set; }

    internal bool IsCleaning { get; private set; }

    private PropertyItemCollection PropertyItems
    {
        get
        {
            return _propertyItemCollection;
        }
    }

    private void OnPropertyContainerPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
    }

    internal void ClearHelper()
    {
        IsCleaning = true;

        if (_propertyContainer != null)
        {
            _propertyContainer.PropertyChanged -= new PropertyChangedEventHandler(OnPropertyContainerPropertyChanged);
        }

        if (ChildrenItemsControl != null)
        {
            ( (IItemContainerGenerator)ChildrenItemsControl.ItemContainerGenerator ).RemoveAll();
        }
        IsCleaning = false;
    }

    internal void PrepareChildrenPropertyItem(PropertyItem propertyItem, object item)
    {
        _isPreparingItemFlag = true;

        // Initialize the parent node
        propertyItem.ParentNode = _propertyContainer;

        PropertyGrid.RaisePreparePropertyItemEvent((UIElement)_propertyContainer, propertyItem, item);

        if (propertyItem.Editor == null)
        {
            FrameworkElement editor = GenerateChildrenEditorElement((PropertyItem)propertyItem);
            if (editor != null)
            {
                // Tag the editor as generated to know if we should clear it.
                SetIsGenerated(editor, true);
                propertyItem.Editor = editor;
            }
        }
        _isPreparingItemFlag = false;
    }

    internal FrameworkElement GenerateChildrenEditorElement(PropertyItem propertyItem)
    {
        FrameworkElement editorElement = null;
        var pd = propertyItem.DescriptorDefinition;
        object definitionKey = null;
        Type definitionKeyAsType = definitionKey as Type;
        ITypeEditor editor = null;

        editor = pd.CreateAttributeEditor();

        if (editor != null)
            editorElement = editor.ResolveEditor(propertyItem);

        if (editorElement == null)
        {
            if (pd.IsReadOnly)
                editor = new TextBlockEditor();

            // Fallback: Use a default type editor.
            if (editor == null)
            {
                editor = ( definitionKeyAsType != null )
                ? PropertyGridUtilities.CreateDefaultEditor(definitionKeyAsType, null, propertyItem)
                : pd.CreateDefaultEditor(propertyItem);
            }

            Debug.Assert(editor != null);

            editorElement = editor.ResolveEditor(propertyItem);
        }

        return editorElement;
    }

    internal Binding CreateChildrenDefaultBinding(PropertyItem propertyItem)
    {
        Binding binding = new Binding("Value");
        binding.Mode = ( ( (PropertyItem)propertyItem ).IsReadOnly ) ? BindingMode.OneWay : BindingMode.TwoWay;
        return binding;
    }


    internal void ClearChildrenPropertyItem(PropertyItem propertyItem, object item)
    {
        if (propertyItem.Editor != null
        && GetIsGenerated(propertyItem.Editor))
        {
            propertyItem.Editor = null;
        }

        propertyItem.ParentNode = null;

        PropertyGrid.RaiseClearPropertyItemEvent((UIElement)_propertyContainer, propertyItem, item);
    }

    internal void GenerateProperties()
    {
        if (( PropertyItems.Count == 0 ))
        {
            RegenerateProperties();
        }
    }

    private void RegenerateProperties()
    {
        GenerateSubPropertiesCore(UpdatePropertyItemsCallback);
    }

    private void GenerateSubPropertiesCore(Action<IEnumerable<PropertyItem>> updatePropertyItemsCallback)
    {
        var propertyItems = new List<PropertyItem>();

        if (_selectedObject != null)
        {
            try
            {
                var descriptors = new List<PropertyDescriptor>();
                {
                    descriptors = GetPropertyDescriptors(_selectedObject, hideInheritedProperties: false);
                }

                foreach (var descriptor in descriptors)
                {
                    bool isBrowsable = false;

                    var isPropertyBrowsable = _propertyContainer.IsPropertyVisible(descriptor);
                    if (isPropertyBrowsable.HasValue)
                    {
                        isBrowsable = isPropertyBrowsable.Value;
                    }
                    else
                    {
                        //browsable attribute wins
                        var browsableAttr = descriptor.GetAttribute<BrowsableAttribute>();

                        if (browsableAttr != null)
                        {
                            isBrowsable = browsableAttr.Browsable;
                        }
                        else
                        {
                            var displayAttribute = descriptor.GetAttribute<DisplayAttribute>();
                            if (displayAttribute != null)
                            {
                                var autoGenerateField = displayAttribute.GetAutoGenerateField();
                                isBrowsable = ( ( autoGenerateField.HasValue && autoGenerateField.Value ) || !autoGenerateField.HasValue );
                            }
                            else
                            {
                                isBrowsable = descriptor.IsBrowsable;
                            }
                        }

                    }

                    if (isBrowsable)
                    {
                        var prop = CreatePropertyItem(descriptor);
                        if (prop != null)
                        {
                            propertyItems.Add(prop);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Property creation failed.");
                Debug.WriteLine(e.StackTrace);
            }
        }

        updatePropertyItemsCallback.Invoke(propertyItems);

    }

    private PropertyItem CreatePropertyItem(PropertyDescriptor property)
    {
        DescriptorPropertyDefinition definition = new DescriptorPropertyDefinition(property, _selectedObject, _propertyContainer);
        definition.InitProperties();

        PropertyItem propertyItem = new PropertyItem(definition);
        Debug.Assert(_selectedObject != null);
        propertyItem.Instance = _selectedObject;
        //propertyItem.CategoryOrder = this.GetCategoryOrder(definition.CategoryValue);

        propertyItem.WillRefreshPropertyGrid = GetWillRefreshPropertyGrid(property);
        return propertyItem;
    }

    private bool GetWillRefreshPropertyGrid(PropertyDescriptor propertyDescriptor)
    {
        if (propertyDescriptor == null)
            return false;

        var attribute = propertyDescriptor.GetAttribute<RefreshPropertiesAttribute>();
        if (attribute != null)
            return attribute.RefreshProperties != RefreshProperties.None;

        return false;
    }

    private void UpdatePropertyItemsCallback(IEnumerable<PropertyItem> subProperties)
    {
        foreach (var propertyItem in subProperties)
        {
            InitializePropertyItem(propertyItem);
        }

        //Remove the event callback from the previous children (if any)
        foreach (var propertyItem in PropertyItems)
        {
            propertyItem.PropertyChanged -= OnChildrenPropertyChanged;
        }

        PropertyItems.UpdateItems(subProperties);

        //Add the event callback to the new childrens
        foreach (var propertyItem in PropertyItems)
        {
            propertyItem.PropertyChanged += OnChildrenPropertyChanged;
        }

    }

    private void OnChildrenPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (IsItemOrderingProperty(e.PropertyName)
          )
        {
            // Refreshing the view while Containers are generated will throw an exception
            if (ChildrenItemsControl.ItemContainerGenerator.Status != GeneratorStatus.GeneratingContainers
              && !_isPreparingItemFlag)
            {
                PropertyItems.RefreshView();
            }
        }
    }

    private bool IsItemOrderingProperty(string propertyName)
    {
        return string.Equals(propertyName, PropertyItemCollection.DisplayNamePropertyName);
    }

    List<PropertyDescriptor> GetPropertyDescriptors(object instance, bool hideInheritedProperties)
    {
        PropertyDescriptorCollection descriptors;

        TypeConverter tc = TypeDescriptor.GetConverter(instance);
        if (tc == null || !tc.GetPropertiesSupported())
        {
            if (instance is ICustomTypeDescriptor ctd)
            {
                descriptors = ctd.GetProperties();
            }
            else
            {
                descriptors = TypeDescriptor.GetProperties(instance.GetType());
            }
        }
        else
        {
            descriptors = tc.GetProperties(instance);
        }

        if (( descriptors != null ))
        {
            var descriptorsProperties = descriptors.Cast<PropertyDescriptor>();
            if (hideInheritedProperties)
            {
                var properties = from p in descriptorsProperties
                                 where p.ComponentType == instance.GetType()
                                 select p;
                return properties.ToList();
            }
            else
            {
                return descriptorsProperties.ToList();
            }
        }

        return null;
    }

    internal PropertyItem ContainerFromItem(object item)
    {
        if (item == null)
            return null;
        // Exception case for ObjectContainerHelperBase. The "Item" may sometimes
        // be identified as a string representing the property name or
        // the PropertyItem itself.
        Debug.Assert(item is PropertyItem || item is string);

        var propertyItem = item as PropertyItem;
        if (propertyItem != null)
            return propertyItem;


        var propertyStr = item as string;
        if (propertyStr != null)
            return PropertyItems.FirstOrDefault((prop) => propertyStr == prop.PropertyDescriptor.Name);

        return null;
    }

    internal object ItemFromContainer(PropertyItem container)
    {
        // Since this call is only used to update the PropertyGrid.SelectedProperty property,
        // return the PropertyName.
        if (container == null)
            return null;

        return container.PropertyDescriptor.Name;
    }

    internal void UpdateValuesFromSource()
    {
        foreach (PropertyItem item in PropertyItems)
        {
            item.DescriptorDefinition.UpdateValueFromSource();
            item.ContainerHelper.UpdateValuesFromSource();
        }
    }

    private void InitializePropertyItem(PropertyItem propertyItem)
    {
        var pd = propertyItem.DescriptorDefinition;
        propertyItem.PropertyDescriptor = pd.PropertyDescriptor;

        propertyItem.IsReadOnly = pd.IsReadOnly;
        propertyItem.DisplayName = pd.DisplayName;
        propertyItem.Description = pd.Description;

        propertyItem.PropertyOrder = pd.DisplayOrder;

        SetupDefinitionBinding(propertyItem, PropertyItem.ValueProperty, pd, nameof(pd.Value), BindingMode.TwoWay);

        // PropertyItem.PropertyType's defaultValue equals current PropertyItem's value => set the DefaultValue attribute
        if (pd.DefaultValue != null)
        {
            var typeDefaultValue = GetTypeDefaultValue(propertyItem.PropertyType);

            if (( ( propertyItem.Value != null ) && propertyItem.Value.Equals(typeDefaultValue) )
                  || ( ( propertyItem.Value == null ) && ( typeDefaultValue == propertyItem.Value ) ))
            {
                propertyItem.SetCurrentValue(PropertyItem.ValueProperty, pd.DefaultValue);
            }
        }
    }

    private object GetTypeDefaultValue(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = type.GetProperty("Value").PropertyType;
        }

        return ( type.IsValueType ? Activator.CreateInstance(type) : null );
    }

    private void SetupDefinitionBinding(
     PropertyItem propertyItem,
     DependencyProperty itemProperty,
     DescriptorPropertyDefinition pd,
     string propertyName,
     BindingMode bindingMode)
    {
        var binding = new Binding(propertyName)
        {
            Source = pd,
            Mode = bindingMode
        };

        propertyItem.SetBinding(itemProperty, binding);
    }
}
