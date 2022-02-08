using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class NetLabel : NetSegmentItem
    {
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

        [XmlAttribute("textColor")]
        public string textColor { get; set; } = "#FFFFFF";

        [XmlAttribute("fontFamily")]
        public string FontFamily { get; set; } = "Segoe UI";

        [XmlAttribute("fontSize")]
        public double FontSize { get; set; } = 8;

        [XmlAttribute("bold")]
        public bool Bold { get; set; }

        [XmlAttribute("italic")]
        public bool Italic { get; set; }

        [XmlAttribute("rot")]
        public double rot
        {
            get; set;
        }

        [XmlAttribute("textDecoration")]
        public TextDecorationEnum TextDecoration { get; set; }

    }
}
