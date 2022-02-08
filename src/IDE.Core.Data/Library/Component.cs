using IDE.Core.BOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    [XmlRoot("component")]
    public class ComponentDocument : LibraryItem
    {


        //U, Q, D, etc
        [XmlAttribute("prefix")]
        public string Prefix
        {
            get; set;
        }

        [XmlElement("comment")]
        public string Comment { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlAttribute("componentType")]
        public ComponentType Type { get; set; }


        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<Property> Properties { get; set; }

        [XmlArray("bomItems")]
        [XmlArrayItem("bomItem")]
        public List<BomItem> BomItems { get; set; }

        [XmlArray("gates")]
        [XmlArrayItem("gate")]
        public List<Gate> Gates { get; set; }

        //[XmlArray("footprints")]
        //[XmlArrayItem("footprint")]
        //public List<FootprintRef> Footprints { get; set; }

        [XmlElement("footprint")]
        public FootprintRef Footprint { get; set; }

    }

    public enum ComponentType
    {
        Standard,
        StandardNoBom,
        Mechanical
    }

}
