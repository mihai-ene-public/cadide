namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public class PartNameObjectGerberAttribute : ObjectGerberAttribute
{
    public string PartName { get; set; }

    public override string ToString()
    {
        return $".C,{PartName}";
    }
}