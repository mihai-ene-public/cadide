using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class AxialParametricPackage3DItem:ParametricPackageBase3DItem
    {
        [XmlAttribute]
        public ComponentAppearance Appearance { get; set; }

        [XmlAttribute]
        public ComponentPlacement Placement { get; set; }

        [XmlAttribute]
        public bool IsDiode { get; set; }

        [XmlAttribute]
        public double D { get; set; }

        [XmlAttribute]
        public double D1 { get; set; }

        [XmlAttribute]
        public double A { get; set; }

        [XmlAttribute]
        public double E { get; set; }

        [XmlAttribute]
        public double B{ get; set; }
    }
}
