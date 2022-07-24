using IDE.Core.Gerber;

namespace IDE.Core.Model.Gerber.Primitives.Apertures
{
    public class ApertureDefinitionRotatedRoundedRectangle : ApertureDefinitionBase
    {
        public ApertureDefinitionRotatedRoundedRectangle()
        {
            ApertureType = ApertureTypes.RotatedRoundedRectangle;
        }

        public double Width { get; set; }
        public double Height { get; set; }

        public double Rot { get; set; }
        public double CornerRadius { get; set; }


        public override bool Equals(object obj)
        {
            var ad = obj as ApertureDefinitionRotatedRoundedRectangle;
            if (ad != null)
            {
                return Width == ad.Width && Height == ad.Height && ad.Rot == Rot && CornerRadius == ad.CornerRadius;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }



}
