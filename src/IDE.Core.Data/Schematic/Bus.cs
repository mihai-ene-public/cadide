using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Bus
    {
        //[XmlAttribute("id")]
        //public long Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

       // [XmlElement("junction", typeof(Junction))]
        [XmlElement("label", typeof(BusLabel))]
        //[XmlElement("pinref", typeof(PinRef))]
        //  [XmlElement("portref", typeof(PortRef))]
        [XmlElement("wire", typeof(BusWire))]
        public List<BusSegmentItem> Items { get; set; } = new List<BusSegmentItem>();




        [XmlArray("nets")]
        [XmlArrayItem("net")]
        public List<BusNet> Nets { get; set; }

    }
}
