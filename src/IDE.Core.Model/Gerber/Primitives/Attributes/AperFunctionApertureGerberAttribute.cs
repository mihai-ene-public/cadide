namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public class AperFunctionApertureGerberAttribute : ApertureGerberAttribute
{
    public AperFunctionType AperFunctionType { get; set; }
    public object Value { get; set; }

    public override string ToString()
    {
        if (Value != null)
            return $".AperFunction,{AperFunctionType},{Value}";

        return $".AperFunction,{AperFunctionType}";
    }
}
