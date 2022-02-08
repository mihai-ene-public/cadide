using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    //this is more for the board; could be used for modules though
    public class Hole : LayerPrimitive, IHole
    {
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


        [XmlAttribute("drill")]
        public double drill
        {
            get; set;
        }

        [XmlAttribute("isPlated")]
        public bool IsPlated { get; set; }

        //slot definition
        [XmlAttribute("height")]
        public double Height { get; set; }

        [XmlAttribute("rot")]
        public double Rot { get; set; }

        [XmlAttribute("drillType")]
        public DrillType DrillType { get; set; }

        [XmlAttribute("isLocked")]
        public bool IsLocked { get; set; }

    }


}
