using System.Collections.Generic;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalRegionPrimitive : GlobalPrimitive, IRegionShape
    {
        public XPoint StartPoint { get; set; }
        public double Width { get; set; }
        public IList<IShape> Items { get; set; }=new List<IShape>();

        public override void AddClearance(double clearance)
        {

        }
    }
}
