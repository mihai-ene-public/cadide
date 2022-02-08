using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class RectangleBoard : LayerPrimitive, ILayerPrimitive, IRectangle
    {
        public RectangleBoard()
        {
        }

        /// <summary>
        /// X coord of the top-left corner
        /// </summary>
        [XmlAttribute("x")]
        public double X { get; set; }

        [XmlAttribute("y")]
        public double Y { get; set; }

        [XmlAttribute("width")]
        public double Width { get; set; }

        [XmlAttribute("height")]
        public double Height { get; set; }

        [XmlAttribute("rot")]
        public double rot
        {
            get; set;
        }


        [XmlAttribute("borderWidth")]
        public double BorderWidth
        {
            get; set;
        } = 0.2;

        [XmlAttribute("isFilled")]
        public bool IsFilled { get; set; }

        [XmlAttribute("cornerRadius")]
        public double CornerRadius { get; set; }

        //[XmlAttribute]
        //public double RadiusY { get; set; }

        [XmlAttribute("layer")]
        public int layerId
        {
            get; set;
        }

        [XmlAttribute("isLocked")]
        public bool IsLocked { get; set; }
    }
}
