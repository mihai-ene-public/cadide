using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using IDE.Controls.WPF.Core.Extensions;

namespace IDE.Controls.WPF.PropertyGrid;

[TemplatePart(Name = PropertyGrid.PART_PropertyItemsControl, Type = typeof(PropertyItemsControl))]
[TemplatePart(Name = PropertyItem.PART_ValueContainer, Type = typeof(ContentControl))]
[TemplatePart(Name = "content", Type = typeof(ContentControl))]
public class PropertyItem : Control, IPropertyContainer
{
    internal const string PART_ValueContainer = "PART_ValueContainer";

    private ContentControl _valueContainer;
    private PropertyContainerHelper _containerHelper;
    private IPropertyContainer _parentNode;
    internal bool _isPropertyGridCategorized;
    internal bool _isSortedAlphabetically = true;

    #region Constructors

    static PropertyItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyItem), new FrameworkPropertyMetadata(typeof(PropertyItem)));
    }

    internal PropertyItem()
    {
        GotFocus += new RoutedEventHandler(PropertyItemBase_GotFocus);
        RequestBringIntoView += this.PropertyItemBase_RequestBringIntoView;
        AddHandler(PropertyItemsControl.PreparePropertyItemEvent, new PropertyItemEventHandler(OnPreparePropertyItemInternal));
        AddHandler(PropertyItemsControl.ClearPropertyItemEvent, new PropertyItemEventHandler(OnClearPropertyItemInternal));
    }

    internal PropertyItem(DescriptorPropertyDefinition definition)
     : this()
    {
        Init(definition);
    }

    #endregion //Constructors

    #region Events

    #region ItemSelectionChanged

    internal static readonly RoutedEvent ItemSelectionChangedEvent = EventManager.RegisterRoutedEvent(
        "ItemSelectionChangedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PropertyItem));

    private void RaiseItemSelectionChangedEvent()
    {
        RaiseEvent(new RoutedEventArgs(PropertyItem.ItemSelectionChangedEvent));
    }

    #endregion

    #region PropertyChanged event

    public event PropertyChangedEventHandler PropertyChanged;

    internal void RaisePropertyChanged(string name)
    {
        this.OnPropertyChanged(PropertyChanged, name);
    }
    #endregion

    #endregion //Events

    #region Properties

    #region IsReadOnly

    /// <summary>
    /// Identifies the IsReadOnly dependency property
    /// </summary>
    public static readonly DependencyProperty IsReadOnlyProperty =
        DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(PropertyItem), new UIPropertyMetadata(false));

    public bool IsReadOnly
    {
        get { return (bool)GetValue(IsReadOnlyProperty); }
        set { SetValue(IsReadOnlyProperty, value); }
    }

    #endregion //IsReadOnly

    #region IsInValid

    /// <summary>
    /// Identifies the IsInvalid dependency property
    /// </summary>
    public static readonly DependencyProperty IsInvalidProperty =
        DependencyProperty.Register("IsInvalid", typeof(bool), typeof(PropertyItem), new UIPropertyMetadata(false, OnIsInvalidChanged));

    public bool IsInvalid
    {
        get
        {
            return (bool)GetValue(IsInvalidProperty);
        }
        internal set
        {
            SetValue(IsInvalidProperty, value);
        }
    }

    private static void OnIsInvalidChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var propertyItem = o as PropertyItem;
        if (propertyItem != null)
            propertyItem.OnIsInvalidChanged((bool)e.OldValue, (bool)e.NewValue);
    }

    protected virtual void OnIsInvalidChanged(bool oldValue, bool newValue)
    {
        var be = this.GetBindingExpression(PropertyItem.ValueProperty);

        if (newValue)
        {
            var validationError = new ValidationError(new InvalidValueValidationRule(), be);
            validationError.ErrorContent = "Value could not be converted.";
            Validation.MarkInvalid(be, validationError);
        }
        else
        {
            Validation.ClearInvalid(be);
        }
    }


    #endregion // IsInvalid

    #region PropertyDescriptor

    public PropertyDescriptor PropertyDescriptor
    {
        get;
        internal set;
    }

    #endregion //PropertyDescriptor

    #region PropertyName

    public string PropertyName
    {
        get
        {
            return ( this.DescriptorDefinition != null ) ? this.DescriptorDefinition.PropertyName : null;
        }
    }

    #endregion

    #region PropertyType

    public Type PropertyType
    {
        get
        {
            return ( PropertyDescriptor != null )
              ? PropertyDescriptor.PropertyType
              : null;
        }
    }

    #endregion //PropertyType

    #region DescriptorDefinition

    internal DescriptorPropertyDefinition DescriptorDefinition
    {
        get;
        private set;
    }

    #endregion DescriptorDefinition    

    #region Instance

    public object Instance
    {
        get;
        internal set;
    }

    #endregion //Instance


    #region PropertyOrder

    public static readonly DependencyProperty PropertyOrderProperty =
        DependencyProperty.Register("PropertyOrder", typeof(int), typeof(PropertyItem), new UIPropertyMetadata(0));

    public int PropertyOrder
    {
        get
        {
            return (int)GetValue(PropertyOrderProperty);
        }
        set
        {
            SetValue(PropertyOrderProperty, value);
        }
    }

    #endregion //PropertyOrder

    #region Value

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(PropertyItem), new UIPropertyMetadata(null, OnValueChanged, OnCoerceValueChanged));
    public object Value
    {
        get
        {
            return (object)GetValue(ValueProperty);
        }
        set
        {
            SetValue(ValueProperty, value);
        }
    }

    private static object OnCoerceValueChanged(DependencyObject o, object baseValue)
    {
        var prop = o as PropertyItem;
        if (prop != null)
            return prop.OnCoerceValueChanged(baseValue);

        return baseValue;
    }

    protected virtual object OnCoerceValueChanged(object baseValue)
    {
        //return baseValue;

        BindingExpression be = this.GetBindingExpression(PropertyItem.ValueProperty);
        this.SetRedInvalidBorder(be);
        return baseValue;
    }

    private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var propertyItem = o as PropertyItem;
        if (propertyItem != null)
        {
            propertyItem.OnValueChanged((object)e.OldValue, (object)e.NewValue);
        }
    }

    protected virtual void OnValueChanged(object oldValue, object newValue)
    {
        if (IsInitialized)
        {
            RaiseEvent(new PropertyValueChangedEventArgs(PropertyGrid.PropertyValueChangedEvent, this, oldValue, newValue));
        }
    }

    #endregion //Value


    #region Description

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(string), typeof(PropertyItem), new UIPropertyMetadata(null));

    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    #endregion //Description

    #region DisplayName

    public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.Register("DisplayName", typeof(string), typeof(PropertyItem), new UIPropertyMetadata(null));

    public string DisplayName
    {
        get { return (string)GetValue(DisplayNameProperty); }
        set { SetValue(DisplayNameProperty, value); }
    }

    #endregion //DisplayName

    #region Editor

    public static readonly DependencyProperty EditorProperty = DependencyProperty.Register("Editor", typeof(FrameworkElement), typeof(PropertyItem), new UIPropertyMetadata(null, OnEditorChanged));
    public FrameworkElement Editor
    {
        get
        {
            return (FrameworkElement)GetValue(EditorProperty);
        }
        set
        {
            SetValue(EditorProperty, value);
        }
    }

    private static void OnEditorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var propertyItem = o as PropertyItem;
        if (propertyItem != null)
            propertyItem.OnEditorChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
    }

    protected virtual void OnEditorChanged(FrameworkElement oldValue, FrameworkElement newValue)
    {
        if (oldValue != null)
        {
            oldValue.DataContext = null;
        }

        if (( newValue != null ) && ( newValue.DataContext == null ))
        {
            newValue.DataContext = this;
        }
    }

    #endregion //Editor

    #region IsSelected

    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(PropertyItem), new UIPropertyMetadata(false, OnIsSelectedChanged));
    public bool IsSelected
    {
        get
        {
            return (bool)GetValue(IsSelectedProperty);
        }
        set
        {
            SetValue(IsSelectedProperty, value);
        }
    }

    private static void OnIsSelectedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var propertyItem = o as PropertyItem;
        if (propertyItem != null)
            propertyItem.OnIsSelectedChanged((bool)e.OldValue, (bool)e.NewValue);
    }

    protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
    {
        this.RaiseItemSelectionChangedEvent();
    }

    #endregion //IsSelected

    #region ParentElement
    /// <summary>
    /// Gets the parent property grid element of this property.
    /// A PropertyItem instance if this is a sub-element, 
    /// or the PropertyGrid itself if this is a first-level property.
    /// </summary>
    public FrameworkElement ParentElement
    {
        get { return this.ParentNode as FrameworkElement; }
    }
    #endregion

    #region ParentNode

    internal IPropertyContainer ParentNode
    {
        get
        {
            return _parentNode;
        }
        set
        {
            _parentNode = value;
        }
    }
    #endregion

    #region ValueContainer

    internal ContentControl ValueContainer
    {
        get
        {
            return _valueContainer;
        }
    }

    #endregion

    #region Level

    public int Level
    {
        get;
        internal set;
    }

    #endregion //Level

    #region Properties

    public IList Properties
    {
        get
        {
            if (_containerHelper == null)
            {
                _containerHelper = new PropertyContainerHelper(this, null);
            }
            return _containerHelper.Properties;
        }
    }

    #endregion //Properties


    #region ContainerHelper

    internal PropertyContainerHelper ContainerHelper
    {
        get
        {
            return _containerHelper;
        }
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            _containerHelper = value;
            // Properties property relies on the "Properties" property of the helper
            // class. Raise a property-changed event.
            RaisePropertyChanged(nameof(Properties));
        }
    }

    #endregion

    #region WillRefreshPropertyGrid

    public static readonly DependencyProperty WillRefreshPropertyGridProperty =
        DependencyProperty.Register("WillRefreshPropertyGrid", typeof(bool), typeof(PropertyItem), new UIPropertyMetadata(false));

    public bool WillRefreshPropertyGrid
    {
        get
        {
            return (bool)GetValue(WillRefreshPropertyGridProperty);
        }
        set
        {
            SetValue(WillRefreshPropertyGridProperty, value);
        }
    }

    #endregion //WillRefreshPropertyGrid

    #endregion Properties

    #region Event Handlers

    private void OnPreparePropertyItemInternal(object sender, PropertyItemEventArgs args)
    {
        // This is the callback of the PreparePropertyItem comming from the template PropertyItemControl.
        args.PropertyItem.Level = this.Level + 1;
        _containerHelper.PrepareChildrenPropertyItem(args.PropertyItem, args.Item);

        args.Handled = true;
    }

    private void OnClearPropertyItemInternal(object sender, PropertyItemEventArgs args)
    {
        _containerHelper.ClearChildrenPropertyItem(args.PropertyItem, args.Item);
        // This is the callback of the PreparePropertyItem comming from the template PropertyItemControl.
        args.PropertyItem.Level = 0;

        args.Handled = true;
    }

    private void PropertyItemBase_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
    {
        e.Handled = true;
    }


    #endregion  //Event Handlers

    #region Methods

    internal void SetRedInvalidBorder(BindingExpression be)
    {
        if (( be != null ) && be.DataItem is DescriptorPropertyDefinition)
        {
            var descriptor = be.DataItem as DescriptorPropertyDefinition;
            if (Validation.GetHasError(descriptor))
            {
                ReadOnlyObservableCollection<ValidationError> errors = Validation.GetErrors(descriptor);
                Validation.MarkInvalid(be, errors[0]);
            }
        }
    }

    protected virtual Type GetPropertyItemType()
    {
        if (PropertyType != null)
            return PropertyType;

        return Value.GetType();
    }

    protected virtual string GetPropertyItemName()
    {
        return PropertyName;

        //return DisplayName
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _containerHelper.ChildrenItemsControl = GetTemplateChild(PropertyGrid.PART_PropertyItemsControl) as PropertyItemsControl;
        _valueContainer = GetTemplateChild(PropertyItem.PART_ValueContainer) as ContentControl;
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        IsSelected = true;
        if (!IsKeyboardFocusWithin)
        {
            Focus();
        }
        // Handle the event; otherwise, the possible 
        // parent property item will select itself too.
        e.Handled = true;
    }


    private void PropertyItemBase_GotFocus(object sender, RoutedEventArgs e)
    {
        IsSelected = true;
        // Handle the event; otherwise, the possible 
        // parent property item will select itself too.
        e.Handled = true;
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        // First check that the raised property is actually a real CLR property.
        // This could be something else like an Attached DP.
        if (//ReflectionHelper.IsPublicInstanceProperty(GetType(), e.Property.Name)
          //&& 
          IsLoaded
          && ( _parentNode != null )
          && !_parentNode.ContainerHelper.IsCleaning)
        {
            RaisePropertyChanged(e.Property.Name);
        }
    }


    #endregion //Methods

    #region Private Methods

    private void OnDefinitionContainerHelperInvalidated(object sender, EventArgs e)
    {
        if (this.ContainerHelper != null)
        {
            this.ContainerHelper.ClearHelper();
        }
        var helper = this.DescriptorDefinition.CreateContainerHelper(this);
        ContainerHelper = helper;
    }

    private void Init(DescriptorPropertyDefinition definition)
    {
        if (definition == null)
            throw new ArgumentNullException(nameof(definition));

        if (ContainerHelper != null)
        {
            ContainerHelper.ClearHelper();
        }
        this.DescriptorDefinition = definition;
        this.ContainerHelper = definition.CreateContainerHelper(this);
        definition.ContainerHelperInvalidated += OnDefinitionContainerHelperInvalidated;
    }


    #endregion


    #region IPropertyContainer Members


    PropertyContainerHelper IPropertyContainer.ContainerHelper
    {
        get
        {
            return this.ContainerHelper;
        }
    }

    bool? IPropertyContainer.IsPropertyVisible(PropertyDescriptor pd)
    {
        if (_parentNode != null)
        {
            return _parentNode.IsPropertyVisible(pd);
        }

        return null;
    }

    #endregion


    private class InvalidValueValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Will always return an error.
            return new ValidationResult(false, null);
        }
    }

}
