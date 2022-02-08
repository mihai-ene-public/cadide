using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Storage
{
    public class Arc : SchematicPrimitive, IArc
    {
        public Arc()
        {

        }

        [XmlElement("startPoint")]
        public XPoint StartPoint { get; set; }

        [XmlElement("endPoint")]
        public XPoint EndPoint { get; set; }

        [XmlElement("size")]
        public XSize Size { get; set; }

        [XmlAttribute("borderWidth")]
        public double BorderWidth { get; set; }

        [XmlAttribute("borderColor")]
        public string BorderColor { get; set; } = "#FF000080";

        [XmlAttribute("fillColor")]
        public string FillColor { get; set; } = "#00FFFFFF";

        [XmlAttribute("isFilled")]
        public bool IsFilled { get; set; }

        [XmlAttribute("sweepDirection")]
        public XSweepDirection SweepDirection { get; set; }

        [XmlAttribute("isLargeArc")]
        public bool IsLargeArc { get; set; }

        [XmlAttribute("rotationAngle")]
        public double RotationAngle { get; set; }

        [XmlAttribute("lineCap")]
        public LineCap LineCap { get; set; } = LineCap.Round;

    }
}
