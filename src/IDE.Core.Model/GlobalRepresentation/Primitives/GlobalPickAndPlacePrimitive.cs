using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    //currently it is best that this class be a global primitive, maybe we can change this later (it is not a shape)
    //it helps create gerber files for assembly pick and place with BOM data
    public class GlobalPickAndPlacePrimitive : GlobalPrimitive
    {
        public string PartName { get; set; }
        public FootprintPlacement Placement { get; set; }
        public string FootprintName { get; set; }
        public XPoint Center { get; set; }
        public double Rot { get; set; }
        public XPoint? Pin1Pos { get; set; }

        public string Manufacturer { get; set; }

        /// <summary>
        /// Manufacturer part number
        /// </summary>
        public string Mpn { get; set; }

        public string SupplierName { get; set; }

        public string SupplierPartNumber { get; set; }

        // public PickAndPlaceMountType MountType { get; set; }

        public IList<GlobalPrimitive> Items { get; set; } = new List<GlobalPrimitive>();

        public override void AddClearance(double clearance)
        {
        }
    }

}
