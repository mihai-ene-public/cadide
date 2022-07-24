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
            var ad = obj as ApertureDefinitionRectangle;
            if (ad != null)
            {
                return //ApertureType == ad.ApertureType &&
                        Width == ad.Width && Height == ad.Height;

            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }



}
