using System.Globalization;

namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public class CreationDateFileGerberAttribute : FileGerberAttribute
{
    public DateTime Value { get; set; }

    public override string ToString()
    {
        return $".CreationDate,{Value.ToString("o", CultureInfo.InvariantCulture)}";
    }
}
