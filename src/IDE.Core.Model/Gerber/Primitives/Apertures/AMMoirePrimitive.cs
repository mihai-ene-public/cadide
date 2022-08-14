using System.IO;

namespace IDE.Core.Model.Gerber.Primitives.Apertures;

/// <summary>
/// a cross hair centered on concentric rings. Exposure is always on.
/// </summary>
public class AMMoirePrimitive : ApertureMacroPrimitive
{
    public AMMoirePrimitive()
    {
        Code = 6;
    }

    public double CenterPointX { get; set; }

    public double CenterPointY { get; set; }
    public double OuterDiameter { get; set; }
    public double RingThickness { get; set; }

    /// <summary>
    /// gap between rings
    /// </summary>
    public double RingsGap { get; set; }

    public int MaxNumberRings { get; set; }
    public double CrossHairThickness { get; set; }
    public double CrossHairLength { get; set; }

    public override void WriteTo(TextWriter writer)
    {
        // writer.WriteLine($"0 {Comment}*");
        writer.WriteLine($"{Code},{WriteDouble(CenterPointX)},{WriteDouble(CenterPointY)},{WriteDouble(OuterDiameter)},{WriteDouble(RingThickness)},{WriteDouble(RingsGap)},{MaxNumberRings},{WriteDouble(CrossHairThickness)},{WriteDouble(CrossHairLength)},$1*");
    }
}



