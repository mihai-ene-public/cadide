using System.IO;

namespace IDE.Core.Model.Gerber.Primitives.Apertures
{
    public class AMCirclePrimitive : ApertureMacroPrimitive
    {
        public AMCirclePrimitive()
        {
            Code = 1;
            Exposure = AMPrimitiveExposure.On;
        }

        public AMPrimitiveExposure Exposure { get; set; }

        public double Diameter { get; set; }

        public double CenterX { get; set; }

        public double CenterY { get; set; }

        ///// <summary>
        ///// Rotation angle of the center, in degrees counterclockwise.
        ///// </summary>
        //public double Rotation { get; set; }

        //rotation will be taken from the definition of the aperture (ADD)

        public override void WriteTo(TextWriter writer)
        {
            writer.WriteLine($"{Code},{(int)Exposure},{WriteDouble(Diameter)},{WriteDouble(CenterX)},{WriteDouble(CenterY)},$1*");
        }
    }



}
