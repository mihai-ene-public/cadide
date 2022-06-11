using System.Collections.Generic;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalFigurePrimitive : GlobalPrimitive
    {
        public IList<GlobalPrimitive> FigureItems { get; set; } = new List<GlobalPrimitive>();

        public override void AddClearance(double clearance)
        {

        }
    }
}
