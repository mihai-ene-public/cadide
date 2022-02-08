using IDE.Core.Types.Media;
//using System.Windows;
//using System.Windows.Media;

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
            var points = BezierGeometryHelper.BezierCurveFromArc(StartPoint, EndPoint, new XSize(0.5*SizeDiameter, 0.5*SizeDiameter), RotationAngle, IsLargeArc,
                                                                        SweepDirection == XSweepDirection.Clockwise);
            int count = points.Count;
            // Debug.Assert((count + 2) % 3 == 0);
            pdfDoc.SetColor(Color);
            pdfDoc.SetLineWidth(Width);
            pdfDoc.MoveTo(points[0].X, points[0].Y);
            for (int idx = 1; idx < count; idx += 3)
                pdfDoc.BezierTo(points[idx].X, points[idx].Y, points[idx + 1].X, points[idx + 1].Y, points[idx + 2].X, points[idx + 2].Y);

            pdfDoc.Stroke();
           // pdfDoc.EndPath();
        }



        //double stepAngle = pi / 180;
        //        void pdfDrawArc(int x, int y, int width, int height, int startAngle, int arcAngle)
        //        {
        //            int n = (int)(Math.Ceiling(Math.Abs(arcAngle / stepAngle)));
        //            int i;
        //            double currentStartAngle = startAngle;
        //            double actualArcAngle = ((double)arcAngle) / n;
        //            for (i = 0; i < n; i++)
        //            {
        //                double[] bezier = bezierCurve(this.x + x, this.y + y, SizeDiameter, SizeDiameter, currentStartAngle, actualArcAngle);
        //                if (i == 0)
        //                    this.pdfMoveTo(bezier[0], bezier[1]);
        //                this.pdfCurveTo(bezier[2], bezier[3], bezier[4], bezier[5], bezier[6], bezier[7]);
        //                this.pdfStroke();
        //                currentStartAngle += actualArcAngle;
        //            }
        //        }

        //        double[] bezierCurve(double ellipseWidth, double ellipseHeight, double startAngleRads)
        //        {
        //            double pi = 3.141592;


        //            double a = ellipseWidth * 0.5;
        //            double b = ellipseHeight * 0.5;

        //            //center
        //            double cx = Center.X;
        //            double cy = Center.Y;

        //            //calculate trigonometric operations so we don't need to repeat the calculus
        //            double cos1 = Math.Cos(startAngleRads);
        //            double sin1 = Math.Sin(startAngleRads);
        //            double cos2 = Math.Cos((startAngleRads + stepAngle));
        //            double sin2 = Math.Sin((startAngleRads + stepAngle));

        //            //point p1. Start point
        //            double p1x = StartPoint.X; //cx + a * cos1;
        //            double p1y = StartPoint.Y;//cy - b * sin1;

        //            //point d1. First derivative at start point.
        //            double d1x = -a * sin1;
        //            double d1y = -b * cos1;

        //            //point p2. End point
        //            double p2x = EndPoint.X;//cx + a * cos2;
        //            double p2y = EndPoint.Y;//cy - b * sin2;

        //            //point d2. First derivative at end point
        //            double d2x = -a * sin2;
        //            double d2y = -b * cos2;

        //            //alpha constant
        //            double aux = Math.Tan((stepAngle / 2) );
        //            double alpha = Math.Sin(stepAngle ) * (Math.Sqrt(4 + 3 * aux * aux) - 1.0) / 3.0;

        //            //point q1. First control point
        //            double q1x = p1x + alpha * d1x;
        //            double q1y = p1y + alpha * d1y;

        //            //point q2. Second control point.
        //            double q2x = p2x - alpha * d2x;
        //            double q2y = p2y - alpha * d2y;
        //            return new double[] { p1x, p1y, q1x, q1y, q2x, q2y, p2x, p2y };
        //        }
    }
}