using System.IO;

namespace IDE.Core.Model.Gerber.Primitives.Apertures
{
    public class AMVectorLinePrimitive : ApertureMacroPrimitive
    {
        public AMVectorLinePrimitive()
        {
            Code = 20;
        }

        public AMPrimitiveExposure Exposure { get; set; }

        public double Width { get; set; }

        public double StartPointX { get; set; }

        public double StartPointY { get; set; }

        public double EndPointX { get; set; }
        public double EndPointY { get; set; }

        public override void WriteTo(TextWriter writer)
        {
            writer.WriteLine($"{Code},{(int)Exposure},{WriteDouble(Width)},{WriteDouble(StartPointX)},{WriteDouble(StartPointY)},{WriteDouble(EndPointX)},{WriteDouble(EndPointY)},$1*");
        }
    }



}
