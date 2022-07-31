namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public class PinObjectGerberAttribute : ObjectGerberAttribute
{
    public string PartName { get; set; }
    public string PinNumber { get; set; }
    public string PinName { get; set; }

    public override string ToString()
    {
        var s = $".P,{PartName},{PinNumber}";
        if (PinName != null)
            s += $",{PinName}";
        return s;
    }
}
