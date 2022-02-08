using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Windows;
using OxyPlot;
using IDE.Core.Types.Media;
using IDE.Core.Interfaces;
//using Color = IDE.Core.Types.Media.XColor;

namespace IDE.Core.PDF
{
    //a proxy class for writing pdf documents
    //this way we will be library independent
    public class PdfDocument : IPdfDocument
    {

        public PdfDocument()
        {
            pdfDoc = new PortableDocument();

            imageHelper = ServiceProvider.Resolve<IBitmapImageHelper>();
        }

        PortableDocument pdfDoc;

        IBitmapImageHelper imageHelper;

        public void AddPage(double width, double height)
        {
            pdfDoc.AddPage(width, height);
        }

        OxyRect GetRect(XRect rect)
        {
            return new OxyRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        OxyColor GetColor(XColor color)
        {
            return OxyColor.FromArgb(color.A, color.R, color.G, color.B);

        }

        public void Save(string filePath)
        {
            using (var stream = System.IO.File.Create(filePath))
            {
                pdfDoc.Save(stream);
            }
        }

        /// <summary>
        /// Draws a polyline.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="stroke">The stroke color.</param>
        /// <param name="thickness">The stroke thickness.</param>
        /// <param name="dashArray">The dash array.</param>
        /// <param name="lineJoin">The line join type.</param>
        /// <param name="aliased">if set to <c>true</c> the shape will be aliased.</param>
        public void DrawLine(
            IList<XPoint> points,
            XColor stroke,
            double thickness,
            double[] dashArray = null,
            LineJoin lineJoin = LineJoin.Round
           )
        {
            pdfDoc.SetColor(GetColor(stroke));
            pdfDoc.SetLineWidth(thickness);
            if (dashArray != null)
            {
                pdfDoc.SetLineDashPattern(dashArray, 0);
            }


            //var h = pdfDoc.PageHeight;
            //pdfDoc.MoveTo(points[0].X, h - points[0].Y);
            pdfDoc.MoveTo(points[0].X, points[0].Y);
            for (int i = 1; i < points.Count; i++)
            {
                //pdfDoc.LineTo(points[i].X, h - points[i].Y);
                pdfDoc.LineTo(points[i].X, points[i].Y);
            }

            pdfDoc.SetLineCap(OxyPlot.LineCap.Round);
            pdfDoc.SetLineJoin(lineJoin);

            pdfDoc.Stroke(false);
            if (dashArray != null)
            {
                pdfDoc.ResetLineDashPattern();
            }
        }

        public void DrawRectangle(XRect _rect, XColor _fill, XColor _stroke, double thickness)
        {
            var rect = GetRect(_rect);
            var fill = GetColor(_fill);
            var stroke = GetColor(_stroke);

            var isStroked = stroke.IsVisible() && thickness > 0;
            var isFilled = fill.IsVisible();
            if (!isStroked && !isFilled)
            {
                return;
            }

            // double y = pdfDoc.PageHeight - rect.Bottom;
            if (isStroked)
            {
                pdfDoc.SetLineWidth(thickness);
                pdfDoc.SetColor(stroke);
                if (isFilled)
                {
                    pdfDoc.SetFillColor(fill);
                }

                pdfDoc.DrawRectangle(rect.Left, rect.Top - rect.Height, rect.Width, rect.Height, isFilled);
            }
            else
            {
                pdfDoc.SetFillColor(fill);
                pdfDoc.FillRectangle(rect.Left, rect.Top - rect.Height, rect.Width, rect.Height);
            }
        }

        /// <summary>
        /// Draws a polygon. The polygon can have stroke and/or fill.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="fill">The fill color.</param>
        /// <param name="stroke">The stroke color.</param>
        /// <param name="thickness">The stroke thickness.</param>
        /// <param name="dashArray">The dash array.</param>
        /// <param name="lineJoin">The line join type.</param>
        /// <param name="aliased">If set to <c>true</c> the shape will be aliased.</param>
        public void DrawPolygon(
            IList<XPoint> points,
            XColor _fill,
            XColor _stroke,
            double thickness,
            double[] dashArray = null,
            LineJoin lineJoin = LineJoin.Round
           )
        {
            var stroke = GetColor(_stroke);
            var fill = GetColor(_fill);

            var isStroked = stroke.IsVisible() && thickness > 0;
            var isFilled = fill.IsVisible();
            if (!isStroked && !isFilled)
            {
                return;
            }

            // var h = this.doc.PageHeight;
            pdfDoc.MoveTo(points[0].X, points[0].Y);
            for (int i = 1; i < points.Count; i++)
            {
                pdfDoc.LineTo(points[i].X, points[i].Y);
            }

            if (isStroked)
            {
                pdfDoc.SetColor(stroke);
                pdfDoc.SetLineWidth(thickness);
                if (dashArray != null)
                {
                    pdfDoc.SetLineDashPattern(dashArray, 0);
                }

                pdfDoc.SetLineJoin(lineJoin);
                if (isFilled)
                {
                    pdfDoc.SetFillColor(fill);
                    pdfDoc.FillAndStroke();
                }
                else
                {
                    pdfDoc.Stroke();
                }

                if (dashArray != null)
                {
                    pdfDoc.ResetLineDashPattern();
                }
            }
            else
            {
                pdfDoc.SetFillColor(fill);
                pdfDoc.Fill();
            }
        }

        public void DrawText(
          XPoint position,
           string text,
           XColor _fill,
           string fontFamily,
           double fontSize,
           double fontWeight,
           double rotate
           )
        {
            text = text.Replace("\n", "\r\n");

            var fill = GetColor(_fill);

            var halign = OxyPlot.HorizontalAlignment.Left;
            var valign = OxyPlot.VerticalAlignment.Top;

            pdfDoc.SaveState();
            pdfDoc.SetFont(fontFamily, fontSize / 96 * 72, fontWeight > 500);
            pdfDoc.SetFillColor(fill);

            double width, height;
            pdfDoc.MeasureText(text, out width, out height);

            var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.None);
            var lineHeight = height;
            if (lines.Length > 0)
            {
                lineHeight = height / lines.Length;
            }


            double dx = 0;
            if (halign == OxyPlot.HorizontalAlignment.Center)
            {
                dx = -width / 2;
            }

            if (halign == OxyPlot.HorizontalAlignment.Right)
            {
                dx = -width;
            }

            double dy = 0;

            if (valign == OxyPlot.VerticalAlignment.Middle)
            {
                dy = -height / 2;
            }

            if (valign == OxyPlot.VerticalAlignment.Top)
            {
                dy = -lineHeight;
            }

            //double y = this.doc.PageHeight - p.Y;

            pdfDoc.Translate(position.X, position.Y);
            if (Math.Abs(rotate) > 1e-6)
            {
                pdfDoc.Rotate(-rotate);
            }

            pdfDoc.Translate(dx, dy);

            //debug
            //pdfDoc.DrawRectangle(0, 0, width, height);

            pdfDoc.SetClippingRectangle(0, 0, width, height);



            double y = 0;
            foreach (var line in lines)
            {
                pdfDoc.DrawText(0, y, line);
                y -= lineHeight;
            }
            pdfDoc.RestoreState();
        }

