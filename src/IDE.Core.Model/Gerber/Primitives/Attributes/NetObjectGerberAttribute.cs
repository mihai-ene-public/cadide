namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public class NetObjectGerberAttribute : ObjectGerberAttribute
{
    /// <summary>
    /// Use null for object not connected to a net
    /// <para>use "N/C" for a single pad net</para>
    /// </summary>
    public string NetName { get; set; }

    public override string ToString()
    {
        return $".N,{NetName}";
    }
}
