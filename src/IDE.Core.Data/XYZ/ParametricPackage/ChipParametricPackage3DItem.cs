using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class ChipParametricPackage3DItem : ParametricPackageBase3DItem
    {
        [XmlAttribute]
        public double E { get; set; }


        [XmlAttribute]
        public double D { get; set; }

        [XmlAttribute]
        public double L { get; set; } 

        [XmlAttribute]
        public double A { get; set; }

        [XmlAttribute]
        public bool IsCapacitor { get; set; }
    }
}
