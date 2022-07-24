using System.IO;

namespace IDE.Core.Model.Gerber.Primitives.Apertures
{
    public class AMCenterLinePrimitive : ApertureMacroPrimitive
    {
        public AMCenterLinePrimitive()
        {
            Code = 21;
        }

        public AMPrimitiveExposure Exposure { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }

        public double CenterPointX { get; set; }

        public double CenterPointY { get; set; }

        public override void WriteTo(TextWriter writer)
        {
            if (Width == 0.00d || Height == 0.00d)
                return;
            writer.WriteLine($"{Code},{(int)Exposure},{WriteDouble(Width)},{WriteDouble(Height)},{WriteDouble(CenterPointX)},{WriteDouble(CenterPointY)},$1*");
        }
    }



}
