using IDE.Core.BOM;
using System.Collections.Generic;

namespace IDE.Documents.Views
{
    public class BomItemDisplay
    {
        public string ImageURLSmall { get; set; }

        public string ImageURLMedium { get; set; }

        public string Supplier { get; set; }

        /// <summary>
        /// Supplier Part Number
        /// </summary>
        public string Sku { get; set; }

        public string Manufacturer { get; set; }

        public string MPN { get; set; }

        public string Description { get; set; }

        public string RoHS { get; set; }

        public string Package { get; set; }

        public string Packaging { get; set; }

        public int Stock { get; set; }

        public string Currency { get; set; }

        public List<NameValuePair> Properties { get; set; } = new List<NameValuePair>();

        public List<PriceDisplay> Prices { get; set; } = new List<PriceDisplay>();

        public List<NameValuePair> Documents { get; set; } = new List<NameValuePair>();
    }
}
