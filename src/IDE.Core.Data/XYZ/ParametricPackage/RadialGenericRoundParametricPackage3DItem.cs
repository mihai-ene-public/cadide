using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class RadialGenericRoundParametricPackage3DItem : ParametricPackageBase3DItem
    {


        [XmlAttribute]
        public double EE { get; set; }


        [XmlAttribute]
        public double D { get; set; }

        [XmlAttribute]
        public double L { get; set; }

        [XmlAttribute]
        public double A { get; set; }

        [XmlAttribute]
        public double B { get; set; }
    }
}
