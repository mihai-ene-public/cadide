using IDE.Core.Adorners;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace IDE.Core.Presentation.Adorners
{
    public class CanvasItemToAdornerMapper
    {

        public static Adorner ShowAdorner(ISelectableItem selectableItem, UIElement element)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            if (adornerLayer != null)
            {
                var adorners = adornerLayer.GetAdorners(element);

                var adornerType = GetAdornerType(selectableItem);

                if (adorners == null || adorners.FirstOrDefault(ad => ad.GetType() == adornerType) == null)
                {
                    //add the adorner
                    var adorner = CreateAdorner(adornerType, element);

                    if (adorner != null)
                        adornerLayer.Add(adorner);
                    return adorner;
                }

            }

            return null;
        }

        static Type GetAdornerType(ISelectableItem selectableItem)
        {
            var mapping = new Dictionary<Type, Type>()
            {

                //line
                { typeof(LineSchematicCanvasItem),typeof(LineAdorner)},
                { typeof(LineBoardCanvasItem),typeof(LineAdorner)},

                //arc
                { typeof(ArcCanvasItem),typeof(ArcAdorner)},
                { typeof(ArcBoardCanvasItem),typeof(ArcAdorner)},

                //text
                //text adorner is interfeering with inline editing of the text, so we remove the adorner
                { typeof(TextCanvasItem),typeof(TextAdorner)},
                { typeof(TextBoardCanvasItem),typeof(TextAdorner)},
               // { typeof(TextSingleLineBoardCanvasItem),typeof(TextPlacementTool)},

                //rect
                { typeof(RectangleCanvasItem),typeof(RectangleAdorner)},
                { typeof(RectangleBoardCanvasItem),typeof(RectangleAdorner)},

                //poly
                { typeof(PolygonCanvasItem),typeof(PolygonAdorner)},
                { typeof(PolygonBoardCanvasItem),typeof(PolygonAdorner)},

                //circle
                { typeof(CircleCanvasItem),typeof(CircleAdorner)},
                { typeof(CircleBoardCanvasItem),typeof(CircleAdorner)},

                //ellipse
                { typeof(EllipseCanvasItem),typeof(EllipseAdorner)},

                //image
                { typeof(ImageCanvasItem),typeof(ImageAdorner)},

                //pin
                { typeof(PinCanvasItem),typeof(PinCanvasItemAdorner)},

                //hole
               // { typeof(HoleCanvasItem),typeof(HolePlacementTool)},

                //pad/smd
                //{ typeof(PadCanvasItem),typeof(PadPlacementTool)},
                //{ typeof(SmdCanvasItem),typeof(PadPlacementTool)},

                //sch net wire
                { typeof(NetWireCanvasItem),typeof(PolyLineAdorner)},

                //junction
               // { typeof(JunctionCanvasItem),typeof(JunctionPlacementTool)},

                //net label
                //{ typeof(NetLabelCanvasItem),typeof(NetLabelPlacementTool)},

                //part
                //{ typeof(SchematicSymbolCanvasItem),typeof(PartPlacementTool)},

                //board via
                //{ typeof(ViaCanvasItem),typeof(ViaPlacementTool)},

                 //track (new routing)
                 { typeof(TrackBoardCanvasItem),typeof(PolyLineAdorner)},

                //bus wire
                 { typeof(BusWireCanvasItem),typeof(PolyLineAdorner)},
            };

            var canvasItemType = selectableItem.GetType();
            if (mapping.ContainsKey(canvasItemType))
            {
                var adornerType = mapping[canvasItemType];
                return adornerType;
            }

            return null;
        }

        /// <summary>
        /// if the item is not supposed to have an adorner, will return null
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        static Adorner CreateAdorner(Type adornerType, UIElement element)
        {

            // var adornerType = GetAdornerType(selectableItem);
            if (adornerType != null)
            {
                var adorner = (Adorner)Activator.CreateInstance(adornerType, element);
                return adorner;
            }

            return null;
        }
    }
}
