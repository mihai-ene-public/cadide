using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Utilities;
using IDE.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace IDE.Core.Editors
{
    public class NetDesignerItemEditor : ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {

            var comboBox = new ComboBox();
            comboBox.IsEditable = true;

            //var net = propertyItem.Value as NetDesignerItem;
            var netLabel = propertyItem.Instance as NetLabelCanvasItem;
            if (netLabel == null)
                return null;
            var net = netLabel.Net;

            var canvasModel = GetCurrentCanvas();
            if (net != null && canvasModel != null)
            {
                var nets = canvasModel.Items.OfType<NetSegmentCanvasItem>()
                                          .Where(n => n.Net != null)
                                          .OrderBy(n => n.Net.Name)
                                          .Select(n => n.Net.Name)
                                          .Distinct()
                                          .ToList();
                comboBox.ItemsSource = nets;
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

        IDrawingViewModel GetCurrentCanvas()
        {
            var canvasDoc = DocumentHelper.GetCurrentDocument<ICanvasDesignerFileViewModel>();
            return canvasDoc?.CanvasModel;
        }
    }
}