        public XSize MeasureText(string text, string fontName, double fontSize)
        {
            if (fontName != null)
            {
                fontName = fontName.ToLower();
            }

            var bold = false;
            var italic = false;

            var font = StandardFonts.Helvetica.GetFont(bold, italic);

            switch (fontName)
            {
                case "arial":
                case "helvetica":
                    font = StandardFonts.Helvetica.GetFont(bold, italic);
                    break;
                case "times":
                case "times new roman":
                    font = StandardFonts.Times.GetFont(bold, italic);
                    break;
                case "courier":
                case "courier new":
                    font = StandardFonts.Courier.GetFont(bold, italic);
                    break;
            }

            font.Measure(text, fontSize, out double width, out double height);
            return new XSize(width, height);

        }

        public void DrawEllipse(XRect rect, XColor _fill, XColor _stroke, double thickness)
        {
            var fill = GetColor(_fill);
            var stroke = GetColor(_stroke);

            var isStroked = stroke.IsVisible() && thickness > 0;
            var isFilled = fill.IsVisible();
            if (!isStroked && !isFilled)
            {
                return;
            }

            //double y = this.doc.PageHeight - rect.Bottom;
            if (isStroked)
            {
                pdfDoc.SetLineWidth(thickness);
                pdfDoc.SetColor(stroke);
                if (isFilled)
                {
                    pdfDoc.SetFillColor(fill);
                    // pdfDoc.DrawEllipse(rect.Left, rect.Bottom, rect.Width, rect.Height, true);
                }
                //else
                //{
                //}
                pdfDoc.DrawEllipse(rect.Left, rect.Top, rect.Width, rect.Height, isFilled);
            }
            else
            {
                pdfDoc.SetFillColor(fill);
                pdfDoc.FillEllipse(rect.Left, rect.Top, rect.Width, rect.Height);
            }
        }

        public void SetLineWidth(double width)
        {
            pdfDoc.SetLineWidth(width);
        }

        public void SetColor(XColor color)
        {
            pdfDoc.SetColor(GetColor(color));
        }

        public void SetFillColor(XColor color)
        {
            pdfDoc.SetFillColor(GetColor(color));
        }

        public void MoveTo(double x, double y)
        {
            pdfDoc.MoveTo(x, y);
        }

