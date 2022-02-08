using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class LineBoard : LayerPrimitive, ILayerPrimitive, ILine
    {
        public LineBoard()
        {
            lineStyle = LineStyle.Solid;
        }


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


        [XmlAttribute("width")]
        public double width
        {
            get; set;
        } = 0.2;


        [XmlAttribute("layer")]
        public int layerId
        {
            get; set;
        }

        [XmlAttribute("lineStyle")]
        public LineStyle lineStyle { get; set; }

        [XmlAttribute("isLocked")]
        public bool IsLocked { get; set; }
    }
}
