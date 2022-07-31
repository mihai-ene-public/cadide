namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public class FlashTextApertureGerberAttribute : ApertureGerberAttribute
{
    public string Text { get; set; }

    public bool IsBarcode { get; set; }
    public bool IsMirrored { get; set; }
    public string FontName { get; set; }
    public double? FontSize { get; set; }
    public string Comment { get; set; }

    public override string ToString()
    {
        var barcodeSpecifier = IsBarcode ? "B" : "C";
        var mirrored = IsMirrored ? "M" : "R";
        return $".FlashText,{Text},{barcodeSpecifier},{mirrored},{FontName},{FontSize},{Comment}";
    }
}