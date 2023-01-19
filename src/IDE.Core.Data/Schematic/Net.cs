using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Net
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }


        [XmlAttribute("classId")]
        public string ClassId { get; set; }

        [XmlElement("junction", typeof(Junction))]
        [XmlElement("label", typeof(NetLabel))]
        [XmlElement("pinref", typeof(PinRef))]
        //  [XmlElement("portref", typeof(PortRef))]
        [XmlElement("wire", typeof(NetWire))]
        public List<NetSegmentItem> Items
        {
            get; set;
        } = new List<NetSegmentItem>();
    }
}
