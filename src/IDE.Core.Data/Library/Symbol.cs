using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    [XmlRoot("symbol")]
    public class Symbol : LibraryItem
    {
        public Symbol()
        {
            Items = new List<SchematicPrimitive>();
        }

        [XmlElement("description")]
        public Description Description { get; set; }

        [XmlElement("circle", typeof(Circle))]
        [XmlElement("ellipse", typeof(Ellipse))]
        [XmlElement("image", typeof(ImagePrimitive))]
        [XmlElement("pin", typeof(Pin))]
        [XmlElement("poly", typeof(Polygon))]
        [XmlElement("rect", typeof(Rectangle))]
        [XmlElement("text", typeof(Text))]
        [XmlElement("line", typeof(LineSchematic))]
        [XmlElement("arc", typeof(Arc))]
        public List<SchematicPrimitive> Items
        {
            get; set;
        }

    }

    // [XmlRoot("symbol")]
    public class FontSymbol : LibraryItem
    {

        public FontSymbol()
        {
            Items = new List<LayerPrimitive>();
        }

        [XmlElement("line", typeof(LineBoard))]
        [XmlElement("arc", typeof(ArcBoard))]
        public List<LayerPrimitive> Items
        {
            get; set;
        }

    }

    [XmlRoot("font")]
    public class FontDocument : LibraryItem
    {
        [XmlElement("symbol", typeof(FontSymbol))]
        public List<FontSymbol> Symbols
        {
            get; set;
        }
    }
}
