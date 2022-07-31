namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public class FilePolarityFileGerberAttribute : FileGerberAttribute
{
    public FilePolarityValue Value { get; set; }

    public override string ToString()
    {
        return $".FilePolarity,{Value}";
    }
}
