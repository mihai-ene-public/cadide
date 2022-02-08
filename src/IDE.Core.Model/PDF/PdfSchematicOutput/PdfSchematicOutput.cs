using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Media;

namespace IDE.Core.PDF
{
    public class PdfSchematicOutput
    {
        public double DocumentWidth { get; set; } = 297.0d;
        public double DocumentHeight { get; set; } = 210.0d;

        public List<string> OutputFiles { get; private set; } = new List<string>();

        public async Task Build(IFileBaseViewModel schematic, IList<SheetDesignerItem> sheets)
        {
            //return Task.Run(() =>
            //{
            var f = schematic as IFileBaseViewModel;
            if (f == null)
                return;
            var project = f.ProjectNode;
            if (project == null)
                return;
            var savePath = Path.Combine(project.GetItemFolderFullPath(), "!Output");//folder
            Directory.CreateDirectory(savePath);
            var schName = Path.GetFileNameWithoutExtension(f.FilePath);
            savePath = Path.Combine(savePath, $"{schName}{GetExtension("pdf")}");

            var pdf = new PdfDocument();
            foreach (var sheet in sheets)
            {
                var page = new PdfSchematicPage(pdf, sheet, DocumentWidth, DocumentHeight);
                await page.BuildPage();
            }
            pdf.Save(savePath);

            OutputFiles.Clear();
            OutputFiles.Add(savePath);
            //});
        }

        string GetExtension(string extension)
        {
            if (extension.StartsWith("."))
                return extension;
            return "." + extension.ToUpper();
        }
    }

    public class PdfSchematicPage
    {
        public PdfSchematicPage(PdfDocument pdf, SheetDesignerItem sheet, double widthMillimeters, double heightMillimeters)
        {
            pdfDoc = pdf;
            innerSheet = sheet;

            sheetWidth = widthMillimeters;
            sheetHeight = heightMillimeters;

            LoadOptions();
        }

        PdfDocument pdfDoc;

        SheetDesignerItem innerSheet;

        // List<PdfPrimitive> addPrimitives = new List<PdfPrimitive>();

        List<PdfPrimitive> buildPlanPrimitives = new List<PdfPrimitive>();

        double sheetWidth;
        double sheetHeight;

        XRect sheetRectangle;
        double sheetOriginX;
        double sheetOriginY;

        XColor backgroundColor = XColor.FromRgb(255,255,255);

        void LoadOptions()
        {
            //todo: we could have colors for primitives and background
            //page size (width and height)
        }

        void PreparePage()
        {
            sheetRectangle = new XRect(0, 0, sheetWidth, sheetHeight);
            sheetOriginX = sheetRectangle.BottomLeft.X;
            sheetOriginY = sheetRectangle.BottomLeft.Y;

            pdfDoc.AddPage(ToPdfSize(sheetWidth), ToPdfSize(sheetHeight));
        }

        void BuildExecutionPlan()
        {
            buildPlanPrimitives = new List<PdfPrimitive>();

            foreach (var item in innerSheet.Items.OrderBy(p => p.ZIndex))
            {
                var pdfPrimitive = GetPrimitive(item);
                if (pdfPrimitive != null)
                {
                    buildPlanPrimitives.Add(pdfPrimitive);
                }
            }
        }

        PdfPrimitive GetPrimitive(ISelectableItem item, bool transformed = true)
        {
            if (item == null)
                return null;

            switch (item)
            {
                case INetWireCanvasItem netWire:
                    return GetNetWirePrimitive(netWire);

                case IBusWireCanvasItem busWire:
                    return GetBusWirePrimitive(busWire);

                case ILineSchematicCanvasItem line:
                    return GetLinePrimitive(line, transformed);

                case IRectangleSchematicCanvasItem rect:
                    return GetRectanglePrimitive(rect);

                case ICircleSchematicCanvasItem circle:
                    return GetCirclePrimitive(circle);

                case IEllipseSchematicCanvasItem ellipse:
                    return GetEllipsePrimitive(ellipse);

                case IPolySchematicCanvasItem poly:
                    return GetPolygonPrimitive(poly);

                case IArcSchematicCanvasItem arc:
                    return GetArcPrimitive(arc, transformed);

                case ITextSchematicCanvasItem text:
                        return GetTextPrimitive(text);

                case IImageCanvasItem img:
                        return GetImagePrimitive(img);

                case IPinCanvasItem pin:
                        return GetPinPrimitive(pin);

                case IJunctionCanvasItem junc:
                        return GetJunctionPrimitive(junc);

                case ILabelCanvasItem netLabel:
                        return GetNetLabelPrimitive(netLabel);

                case ISymbolCanvasItem symbol:
                        return GetSymbolPrimitive(symbol);
            }

            return null;
        }

