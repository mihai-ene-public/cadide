using IDE.Core.Interfaces;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalHolePrimitive : GlobalPrimitive, IHoleShape
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Drill { get; set; }

        public override void AddClearance(double clearance)
        {
            Drill += 2 * clearance;
        }
    }
}
