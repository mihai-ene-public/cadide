using System.Collections.Generic;
using IDE.Core.Types.Media;

namespace IDE.Core.PDF
{
    public class PdfRectanglePrimitive : PdfPrimitive, IFilledPdfPrimitive
    {
        public XColor FillColor { get; set; }

        public double BorderWidth { get; set; }

        public List<XPoint> Points { get; set; }

        public override void WriteTo(IPdfDocument pdfDoc)
        {
            pdfDoc.DrawPolygon(Points, FillColor, Color, BorderWidth);
        }
    }
}