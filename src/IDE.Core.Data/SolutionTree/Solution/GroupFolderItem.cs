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
    /// a virtual folder inside the solution
    /// </summary>
    public class GroupFolderItem : ProjectBaseFileRef, IGroupFolderItem
    {

        [XmlElement("Project", typeof(SolutionProjectItem))]
        [XmlElement("GroupFolder", typeof(GroupFolderItem))]
        public List<ProjectBaseFileRef> Children { get; set; }

    }
}
