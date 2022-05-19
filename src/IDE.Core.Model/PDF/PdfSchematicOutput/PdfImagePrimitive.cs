using IDE.Core.Types.Media;

namespace IDE.Core.PDF
{
    public class PdfImagePrimitive : PdfPrimitive, IFilledPdfPrimitive
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Rot { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }

        public double BorderWidth { get; set; }

        public XColor BorderColor { get; set; }

        public XColor FillColor { get; set; }

        public byte[] ImageBytes { get; set; }

        public XStretch Stretch { get; set; }

        public override void WriteTo(IPdfDocument pdfDoc)
        {
            var rect = new XRect(X, Y, Width, Height);
            pdfDoc.DrawRectangle(rect, FillColor, BorderColor, BorderWidth);

            pdfDoc.DrawImage(ImageBytes, rect, Stretch);
        }
    }
}