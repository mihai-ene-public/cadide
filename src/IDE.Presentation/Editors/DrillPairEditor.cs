using IDE.Core.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using IDE.Controls.WPF.PropertyGrid;
using IDE.Controls.WPF.PropertyGrid.Editors;

namespace IDE.Core.Editors
{
    public class DrillPairEditor : ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {

            var comboBox = new ComboBox();

            var layeredDoc = GetCurrentBoardDocument();
            comboBox.ItemsSource = layeredDoc?.DrillPairs;

            //create the binding from the bound property item to the editor
            var _binding = new Binding("Value"); //bind to the Value property of the PropertyItem
            _binding.Source = propertyItem;
            _binding.ValidatesOnExceptions = true;
            _binding.ValidatesOnDataErrors = true;
            _binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(comboBox, ComboBox.SelectedItemProperty, _binding);

            return comboBox;
        }

        IBoardDesigner GetCurrentBoardDocument()
        {
            var app = ServiceProvider.Resolve<IApplicationViewModel>();
            if (app != null)
            {
                var layeredDoc = app.ActiveDocument as IBoardDesigner;
                return layeredDoc;
            }
            return null;
        }
    }
}
