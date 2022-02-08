using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Storage
{
    public class TextBoard : LayerPrimitive, ILayerPrimitive, IText
    {
        public TextBoard()
        {
            TextAlign = XTextAlignment.Center;
            TextDecoration = TextDecorationEnum.None;
            FontSize = 24;
            FontFamily = "Segoe UI";
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

        [XmlAttribute("wordWrap")]
        public bool WordWrap { get; set; }



        [XmlAttribute("width")]
        public double Width { get; set; }

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

        [XmlAttribute("bold")]
        public bool Bold { get; set; }

        [XmlAttribute("italic")]
        public bool Italic { get; set; }

        [XmlAttribute("isLocked")]
        public bool IsLocked { get; set; }

        [XmlText]
        public string Value
        {
            get; set;
        }

    }
}
