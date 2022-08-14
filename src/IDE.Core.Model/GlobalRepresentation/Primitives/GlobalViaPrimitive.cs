using IDE.Core.Interfaces;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalViaPrimitive : GlobalPrimitive, IViaShape
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Drill { get; set; }
        public double PadDiameter { get; set; }

        public override void AddClearance(double clearance)
        {
            PadDiameter += 2 * clearance;
        }
    }

}
