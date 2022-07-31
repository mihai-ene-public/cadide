namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public class FileFunctionTypeCopperValue
{
    public int LayerNumber { get; set; }
    public FileFunctionTypeCopperLayerSide CopperLayerSide { get; set; }

    public override string ToString()
    {
        return $"L{LayerNumber},{CopperLayerSide}";
    }
}