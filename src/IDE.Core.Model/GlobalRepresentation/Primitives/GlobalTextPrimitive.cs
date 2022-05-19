namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public class GlobalTextPrimitive : GlobalPrimitive
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Rot { get; set; }
        public double FontSize { get; set; }
        public double Width { get; set; }
        public string FontFamily { get; set; }
        public string Text { get; set; }
        public bool Italic { get; set; }
        public bool Bold { get; set; }

        public override void AddClearance(double clearance)
        {

        }
    }
}
