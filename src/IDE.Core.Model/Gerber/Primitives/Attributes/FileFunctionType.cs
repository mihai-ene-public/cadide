namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public enum FileFunctionType
{
    /// <summary>
    /// A conductor or copper layer
    /// </summary>
    Copper,

    /// <summary>
    /// A file containing only the board outline
    /// </summary>
    Profile,

    /// <summary>
    /// Solder mask or solder resist
    /// </summary>
    Soldermask,

    /// <summary>
    /// Silk or silkscreen
    /// </summary>
    Legend,

    Paste,

    Glue,

    Carbonmask,

    Goldmask,
    Heatsinkmask,
    Peelablemask,
    Silvermask,
    Tinmask,

    //Drawing layers
    AssemblyDrawing,

    //component (assembly)
    Component
}
