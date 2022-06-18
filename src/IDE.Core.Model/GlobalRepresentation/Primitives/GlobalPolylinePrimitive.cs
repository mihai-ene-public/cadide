using System.Collections.Generic;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalPolylinePrimitive : GlobalPrimitive, IPolylineShape
    {
        public double Width { get; set; }

        public IList<XPoint> Points { get; set; }

        public LineStyle LineStyle { get; set; } = LineStyle.Solid;

        public override void AddClearance(double clearance)
        {
            Width += 2 * clearance;
        }
    }
}
