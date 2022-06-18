using System.Collections.Generic;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalPolygonPrimitive : GlobalPrimitive, IPolygonShape
    {
        public IList<XPoint> Points { get; set; } = new List<XPoint>();

        public double BorderWidth { get; set; }

        public bool IsFilled { get; set; }

        public override void AddClearance(double clearance)
        {
            //todo: offset outline
        }
    }
}
