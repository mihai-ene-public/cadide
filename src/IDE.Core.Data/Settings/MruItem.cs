namespace IDE.Core.Settings
{
    using System.Xml.Serialization;

    public class MruItem
    {
        [XmlAttribute("path")]
        public string FilePath { get; set; }

        [XmlAttribute("isPinned")]
        public bool IsPinned { get; set; }
    }
}
