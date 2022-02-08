using IDE.Core.Converters;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
namespace IDE.Core.Editors
{
    public class PositionYUnitsEditor : ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            return GetEditorElement(propertyItem, nameof(propertyItem.Value));
        }

        public FrameworkElement GetEditorElement(PropertyItem propertyItem, string propertyName)
        {
            //var app = ServiceProvider.GetService<IApplicationViewModel>();
            IDrawingViewModel canvasModel = CanvasHelper.GetCanvasModelFromCurrentActiveDocument();
            //if (app != null)
            //{
            //    var currentDoc = app.ActiveDocument as CanvasDesignerFileViewModel;
            //    canvasModel = currentDoc?.CanvasModel;
            //}

            //if (propertyItem.Instance is ISelectableItem)
            //    parent = (propertyItem.Instance as ISelectableItem).Parent;
            //else if (propertyItem.Instance is MultipleSelectionObject)
            //    parent = (propertyItem.Instance as MultipleSelectionObject).Parent;

            if (canvasModel == null)
                return null;

            var converter = new TextToMilimetersPositionYConverter { CanvasGrid = (CanvasGrid)canvasModel.CanvasGrid };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            //textbox
            var textBox = new TextBox();
            textBox.BorderBrush = Brushes.Transparent;
            textBox.Background = Brushes.Transparent;
            //textBox.HorizontalContentAlignment = HorizontalAlignment.Right;
            textBox.VerticalContentAlignment = VerticalAlignment.Center;
            grid.Children.Add(textBox);
            textBox.SetValue(Grid.ColumnProperty, 0);
            textBox.GotKeyboardFocus += (s, e) => { textBox.SelectAll(); };
            //textBox.GotMouseCapture += (s, e) => { textBox.SelectAll(); };
            textBox.MouseDoubleClick += (s, e) => { textBox.SelectAll(); };
            textBox.PreviewMouseLeftButtonDown += (s, e) =>
            {
                if (!textBox.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    textBox.Focus();
                }
            };



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
            tb.Text = ((CanvasGrid)canvasModel.CanvasGrid).GridSizeModel.SelectedItem.DisplayNameShort;

            return grid;
        }
    }
}
