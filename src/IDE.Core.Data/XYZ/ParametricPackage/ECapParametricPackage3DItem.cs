using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class ECapParametricPackage3DItem : ParametricPackageBase3DItem
    {


        [XmlAttribute]
        public double D1 { get; set; }

        [XmlAttribute]
        public double D { get; set; }

        [XmlAttribute]
        public double L { get; set; }

        [XmlAttribute]
        public double B { get; set; }

        [XmlAttribute]
        public double A { get; set; }
    }
}
