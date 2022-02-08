using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    /// <summary>
    /// contains reference data to a footprint
    /// </summary>
    public class FootprintRef
    {
        /// <summary>
        /// the name of the library that the footprint belongs to.
        /// <para> May be null if the footprint is local (same library or same project)</para>
        /// </summary>
        [XmlAttribute("libraryName")]
        public string LibraryName { get; set; }

        [XmlAttribute("footprintId")]
        public long footprintId
        {
            get; set;
        }

        [XmlAttribute("footprintName")]
        public string footprintName
        {
            get; set;
        }


        [XmlArray("pad2pinMaps")]
        [XmlArrayItem("map")]
        public List<Connect> Connects
        {
            get; set;
        }

    }
}
