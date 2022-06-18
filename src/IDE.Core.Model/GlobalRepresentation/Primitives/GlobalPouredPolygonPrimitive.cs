using System.Collections.Generic;
using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalPouredPolygonPrimitive : GlobalPrimitive, IPouredPolygonShape
    {
        public GlobalPrimitive FillPrimitive { get; set; }

        public List<GlobalPrimitive> RemovePrimitives { get; set; }

        public List<GlobalPrimitive> Thermals { get; set; }

        public IGeometryOutline FinalGeometry { get; set; }

        public override void AddClearance(double clearance)
        {

        }
    }
}
