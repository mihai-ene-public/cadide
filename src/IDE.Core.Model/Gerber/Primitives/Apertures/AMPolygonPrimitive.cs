using System.IO;

namespace IDE.Core.Model.Gerber.Primitives.Apertures
{
    /// <summary>
    /// regular polygon defined by the number of vertices n, the center point and the diameter of the circumscribed circle
    /// </summary>
    public class AMPolygonPrimitive : ApertureMacroPrimitive
    {
        public AMPolygonPrimitive()
        {
            Code = 5;
        }

        public AMPrimitiveExposure Exposure { get; set; }

        public int NumberVertices { get; set; }
        public double CircleDiameter { get; set; }

        public double CenterPointX { get; set; }

        public double CenterPointY { get; set; }

        public override void WriteTo(TextWriter writer)
        {
            //writer.WriteLine($"0 {Comment}*");
            writer.WriteLine($"{Code},{(int)Exposure},{NumberVertices},{WriteDouble(CenterPointX)},{WriteDouble(CenterPointY)},{WriteDouble(CircleDiameter)},$1*");
        }
    }



}
