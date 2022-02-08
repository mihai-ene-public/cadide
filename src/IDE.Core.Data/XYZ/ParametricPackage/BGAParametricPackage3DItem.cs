using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class BGAParametricPackage3DItem : ParametricPackageBase3DItem
    {
        [XmlAttribute]
        public int NumberRows { get; set; }

        [XmlAttribute]
        public int NumberColumns { get; set; }

       [XmlAttribute]
        public double E { get; set; }

        [XmlAttribute]
        public double D1 { get; set; }

        [XmlAttribute]
        public double D { get; set; }

        [XmlAttribute]
        public double EE { get; set; }

        [XmlAttribute]
        public double B { get; set; }

        [XmlAttribute]
        public double A { get; set; }
    }
}
