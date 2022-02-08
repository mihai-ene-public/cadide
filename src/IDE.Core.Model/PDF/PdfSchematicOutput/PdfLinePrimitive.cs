using System.Linq;
using System.Collections.Generic;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.PDF
{
    internal class PdfLinePrimitive : PdfPrimitive
    {
        public XPoint StartPoint { get; set; }
        public XPoint EndPoint { get; set; }
        public double Width { get; set; }

        public LineStyle LineStyle { get; set; } = LineStyle.Solid;

        public override void WriteTo(IPdfDocument pdfDoc)
        {
            double[] dashArray = null;
            switch (LineStyle)
            {
                case LineStyle.Dash:
                    dashArray = new[] { 2.0, 3.0 };
                    break;

                case LineStyle.Dot:
                    dashArray = new[] { 0.0, 2 };
                    break;

                case LineStyle.DashDot:
                    dashArray = new[] { 2.0, 2.0, 0.0, 2.0 };
                    break;
            }

            pdfDoc.DrawLine(new List<XPoint> { StartPoint, EndPoint }, Color, Width, dashArray);
        }
    }

    public class PdfPolylinePrimitive:PdfPrimitive
    {
        public double Width { get; set; }

        public List<XPoint> Points { get; set; }

        public LineStyle LineStyle { get; set; } = LineStyle.Solid;

        public override void WriteTo(IPdfDocument pdfDoc)
        {
            double[] dashArray = null;
            switch (LineStyle)
            {
                case LineStyle.Dash:
                    dashArray = new[] { 2.0, 3.0 };
                    break;

                case LineStyle.Dot:
                    dashArray = new[] { 0.0, 2 };
                    break;

                case LineStyle.DashDot:
                    dashArray = new[] { 2.0, 2.0, 0.0, 2.0 };
                    break;
            }

            pdfDoc.DrawLine(Points, Color, Width, dashArray);
        }
    }

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

    public class PdfPolygonPrimitive : PdfPrimitive, IFilledPdfPrimitive
    {
        public XColor FillColor { get; set; }

        public double BorderWidth { get; set; }

        public List<XPoint> Points { get; set; }

        public override void WriteTo(IPdfDocument pdfDoc)
        {
            pdfDoc.DrawPolygon(Points, FillColor, Color, BorderWidth);
        }
    }

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