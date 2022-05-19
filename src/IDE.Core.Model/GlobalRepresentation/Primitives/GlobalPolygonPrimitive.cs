using System.Collections.Generic;
using IDE.Core.Types.Media;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalPolygonPrimitive : GlobalPrimitive
    {
        public List<XPoint> Points { get; set; } = new List<XPoint>();

        public override void AddClearance(double clearance)
        {
            //todo: offset outline
        }
    }
}
