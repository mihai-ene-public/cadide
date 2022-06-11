using IDE.Core.Types.Media;

namespace IDE.Core.PDF
{
    internal class PdfArcPrimitive : PdfPrimitive
    {
        public XPoint StartPoint { get; set; }
        public XPoint EndPoint { get; set; }
        public double Width { get; set; }
        public double SizeDiameter { get; set; }

        public XPoint Center { get; set; }
        public XSweepDirection SweepDirection { get; set; }

        public bool IsLargeArc { get; set; }

        public double RotationAngle { get; set; }

        public override void WriteTo(IPdfDocument pdfDoc)
        {
            var points = BezierGeometryHelper.BezierCurveFromArc(StartPoint, EndPoint, new XSize(0.5 * SizeDiameter, 0.5 * SizeDiameter), RotationAngle, IsLargeArc,
                                                                 SweepDirection == XSweepDirection.Clockwise);
            int count = points.Count;

            pdfDoc.SetColor(Color);
            pdfDoc.SetLineWidth(Width);
            pdfDoc.MoveTo(points[0].X, points[0].Y);

            for (int idx = 1; idx < count; idx += 3)
                pdfDoc.BezierTo(points[idx].X, points[idx].Y, points[idx + 1].X, points[idx + 1].Y, points[idx + 2].X, points[idx + 2].Y);

            pdfDoc.Stroke();
        }
    }
}