using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace IDE.Core.StyleSelectors
{
    public class CanvasItemStyleSelector : StyleSelector
    {
        static CanvasItemStyleSelector()
        {
            Instance = new CanvasItemStyleSelector();
        }

        public static CanvasItemStyleSelector Instance
        {
            get;
            private set;
        }


        public override Style SelectStyle(object item, DependencyObject container)
        {
            var itemsControl = ItemsControl.ItemsControlFromItemContainer(container);
            if (itemsControl == null)
                throw new InvalidOperationException("DesignerItemsControlItemStyleSelector : Could not find ItemsControl");

            //if (item is ILineCanvasItem
            //    || item is IJunctionCanvasItem
            //    || item is ICircleCanvasItem
            //    || item is IEllipseCanvasItem
            //    || item is IPolygonCanvasItem
            //    || item is IRectangleCanvasItem
            //    || item is IHoleCanvasItem
            //    || item is IViaCanvasItem
            //    || item is IPadCanvasItem
            //    || item is ITextCanvasItem
            //    || item is IPinCanvasItem
            //    || item is ISymbolCanvasItem
            //    || item is IRegionCanvasItem
            //    )
            //{
            //    return (Style)itemsControl.FindResource("floatingItemStyle");
            //}
            //if (item is BaseCanvasItem)
            //{
            //    return (Style)itemsControl.FindResource("floatingItemStyle");
            //}

            //return null;

            var style = ResourceLocator.GetResource<Style>(
                                      "IDE.Presentation",
                                        "Resources/Styles/SchematicDesignerStyles.xaml",
                                        "floatingItemStyle");

            return style;
        }
    }
}
