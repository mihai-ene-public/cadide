using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Cylinder3DItem : MeshPrimitive
    {

        [XmlAttribute("originX")]
        public double OriginX { get; set; }

        [XmlAttribute("originY")]
        public double OriginY { get; set; }

        [XmlAttribute("originZ")]
        public double OriginZ { get; set; }

        [XmlAttribute("directionX")]
        public double DirectionX { get; set; }

        [XmlAttribute("directionY")]
        public double DirectionY { get; set; }

        [XmlAttribute("directionZ")]
        public double DirectionZ { get; set; }

        [XmlAttribute("showBaseCap")]
        public bool ShowBaseCap { get; set; }

        [XmlAttribute("radius")]
        public double Radius { get; set; }

        [XmlAttribute("showTopCap")]
        public bool ShowTopCap { get; set; }

        [XmlAttribute("height")]
        public double Height { get; set; }

        [XmlAttribute("thetaDivisions")]
        public int ThetaDivisions { get; set; }

        [XmlAttribute("rotationX")]
        public double RotationX { get; set; }

        [XmlAttribute("rotationY")]
        public double RotationY { get; set; }

        [XmlAttribute("rotationZ")]
        public double RotationZ { get; set; }

    }
}
