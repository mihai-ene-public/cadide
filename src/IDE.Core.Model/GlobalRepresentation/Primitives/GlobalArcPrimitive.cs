using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalArcPrimitive : GlobalPrimitive, IArcShape
    {
        public XPoint StartPoint { get; set; }

        public XPoint EndPoint { get; set; }

        //stroke width
        public double Width { get; set; }

        public bool IsLargeArc { get; set; }

        public XSweepDirection SweepDirection { get; set; }

        public double SizeDiameter { get; set; }
        
        public bool IsMirrored { get; set; }

        public XPoint Center { get; set; }
        public double RotationAngle { get; set; }

        public override void AddClearance(double clearance)
        {
            Width += 2 * clearance;
        }
    }
}
