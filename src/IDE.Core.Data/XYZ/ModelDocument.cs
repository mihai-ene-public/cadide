using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    [XmlRoot("model")]
    public class ModelDocument : LibraryItem
    {
        public ModelDocument()
        {
            Items = new List<MeshPrimitive>();
        }

        [XmlElement("box", typeof(Box3DItem))]
        [XmlElement("cone", typeof(Cone3DItem))]
        [XmlElement("sphere", typeof(Sphere3DItem))]
        [XmlElement("cylinder", typeof(Cylinder3DItem))]
        [XmlElement("ellipsoid", typeof(Ellipsoid3DItem))]
        [XmlElement("poly", typeof(ExtrudedPoly3DItem))]
        [XmlElement("group", typeof(Group3DItem))]
        [XmlElement("mesh", typeof(Mesh3DItem))]
        [XmlElement("text", typeof(Text3DItem))]

        [XmlElement("axialParametricPackage", typeof(AxialParametricPackage3DItem))]
        [XmlElement("bgaParametricPackage", typeof(BGAParametricPackage3DItem))]
        [XmlElement("chipParametricPackage", typeof(ChipParametricPackage3DItem))]
        [XmlElement("crystalSMDParametricPackage", typeof(CrystalSMDParametricPackage3DItem))]
        [XmlElement("dipParametricPackage", typeof(DIPParametricPackage3DItem))]
        [XmlElement("dfnParametricPackage", typeof(DFNParametricPackage3DItem))]
        [XmlElement("dpakParametricPackage", typeof(DPakParametricPackage3DItem))]
        [XmlElement("ecapParametricPackage", typeof(ECapParametricPackage3DItem))]
        [XmlElement("melfParametricPackage", typeof(MelfParametricPackage3DItem))]
        [XmlElement("sodParametricPackage", typeof(SODParametricPackage3DItem))]
        [XmlElement("smaParametricPackage", typeof(SMAParametricPackage3DItem))]
        [XmlElement("soicParametricPackage", typeof(SOICParametricPackage3DItem))]
        [XmlElement("sot23ParametricPackage", typeof(SOT23ParametricPackage3DItem))]
        [XmlElement("sot223ParametricPackage", typeof(SOT223ParametricPackage3DItem))]
        [XmlElement("qfnParametricPackage", typeof(QFNParametricPackage3DItem))]
        [XmlElement("qfpParametricPackage", typeof(QFPParametricPackage3DItem))]
        [XmlElement("pinHeaderStraightParametricPackage", typeof(PinHeaderStraightParametricPackage3DItem))]
        [XmlElement("radialGenericRoundParametricPackage", typeof(RadialGenericRoundParametricPackage3DItem))]
        [XmlElement("radialLEDParametricPackage", typeof(RadialLEDParametricPackage3DItem))]
        public List<MeshPrimitive> Items
        {
            get; set;
        }

    }
}
