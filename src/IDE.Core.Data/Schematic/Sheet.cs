using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    /// <summary>
    /// a sheet that belongs to a schematic document
    /// </summary>
    public class Sheet
    {
        [XmlElement("description")]
        public Description description
        {
            get; set;
        }

        //Id?

        [XmlAttribute("name")]
        public string Name { get; set; }

#if VERSION20
        [XmlArray("instancedModules")]
        [XmlArrayItem("module")]
        public List<InstancedModule> InstancedModules { get; set; }
#endif

        //todo: consider rename instance -> partGate
        [XmlArray("partGates")]
        [XmlArrayItem("partGate")]
        public List<Instance> Instances { get; set; } = new List<Instance>();

        // public Plain Plain { get; set; }

        [XmlArray("items")]
        [XmlArrayItem("circle", typeof(Circle))]
        [XmlArrayItem("ellipse", typeof(Ellipse))]
        [XmlArrayItem("image", typeof(ImagePrimitive))]
        [XmlArrayItem("poly", typeof(Polygon))]
        [XmlArrayItem("rect", typeof(Rectangle))]
        [XmlArrayItem("text", typeof(Text))]
        [XmlArrayItem("line", typeof(LineSchematic))]
        [XmlArrayItem("arc", typeof(Arc))]
        public List<SchematicPrimitive> PlainItems
        {
            get; set;
        } = new List<SchematicPrimitive>();


        [XmlArray("busses")]
        [XmlArrayItem("bus")]
        public List<Bus> Busses { get; set; } = new List<Bus>();

        [XmlArray("nets")]
        [XmlArrayItem("net")]
        public List<Net> Nets { get; set; } = new List<Net>();
    }
}
