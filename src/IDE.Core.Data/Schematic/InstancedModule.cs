using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class InstancedModule
    {
        [XmlElement("property")]
        public List<Property> Properties
        {
            get; set;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get; set;
        }


        [XmlAttribute("module")]
        public string Module
        {
            get; set;
        }

        //?
        [XmlAttribute("modulevariant")]
        public string ModuleVariant
        {
            get; set;
        }


        [XmlAttribute("x")]
        public double x
        {
            get; set;
        }


        [XmlAttribute("y")]
        public double y
        {
            get; set;
        }


        [XmlAttribute("offset")]
        public int Offset
        {
            get; set;
        }

        [XmlAttribute("rot")]
        public string rot
        {
            get; set;
        }
    }
}
