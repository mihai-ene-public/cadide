using IDE.Core.Gerber;

namespace IDE.Core.Model.Gerber.Primitives.Apertures;

public class ApertureDefinitionCircle : ApertureDefinitionBase
{
    public ApertureDefinitionCircle()
    {
        ApertureType = ApertureTypes.Circle;
    }

    public double Diameter { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is ApertureDefinitionCircle ad)
        {
            return  Diameter == ad.Diameter && HasSameApertureFunctionAs(ad);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
