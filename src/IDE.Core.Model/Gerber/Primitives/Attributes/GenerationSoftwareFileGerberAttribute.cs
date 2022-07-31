namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public class GenerationSoftwareFileGerberAttribute : FileGerberAttribute
{
    public override string ToString()
    {
        return ".GenerationSoftware,cadide,XnoCAD";
    }
}