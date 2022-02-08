using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Circle : SchematicPrimitive, ICircle
    {
        public Circle()
        {
            BorderWidth = 0.5;
            BorderColor = "#FF000080";
            FillColor = "#00FFFFFF";
        }

        /// <summary>
        /// Center X
        /// </summary>
        [XmlAttribute("x")]
        public double x
        {
            get; set;
        }

        /// <summary>
        /// Center Y
        /// </summary>
        [XmlAttribute("y")]
        public double y
        {
            get; set;
        }


        [XmlAttribute("diameter")]
        public double Diameter
        {
            get; set;
        }


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
