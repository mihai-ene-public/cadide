namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public class FileFunctionFileGerberAttribute : FileGerberAttribute
{
    public FileFunctionType FileFunctionType { get; set; }

    public object Value { get; set; }

    public override string ToString()
    {
        return $".FileFunction,{FileFunctionType},{Value}";
    }
}
