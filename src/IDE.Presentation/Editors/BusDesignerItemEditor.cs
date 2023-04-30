using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using IDE.Controls.WPF.PropertyGrid;
using IDE.Controls.WPF.PropertyGrid.Editors;

namespace IDE.Core.Editors
{
    public class BusDesignerItemEditor : ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {

            var comboBox = new ComboBox();
            comboBox.IsEditable = true;

            //var net = propertyItem.Value as NetDesignerItem;
            var netLabel = propertyItem.Instance as BusLabelCanvasItem;
            var bus = netLabel.Bus;
            var canvasModel = GetCurrentCanvas();
            if (bus != null && canvasModel != null)
            {
                var busses = canvasModel.Items.OfType<BusSegmentCanvasItem>()
                                          .Where(n => n.Bus != null)
                                          .OrderBy(n => n.Bus.Name)
                                          .Select(n => n.Bus.Name)
                                          .Distinct()
                                          .ToList();
                comboBox.ItemsSource = busses;
            }

            //create the binding from the bound property item to the editor
            var _binding = new Binding("Value"); //bind to the Value property of the PropertyItem
            _binding.Source = propertyItem;
            _binding.ValidatesOnExceptions = true;
            _binding.ValidatesOnDataErrors = true;
            _binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(comboBox, ComboBox.TextProperty, _binding);

            return comboBox;
        }

        ICanvasDesignerFileViewModel GetCurrentCanvas()
        {
            var canvasDoc = DocumentHelper.GetCurrentDocument<ICanvasDesignerFileViewModel>();
            return canvasDoc;
        }
    }
}
