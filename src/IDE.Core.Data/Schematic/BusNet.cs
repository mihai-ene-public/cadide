using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class BusNet
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
