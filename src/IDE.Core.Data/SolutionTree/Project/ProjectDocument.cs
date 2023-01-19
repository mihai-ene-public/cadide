using IDE.Core.Data.Packages;
using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    /// <summary>
    /// a project file on disk. This is serialized.
    /// Contains references to other files or references to libraries or other projects
    /// </summary>
    [XmlRoot("Project")]
    public class ProjectDocument : IProjectDocument
    {
        public const string ProjectExtension = ".project";

        /// <summary>
        /// OutputType gives the project type
        /// </summary>
        [XmlAttribute]
        public ProjectOutputType OutputType { get; set; }

        [XmlArray("References")]
        [XmlArrayItem("Library", typeof(LibraryProjectReference))]
        [XmlArrayItem("ProjectReference", typeof(ProjectProjectReference))]
        [XmlArrayItem("PackageReference", typeof(PackageProjectReference))]
        public List<ProjectDocumentReference> References { get; set; } = new List<ProjectDocumentReference>();

        public ProjectProperties Properties { get; set; } = new ProjectProperties();

        [XmlElement("package")]
        public PackageMetadata Package { get; set; }
    }
}
