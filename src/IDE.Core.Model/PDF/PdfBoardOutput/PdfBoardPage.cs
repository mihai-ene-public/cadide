using IDE.Core.Build;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Media;

namespace IDE.Core.PDF
{
    public class PdfBoardPage
    {
        public PdfBoardPage(IBoardDesigner _board, PdfDocument pdf, BoardLayerOutput layer, double widthMillimeters, double heightMillimeters)
        {
            board = _board;
            pdfDoc = pdf;
            innerLayer = layer;

            sheetWidth = widthMillimeters;
            sheetHeight = heightMillimeters;

            GeometryHelper = ServiceProvider.Resolve<IGeometryHelper>();

            LoadOptions();
        }
        IGeometryHelper GeometryHelper;

        IBoardDesigner board;
        PdfDocument pdfDoc;
        BoardLayerOutput innerLayer;

        List<PdfPrimitive> buildPlanPrimitives = new List<PdfPrimitive>();

        double sheetWidth;
        double sheetHeight;

        XRect boardRectangle;
        double boardOriginX;
        double boardOriginY;

        XColor backgroundColor = XColor.FromRgb(255, 255, 255);//Colors.White;

        XColor darkColor = XColor.FromRgb(0, 0, 0);

        bool drawBoardOutline = true;

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

        void LoadOptions()
        {
            drawBoardOutline = board.BuildOptions.GerberPlotBoardOutlineOnAllLayers;
        }

        void PreparePage()
        {
            PrepareBoardOutline();
            pdfDoc.AddPage(ToPdfSize(sheetWidth), ToPdfSize(sheetHeight));
        }

        void PrepareBoardOutline()
        {
            //calculate gerber board origin
            if (board.BoardOutline == null)
                return;
            var upperLeft = board.BoardOutline.StartPoint;
            var lowerRight = upperLeft;
            var startPoint = board.BoardOutline.StartPoint;
            foreach (var item in board.BoardOutline.Items)
            {
                //upperLeft: Xmin, Ymin
                if (upperLeft.X > item.EndPointX)
                    upperLeft.X = item.EndPointX;
                if (upperLeft.Y > item.EndPointY)
                    upperLeft.Y = item.EndPointY;

                //lowerRight: Xmax, Ymax
                if (lowerRight.X < item.EndPointX)
                    lowerRight.X = item.EndPointX;
                if (lowerRight.Y < item.EndPointY)
                    lowerRight.Y = item.EndPointY;
            }

            boardRectangle = new XRect(upperLeft, lowerRight);
            boardOriginX = boardRectangle.BottomLeft.X;
            boardOriginY = boardRectangle.BottomLeft.Y;
        }

        void BuildExecutionPlan()
        {
            buildPlanPrimitives = new List<PdfPrimitive>();

            //dark items
            if (innerLayer.AddItems != null)
            {
                if (drawBoardOutline)
                {
                    var pdfPrimitive = GetPrimitive(board.BoardOutline);
                    if (pdfPrimitive != null)
                    {
                        pdfPrimitive.Color = darkColor;
                        buildPlanPrimitives.Add(pdfPrimitive);
                    }
                }

                foreach (ISelectableItem item in innerLayer.AddItems)
                {
                    var pdfPrimitive = GetPrimitive(item);
                    if (pdfPrimitive != null)
                    {
                        pdfPrimitive.Color = darkColor;
                        buildPlanPrimitives.Add(pdfPrimitive);
                    }
                }
            }

            //clear items
            if (innerLayer.ExtractItems != null)
            {
                foreach (ISelectableItem item in innerLayer.ExtractItems)
                {
                    var pdfPrimitive = GetPrimitive(item);
                    if (pdfPrimitive != null)
                    {
                        // pdfPrimitive.Color = backgroundColor;
                        SetClearColor(pdfPrimitive);
                        buildPlanPrimitives.Add(pdfPrimitive);
                    }
                }
            }
        }

        void WritePrimitives()
        {
            foreach (var p in buildPlanPrimitives)
                p.WriteTo(pdfDoc);
        }

        double ToPdfX(double x)//x is in mm
        {
            return 72 * (x - boardOriginX) / 25.4;
        }

        double ToPdfY(double y)//y is in mm
        {
            return 72 * (boardOriginY - y) / 25.4;
        }

        double ToPdfSize(double mmSize)
        {
            return 72 * mmSize / 25.4;
        }

        XPoint ToPdfPoint(XPoint p)
        {
            return new XPoint(ToPdfX(p.X), ToPdfY(p.Y));
        }

