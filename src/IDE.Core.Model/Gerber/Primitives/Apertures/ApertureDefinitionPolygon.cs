using IDE.Core.Gerber;

namespace IDE.Core.Model.Gerber.Primitives.Apertures;

public class ApertureDefinitionPolygon : ApertureDefinitionBase
{
    public ApertureDefinitionPolygon()
    {
        ApertureType = ApertureTypes.Polygon;
    }

    public double OuterDiameter { get; set; }
    public int NumberVertices { get; set; }
    public double Rot { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is ApertureDefinitionPolygon ad)
        {
            return OuterDiameter == ad.OuterDiameter 
                && NumberVertices == ad.NumberVertices
                && Rot == ad.Rot
                && HasSameApertureFunctionAs(ad);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
