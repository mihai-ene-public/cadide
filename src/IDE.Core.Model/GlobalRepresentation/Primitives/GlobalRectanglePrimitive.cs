namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalRectanglePrimitive : GlobalPrimitive
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double CornerRadius { get; set; }

        public bool IsFilled { get; set; } = true;

        public double Rot { get; set; }

        public override void AddClearance(double clearance)
        {
            Width += 2 * clearance;
            Height += 2 * clearance;

            if (CornerRadius > 0.0d)
            {
                CornerRadius += clearance;
            }
        }
    }
}
