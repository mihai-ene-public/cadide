using System.Collections.Generic;

namespace IDE.Core.PDF
{
    internal class PdfRegionPrimitive : PdfPrimitive
    {
        public List<PdfPrimitive> RegionItems { get; set; } = new List<PdfPrimitive>();
    }
}