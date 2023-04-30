using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.Presentation.Placement
{
    public class PinCanvasItemHelper
    {

        public PinCanvasItemHelper(ICanvasDesignerFileViewModel canvasModel)
        {
            pins = from part in canvasModel.Items.OfType<SchematicSymbolCanvasItem>()
                   from p in part.Pins
                   select p;
        }

        IEnumerable<PinCanvasItem> pins;

        public List<PinCanvasItem> GetPinsCollision(XPoint point, double diameter)
        {
            var hitPins = new List<PinCanvasItem>();

            foreach (var pin in pins)
            {
                var pinTransform = pin.GetTransform();
                var pinPoint = new XPoint();
                pinPoint = pinTransform.Transform(pinPoint);
                if (Geometry2DHelper.CirclesIntersect(point, 0.5 * diameter, pinPoint, 0.5 * pin.Width))
                {
                    hitPins.Add(pin);
                    break;//only the 1st pin
                }
            }

            return hitPins;
        }
    }
}
