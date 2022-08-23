using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using IDE.Controls.WPF.Core.Extensions;

namespace IDE.Controls.WPF.PropertyGrid;
public class PropertyGrid : Control, ISupportInitialize, IPropertyContainer
{
    public event PropertyChangedEventHandler PropertyChanged;

    private const string PART_DragThumb = "PART_DragThumb";
    internal const string PART_PropertyItemsControl = "PART_PropertyItemsControl";

    private Thumb _dragThumb;
    private bool _hasPendingSelectedObjectChanged;
    private int _initializationCount;
    private PropertyContainerHelper _containerHelper;


    #region Constructors

    static PropertyGrid()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
    }

    public PropertyGrid()
    {
        UpdateContainerHelper();

        PropertyValueChanged += PropertyGrid_PropertyValueChanged;

        AddHandler(PropertyItem.ItemSelectionChangedEvent, new RoutedEventHandler(OnItemSelectionChanged));
        AddHandler(PropertyItemsControl.PreparePropertyItemEvent, new PropertyItemEventHandler(OnPreparePropertyItemInternal));
        AddHandler(PropertyItemsControl.ClearPropertyItemEvent, new PropertyItemEventHandler(OnClearPropertyItemInternal));
    }

    #endregion //Constructors


    #region NameColumnWidth

    public static readonly DependencyProperty NameColumnWidthProperty = DependencyProperty.Register("NameColumnWidth", typeof(double), typeof(PropertyGrid), new UIPropertyMetadata(150.0, OnNameColumnWidthChanged));
    public double NameColumnWidth
    {
        get
        {
            return (double)GetValue(NameColumnWidthProperty);
        }
        set
        {
            SetValue(NameColumnWidthProperty, value);
        }
    }

    private static void OnNameColumnWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        PropertyGrid propertyGrid = o as PropertyGrid;
        if (propertyGrid != null)
            propertyGrid.OnNameColumnWidthChanged((double)e.OldValue, (double)e.NewValue);
    }

    protected virtual void OnNameColumnWidthChanged(double oldValue, double newValue)
    {
        if (_dragThumb != null)
            ( (TranslateTransform)_dragThumb.RenderTransform ).X = newValue;
    }

    #endregion //NameColumnWidth


    #region Properties

    public IList Properties
    {
        get
        {
            return ( _containerHelper != null ) ? _containerHelper.Properties : null;
        }
    }

    #endregion //Properties


    #region SelectedObject

    public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyGrid), new UIPropertyMetadata(null, OnSelectedObjectChanged));
    public object SelectedObject
    {
        get
        {
            return (object)GetValue(SelectedObjectProperty);
        }
        set
        {
            SetValue(SelectedObjectProperty, value);
        }
    }

    private static void OnSelectedObjectChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        PropertyGrid propertyInspector = o as PropertyGrid;
        if (propertyInspector != null)
            propertyInspector.OnSelectedObjectChanged((object)e.OldValue, (object)e.NewValue);
    }

    protected virtual void OnSelectedObjectChanged(object oldValue, object newValue)
    {
        // We do not want to process the change now if the grid is initializing (ie. BeginInit/EndInit).
        if (_initializationCount != 0)
        {
            _hasPendingSelectedObjectChanged = true;
            return;
        }

        this.UpdateContainerHelper();

        RaiseEvent(new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, PropertyGrid.SelectedObjectChangedEvent));
    }

    #endregion //SelectedObject


    #region SelectedPropertyItem

    private static readonly DependencyPropertyKey SelectedPropertyItemPropertyKey = DependencyProperty.RegisterReadOnly("SelectedPropertyItem", typeof(PropertyItem), typeof(PropertyGrid), new UIPropertyMetadata(null, OnSelectedPropertyItemChanged));
    public static readonly DependencyProperty SelectedPropertyItemProperty = SelectedPropertyItemPropertyKey.DependencyProperty;
    public PropertyItem SelectedPropertyItem
    {
        get
        {
            return (PropertyItem)GetValue(SelectedPropertyItemProperty);
        }
        internal set
        {
            SetValue(SelectedPropertyItemPropertyKey, value);
        }
    }

    private static void OnSelectedPropertyItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        PropertyGrid propertyGrid = o as PropertyGrid;
        if (propertyGrid != null)
            propertyGrid.OnSelectedPropertyItemChanged((PropertyItem)e.OldValue, (PropertyItem)e.NewValue);
    }

    protected virtual void OnSelectedPropertyItemChanged(PropertyItem oldValue, PropertyItem newValue)
    {
        if (oldValue != null)
            oldValue.IsSelected = false;

        if (newValue != null)
            newValue.IsSelected = true;

        this.SelectedProperty = ( ( newValue != null ) && ( _containerHelper != null ) ) ? _containerHelper.ItemFromContainer(newValue) : null;

        RaiseEvent(new RoutedPropertyChangedEventArgs<PropertyItem>(oldValue, newValue, PropertyGrid.SelectedPropertyItemChangedEvent));
    }

    #endregion //SelectedPropertyItem

    #region SelectedProperty

    /// <summary>
    /// Identifies the SelectedProperty dependency property
    /// </summary>
    public static readonly DependencyProperty SelectedPropertyProperty =
        DependencyProperty.Register("SelectedProperty", typeof(object), typeof(PropertyGrid), new UIPropertyMetadata(null, OnSelectedPropertyChanged));

    /// <summary>
    /// Gets or sets the selected property or returns null if the selection is empty.
    /// </summary>
    public object SelectedProperty
    {
        get { return (object)GetValue(SelectedPropertyProperty); }
        set { SetValue(SelectedPropertyProperty, value); }
    }

    private static void OnSelectedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        PropertyGrid propertyGrid = sender as PropertyGrid;
        if (propertyGrid != null)
        {
            propertyGrid.OnSelectedPropertyChanged((object)args.OldValue, (object)args.NewValue);
        }
    }

    private void OnSelectedPropertyChanged(object oldValue, object newValue)
    {
        // Do not update the SelectedPropertyItem if the Current SelectedPropertyItem
        // item is the same as the new SelectedProperty. There may be 
        // duplicate items and the result could be to change the selection to the wrong item.
        if (_containerHelper != null)
        {
            object currentSelectedProperty = _containerHelper.ItemFromContainer(this.SelectedPropertyItem);
            if (!object.Equals(currentSelectedProperty, newValue))
            {
                this.SelectedPropertyItem = _containerHelper.ContainerFromItem(newValue);
            }
        }
    }

    #endregion //SelectedProperty

    #region ShowTitle

    public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register("ShowTitle", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
    public bool ShowTitle
    {
        get
        {
            return (bool)GetValue(ShowTitleProperty);
        }
        set
        {
            SetValue(ShowTitleProperty, value);
        }
    }

    #endregion //ShowTitle

    #region ShowSummary

    public static readonly DependencyProperty ShowSummaryProperty = DependencyProperty.Register("ShowSummary", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
    public bool ShowSummary
    {
        get
        {
            return (bool)GetValue(ShowSummaryProperty);
        }
        set
        {
            SetValue(ShowSummaryProperty, value);
        }
    }

    #endregion //ShowSummary


    #region Base Class Overrides

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_dragThumb != null)
            _dragThumb.DragDelta -= DragThumb_DragDelta;
        _dragThumb = GetTemplateChild(PART_DragThumb) as Thumb;
        if (_dragThumb != null)
            _dragThumb.DragDelta += DragThumb_DragDelta;

        if (_containerHelper != null)
        {
            _containerHelper.ChildrenItemsControl = GetTemplateChild(PART_PropertyItemsControl) as PropertyItemsControl;
        }

        //Update TranslateTransform in code-behind instead of XAML to remove the
        //output window error.
        //When we use FindAncesstor in custom control template for binding internal elements property 
        //into its ancestor element, Visual Studio displays data warning messages in output window when 
        //binding engine meets unmatched target type during visual tree traversal though it does the proper 
        //binding when it receives expected target type during visual tree traversal
        //ref : http://www.codeproject.com/Tips/124556/How-to-suppress-the-System-Windows-Data-Error-warn
        TranslateTransform _moveTransform = new TranslateTransform();
        _moveTransform.X = NameColumnWidth;
        if (_dragThumb != null)
        {
            _dragThumb.RenderTransform = _moveTransform;
        }

        this.UpdateThumb();
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        // First check that the raised property is actually a real CLR property.
        // This could be something else like a Attached DP.
        this.OnPropertyChanged(PropertyChanged, e.Property.Name);
    }


    #endregion //Base Class Overrides

    #region Event Handlers

    private void OnItemSelectionChanged(object sender, RoutedEventArgs args)
    {
        PropertyItem item = (PropertyItem)args.OriginalSource;
        if (item.IsSelected)
        {
            SelectedPropertyItem = item;
        }
        else
        {
            if (object.ReferenceEquals(item, SelectedPropertyItem))
            {
                SelectedPropertyItem = null;
            }
        }
    }

    private void OnPreparePropertyItemInternal(object sender, PropertyItemEventArgs args)
    {
        if (_containerHelper != null)
        {
            _containerHelper.PrepareChildrenPropertyItem(args.PropertyItem, args.Item);
        }
        args.Handled = true;
    }

    private void OnClearPropertyItemInternal(object sender, PropertyItemEventArgs args)
    {
        if (_containerHelper != null)
        {
            _containerHelper.ClearChildrenPropertyItem(args.PropertyItem, args.Item);
        }
        args.Handled = true;
    }

    private void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        NameColumnWidth = Math.Min(Math.Max(this.ActualWidth * 0.1, NameColumnWidth + e.HorizontalChange), this.ActualWidth * 0.9);
    }


    private void PropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
    {
        var modifiedPropertyItem = e.OriginalSource as PropertyItem;
        if (modifiedPropertyItem != null)
        {
            // Need to refresh the PropertyGrid Properties.
            if (modifiedPropertyItem.WillRefreshPropertyGrid)
            {
                // Refresh the PropertyGrid...this will set the initial Categories states.
                this.UpdateContainerHelper();
            }
        }
    }


    #endregion //Event Handlers

    private void UpdateContainerHelper()
    {
        // Keep a backup of the template element and initialize the
        // new helper with it.
        ItemsControl childrenItemsControl = null;
        if (_containerHelper != null)
        {
            childrenItemsControl = _containerHelper.ChildrenItemsControl;
            _containerHelper.ClearHelper();
        }

        _containerHelper = new PropertyContainerHelper(this, SelectedObject);
        _containerHelper.GenerateProperties();


        if (_containerHelper != null)
        {
            _containerHelper.ChildrenItemsControl = childrenItemsControl;
        }


        // Since the template will bind on this property and this property
        // will be different when the property parent is updated.
        this.OnPropertyChanged(PropertyChanged, nameof(Properties));
    }

    private void UpdateThumb()
    {
        if (_dragThumb != null)
        {
            _dragThumb.Margin = new Thickness(-1, 0, 0, 0);
        }
    }

    #region ISupportInitialize Members

    public override void BeginInit()
    {
        base.BeginInit();
        _initializationCount++;
    }

    public override void EndInit()
    {
        base.EndInit();
        if (--_initializationCount == 0)
        {
            if (_hasPendingSelectedObjectChanged)
            {
                //This will update SelectedObject, Type, Name based on the actual config.
                UpdateContainerHelper();
                _hasPendingSelectedObjectChanged = false;
            }
        }
    }

    #endregion

    #region SelectedPropertyItemChangedEvent Routed Event

    public static readonly RoutedEvent SelectedPropertyItemChangedEvent = EventManager.RegisterRoutedEvent("SelectedPropertyItemChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<PropertyItem>), typeof(PropertyGrid));
    public event RoutedPropertyChangedEventHandler<PropertyItem> SelectedPropertyItemChanged
    {
        add
        {
            AddHandler(SelectedPropertyItemChangedEvent, value);
        }
        remove
        {
            RemoveHandler(SelectedPropertyItemChangedEvent, value);
        }
    }
    #endregion

    #region SelectedObjectChangedEventRouted Routed Event

    public static readonly RoutedEvent SelectedObjectChangedEvent = EventManager.RegisterRoutedEvent("SelectedObjectChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(PropertyGrid));
    public event RoutedPropertyChangedEventHandler<object> SelectedObjectChanged
    {
        add
        {
            AddHandler(SelectedObjectChangedEvent, value);
        }
        remove
        {
            RemoveHandler(SelectedObjectChangedEvent, value);
        }
    }

    #endregion


    #region PropertyValueChangedEvent Routed Event
    public static readonly RoutedEvent PropertyValueChangedEvent = EventManager.RegisterRoutedEvent("PropertyValueChanged", RoutingStrategy.Bubble, typeof(PropertyValueChangedEventHandler), typeof(PropertyGrid));
    public event PropertyValueChangedEventHandler PropertyValueChanged
    {
        add
        {
            AddHandler(PropertyValueChangedEvent, value);
        }
        remove
        {
            RemoveHandler(PropertyValueChangedEvent, value);
        }
    }
    #endregion

    #region IsPropertyBrowsable Event

    public event IsPropertyBrowsableHandler IsPropertyBrowsable;

    #endregion

    #region PreparePropertyItemEvent Attached Routed Event

    /// <summary>
    /// Identifies the PreparePropertyItem event.
    /// This attached routed event may be raised by the PropertyGrid itself or by a
    /// PropertyItemBase containing sub-items.
    /// </summary>
    public static readonly RoutedEvent PreparePropertyItemEvent = EventManager.RegisterRoutedEvent("PreparePropertyItem", RoutingStrategy.Bubble, typeof(PropertyItemEventHandler), typeof(PropertyGrid));

    /// <summary>
    /// This event is raised when a property item is about to be displayed in the PropertyGrid.
    /// This allow the user to customize the property item just before it is displayed.
    /// </summary>
    public event PropertyItemEventHandler PreparePropertyItem
    {
        add
        {
            AddHandler(PropertyGrid.PreparePropertyItemEvent, value);
        }
        remove
        {
            RemoveHandler(PropertyGrid.PreparePropertyItemEvent, value);
        }
    }

    /// <summary>
    /// Adds a handler for the PreparePropertyItem attached event
    /// </summary>
    /// <param name="element">the element to attach the handler</param>
    /// <param name="handler">the handler for the event</param>
    public static void AddPreparePropertyItemHandler(UIElement element, PropertyItemEventHandler handler)
    {
        element.AddHandler(PropertyGrid.PreparePropertyItemEvent, handler);
    }

    /// <summary>
    /// Removes a handler for the PreparePropertyItem attached event
    /// </summary>
    /// <param name="element">the element to attach the handler</param>
    /// <param name="handler">the handler for the event</param>
    public static void RemovePreparePropertyItemHandler(UIElement element, PropertyItemEventHandler handler)
    {
        element.RemoveHandler(PropertyGrid.PreparePropertyItemEvent, handler);
    }

    internal static void RaisePreparePropertyItemEvent(UIElement source, PropertyItem propertyItem, object item)
    {
        source.RaiseEvent(new PropertyItemEventArgs(PropertyGrid.PreparePropertyItemEvent, source, propertyItem, item));
    }

    #endregion

    #region ClearPropertyItemEvent Attached Routed Event

    /// <summary>
    /// Identifies the ClearPropertyItem event.
    /// This attached routed event may be raised by the PropertyGrid itself or by a
    /// PropertyItemBase containing sub items.
    /// </summary>
    public static readonly RoutedEvent ClearPropertyItemEvent = EventManager.RegisterRoutedEvent("ClearPropertyItem", RoutingStrategy.Bubble, typeof(PropertyItemEventHandler), typeof(PropertyGrid));
    /// <summary>
    /// This event is raised when an property item is about to be remove from the display in the PropertyGrid
    /// This allow the user to remove any attached handler in the PreparePropertyItem event.
    /// </summary>
    public event PropertyItemEventHandler ClearPropertyItem
    {
        add
        {
            AddHandler(PropertyGrid.ClearPropertyItemEvent, value);
        }
        remove
        {
            RemoveHandler(PropertyGrid.ClearPropertyItemEvent, value);
        }
    }

    /// <summary>
    /// Adds a handler for the ClearPropertyItem attached event
    /// </summary>
    /// <param name="element">the element to attach the handler</param>
    /// <param name="handler">the handler for the event</param>
    public static void AddClearPropertyItemHandler(UIElement element, PropertyItemEventHandler handler)
    {
        element.AddHandler(PropertyGrid.ClearPropertyItemEvent, handler);
    }

    /// <summary>
    /// Removes a handler for the ClearPropertyItem attached event
    /// </summary>
    /// <param name="element">the element to attach the handler</param>
    /// <param name="handler">the handler for the event</param>
    public static void RemoveClearPropertyItemHandler(UIElement element, PropertyItemEventHandler handler)
    {
        element.RemoveHandler(PropertyGrid.ClearPropertyItemEvent, handler);
    }

    internal static void RaiseClearPropertyItemEvent(UIElement source, PropertyItem propertyItem, object item)
    {
        source.RaiseEvent(new PropertyItemEventArgs(PropertyGrid.ClearPropertyItemEvent, source, propertyItem, item));
    }

    #endregion


    #region IPropertyContainer Members


    PropertyContainerHelper IPropertyContainer.ContainerHelper
    {
        get
        {
            return _containerHelper;
        }
    }

    bool? IPropertyContainer.IsPropertyVisible(PropertyDescriptor pd)
    {
        var handler = this.IsPropertyBrowsable;
        //If anyone is registered to PropertyGrid.IsPropertyBrowsable event
        if (handler != null)
        {
            var isBrowsableArgs = new IsPropertyBrowsableArgs(pd);
            handler(this, isBrowsableArgs);

            return isBrowsableArgs.IsBrowsable;
        }

        return null;
    }




    #endregion

}

#region PropertyValueChangedEvent Handler/Args
public delegate void PropertyValueChangedEventHandler(object sender, PropertyValueChangedEventArgs e);
public class PropertyValueChangedEventArgs : RoutedEventArgs
{
    public object NewValue
    {
        get;
        set;
    }
    public object OldValue
    {
        get;
        set;
    }

    public PropertyValueChangedEventArgs(RoutedEvent routedEvent, object source, object oldValue, object newValue)
      : base(routedEvent, source)
    {
        NewValue = newValue;
        OldValue = oldValue;
    }
}
#endregion

#region PropertyItemCreatedEvent Handler/Args
public delegate void PropertyItemEventHandler(object sender, PropertyItemEventArgs e);
public class PropertyItemEventArgs : RoutedEventArgs
{
    public PropertyItem PropertyItem
    {
        get;
        private set;
    }

    public object Item
    {
        get;
        private set;
    }

    public PropertyItemEventArgs(RoutedEvent routedEvent, object source, PropertyItem propertyItem, object item)
      : base(routedEvent, source)
    {
        PropertyItem = propertyItem;
        Item = item;
    }
}
#endregion

#region IsPropertyArgs class

public class PropertyArgs : RoutedEventArgs
{
    public PropertyArgs(PropertyDescriptor pd)
    {
        this.PropertyDescriptor = pd;
    }

    public PropertyDescriptor PropertyDescriptor
    {
        get;
        private set;
    }

}

#endregion

#region isPropertyBrowsableEvent Handler/Args

public delegate void IsPropertyBrowsableHandler(object sender, IsPropertyBrowsableArgs e);

public class IsPropertyBrowsableArgs : PropertyArgs
{

    public IsPropertyBrowsableArgs(PropertyDescriptor pd)
      : base(pd)
    {
    }


    public bool? IsBrowsable
    {
        get;
        set;
    }

}

#endregion
