using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class PolygonBoard : LayerPrimitive, ILayerPrimitive, IPolygon
    {

        public PolygonBoard()
        {
            vertices = new List<Vertex>();

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

        [XmlAttribute("isFilled")]
        public bool IsFilled { get; set; }

        [XmlAttribute("order")]
        public int DrawOrder { get; set; }

        [XmlAttribute("type")]
        public PolygonType Type { get; set; } = PolygonType.Fill;

        [XmlAttribute("generateThermals")]
        public bool GenerateThermals { get; set; } = true;

        [XmlAttribute("thermalWidth")]
        public double ThermalWidth { get; set; } = 0.2;

        //[XmlAttribute()]
        //public double spacing
        //{
        //    get; set;
        //}


        //[XmlAttribute()]
        //public polygonPour pour
        //{
        //    get; set;
        //}


        //[XmlAttribute()]
        //public double isolate
        //{
        //    get; set;
        //}


        //[XmlAttribute()]
        //public bool orphans
        //{
        //    get; set;
        //}


        //[XmlAttribute()]
        //public bool thermals
        //{
        //    get; set;
        //}


        //[XmlAttribute()]
        //public int rank
        //{
        //    get; set;
        //}

        [XmlAttribute("layer")]
        public int layerId
        {
            get; set;
        }

        [XmlAttribute("isLocked")]
        public bool IsLocked { get; set; }

    }


}
