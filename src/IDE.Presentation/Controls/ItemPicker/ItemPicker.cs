using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IDE.Controls
{
    [TemplatePart(Name = PART_ItemsSource, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_ItemPickerToggleButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = PART_ItemPickerPopup, Type = typeof(Popup))]
    public class ItemPicker : Control
    {
        private const string PART_ItemsSource = "PART_ItemsSource";
        private const string PART_ItemPickerToggleButton = "PART_ItemPickerToggleButton";
        private const string PART_ItemPickerPopup = "PART_ItemPickerPopup";


        static ItemPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemPicker), new FrameworkPropertyMetadata(typeof(ItemPicker)));
        }

        public ItemPicker()
        {
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideCapturedElement);

        }

        private ListBox _list;
        private ToggleButton _toggleButton;
        private bool _selectionChanged;

        #region IsOpen

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(ItemPicker), new UIPropertyMetadata(false));
        public bool IsOpen
        {
            get
            {
                return (bool)GetValue(IsOpenProperty);
            }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        #endregion IsOpen

        #region SelectedItem

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object),
                                                                                                     typeof(ItemPicker),
                                                                                                     new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public object SelectedItem
        {
            get
            {
                return GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }



        #endregion //SelectedColor

        #region SelectedItemTemplate

        public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register("SelectedItemTemplate", typeof(DataTemplate),
                                                                                                    typeof(ItemPicker),
                                                                                                    new FrameworkPropertyMetadata(null));
        public DataTemplate SelectedItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(SelectedItemTemplateProperty);
            }
            set
            {
                SetValue(SelectedItemTemplateProperty, value);
            }
        }

        #endregion SelectedItemTemplate

        #region ItemsSource

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
                                                                                             typeof(ItemPicker),
                                                                                             new FrameworkPropertyMetadata());


        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        #endregion

        #region ItemTemplate

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate),
                                                                                                    typeof(ItemPicker),
                                                                                                    new FrameworkPropertyMetadata(null));
        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemTemplateProperty);
            }
            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }

        #endregion SelectedItemTemplate

        private void OnMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
        {
            CloseColorPicker();
        }

        //maybe we don't need this; we could bind listbox.SelectedItem
        private void Color_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lb = (ListBox)sender;

            if (e.AddedItems.Count > 0)
            {
                SelectedItem = e.AddedItems[0];

                _selectionChanged = true;
                lb.SelectedIndex = -1; //for now we don't care about keeping track of the selected color
            }
        }

        private void CloseColorPicker()
        {
            if (IsOpen)
                IsOpen = false;
            ReleaseMouseCapture();

            _toggleButton?.Focus();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            // Close ColorPicker on MouseUp to prevent action of MouseUp on controls behind the ColorPicker.
            if (_selectionChanged)
            {
                CloseColorPicker();
                _selectionChanged = false;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_list != null)
                _list.SelectionChanged -= Color_SelectionChanged;

            _list = GetTemplateChild(PART_ItemsSource) as ListBox;
            if (_list != null)
                _list.SelectionChanged += Color_SelectionChanged;

            _toggleButton = this.Template.FindName(PART_ItemPickerToggleButton, this) as ToggleButton;
        }
    }
}
