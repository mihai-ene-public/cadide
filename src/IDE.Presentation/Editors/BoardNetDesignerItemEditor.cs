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
    public class BoardNetDesignerItemEditor : ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {

            var comboBox = new ComboBox();
            var vis = Visibility.Collapsed;
            var signalPrimitive = propertyItem.Instance as ISignalPrimitiveCanvasItem;
            if (signalPrimitive != null)
            {
                var brd = (signalPrimitive as BoardCanvasItemViewModel).LayerDocument as IBoardDesigner;
                if (brd != null)
                {

                    var signals = brd.NetList.OrderBy(n => n.Name).ToList();
                    signals.Insert(0, new BoardNetDesignerItem(brd) { Id = -1, Name = "< None >" });
                    comboBox.ItemsSource = signals;

                    vis = Visibility.Visible;

                    //create the binding from the bound property item to the editor
                    var _binding = new Binding("Value"); //bind to the Value property of the PropertyItem
                    _binding.Source = propertyItem;
                    _binding.ValidatesOnExceptions = true;
                    _binding.ValidatesOnDataErrors = true;
                    _binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
                    BindingOperations.SetBinding(comboBox, ComboBox.SelectedItemProperty, _binding);
                }
            }

            propertyItem.Visibility = vis;

            return comboBox;
        }
    }
}
