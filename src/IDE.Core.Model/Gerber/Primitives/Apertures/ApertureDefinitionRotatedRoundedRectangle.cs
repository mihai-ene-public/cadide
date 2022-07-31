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
            if (obj is ApertureDefinitionRotatedRoundedRectangle ad)
            {
                return Width == ad.Width 
                    && Height == ad.Height 
                    && ad.Rot == Rot 
                    && CornerRadius == ad.CornerRadius
                    && HasSameApertureFunctionAs(ad);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }



}
