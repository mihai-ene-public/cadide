using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    /// <summary>
    /// A line to draw on a symbol or a schematic
    /// </summary>
    public class LineSchematic : SchematicPrimitive, ILine
    {
        public LineSchematic()
        {
            lineStyle = LineStyle.Solid;
            LineColor = "#FF000080";
        }

        //coordinates are in mm from top-left of a canvas 2D

        [XmlAttribute("x1")]
        public double x1
        {
            get; set;
        }


        [XmlAttribute("y1")]
        public double y1
        {
            get; set;
        }


        [XmlAttribute("x2")]
        public double x2
        {
            get; set;
        }


        [XmlAttribute("y2")]
        public double y2
        {
            get; set;
        }

        /// <summary>
        /// thickness in mm
        /// </summary>
        [XmlAttribute("width")]
        public double width
        {
            get; set;
        }

        [XmlAttribute("lineStyle")]
        public LineStyle lineStyle { get; set; }

        [XmlAttribute("lineCap")]
        public LineCap LineCap { get; set; } = LineCap.Round;

        [XmlAttribute("lineColor")]
        public string LineColor { get; set; }

    }
}
