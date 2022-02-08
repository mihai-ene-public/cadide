using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class TraceBoard : LayerPrimitive, ILayerPrimitive, ILine
    {
        public TraceBoard()
        {
        }


        [XmlAttribute]
        public double x1
        {
            get; set;
        }


        [XmlAttribute]
        public double y1
        {
            get; set;
        }


        [XmlAttribute]
        public double x2
        {
            get; set;
        }


        [XmlAttribute]
        public double y2
        {
            get; set;
        }


        [XmlAttribute]
        public double width
        {
            get; set;
        } = 0.2;


        [XmlAttribute("layer")]
        public int layerId
        {
            get; set;
        }
    }
}
