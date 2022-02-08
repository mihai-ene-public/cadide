using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    class Torus3DItem : MeshPrimitive
    {
        [XmlAttribute("torusDiameter")]
        public double TorusDiameter { get; set; }

        [XmlAttribute("tubeDiameter")]
        public double TubeDiameter { get; set; }

        [XmlAttribute("thetaDivisions")]
        public int ThetaDivisions { get; set; }

        [XmlAttribute("phiDivisions")]
        public int PhiDivisions { get; set; }

    }
}
