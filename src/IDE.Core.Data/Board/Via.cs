using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    //public enum viaShape
    //{
    //    round,
    //    square,
    //    octagon,
    //}

    public class Via : LayerPrimitive
    {
        [XmlAttribute("x")]
        public double x
        {
            get; set;
        }


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

        [XmlAttribute("diameter")]
        public double diameter { get; set; }

        [XmlAttribute("startLayer")]
        public int startLayer { get; set; }

        [XmlAttribute("endLayer")]
        public int endLayer { get; set; }

        [XmlAttribute("tentViaOnTop")]
        public bool TentViaOnTop { get; set; } = false;

        [XmlAttribute("tentViaOnBottom")]
        public bool TentViaOnBottom { get; set; } = false;

        [XmlAttribute("isLocked")]
        public bool IsLocked { get; set; }
    }
}
