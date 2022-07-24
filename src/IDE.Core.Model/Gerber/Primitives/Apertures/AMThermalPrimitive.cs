using System.IO;

namespace IDE.Core.Model.Gerber.Primitives.Apertures
{
    /// <summary>
    /// The thermal primitive is a ring (annulus) interrupted by four gaps. Exposure is always on.
    /// </summary>
    public class AMThermalPrimitive : ApertureMacroPrimitive
    {
        public AMThermalPrimitive()
        {
            Code = 7;
        }

        public double CenterPointX { get; set; }

        public double CenterPointY { get; set; }
        public double OuterDiameter { get; set; }
        public double InnerDiameter { get; set; }
        public double GapThickness { get; set; }

        public override void WriteTo(TextWriter writer)
        {
            writer.WriteLine($"{Code},{WriteDouble(CenterPointX)},{WriteDouble(CenterPointY)},{WriteDouble(OuterDiameter)},{WriteDouble(InnerDiameter)},{WriteDouble(GapThickness)},$1*");
        }
    }



}
