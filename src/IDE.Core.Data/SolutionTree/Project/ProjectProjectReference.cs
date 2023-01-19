using System.IO;
using System.Xml.Serialization;

namespace IDE.Core.Storage;

/// <summary>
/// a reference to a project
/// </summary>
public class ProjectProjectReference : ProjectDocumentReference
{
    /// <summary>
    /// the relative path to this project
    /// </summary>
    [XmlAttribute("projectPath")]
    public string ProjectPath { get; set; }

    public override string ToString()
    {
        return Path.GetFileNameWithoutExtension(ProjectPath);
    }
}
