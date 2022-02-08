using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Ellipsoid3DItem : MeshPrimitive
    {
        [XmlAttribute("centerX")]
        public double CenterX { get; set; }

        [XmlAttribute("centerY")]
        public double CenterY { get; set; }

        [XmlAttribute("centerZ")]
        public double CenterZ { get; set; }

        [XmlAttribute("radiusX")]
        public double RadiusX { get; set; }

        [XmlAttribute("radiusY")]
        public double RadiusY { get; set; }

        [XmlAttribute("radiusZ")]
        public double RadiusZ { get; set; }

        [XmlAttribute("thetaDivisions")]
        public int ThetaDivisions { get; set; }

        [XmlAttribute("phiDivisions")]
        public int PhiDivisions { get; set; }

    }
}
