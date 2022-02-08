using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace IDE.Core.Editors
{
    public class GenericValueEditor : ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            return GetEditorElement(propertyItem, nameof(propertyItem.Value));
        }

        private FrameworkElement GetEditorElement(PropertyItem propertyItem, string propertyName)
        {
            //textbox
            var textBox = new TextBox();
            textBox.BorderBrush = Brushes.Transparent;
            textBox.Background = Brushes.Transparent;
            textBox.VerticalContentAlignment = VerticalAlignment.Center;

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

            //create the binding from the bound property item to the editor
            var binding = new Binding(propertyName); //bind to the Value property of the PropertyItem
            binding.Source = propertyItem;
            binding.ValidatesOnExceptions = true;
            binding.ValidatesOnDataErrors = true;
            binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;

            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);

            return textBox;
        }
    }
}
