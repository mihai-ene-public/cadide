using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{

    public enum TextDecorationEnum
    {
        None,
        Baseline,
        OverLine,
        Strikethrough,
        Underline
    }

    public class Text : SchematicPrimitive, IText
    {
        public Text()
        {
            TextAlign = XTextAlignment.Center;
            TextDecoration = TextDecorationEnum.None;
            textColor = "#FFFFFF";
            backgroundColor = "#00FFFFFF";
            FontSize = 24;
            FontFamily = "Segoe UI";
        }

        //positions in the top-left corrdinates in mm
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
        public string textColor { get; set; }

        [XmlAttribute("backgroundColor")]
        public string backgroundColor { get; set; }

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


        [XmlText]
        public string Value
        {
            get; set;
        }

    }
}
