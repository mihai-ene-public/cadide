using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalLinePrimitive : GlobalPrimitive
    {
        public XPoint StartPoint { get; set; }
        public XPoint EndPoint { get; set; }
        public double Width { get; set; }

        public LineStyle LineStyle { get; set; } = LineStyle.Solid;

        public override void AddClearance(double clearance)
        {
            Width += 2 * clearance;
        }
    }
}
