using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Rectangle : SchematicPrimitive, IRectangle
    {

        public Rectangle()
        {
            BorderWidth = 0.5;
            BorderColor = "#FF000080";
            FillColor= "#00FFFFFF";
        }

        /// <summary>
        /// Left coord of the top-left corner
        /// </summary>
        [XmlAttribute("x")]
        public double X { get; set; }

        /// <summary>
        /// Top coord of the top-left corner
        /// </summary>
        [XmlAttribute("y")]
        public double Y { get; set; }

        [XmlAttribute("rot")]
        public double Rot { get; set; }

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

        [XmlAttribute("radiusX")]
        public double RadiusX { get; set; }

        [XmlAttribute("radiusY")]
        public double RadiusY { get; set; }

    }
}
