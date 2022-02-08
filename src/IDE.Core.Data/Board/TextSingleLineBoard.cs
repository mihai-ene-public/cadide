using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows;
using IDE.Core.Interfaces;

namespace IDE.Core.Storage
{
    public class TextSingleLineBoard : LayerPrimitive, ILayerPrimitive
    {
        public TextSingleLineBoard()
        {
            FontSize = 1;
            FontName = "Default";
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

        [XmlAttribute("layer")]
        public int layerId
        {
            get; set;
        }

        [XmlAttribute("rot")]
        public double rot
        {
            get; set;
        }

        [XmlAttribute("fontName")]
        public string FontName { get; set; }

        [XmlAttribute("fontSize")]
        public double FontSize { get; set; }

        [XmlAttribute("strokeWidth")]
        public double StrokeWidth { get; set; } = 0.2;

        [XmlAttribute("isLocked")]
        public bool IsLocked { get; set; }

        [XmlText]
        public string Value
        {
            get; set;
        }

    }
}
