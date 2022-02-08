using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    [XmlRoot("library")]
    public class LibraryDocument
    {

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("namespace")]
        public string Namespace { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Description serving as comment for documentation
        /// </summary>
        [XmlElement("description")]
        public Description Description { get; set; }

       
        //future: for all these, we can arrange them in folders; would have some categories

        [XmlArray("symbols")]
        [XmlArrayItem("symbol")]
        public List<Symbol> Symbols { get; set; }

        [XmlArray("footprints")]
        [XmlArrayItem("footprint")]
        public List<Footprint> Footprints { get; set; }

        [XmlArray("models")]
        [XmlArrayItem("model")]
        public List<ModelDocument> Models { get; set; }

        [XmlArray("components")]
        [XmlArrayItem("component")]
        public List<ComponentDocument> Components { get; set; }


       

    }

}
