using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Frame
    {
        [XmlAttribute()]
        public double x1
        {
            get; set;
        }


        [XmlAttribute()]
        public double y1
        {
            get; set;
        }


        [XmlAttribute()]
        public double x2
        {
            get; set;
        }


        [XmlAttribute()]
        public double y2
        {
            get; set;
        }


        [XmlAttribute()]
        public int columns
        {
            get; set;
        }


        [XmlAttribute()]
        public int rows
        {
            get; set;
        }


        [XmlAttribute()]
        public int layer
        {
            get; set;
        }


        [XmlAttribute("border-left")]
        public bool borderleft
        {
            get; set;
        }


        [XmlAttribute("border-top")]
        public bool bordertop
        {
            get; set;
        }


        [XmlAttribute("border-right")]
        public bool borderright
        {
            get; set;
        }


        [XmlAttribute("border-bottom")]
        public bool borderbottom
        {
            get; set;
        }
    }
}