        private PdfPrimitive GetSymbolPrimitive(ISymbolCanvasItem symbol)
        {
            var t = new XTranslateTransform(symbol.X, symbol.Y);

            var figure = new PdfFigure();

            //comment
            if (symbol.ShowComment)
            {
                var commentPos = new XPoint(symbol.CommentPosition.X, symbol.CommentPosition.Y);
                commentPos = GetPointTransform(t, commentPos);

                var pComment = new PdfTextPrimitive
                {
                    Text = symbol.Comment,
                    Position = ToPdfPoint(commentPos),
                    Rot = ToPdfRot(symbol.CommentPosition.Rotation),
                    FontFamily = "Arial",
                    FontSize = 10,
                    FontWeight = 0
                };

                figure.FigureItems.Add(pComment);
            }

            //part name
            if (symbol.ShowName)
            {
                var namePos = new XPoint(symbol.SymbolNamePosition.X, symbol.SymbolNamePosition.Y);
                namePos = GetPointTransform(t, namePos);

                var pName = new PdfTextPrimitive
                {
                    Text = symbol.SymbolName,
                    Position = ToPdfPoint(namePos),
                    Rot = ToPdfRot(symbol.SymbolNamePosition.Rotation),
                    FontFamily = "Arial",
                    FontSize = 10,
                    FontWeight = 0
                };

                figure.FigureItems.Add(pName);
            }

            foreach (var item in symbol.Items)
            {
                var p = GetPrimitive(item);
                if (p != null)
                    figure.FigureItems.Add(p);
            }


            return figure;
        }

        private PdfPrimitive GetNetLabelPrimitive(ILabelCanvasItem netLabel)
        {
            return new PdfTextPrimitive
            {
                Position = ToPdfPoint(new XPoint(netLabel.X, netLabel.Y)),
                Rot = ToPdfRot(netLabel.Rot),
                Text = netLabel.Text,
                FontFamily = netLabel.FontFamily.ToString(),
                FontSize = netLabel.FontSize,
                FontWeight = netLabel.Bold ? 600 : 0,
                Color = ToPdfColor(netLabel.TextColor)
            };
        }

        private PdfPrimitive GetJunctionPrimitive(IJunctionCanvasItem junction)

        {
            var t = junction.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);

            return new PdfEllipsePrimitive
            {
                X = ToPdfX(position.X),
                Y = ToPdfY(position.Y),
                Width = ToPdfSize(junction.Diameter),
                Height = ToPdfSize(junction.Diameter),
                BorderWidth = 0,
                FillColor = ToPdfColor(junction.Color)
            };
        }

        private PdfPrimitive GetPinPrimitive(IPinCanvasItem pin)
        {
            var figure = new PdfFigure();

            var sp = new XPoint(); //new Point(pin.X, pin.Y);
            var ep = new XPoint(pin.PinLength, 0); //new Point(sp.X + pin.PinLength, pin.Y);

            var localStartPoint = sp;
            var localEndpoint = ep;

            var t = pin.GetTransform();
            sp = GetPointTransform(t, sp);
            ep = GetPointTransform(t, ep);

            //pin name
            if (pin.ShowName)
            {
                var nameRot = pin.GetAngleOrientation(true);//local rotation

                var namePos = localEndpoint + new XVector(pin.PinNamePosition.X, pin.PinNamePosition.Y);

                if (pin.Orientation == pinOrientation.Up || pin.Orientation == pinOrientation.Right)
                {
                    var sizeText = pdfDoc.MeasureText(pin.Name, "Arial", 8.0 / 96.0 * 72.0);
                    var textWidthMM = sizeText.Width * 25.4d / 72.0d;

                    namePos = localEndpoint + new XVector(pin.PinNamePosition.X, pin.PinNamePosition.Y) + new XVector(textWidthMM + 0.5, 0);
                    namePos.Y = -pin.PinNamePosition.Y;
                }

                namePos = GetPointTransform(t, namePos);

                var rot = pin.GetAngleOrientation(false) + nameRot;

                figure.FigureItems.Add(new PdfTextPrimitive
                {
                    Text = pin.Name,
                    Position = ToPdfPoint(namePos),
                    Rot = ToPdfRot(rot),
                    FontFamily = "Arial",
                    FontSize = 8,
                    FontWeight = 0
                });
            }

            //pin number
            if (pin.ShowNumber)
            {
                var numberRot = pin.GetAngleOrientation(true);//local rotation

                var fontSize = 6.0d;

                var numberPos = localStartPoint + new XVector(pin.PinNumberPosition.X, pin.PinNumberPosition.Y);

                if (pin.Orientation == pinOrientation.Up || pin.Orientation == pinOrientation.Right)
                {
                    var sizeText = pdfDoc.MeasureText(pin.Number, "Arial", fontSize / 96.0 * 72.0);
                    var textWidthMM = sizeText.Width * 25.4d / 72.0d;

                    numberPos = localStartPoint + new XVector(pin.PinNumberPosition.X, pin.PinNumberPosition.Y) + new XVector(textWidthMM, 0);
                    numberPos.Y = -pin.PinNumberPosition.Y;
                }

                numberPos = GetPointTransform(t, numberPos);

                var rot = pin.GetAngleOrientation(false) + numberRot;

                figure.FigureItems.Add(new PdfTextPrimitive
                {
                    Text = pin.Number,
                    Position = ToPdfPoint(numberPos),
                    Rot = ToPdfRot(rot),
                    FontFamily = "Arial",
                    FontSize = fontSize,
                    FontWeight = 0
                });
            }

            figure.FigureItems.Add(new PdfLinePrimitive
            {
                StartPoint = ToPdfPoint(sp),
                EndPoint = ToPdfPoint(ep),
                Width = ToPdfSize(pin.Width),
                Color = ToPdfColor(pin.PinColor)
            });

            return figure;
        }

