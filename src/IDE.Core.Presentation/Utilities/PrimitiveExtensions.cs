using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Linq;

namespace IDE.Core.Presentation.Utilities
{
    public static class PrimitiveExtensions
    {
        public static XPoint GetAbsolutePoint(this IPadCanvasItem padItem)
        {
            var point = new XPoint();

            var footprint = padItem.ParentObject as FootprintBoardCanvasItem;

            if (footprint != null && padItem != null)
            {
                var t = new XTransformGroup();

                t.Children.Add(new XRotateTransform(footprint.Rot));
                if (footprint.Placement == FootprintPlacement.Bottom)
                    t.Children.Add(new XScaleTransform
                    {
                        ScaleX = -1,
                    });

                t.Children.Add(new XTranslateTransform(footprint.X, footprint.Y));

                //if placement is on top, the point is the center of the pad
                //if placement is on bottom, the point is from a mirrorred pad
                var p = new XPoint(padItem.X, padItem.Y);

                p = t.Value.Transform(p);

                point = p;
            }


            return point;

        }



        public static ILayerDesignerItem GetLayer(this ILayeredViewModel document, int layerId)
        {
            if (layerId != 0 && document != null && document.LayerItems != null)
            {
                return document.LayerItems.FirstOrDefault(l => l.LayerId == layerId);
            }

            return null;
        }
    }
}
