using IDE.Core.Converters;
using IDE.Core.Designers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using System.Windows.Media;
using IDE.Core.ViewModels;
using IDE.Core.Interfaces;

namespace IDE.Core.Editors
{
    public class SizeMilimetersUnitsEditor : ITypeEditor
    {

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            return GetEditorElement(propertyItem, nameof(propertyItem.Value));
        }

        public FrameworkElement GetEditorElement(PropertyItem propertyItem, string propertyName)
        {
            var grid = new Grid();
            //grid.Background = new SolidColorBrush(Color.FromRgb(21, 21, 21));
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            //textbox
            var textBox = new TextBox();
            textBox.BorderBrush = Brushes.Transparent;
            textBox.Background = Brushes.Transparent;
            textBox.VerticalContentAlignment = VerticalAlignment.Center;
            grid.Children.Add(textBox);
            textBox.SetValue(Grid.ColumnProperty, 0);
            textBox.GotKeyboardFocus += (s, e) => { textBox.SelectAll(); };
            textBox.MouseDoubleClick += (s, e) => { textBox.SelectAll(); };
            textBox.PreviewMouseLeftButtonDown += (s, e) =>
            {
                if (!textBox.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    textBox.Focus();
                }
            };

            IDrawingViewModel parent = CanvasHelper.GetCanvasModelFromCurrentActiveDocument();
            //if (propertyItem.Instance is ISelectableItem)
            //    parent = (propertyItem.Instance as ISelectableItem).Parent;
            //else if (propertyItem.Instance is MultipleSelectionObject)
            //    parent = (propertyItem.Instance as MultipleSelectionObject).Parent;

            if (parent == null)
                return null;

            var converter = new TextToMilimetersSizeConverter { CanvasGrid = (CanvasGrid)parent.CanvasGrid };

            //create the binding from the bound property item to the editor
            var binding = new Binding(propertyName); //bind to the Value property of the PropertyItem
            binding.Source = propertyItem;
            binding.ValidatesOnExceptions = true;
            binding.ValidatesOnDataErrors = true;
            binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            binding.Converter = converter;
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);

            //textblock
            var tb = new TextBlock();
            tb.Margin = new Thickness(5);
            tb.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(tb);
            tb.SetValue(Grid.ColumnProperty, 1);
            tb.Text = ((CanvasGrid)parent.CanvasGrid).GridSizeModel.SelectedItem.DisplayNameShort;

            return grid;
        }
    }
}
