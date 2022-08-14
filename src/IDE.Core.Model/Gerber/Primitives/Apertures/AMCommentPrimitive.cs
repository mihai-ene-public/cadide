using System.IO;

namespace IDE.Core.Model.Gerber.Primitives.Apertures;

public class AMCommentPrimitive : ApertureMacroPrimitive
{
    public AMCommentPrimitive()
    {
        Code = 0;
    }

    public string Comment { get; set; }

    public override void WriteTo(TextWriter writer)
    {
        writer.WriteLine($"{Code} {Comment}*");
    }
}



