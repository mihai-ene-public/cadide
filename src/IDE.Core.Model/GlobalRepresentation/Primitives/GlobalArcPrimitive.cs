using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Types.Media;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalArcPrimitive : GlobalPrimitive
    {
        public XPoint StartPoint { get; set; }

        public XPoint EndPoint { get; set; }

        //stroke width
        public double Width { get; set; }

        public XSweepDirection SweepDirection { get; set; }

        //this is actual the radius
        public double SizeDiameter { get; set; }

        public override void AddClearance(double clearance)
        {
            Width += 2 * clearance;
        }
    }
}
