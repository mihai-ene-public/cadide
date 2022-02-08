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

namespace IDE.Core.Editors
{
    public class OrderEditor : ITypeEditor
    {

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var sp = new StackPanel { Orientation = Orientation.Horizontal };

            var btnDecreaseOrder = new Button();
            btnDecreaseOrder.Content = "-";
            btnDecreaseOrder.FontSize = 18;
            btnDecreaseOrder.Width = btnDecreaseOrder.Height = 24;
            btnDecreaseOrder.Padding = new Thickness(0);
            btnDecreaseOrder.Click += (s, e) =>
            {
                propertyItem.Value = (int)propertyItem.Value - 1;
            };

            var btnIncreaseOrder = new Button();
            btnIncreaseOrder.Margin = new Thickness(3, 0, 0, 0);
            btnIncreaseOrder.Content = "+";
            btnIncreaseOrder.FontSize = 18;
            btnIncreaseOrder.Width = btnIncreaseOrder.Height = 24;
            btnIncreaseOrder.Padding = new Thickness(0);
            btnIncreaseOrder.Click += (s, e) =>
            {
                propertyItem.Value = (int)propertyItem.Value + 1;
            };

            sp.Children.Add(btnDecreaseOrder);
            sp.Children.Add(btnIncreaseOrder);

            return sp;
        }
    }
}
