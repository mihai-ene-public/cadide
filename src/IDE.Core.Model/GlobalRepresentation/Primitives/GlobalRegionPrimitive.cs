using System.Collections.Generic;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalRegionPrimitive : GlobalPrimitive
    {
        public List<GlobalPrimitive> RegionItems { get; set; } = new List<GlobalPrimitive>();

        public override void AddClearance(double clearance)
        {

        }
    }
}
