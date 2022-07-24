using IDE.Core.Gerber;

namespace IDE.Core.Model.Gerber.Primitives.Apertures
{
    public class ApertureDefinitionCircle : ApertureDefinitionBase
    {
        public ApertureDefinitionCircle()
        {
            ApertureType = ApertureTypes.Circle;
        }

        public double Diameter { get; set; }

        public override bool Equals(object obj)
        {
            var ad = obj as ApertureDefinitionCircle;
            if (ad != null)
            {
                return //ApertureType == ad.ApertureType && 
                        Diameter == ad.Diameter;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }



}
