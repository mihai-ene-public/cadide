using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    /// <summary>
    /// a reference to a file in a ProjectDocument or SolutionDocument
    /// </summary>
    public abstract class ProjectBaseFileRef: IProjectFileRef
    {
        [XmlAttribute("Path")]
        public string RelativePath { get; set; }

        public virtual ProjectBaseFileRef Clone()
        {
            return MemberwiseClone() as ProjectBaseFileRef;
        }
    }
}
