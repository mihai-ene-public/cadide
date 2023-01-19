using IDE.Core.Data.Packages;
using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    //can have projects
    //solution folders; projects can be deployed in these solution folders
    //this will be serialized 

    [XmlRoot("Solution")]
    public class SolutionDocument: ISolutionDocument
    {
        public const string SolutionExtension = "solution";

        [XmlElement("Project", typeof(SolutionProjectItem))]
        [XmlElement("GroupFolder", typeof(GroupFolderItem))]
        public List<ProjectBaseFileRef> Children { get; set; } = new List<ProjectBaseFileRef>();

        [XmlElement("package")]
        public PackageMetadata Package { get; set; }
    }
}
