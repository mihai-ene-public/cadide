using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class SOT23ParametricPackage3DItem : ParametricPackageBase3DItem
    {
        [XmlAttribute]
        public int NumberPads { get; set; }


        [XmlAttribute]
        public double E { get; set; }

        [XmlAttribute]
        public double E1 { get; set; } 

        [XmlAttribute]
        public double D { get; set; }

        [XmlAttribute]
        public double EE { get; set; }

        [XmlAttribute]
        public double B { get; set; }

        [XmlAttribute]
        public double L { get; set; }

        [XmlAttribute]
        public double A { get; set; }

        [XmlAttribute]
        public double A1 { get; set; }
    }

}
