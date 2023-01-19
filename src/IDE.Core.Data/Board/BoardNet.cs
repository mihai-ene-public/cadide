using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class BoardNet
    {
        /// <summary>
        /// Id of net from the schematic (lowest id?)
        /// </summary>
        [XmlAttribute("id")]
        public string NetId { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("classId")]
        public string ClassId { get; set; }

        /// <summary>
        /// in this net all pads must be connected together (net list)
        /// </summary>
        [XmlArray("pads")]
        [XmlArrayItem("pad")]//, typeof(PadRef))]
        public List<PadRef> Pads { get; set; } = new List<PadRef>();

        /// <summary>
        /// the actual routing of this net's instance (Signal Items)
        /// </summary>
        [XmlElement("poly", typeof(PolygonBoard))]
        [XmlElement("via", typeof(Via))]
        //[XmlElement("trace", typeof(TraceBoard))]
        [XmlElement("track", typeof(TrackBoard))]
        [XmlElement("plane", typeof(PlaneBoard))]
        [XmlElement("pad", typeof(Pad))]
        [XmlElement("smd", typeof(Smd))]
        public List<LayerPrimitive> Items { get; set; } = new List<LayerPrimitive>();

    }
}
