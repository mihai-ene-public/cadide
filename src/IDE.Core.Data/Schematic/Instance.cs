using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{

    public class Instance : SchematicPrimitive//todo this should higher than a primitive (something that inherits from Primitive)
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("partId")]
        public string PartId { get; set; }

        [XmlAttribute("gateId")]
        public long GateId { get; set; }


        [XmlElement("property")]
        public List<Property> Properties
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

        [XmlAttribute("scaleX")]
        public double ScaleX { get; set; } = 1.0;


        [XmlAttribute("scaleY")]
        public double ScaleY { get; set; } = 1.0;

        [XmlAttribute("rot")]
        public double Rot
        {
            get; set;
        }

        [XmlAttribute("partNameX")]
        public double PartNameX { get; set; }

        [XmlAttribute("partNameY")]
        public double PartNameY { get; set; }

        [XmlAttribute("partNameRot")]
        public double PartNameRot { get; set; }

        [XmlAttribute("showName")]
        public bool ShowName { get; set; } = true;

        [XmlAttribute("commentX")]
        public double CommentX { get; set; }

        [XmlAttribute("commentY")]
        public double CommentY { get; set; }

        [XmlAttribute("commentRot")]
        public double CommentRot { get; set; }

        [XmlAttribute("comment")]
        public string Comment { get; set; }

        [XmlAttribute("showComment")]
        public bool ShowComment { get; set; }

    }
}
