using IDE.Core.Interfaces;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class PlaneBoard: LayerPrimitive, ILayerPrimitive
    {
        [XmlAttribute("isFilled")]
        public bool IsFilled { get; set; } = true;

        [XmlAttribute("generateThermals")]
        public bool GenerateThermals { get; set; } = true;

        [XmlAttribute("thermalWidth")]
        public double ThermalWidth { get; set; } = 0.2;

        [XmlAttribute("layer")]
        public int layerId
        {
            get; set;
        }

        [XmlAttribute("isLocked")]
        public bool IsLocked { get; set; }
    }


}
