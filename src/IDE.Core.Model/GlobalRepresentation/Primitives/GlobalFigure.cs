using System.Collections.Generic;
using IDE.Core.Interfaces;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalFigurePrimitive : GlobalPrimitive, IFigureShape
    {
        public IList<GlobalPrimitive> FigureItems { get; set; } = new List<GlobalPrimitive>();

        public IList<IShape> FigureShapes
        {
            get { return FigureItems.Cast<IShape>().ToList(); }
        }
        public override void AddClearance(double clearance)
        {

        }
    }
}
