using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace IDE.Core.Storage
{



    /// <summary>
    /// this is a solid body mesh. It is usually imported from somewhere
    /// </summary>
    //[XmlInclude(typeof(MatrixTransform3D))]
    //[XmlInclude(typeof(GeometryModel3D))]
    //[XmlInclude(typeof(MeshGeometry3D))]
    public class Mesh3DItem : MeshPrimitive
    {
        public Mesh3DItem()
        {

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


        [XmlElement("model")]
        public XmlCDataSection ModelCData
        {
            get;set;
        }

    }
}
