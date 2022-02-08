using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class ParametricPackageBase3DItem : MeshPrimitive
    {
        [XmlAttribute("centerX")]
        public double CenterX { get; set; }

        [XmlAttribute("centerY")]
        public double CenterY { get; set; }

        [XmlAttribute("centerZ")]
        public double CenterZ { get; set; }


        [XmlAttribute("rotationX")]
        public double RotationX { get; set; }

        [XmlAttribute("rotationY")]
        public double RotationY { get; set; }

        [XmlAttribute("rotationZ")]
        public double RotationZ { get; set; }
    }
}
