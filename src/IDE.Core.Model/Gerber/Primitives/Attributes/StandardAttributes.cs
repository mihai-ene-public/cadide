namespace IDE.Core.Model.Gerber.Primitives.Attributes;

/// <summary>
/// attributes attached to file
/// </summary>
public enum StandardFileAttributes
{
    Part,
    FileFunction,
    FilePolarity,
    CreationDate,
    GenerationSoftware,
    ProjectId,
    MD5,

    
}

/// <summary>
/// Attributes attached to an aperture
/// </summary>
public enum StandardApertureAttributes
{
    AperFunction,
    FlashText,
}

/// <summary>
/// attributes attached to graphical object
/// </summary>
public enum StandardObjectAttributes
{
    
    /// <summary>
    /// net name
    /// </summary>
    Net,

    /// <summary>
    /// pin number and reference descriptor
    /// </summary>
    Pin,

    /// <summary>
    /// component reference designator linked to an object
    /// </summary>
    PartName,

}