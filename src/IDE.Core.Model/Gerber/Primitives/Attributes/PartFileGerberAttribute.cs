namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public class PartFileGerberAttribute : FileGerberAttribute
{
    public PartFileGerberAttributeValue PartFileGerberAttributeValue { get; set; }
    public override string ToString()
    {
        var valueString = PartFileGerberAttributeValue.ToString();
        if (PartFileGerberAttributeValue == PartFileGerberAttributeValue.Other)
            valueString += ",etc";

        return $".Part,{valueString}";
    }
}
