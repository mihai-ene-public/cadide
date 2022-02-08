using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Sphere3DItem : MeshPrimitive
    {
        [XmlAttribute("centerX")]
        public double CenterX { get; set; }

        [XmlAttribute("centerY")]
        public double CenterY { get; set; }

        [XmlAttribute("centerZ")]
        public double CenterZ { get; set; }

        [XmlAttribute("radius")]
        public double Radius { get; set; }

        [XmlAttribute("thetaDivisions")]
        public int ThetaDivisions { get; set; }

        [XmlAttribute("phiDivisions")]
        public int PhiDivisions { get; set; }

    }
}