        private PdfPrimitive GetImagePrimitive(IImageCanvasItem img)
        {
            var t = img.GetTransform();
            var pos = new XPoint(img.X, img.Y);
            pos = GetPointTransform(t, pos);
            pos = ToPdfPoint(pos);

            return new PdfImagePrimitive
            {
                X = pos.X,
                Y = pos.Y,
                Width = ToPdfSize(img.Width),
                Height = ToPdfSize(img.Height),
                Rot = ToPdfRot(img.Rot),
                BorderColor = img.BorderColor,
                BorderWidth = ToPdfSize(img.BorderWidth),
                FillColor = img.FillColor,
                ImageBytes = img.ImageBytes,
                Stretch = img.Stretch
            };
        }

        private PdfPrimitive GetEllipsePrimitive(IEllipseSchematicCanvasItem ellipse)
        {
            var t = ellipse.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);

            return new PdfEllipsePrimitive
            {
                X = ToPdfX(position.X),
                Y = ToPdfY(position.Y),
                Width = ToPdfSize(ellipse.Width),
                Height = ToPdfSize(ellipse.Height),
                BorderWidth = ToPdfSize(ellipse.BorderWidth),
                Color = ToPdfColor(ellipse.BorderColor),
                FillColor = ToPdfColor(ellipse.FillColor)
            };
        }

        PdfPrimitive GetCirclePrimitive(ICircleSchematicCanvasItem item)
        {
            var t = item.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);

            return new PdfEllipsePrimitive
            {
                X = ToPdfX(position.X),
                Y = ToPdfY(position.Y),
                Width = ToPdfSize(item.Diameter),
                Height = ToPdfSize(item.Diameter),
                BorderWidth = ToPdfSize(item.BorderWidth),
                Color = ToPdfColor(item.BorderColor),
                FillColor = ToPdfColor(item.FillColor)
            };
        }

        PdfPrimitive GetPolygonPrimitive(IPolySchematicCanvasItem item)
        {
            // var figure = new PdfFigure();
            var t = item.GetTransform();

            return new PdfPolygonPrimitive
            {
                BorderWidth = ToPdfSize(item.BorderWidth),
                Points = item.PolygonPoints.Select(p => ToPdfPoint(GetPointTransform(t, p))).ToList(),
                Color = ToPdfColor(item.BorderColor),
                FillColor = ToPdfColor(item.FillColor)
            };

           
        }


        PdfPrimitive GetArcPrimitive(IArcSchematicCanvasItem item, bool transformed = true)
        {
            var sp = new XPoint(item.StartPointX, item.StartPointY);
            var ep = new XPoint(item.EndPointX, item.EndPointY);
            var center = item.GetCenter();
            if (transformed)
            {
                var t = item.GetTransform();
                sp = GetPointTransform(t, sp);
                ep = GetPointTransform(t, ep);
                center = GetPointTransform(t, center);
            }

            return new PdfArcPrimitive
            {
                StartPoint = ToPdfPoint(sp),
                EndPoint = ToPdfPoint(ep),
                Width = ToPdfSize(item.BorderWidth),
                SizeDiameter = ToPdfSize(2 * item.Radius),
                Center = ToPdfPoint(center),
                IsLargeArc = item.IsLargeArc,
                RotationAngle = ToPdfRot(item.RotationAngle),
                SweepDirection = item.SweepDirection,
                Color = item.BorderColor
            };
        }

        PdfPrimitive GetTextPrimitive(ITextSchematicCanvasItem text)
        {
            var pos = new XPoint(text.X, text.Y);

            var t = text.GetTransform();
            pos = GetPointTransform(t, pos);

            return new PdfTextPrimitive
            {
                Position = ToPdfPoint(pos),
                Rot = ToPdfRot(text.Rot),
                Text = text.Text,
                FontFamily = text.FontFamily.ToString(),
                FontSize = text.FontSize,
                FontWeight = text.Bold ? 600 : 0,
                Color = ToPdfColor(text.TextColor)
            };
        }

        PdfPrimitive GetRectanglePrimitive(IRectangleSchematicCanvasItem item)
        {
            var t = item.GetTransform();

            var points = new List<XPoint>();
            var rect = new XRect(-0.5 * item.Width, -0.5 * item.Height, item.Width, item.Height);
            points.Add(rect.TopLeft);
            points.Add(rect.TopRight);
            points.Add(rect.BottomRight);
            points.Add(rect.BottomLeft);

            //transform points
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = t.Transform(points[i]);
            }

            return new PdfRectanglePrimitive
            {
                BorderWidth = ToPdfSize(item.BorderWidth),
                Points = points.Select(p => ToPdfPoint(p)).ToList(),
                FillColor = item.FillColor,
                Color = item.BorderColor
            };


        }

        PdfPrimitive GetLinePrimitive(ILineSchematicCanvasItem item, bool transformed = true)
        {
            var sp = new XPoint(item.X1, item.Y1);
            var ep = new XPoint(item.X2, item.Y2);
            if (transformed)
            {
                var t = item.GetTransform();
                sp = GetPointTransform(t, sp);
                ep = GetPointTransform(t, ep);
            }

            return new PdfLinePrimitive
            {
                StartPoint = ToPdfPoint(sp),
                EndPoint = ToPdfPoint(ep),
                Width = ToPdfSize(item.Width),
                Color = ToPdfColor(item.LineColor),
                LineStyle = item.LineStyle
            };
        }

        PdfPrimitive GetPolylinePrimitive(IPolylineCanvasItem item)
        {
            return new PdfPolylinePrimitive
            {
                Width = ToPdfSize(item.Width),
                Points = item.Points.Select(p => ToPdfPoint(p)).ToList()
            };
        }

        PdfPrimitive GetNetWirePrimitive(INetWireCanvasItem item)
        {
            var p = GetPolylinePrimitive(item);
            p.Color = item.LineColor;

            return p;
        }
        PdfPrimitive GetBusWirePrimitive(IBusWireCanvasItem item)
        {
            var p = GetPolylinePrimitive(item);
            p.Color = item.LineColor;

            return p;
        }

        XPoint GetPointTransform(XTransform transform, XPoint p)
        {
            p = transform.Transform(p);
            return p;
        }

        //double GetRotationTransform(XTransform transform)
        //{
        //    return GeometryHelper.GetRotationAngleFromMatrix(transform.Value);
        //}

        public Task BuildPage()
        {
            return Task.Run(() => CreatePage());

        }

        void CreatePage()
        {
            PreparePage();
            BuildExecutionPlan();

            WritePrimitives();
        }

        void WritePrimitives()
        {
            foreach (var p in buildPlanPrimitives)
                p.WriteTo(pdfDoc);
        }

        double ToPdfX(double x)//x is in mm
        {
            return 72 * (x - sheetOriginX) / 25.4;
        }

        double ToPdfY(double y)//y is in mm
        {
            return 72 * (sheetOriginY - y) / 25.4;
        }

        double ToPdfSize(double mmSize)
        {
            return 72 * mmSize / 25.4;
        }

        XPoint ToPdfPoint(XPoint p)
        {
            return new XPoint(ToPdfX(p.X), ToPdfY(p.Y));
        }

        XColor ToPdfColor(XColor c)
        {
            if (c == backgroundColor)
                c = XColor.FromRgb((byte)(255 - c.R), (byte)(255 - c.G), (byte)(255 - c.B));

            return c;
        }

        double ToPdfRot(double rot)
        {
            return rot;
        }
    }
}
