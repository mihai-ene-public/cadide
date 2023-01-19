using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Gate
    {
        public Gate()
        {

        }

        /// <summary>
        /// Gate id
        /// </summary>
        [XmlAttribute("id")]
        public long Id { get; set; }


        /// <summary>
        /// Gate name
        /// </summary>
        [XmlAttribute("name")]
        public string name
        {
            get; set;
        }

        /// <summary>
        /// the name of the library that the symbol belongs to. May be null if the symbol is local (same library or same project)
        /// </summary>
        [XmlAttribute("libraryName")]
        public string LibraryName { get; set; }

        /// <summary>
        /// the reference to the symbol
        /// </summary>
        [XmlAttribute("symbolId")]
        public string symbolId { get; set; }
        
        /// <summary>
        /// name of the symbol
        /// </summary>
        [XmlAttribute("symbolName")]
        public string symbolName { get; set; }

        /// <summary>
        /// the order level that 2 gates in the same package can be swapped
        /// <para>used for easing the routing</para>
        /// </summary>
        [XmlAttribute("swaplevel")]
        public int swaplevel
        {
            get; set;
        }

    }
}
