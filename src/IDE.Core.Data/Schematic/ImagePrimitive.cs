using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class ImagePrimitive : SchematicPrimitive, IImage
    {
        public ImagePrimitive()
        {


            BorderWidth = 0.5;
            BorderColor = "#FF000080";
            FillColor = "#00FFFFFF";
        }

        /// <summary>
        /// Left coordinate of the top-left corner
        /// </summary>
        [XmlAttribute("x")]
        public double X { get; set; }

        /// <summary>
        /// Top coordinate of the top-left corner
        /// </summary>
        [XmlAttribute("y")]
        public double Y { get; set; }

        [XmlAttribute("scaleX")]
        public double ScaleX { get; set; } = 1.0;


        [XmlAttribute("scaleY")]
        public double ScaleY { get; set; } = 1.0;

        [XmlAttribute("rot")]
        public double Rot
        {
            get; set;
        }

        [XmlAttribute("width")]
        public double Width { get; set; }

        [XmlAttribute("height")]
        public double Height { get; set; }


        [XmlAttribute("borderWidth")]
        public double BorderWidth
        {
            get; set;
        }

        [XmlAttribute("borderColor")]
        public string BorderColor { get; set; }

        [XmlAttribute("fillColor")]
        public string FillColor { get; set; }

        [XmlAttribute("cornerRadius")]
        public double CornerRadius { get; set; }

        //[XmlAttribute]
        //public double RadiusY { get; set; }
        [XmlAttribute("stretchMode")]
        public XStretch StretchMode { get; set; }

        [XmlElement("image")]
        public EmbeddedImage Image { get; set; }

    }

    public class EmbeddedImage
    {
        //[XmlAttribute]
        //public double Width { get; set; }

        //[XmlAttribute]
        //public double Height { get; set; }

        public byte[] Bytes { get; set; }
    }
}
