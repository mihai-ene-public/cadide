namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public enum AperFunctionFiducialPadValue
{
    /// <summary>
    /// Locate position of an individual component
    /// </summary>
    Local,

    /// <summary>
    /// Locates a single PCB
    /// </summary>
    Global,

    /// <summary>
    /// Locates a panel
    /// </summary>
    Panel
}

