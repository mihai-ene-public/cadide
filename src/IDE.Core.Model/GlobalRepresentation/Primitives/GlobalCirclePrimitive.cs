namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalCirclePrimitive : GlobalPrimitive
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Diameter { get; set; }

        public bool IsFilled { get; set; } = true;
        public double BorderWidth { get; set; }

        public override void AddClearance(double clearance)
        {
            Diameter += 2 * clearance;
        }
    }
}
