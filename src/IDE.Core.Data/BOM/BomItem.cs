using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.BOM
{
    public class BomItem
    {
        // [XmlAttribute("imageUrlSmall")]
        [XmlIgnore]
        public string ImageURLSmall { get; set; }

        // [XmlAttribute("imageUrlMedium")]
        [XmlIgnore]
        public string ImageURLMedium { get; set; }

        [XmlAttribute("supplier")]
        public string Supplier { get; set; }

        [XmlAttribute("sku")]
        //Supplier Part Number
        public string Sku { get; set; }

        [XmlAttribute("manufacturer")]
        public string Manufacturer { get; set; }

        [XmlAttribute("mpn")]
        public string MPN { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("rohs")]
        public string RoHS { get; set; }

        [XmlAttribute("package")]
        public string Package { get; set; }

        [XmlAttribute("packaging")]
        public string Packaging { get; set; }

        [XmlAttribute("stock")]
        public int Stock { get; set; }

        [XmlAttribute("currency")]
        public string Currency { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<NameValuePair> Properties { get; set; }

        [XmlArray("prices")]
        [XmlArrayItem("price")]
        public List<PriceDisplay> Prices { get; set; }

        [XmlArray("documents")]
        [XmlArrayItem("doc")]
        public List<NameValuePair> Documents { get; set; }
    }

    public class BomItemRef
    {
        [XmlAttribute("supplier")]
        public string Supplier { get; set; }

        [XmlAttribute("sku")]
        //Supplier Part Number
        public string Sku { get; set; }

        [XmlAttribute("manufacturer")]
        public string Manufacturer { get; set; }

        [XmlAttribute("mpn")]
        public string MPN { get; set; }
    }

    public class BomSpec
    {
        //in schematic will override with add what is defined on ComponentDocument
        //in board will add on what is defined in SchematicDocument
        [XmlElement("add")]
        public List<BomItem> Added { get; set; } = new List<BomItem>();

        //a reference to a bomitem that is the merge of the list from ComponentDocument, and SchematicDocument.Added and Board.Added
        [XmlElement("selected")]
        public BomItemRef Selected { get; set; }
    }

    public class PriceDisplay
    {
        [XmlAttribute]
        public int Number { get; set; }

        [XmlAttribute]
        public double Price { get; set; }
    }

    public class NameValuePair
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Value { get; set; }
    }
}
