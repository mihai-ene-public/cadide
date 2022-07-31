using IDE.Core.Gerber;

namespace IDE.Core.Model.Gerber.Primitives.Apertures
{
    public class ApertureDefinitionRectangle : ApertureDefinitionBase
    {
        public ApertureDefinitionRectangle()
        {
            ApertureType = ApertureTypes.Rectangle;
        }

        public double Width { get; set; }
        public double Height { get; set; }


        public override bool Equals(object obj)
        {
            if (obj is ApertureDefinitionRectangle ad)
            {
                return Width == ad.Width && Height == ad.Height && HasSameApertureFunctionAs(ad);

            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }



}
