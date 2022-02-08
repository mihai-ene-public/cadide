using IDE.Core.Types.Media;

namespace IDE.Core.PDF
{
    internal class PdfEllipsePrimitive : PdfPrimitive, IFilledPdfPrimitive
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public double BorderWidth { get; set; }

        public XColor FillColor { get; set; }

        public override void WriteTo(IPdfDocument pdfDoc)
        {
            var x = X - 0.5 * Width;
            var y = Y - 0.5 * Height;
            var r = new XRect(x, y, Width, Height);
            pdfDoc.DrawEllipse(r, FillColor, Color, BorderWidth);
        }
    }
}