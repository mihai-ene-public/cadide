using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class CircleBoard : LayerPrimitive, ILayerPrimitive, ICircle
    {
        public CircleBoard()
        {
            BorderWidth = 0.2;
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

        [XmlAttribute("isFilled")]
        public bool IsFilled { get; set; }


        [XmlAttribute("layer")]
        public int layerId{ get; set;}

        [XmlAttribute("isLocked")]
        public bool IsLocked { get; set; }

    }
}
