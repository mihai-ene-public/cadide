using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Module
    {
        [XmlAttribute("name")]
        public string Name
        {
            get; set;
        }

        //?
        [XmlAttribute("prefix")]
        public string Prefix
        {
            get; set;
        }


        [XmlAttribute("dx")]
        public double dx
        {
            get; set;
        }


        [XmlAttribute("dy")]
        public double dy
        {
            get; set;
        }

        [XmlElement("description")]
        public Description Description
        {
            get; set;
        }

        [XmlArray("ports")]
        [XmlArrayItem("port")]
        public List<Port> Ports { get; set; }

        [XmlArray("parts")]
        [XmlArrayItem("part")]
        public List<Part> Parts { get; set; }

        //Variants

        [XmlArray("sheets")]
        [XmlArrayItem("sheet")]
        public List<Sheet> Sheets { get; set; }
    }
}
