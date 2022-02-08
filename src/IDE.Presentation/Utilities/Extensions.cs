using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IDE.Core
{
   public static class Extensions
    {
        public static T GetDataFromTreeControl<T>(this ItemsControl itemsControl, UIElement originalSource)
        {
            if (originalSource != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    if (originalSource is TreeViewItem)
                    {
                        data = (originalSource as TreeViewItem).Header;
                        if (data is T)
                            return (T)data;
                    }
                    else
                    {
                        data = itemsControl.ItemContainerGenerator.ItemFromContainer(originalSource);
                    }

                    if (data == DependencyProperty.UnsetValue)
                        originalSource = VisualTreeHelper.GetParent(originalSource) as UIElement;

                    if (originalSource == itemsControl)
                        return default(T);
                }

                if (data != DependencyProperty.UnsetValue && data is T)
                    return (T)data;
            }

            return default(T);
        }
    }
}
