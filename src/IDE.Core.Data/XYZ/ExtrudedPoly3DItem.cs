using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows;
using IDE.Core.Types.Media;

namespace IDE.Core.Storage
{
    public class ExtrudedPoly3DItem : MeshPrimitive
    {

        [XmlAttribute("centerX")]
        public double CenterX { get; set; }

        [XmlAttribute("centerY")]
        public double CenterY { get; set; }

        [XmlAttribute("centerZ")]
        public double CenterZ { get; set; }

        [XmlAttribute("height")]
        public double Height { get; set; }

        [XmlAttribute("rotationX")]
        public double RotationX { get; set; }

        [XmlAttribute("rotationY")]
        public double RotationY { get; set; }

        [XmlAttribute("rotationZ")]
        public double RotationZ { get; set; }

        [XmlArray("points")]
        [XmlArrayItem("p")]
        public List<XPoint> Points { get; set; }

    }
}
