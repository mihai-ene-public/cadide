using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class SODParametricPackage3DItem : ParametricPackageBase3DItem
    {


        [XmlAttribute]
        public double E { get; set; }


        [XmlAttribute]
        public double D { get; set; }

        [XmlAttribute]
        public double D1 { get; set; }

        [XmlAttribute]
        public double L { get; set; }

        [XmlAttribute]
        public double B { get; set; }

        [XmlAttribute]
        public double A { get; set; }

        [XmlAttribute]
        public double A1 { get; set; } 
    }
}
