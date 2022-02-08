using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class SOICParametricPackage3DItem : ParametricPackageBase3DItem 
    {
        [XmlAttribute]
        public int NumberPads { get; set; }


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
        public double L { get; set; }

        [XmlAttribute]
        public double A { get; set; }

        [XmlAttribute]
        public double A1 { get; set; }
    }

    public class QFPParametricPackage3DItem : ParametricPackageBase3DItem
    {
        [XmlAttribute]
        public int NumberPads { get; set; }


        [XmlAttribute]
        public double E { get; set; }

        [XmlAttribute]
        public double D1 { get; set; }

        [XmlAttribute]
        public double D { get; set; }

        [XmlAttribute]
        public double B { get; set; }

        [XmlAttribute]
        public double L { get; set; }

        [XmlAttribute]
        public double A { get; set; }

        [XmlAttribute]
        public double A1 { get; set; }
    }

    public class PinHeaderStraightParametricPackage3DItem : ParametricPackageBase3DItem
    {
        [XmlAttribute]
        public int NumberRows { get; set; }

        [XmlAttribute]
        public int NumberColumns { get; set; }

        [XmlAttribute]
        public double E { get; set; }

        [XmlAttribute]
        public double D { get; set; }

        [XmlAttribute]
        public double PinPitchE { get; set; }

        [XmlAttribute]
        public double PinPitchD { get; set; }

        [XmlAttribute]
        public double B { get; set; }

        [XmlAttribute]
        public double L { get; set; }

        [XmlAttribute]
        public double L1 { get; set; }

        [XmlAttribute]
        public double L2 { get; set; }

        [XmlAttribute]
        public bool IsFemale { get; set; }
    }

}
