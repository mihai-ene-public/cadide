namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalHolePrimitive : GlobalPrimitive
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
