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
    public class ArcBoard : LayerPrimitive, ILayerPrimitive, IArc
    {
        public ArcBoard()
        {

        }

        [XmlElement("startPoint")]
        public XPoint StartPoint { get; set; }

        [XmlElement("endPoint")]
        public XPoint EndPoint { get; set; }


        /// <summary>
        /// this seems to be the radius of the arc (review this property)
        /// </summary>
        [XmlAttribute("sizeDiameter")]
        public double SizeDiameter { get; set; }

        [XmlAttribute("borderWidth")]
        public double BorderWidth { get; set; }


        [XmlAttribute("isFilled")]
        public bool IsFilled { get; set; }

        [XmlAttribute("sweepDirection")]
        public XSweepDirection SweepDirection { get; set; }

        [XmlAttribute("isLargeArc")]
        public bool IsLargeArc { get; set; }

        [XmlAttribute("rotationAngle")]
        public double RotationAngle { get; set; }

        [XmlAttribute("layer")]
        public int layerId
        {
            get; set;
        }

        [XmlAttribute("isLocked")]
        public bool IsLocked { get; set; }
    }
}
