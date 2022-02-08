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


        public ProjectDocument()
        {
            References = new List<ProjectDocumentReference>();
        }

        /// <summary>
        /// OutputType gives the project type
        /// </summary>
        [XmlAttribute]
        public ProjectOutputType OutputType { get; set; }

        [XmlArray("References")]
        [XmlArrayItem("Library", typeof(LibraryProjectReference))]
        [XmlArrayItem("ProjectReference", typeof(ProjectProjectReference))]
        public List<ProjectDocumentReference> References { get; set; }

        public ProjectProperties Properties { get; set; } = new ProjectProperties();

        [XmlIgnore]
        public string FilePath { get; set; }

        public void Save(string filePath)
        {
            var ser = new XmlSerializer(GetType());
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            using (var sw = new StreamWriter(filePath))
            {
                ser.Serialize(sw, this, ns);
            }
        }

        public void Save()
        {
            Save(FilePath);
        }

        public static ProjectDocument Load(string filePath)
        {
            var ser = new XmlSerializer(typeof(ProjectDocument));
            using (var sr = new StreamReader(filePath))
            {
                var p = (ProjectDocument)ser.Deserialize(sr);
                p.FilePath = filePath;
                return p;
            }
        }
    }
}
