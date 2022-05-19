using IDE.Core.Types.Media;

namespace IDE.Core.PDF
{
    //public class PdfRoundedRectanglePrimitive : PdfPrimitive, IFilledPdfPrimitive
    //{
    //    public Color FillColor { get; set; }

    //    public double BorderWidth { get; set; }

    //    public double X { get; set; }
    //    public double Y { get; set; }
    //    public double Width { get; set; }
    //    public double Height { get; set; }
    //    public double CornerRadius { get; set; }

    //    public double Rot { get; set; }

    //    public override void WriteTo(IPdfDocument pdfDoc)
    //    {
    //        //pdfDoc.DrawPolygon(Points, FillColor, Color, BorderWidth);
    //    }
    //}

    public class PdfTextPrimitive : PdfPrimitive
    {
        public XPoint Position { get; set; }

        public double Rot { get; set; }

        public string Text { get; set; }

        public string FontFamily { get; set; }

        public double FontSize { get; set; }

        public double FontWeight { get; set; }

        public override void WriteTo(IPdfDocument pdfDoc)
        {
            pdfDoc.DrawText(Position, Text, Color, FontFamily, FontSize, FontWeight, Rot);
        }
    }
}