        public void BezierTo(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            pdfDoc.AppendCubicBezier(x1, y1, x2, y2, x3, y3);
        }

        public void Stroke()
        {
            pdfDoc.Stroke(false);
        }

        public void EndPath()
        {
            pdfDoc.EndPath();
        }

        public void DrawImage(byte[] imageBytes, XRect destRect, XStretch imageStretch)
        {
            // var img = new OxyImage(imageBytes);

            var portableImage = GetImage(imageBytes);//PortableDocumentImageUtilities.Convert(img, true);
            pdfDoc.SaveState();

            var sourceRect = new XRect(0, 0, portableImage.Width, portableImage.Height);
            destRect = GetDestRect(sourceRect, destRect, imageStretch);

            var x = destRect.X - (sourceRect.X / sourceRect.Width * destRect.Width);
            var y = destRect.Y - (sourceRect.Y / sourceRect.Height * destRect.Height);
            double width = portableImage.Width / sourceRect.Width * destRect.Width;
            double height = portableImage.Height / sourceRect.Height * destRect.Height;
            pdfDoc.SetClippingRectangle(destRect.X, destRect.Y - destRect.Height, destRect.Width, destRect.Height);
            pdfDoc.Translate(destRect.X, y - destRect.Height);
            pdfDoc.Scale(width, height);
            pdfDoc.DrawImage(portableImage);
            pdfDoc.RestoreState();
        }

        XSize GetScaleFactor(XSize inputSize, XSize naturalSize, XStretch stretch)
        {
            double scaleX = inputSize.Width / naturalSize.Width;
            double scaleY = inputSize.Height / naturalSize.Height;

            switch (stretch)
            {
                case XStretch.Uniform:
                    double minscale = scaleX < scaleY ? scaleX : scaleY;
                    scaleX = scaleY = minscale;
                    break;

                case XStretch.UniformToFill:
                    double maxscale = scaleX > scaleY ? scaleX : scaleY;
                    scaleX = scaleY = maxscale;
                    break;
            }

            return new XSize(scaleX, scaleY);
        }

        XRect GetDestRect(XRect imgRect, XRect destRect, XStretch stretch)
        {
            //desired size
            var desiredSize = new XSize(destRect.Width, destRect.Height);
            var naturalSize = new XSize(imgRect.Width, imgRect.Height);
            var scaleFactor = GetScaleFactor(desiredSize, naturalSize, stretch);
            var destSize = new XSize(naturalSize.Width * scaleFactor.Width, naturalSize.Height * scaleFactor.Height);
            if (destSize.Width > desiredSize.Width)
                destSize.Width = desiredSize.Width;
            if (destSize.Height > desiredSize.Height)
                destSize.Height = desiredSize.Height;

            destRect.Width = destSize.Width;
            destRect.Height = destSize.Height;
            destRect.X += 0.5 * (destSize.Width - desiredSize.Width);
            destRect.Y += 0.5 * (destSize.Height - desiredSize.Height);

            //var center = destRect.GetCenter();
            //var t = new ScaleTransform(scaleFactor.Width, scaleFactor.Height)
            //{
            //    CenterX = center.X,
            //    CenterY = center.Y
            //};
            //destRect = t.TransformBounds(destRect);

            return destRect;
        }

        PortableDocumentImage GetImage(byte[] imageBytes)
        {
            var image = imageHelper.GetImageData(imageBytes);

            return new PortableDocumentImage(image.PixelWidth, image.PixelHeight, 8, image.Bits, image.MaskBits, true);
        }
    }

    /// <summary>
    /// all dimension are in PDF points (72 dpi)
    /// </summary>
    public interface IPdfDocument
    {
        void AddPage(double width, double height);

        void MoveTo(double x, double y);

        void BezierTo(double x1, double y1, double x2, double y2, double x3, double y3);

        void DrawLine(
            IList<XPoint> points,
            XColor stroke,
            double thickness,
            double[] dashArray = null,
            LineJoin lineJoin = LineJoin.Round
           );

        void DrawRectangle(XRect rect, XColor fill, XColor stroke, double thickness);

        void DrawPolygon(
            IList<XPoint> points,
            XColor _fill,
            XColor _stroke,
            double thickness,
            double[] dashArray = null,
            LineJoin lineJoin = LineJoin.Round
           );

        void DrawText(
           XPoint position,
            string text,
            XColor fill,
            string fontFamily,
            double fontSize,
            double fontWeight,
            double rotate
            );

        XSize MeasureText(string text, string fontName, double fontSize);

        void DrawEllipse(XRect rect, XColor fill, XColor stroke, double thickness);
        void SetLineWidth(double width);

        void SetColor(XColor color);

        void SetFillColor(XColor color);

        void Stroke();

        void EndPath();

        void DrawImage(byte[] imageBytes, XRect destRect, XStretch imageStretch);
    }
}
