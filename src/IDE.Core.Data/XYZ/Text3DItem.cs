using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Windows;
using IDE.Core.Types.Media;

namespace IDE.Core.Storage
{
    public class Text3DItem : MeshPrimitive
    {
        public Text3DItem()
        {
            TextAlign = XTextAlignment.Center;
            TextDecoration = TextDecorationEnum.None;
            FontSize = 24;
            FontFamily = "Segoe UI";

        }

        [XmlAttribute("centerX")]
        public double CenterX { get; set; }

        [XmlAttribute("centerY")]
        public double CenterY { get; set; }

        [XmlAttribute("centerZ")]
        public double CenterZ { get; set; }

        [XmlAttribute("rotationX")]
        public double RotationX { get; set; }

        [XmlAttribute("rotationY")]
        public double RotationY { get; set; }

        [XmlAttribute("rotationZ")]
        public double RotationZ { get; set; }

        [XmlAttribute("textAlign")]
        public XTextAlignment TextAlign
        {
            get; set;
        }



        [XmlAttribute("textDecoration")]
        public TextDecorationEnum TextDecoration { get; set; }

        [XmlAttribute("fontFamily")]
        public string FontFamily { get; set; }

        [XmlAttribute("fontSize")]
        public double FontSize { get; set; }

        [XmlAttribute("height")]
        public double Height { get; set; }

        [XmlAttribute("bold")]
        public bool Bold { get; set; }

        [XmlAttribute("italic")]
        public bool Italic { get; set; }

        [XmlText]
        public string Value
        {
            get; set;
        }
    }
}
