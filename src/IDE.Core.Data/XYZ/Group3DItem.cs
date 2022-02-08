using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Group3DItem : MeshPrimitive
    {
        public Group3DItem()
        {
            Items = new List<MeshPrimitive>();
        }

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

        [XmlElement("box", typeof(Box3DItem))]
        [XmlElement("cone", typeof(Cone3DItem))]
        [XmlElement("sphere", typeof(Sphere3DItem))]
        [XmlElement("cylinder", typeof(Cylinder3DItem))]
        [XmlElement("ellipsoid", typeof(Ellipsoid3DItem))]
        [XmlElement("poly", typeof(ExtrudedPoly3DItem))]
        [XmlElement("group", typeof(Group3DItem))]
        [XmlElement("mesh", typeof(Mesh3DItem))]
        [XmlElement("text", typeof(Text3DItem))]
        public List<MeshPrimitive> Items
        {
            get; set;
        }

    }
}
