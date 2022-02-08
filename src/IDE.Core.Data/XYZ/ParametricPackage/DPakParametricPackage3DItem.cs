using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class DPakParametricPackage3DItem : ParametricPackageBase3DItem
    {
        [XmlAttribute]
        public double BodyWidth { get; set; }

        [XmlAttribute]
        public double BodyHeight { get; set; }

        [XmlAttribute]
        public double BodyExtrusion { get; set; }
        
        [XmlAttribute]
        public double PadWidth { get; set; }

        [XmlAttribute]
        public double PadHeight { get; set; }

        [XmlAttribute]
        public double PadExtrusion { get; set; }

        [XmlAttribute]
        public double PadOuterOffset { get; set; }

        [XmlAttribute]
        public double PinWidth { get; set; }

        [XmlAttribute]
        public double PinThickness { get; set; }

        [XmlAttribute]
        public double PinPitch { get; set; }

        [XmlAttribute]
        public double PinBodyExitLength { get; set; }

        [XmlAttribute]
        public double PinLengthTotal { get; set; }
    }
}
