using System.Xml.Serialization;

namespace IDE.Core.Storage;

/// <summary>
/// A reference to a package
/// </summary>
public class PackageProjectReference : ProjectDocumentReference
{
    [XmlAttribute("id")]
    public string PackageId { get; set; }

    [XmlAttribute("version")]
    public string PackageVersion { get; set; }

    /// <summary>
    /// Url or a folder path
    /// </summary>
    [XmlAttribute("source")]
    public string PackageSource { get; set; }

    public override string ToString()
    {
        return $"{PackageId} ({PackageVersion})";
    }
}
