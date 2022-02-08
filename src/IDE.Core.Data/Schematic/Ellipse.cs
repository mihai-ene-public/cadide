using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Ellipse : SchematicPrimitive, IEllipse
    {

        public Ellipse()
        {
            BorderWidth = 0.5;
            BorderColor = "#FF000080";
            FillColor = "#00FFFFFF";
        }


        /// <summary>
        /// center X
        /// </summary>
        [XmlAttribute("x")]
        public double x
        {
            get; set;
        }

        /// <summary>
        /// center Y
        /// </summary>
        [XmlAttribute("y")]
        public double y
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

    }
}
