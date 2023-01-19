using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public enum polygonPour
    {
        solid,
        hatch,
        cutout,
    }

    public class Polygon : SchematicPrimitive, IPolygon
    {
        public Polygon()
        {
            vertices = new List<Vertex>();

            BorderWidth = 0.5;
            BorderColor = "#FF000080";
            FillColor = "#00FFFFFF";
        }

        [XmlElement("vertex")]
        public List<Vertex> vertices
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