        double ToPdfRot(double rot)
        {
            return rot;
        }

        XPoint GetPointTransform(XTransform transform, XPoint p)
        {
            p = transform.Transform(p);
            return p;
        }

        double GetWorldRotation(XTransform transform, FootprintPlacement placement)
        {
            var rotAngle = GetRotationTransform(transform);
            var myRot = rotAngle;

            if (placement == FootprintPlacement.Bottom)
            {
                myRot = 180.0d - myRot;

                var sign = Math.Sign(rotAngle);
                rotAngle = sign * (180 - Math.Abs(rotAngle));
            }


            return myRot;
        }

        double GetRotationTransform(XTransform transform)
        {
            return Geometry2DHelper.GetRotationAngleFromMatrix(transform.Value);
        }

        PdfPrimitive GetPrimitive(ISelectableItem item, bool transformed = true, double clearance = 0)
        {
            if (item != null)
            {
                switch (item)
                {
                    case ILineCanvasItem line:
                        return GetLinePrimitive(line, transformed, clearance);

                    case IPolylineCanvasItem polyline:
                        return GetPolylinePrimitive(polyline, clearance);

                    case IRectangleCanvasItem rectangle:
                        return GetRectanglePrimitive(rectangle, clearance);

                    case ICircleCanvasItem circle:
                        return GetCirclePrimitive(circle, clearance);

                    case IHoleCanvasItem hole:
                        return GetHolePrimitive(hole, clearance);

                    case IViaCanvasItem via:
                        return GetViaPrimitive(via, clearance);

                    case IPadCanvasItem pad:
                        return GetPadPrimitive(pad, clearance);

                    case IPolygonBoardCanvasItem poly:
                        return GetPolygonPrimitive(poly, clearance);

                    case IArcCanvasItem arc:
                        return GetArcPrimitive(arc, transformed, clearance);

                    case ITextCanvasItem text:
                        return GetTextPrimitive(text);

                    case ITextMonoLineCanvasItem textMono:
                        return GetTextPrimitive(textMono);

                    case IRegionCanvasItem region:
                        return GetRegionPrimitive(region);

                    //case IPlaneBoardCanvasItem plane:
                    //    return GetPlanePrimitive(plane);
                }


            }

            return null;
        }

        PdfPrimitive GetPrimitive(IRegionItem item, XPoint startPoint, double width)
        {
            if (item != null)
            {
                if (item is ILineRegionItem)
                    return GetLinePrimitive((ILineRegionItem)item, startPoint, width);
                else if (item is IArcRegionItem)
                    return GetArcPrimitive((IArcRegionItem)item, startPoint, width);
            }

            return null;
        }

        PdfPrimitive GetLinePrimitive(ILineRegionItem item, XPoint startPoint, double width)
        {
            return new PdfLinePrimitive
            {
                StartPoint = ToPdfPoint(startPoint),
                EndPoint = ToPdfPoint(item.EndPoint),
                Width = width
            };
        }

        PdfPrimitive GetLinePrimitive(ILineCanvasItem item, bool transformed = true, double clearance = 0)
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
                Width = ToPdfSize(item.Width + 2 * clearance),
            };
        }

        PdfPrimitive GetPolylinePrimitive(IPolylineCanvasItem item, double clearance = 0)
        {
            return new PdfPolylinePrimitive
            {
                Width = ToPdfSize(item.Width + 2 * clearance),
                Points = item.Points.Select(p => ToPdfPoint(p)).ToList()
            };
        }

        PdfPrimitive GetRectanglePrimitive(IRectangleCanvasItem item, double clearance = 0)
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
                FillColor = item.IsFilled ? darkColor : backgroundColor,
            };
        }

        XColor GetFillColor(bool isFilled)
        {
            return isFilled ? darkColor : backgroundColor;
        }

        PdfPrimitive GetCirclePrimitive(ICircleCanvasItem item, double clearance = 0.0)
        {
            var t = item.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);

            return new PdfEllipsePrimitive
            {
                X = ToPdfX(position.X),
                Y = ToPdfY(position.Y),
                Width = ToPdfSize(item.Diameter + 2 * clearance),
                Height = ToPdfSize(item.Diameter + 2 * clearance),
                BorderWidth = ToPdfSize(item.BorderWidth),
                //Color = ToPdfColor(item.BorderColor),
                FillColor = GetFillColor(item.IsFilled)
            };
        }

        PdfPrimitive GetViaPrimitive(IViaCanvasItem item, double clearance = 0)
        {
            var figure = new PdfFigure();
            //var pos = new Point(item.X, item.Y);

            figure.FigureItems.Add(new PdfEllipsePrimitive
            {
                X = ToPdfX(item.X),
                Y = ToPdfY(item.Y),
                Width = ToPdfSize(item.Diameter + 2 * clearance),
                Height = ToPdfSize(item.Diameter + 2 * clearance),
                BorderWidth = 0,
                FillColor = GetFillColor(true)
            });

            figure.FigureItems.Add(new PdfEllipsePrimitive
            {
                X = ToPdfX(item.X),
                Y = ToPdfY(item.Y),
                Width = ToPdfSize(item.Drill),
                Height = ToPdfSize(item.Drill),
                BorderWidth = 0,
                FillColor = GetFillColor(false)
            });

            return figure;

        }

        PdfPrimitive GetHolePrimitive(IHoleCanvasItem item, double clearance = 0)
        {
            var t = item.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);

            if (item.DrillType == DrillType.Drill)
            {
                return new PdfEllipsePrimitive
                {
                    X = ToPdfX(position.X),
                    Y = ToPdfY(position.Y),
                    Width = ToPdfSize(item.Drill + 2 * clearance),
                    Height = ToPdfSize(item.Drill + 2 * clearance),
                    BorderWidth = 0,
                    //Color = ToPdfColor(item.BorderColor),
                    FillColor = GetFillColor(true)
                };
            }
            else
            {
                //var placement = FootprintPlacement.Top;
                //var fp = item.ParentObject as IFootprintBoardCanvasItem;
                //if (fp != null)
                //    placement = fp.Placement;

                //var rot = GetWorldRotation(t, placement);

                //return new GerberRectanglePrimitive
                //{
                //    X = ToGerberX(position.X),
                //    Y = ToGerberY(position.Y),
                //    Width = item.Drill,
                //    Height = item.Height,
                //    CornerRadius = item.Drill * 0.5,
                //    Rot = ToGerberRot(rot)
                //};
            }

            return null;
        }

        PdfPrimitive GetPadPrimitive(IPadCanvasItem item, double clearance = 0)
        {
            var t = item.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);
            var placement = FootprintPlacement.Top;
            if (item.ParentObject is IFootprintBoardCanvasItem fp)
                placement = fp.Placement;
            var rot = GetWorldRotation(t, placement);

            if (item.CornerRadius > 0.0d)
            {
                var figure = GetRoundedRectangle(position.X, position.Y,
                                                item.Width + 2 * clearance,
                                                item.Height + 2 * clearance,
                                                item.CornerRadius + clearance, t);

                return figure;
                //return new PdfRoundedRectanglePrimitive
                //{
                //    FillColor = GetFillColor(true),
                //    X = ToPdfX(position.X),
                //    Y = ToPdfY(position.Y),
                //    Width = ToPdfSize(item.Width),
                //    Height = ToPdfSize(item.Height),
                //    CornerRadius = ToPdfSize(item.CornerRadius),
                //    Rot = ToPdfRot(rot)
                //};
            }
            else
            {
                var width = item.Width + 2 * clearance;
                var height = item.Height + 2 * clearance;
                var rect = new XRect(-0.5 * width, -0.5 * height, width, height);
                var points = GetPointsFromRect(rect, t);

                return new PdfRectanglePrimitive
                {
                    Points = points.Select(p => ToPdfPoint(p)).ToList(),
                    FillColor = GetFillColor(true),
                };
            }
        }

        PdfPrimitive GetRoundedRectangle(double centerX, double centerY, double width, double height, double cornerRadius, XTransform t)
        {
            var figure = new PdfFigure();

            //add two overlapping rectangles as box body
            //horizontal rectangle
            var hWidth = width;
            var hHeight = height - 2 * cornerRadius;
            var rect = new XRect(-0.5 * hWidth, -0.5 * hHeight, hWidth, hHeight);
            var points = GetPointsFromRect(rect, t);

            figure.FigureItems.Add(new PdfRectanglePrimitive
            {
                Points = points.Select(p => ToPdfPoint(p)).ToList(),
                FillColor = GetFillColor(true),
            });

            //vertical rectangle
            var vWidth = width - 2 * cornerRadius;
            var vHeight = height;
            rect = new XRect(-0.5 * vWidth, -0.5 * vHeight, vWidth, vHeight);
            points = GetPointsFromRect(rect, t);

            figure.FigureItems.Add(new PdfRectanglePrimitive
            {
                Points = points.Select(p => ToPdfPoint(p)).ToList(),
                FillColor = GetFillColor(true),
            });

            //add four circles for the rounded corners
            var diameter = 2 * cornerRadius;
            var centerCircles = new[]{
                                    new XPoint(0.5 * width - cornerRadius, 0.5 * height - cornerRadius),
                                    new XPoint(-0.5 * width + cornerRadius, 0.5 * height - cornerRadius),
                                    new XPoint(-0.5 * width + cornerRadius, -0.5 * height + cornerRadius),
                                    new XPoint(0.5 * width - cornerRadius, -0.5 * height + cornerRadius),
                                    };
            foreach (var c in centerCircles)
            {
                var centerCircle = GetPointTransform(t, c);

                figure.FigureItems.Add(new PdfEllipsePrimitive
                {
                    X = ToPdfX(centerCircle.X),
                    Y = ToPdfY(centerCircle.Y),
                    Width = ToPdfSize(diameter),
                    Height = ToPdfSize(diameter),
                    FillColor = GetFillColor(true)
                });
            }


            return figure;
        }

        XRect GetRectNotTranslated(double width, double height)
        {
            return new XRect(-0.5 * width, -0.5 * height, width, height);
        }

        List<XPoint> GetPointsFromRect(XRect rect, XTransform t)
        {
            var points = new List<XPoint>();
            points.Add(rect.TopLeft);
            points.Add(rect.TopRight);
            points.Add(rect.BottomRight);
            points.Add(rect.BottomLeft);

            //transform points
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = t.Transform(points[i]);
            }

            return points;
        }

        PdfPrimitive GetPolygonPrimitive(IPolygonBoardCanvasItem item, double clearance)
        {

            var figure = new PdfFigure();
            var t = item.GetTransform();

            var borderWidth = item.BorderWidth + 2 * clearance;

            //if (borderWidth > 0.0)
            {
                figure.FigureItems.Add(new PdfPolygonPrimitive
                {
                    BorderWidth = ToPdfSize(borderWidth),
                    Points = item.PolygonPoints.Select(p => ToPdfPoint(GetPointTransform(t, p))).ToList(),
                    Color = darkColor,
                    FillColor = GetFillColor(item.IsFilled)
                });
            }

            if (item.IsFilled)
            {
                //substract the other geometries on the same layer (polygons and tracks with its clearances)
                var excludedItems = item.GetExcludedItems();

                var thermalPrimitives = new List<PdfPrimitive>();

                var padsOfThisSignal = new List<IItemWithClearance>();
                var restOfItems = new List<IItemWithClearance>();

                foreach (var excludedItem in excludedItems)
                {
                    var clearanceItem = excludedItem.CanvasItem;

                    if (clearanceItem is IPadCanvasItem pad)
                    {
                        var polySignal = item as ISignalPrimitiveCanvasItem;
                        if (polySignal.Signal != null && pad.Signal != null &&
                            polySignal.Signal.Name == pad.Signal.Name)
                        {
                            padsOfThisSignal.Add(excludedItem);
                            if (item.GenerateThermals)
                            {
                                var thermal = GetThermalsForPad(excludedItem, item);
                                thermalPrimitives.Add(thermal);
                            }
                        }
                        else
                        {
                            restOfItems.Add(excludedItem);
                        }
                    }
                    else
                    {
                        restOfItems.Add(excludedItem);
                    }
                }

                foreach (var clearedPad in padsOfThisSignal)
                {
                    AddClearFigure(clearedPad, figure);
                }

                //add thermals
                figure.FigureItems.AddRange(thermalPrimitives);

                //clear the rest of items
                foreach (var ci in restOfItems)
                {
                    AddClearFigure(ci, figure);
                }
            }

            return figure;
        }

        void AddClearFigure(IItemWithClearance clearanceItem, PdfFigure figure)
        {
            var clearFigure = GetPrimitive(clearanceItem.CanvasItem, transformed: true, clearanceItem.Clearance);

            SetClearColor(clearFigure);

            figure.FigureItems.Add(clearFigure);
        }

        void SetClearColor(PdfPrimitive clearPrimitive)
        {
            clearPrimitive.Color = backgroundColor;
            if (clearPrimitive is IFilledPdfPrimitive filled)
                filled.FillColor = backgroundColor;
            if (clearPrimitive is PdfFigure gFig)
            {
                foreach (var fi in gFig.FigureItems)
                {
                    SetClearColor(fi);
                }
            }
        }

        PdfPrimitive GetThermalsForPad(IItemWithClearance padInflated, IPolygonBoardCanvasItem thisPoly)
        {
            var pad = padInflated.CanvasItem as IPadCanvasItem;

            var figure = new PdfFigure();

            //2 rectangles in cross rotated by pad and in center of the pad
            var thermalWidth = thisPoly.ThermalWidth;
            var thermalClearance = padInflated.Clearance;

            var t = pad.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);
            var placement = FootprintPlacement.Top;
            if (pad.ParentObject is IFootprintBoardCanvasItem fp)
                placement = fp.Placement;
            var rot = GetWorldRotation(t, placement);

            //horizontal
            //var tFigure = new PdfRoundedRectanglePrimitive
            //{
            //    X = ToPdfX(position.X),
            //    Y = ToPdfY(position.Y),
            //    Width = pad.Width,
            //    Height = thermalWidth,
            //    Rot = ToPdfRot(rot)
            //};
            var rect = GetRectNotTranslated(pad.Width, thermalWidth);
            var points = GetPointsFromRect(rect, t);
            var tFigure = new PdfRectanglePrimitive
            {
                Points = points.Select(p => ToPdfPoint(p)).ToList(),
                FillColor = GetFillColor(true),
            };
            figure.FigureItems.Add(tFigure);

            //vertical
            //tFigure = new PdfRoundedRectanglePrimitive
            //{
            //    X = ToPdfX(position.X),
            //    Y = ToPdfY(position.Y),
            //    Width = thermalWidth,
            //    Height = pad.Height,
            //    Rot = ToPdfRot(rot)
            //};
            rect = GetRectNotTranslated(thermalWidth, pad.Height);
            points = GetPointsFromRect(rect, t);
            tFigure = new PdfRectanglePrimitive
            {
                Points = points.Select(p => ToPdfPoint(p)).ToList(),
                FillColor = GetFillColor(true),
            };
            figure.FigureItems.Add(tFigure);

            return figure;
        }

        PdfPrimitive GetArcPrimitive(IArcCanvasItem item, bool transformed, double clearance = 0)
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
                Width = ToPdfSize(item.BorderWidth) + 2 * clearance,
                SizeDiameter = ToPdfSize(2 * item.Radius),
                Center = ToPdfPoint(center),
                IsLargeArc = item.IsLargeArc,
                RotationAngle = ToPdfRot(item.RotationAngle),
                SweepDirection = item.SweepDirection,
                // Color = item.BorderColor
            };
        }

        PdfPrimitive GetArcPrimitive(IArcRegionItem item, XPoint startPoint, double width)
        {

            var sp = startPoint;
            var ep = item.EndPoint;
            var center = Geometry2DHelper.GetArcCenter(startPoint, ep, item.SizeDiameter, item.SizeDiameter, item.SweepDirection);

            return new PdfArcPrimitive
            {
                StartPoint = ToPdfPoint(sp),
                EndPoint = ToPdfPoint(ep),
                Width = ToPdfSize(width),
                SizeDiameter = ToPdfSize(item.SizeDiameter),
                Center = ToPdfPoint(center),
                IsLargeArc = item.IsLargeArc,
                SweepDirection = item.SweepDirection,
            };
        }

        PdfPrimitive GetTextPrimitive(ITextCanvasItem text)
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
                //Color = ToPdfColor(text.TextColor)
            };
        }

        PdfPrimitive GetTextPrimitive(ITextMonoLineCanvasItem item)
        {
            var t = item.GetTransform();

            var figure = new PdfFigure();
            double lx = 0;
            foreach (var letter in item.LetterItems)
            {
                var local = new XTranslateTransform(lx, 0);
                foreach (var li in letter.Items.Select(l => (ISelectableItem)l.Clone()))
                {
                    li.TransformBy(local.Value * t.Value);

                    var primitive = GetPrimitive(li, false);
                    figure.FigureItems.Add(primitive);
                }

                lx += item.FontSize;
            }


            return figure;
        }

        PdfPrimitive GetRegionPrimitive(IRegionCanvasItem item)
        {
            //we shouldn't convert to Gerber coordinates here
            var figure = new PdfFigure();

            var startPoint = item.StartPoint;

            foreach (var regionItem in item.Items)
            {
                var primitive = GetPrimitive(regionItem, startPoint, item.Width);
                startPoint = regionItem.EndPoint;
                if (primitive != null)
                    figure.FigureItems.Add(primitive);
            }

            return figure;
        }
    }
}
