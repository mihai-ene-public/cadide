using IDE.Core.Build;
using IDE.Core.Interfaces;
using IDE.Core.Model.GlobalRepresentation.Primitives;
using IDE.Core.Types.Media;

namespace IDE.Core.PDF
{
    public class PdfBoardPageBuilder
    {
        double boardOriginX;
        double boardOriginY;

        XColor backgroundColor = XColor.FromRgb(255, 255, 255);

        XColor darkColor = XColor.FromRgb(0, 0, 0);

        bool drawBoardOutline = true;

        public Task Build(IBoardDesigner board, PdfDocument pdfDoc, BoardGlobalLayerOutput layer, double widthMillimeters, double heightMillimeters)
        {
            LoadOptions(board);

            PrepareBoardOutline(board);

            pdfDoc.AddPage(ToPdfSize(widthMillimeters), ToPdfSize(heightMillimeters));

            var buildPlanPrimitives = BuildExecutionPlan(layer);

            WritePrimitives(buildPlanPrimitives, pdfDoc);

            return Task.CompletedTask;
        }



        private void LoadOptions(IBoardDesigner board)
        {
            drawBoardOutline = board.BuildOptions.GerberPlotBoardOutlineOnAllLayers;
        }

        private void PrepareBoardOutline(IBoardDesigner board)
        {
            if (board.BoardOutline == null)
                return;
            var boardRectangle = board.GetBoardRectangle();

            boardOriginX = boardRectangle.BottomLeft.X;
            boardOriginY = boardRectangle.BottomLeft.Y;
        }

        private IList<PdfPrimitive> BuildExecutionPlan(BoardGlobalLayerOutput layer)
        {
            var buildPlanPrimitives = new List<PdfPrimitive>();

            //dark items
            if (layer.AddItems != null)
            {
                var itemsToAdd = new List<GlobalPrimitive>();

                if (drawBoardOutline)
                    itemsToAdd.Add(layer.BoardOutline);

                //add poured polygons first (so that it won't clear wanted traces)
                var pouredPolys = layer.AddItems.OfType<GlobalPouredPolygonPrimitive>().ToList();
                itemsToAdd.AddRange(pouredPolys);
                itemsToAdd.AddRange(layer.AddItems.Except(pouredPolys));

                foreach (var item in itemsToAdd)
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
            if (layer.ExtractItems != null)
            {
                foreach (var item in layer.ExtractItems)
                {
                    var pdfPrimitive = GetPrimitive(item);
                    if (pdfPrimitive != null)
                    {
                        SetClearColor(pdfPrimitive);
                        buildPlanPrimitives.Add(pdfPrimitive);
                    }
                }
            }

            return buildPlanPrimitives;
        }

        private void WritePrimitives(IList<PdfPrimitive> buildPlanPrimitives, PdfDocument pdfDoc)
        {
            foreach (var p in buildPlanPrimitives)
                p.WriteTo(pdfDoc);
        }

        double ToPdfX(double x)//x is in mm
        {
            return 72 * ( x - boardOriginX ) / 25.4;
        }

        double ToPdfY(double y)//y is in mm
        {
            return 72 * ( boardOriginY - y ) / 25.4;
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

        XColor GetFillColor(bool isFilled)
        {
            return isFilled ? darkColor : backgroundColor;
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

        PdfPrimitive GetPrimitive(GlobalPrimitive item)
        {
            switch (item)
            {
                case GlobalLinePrimitive line:
                    return GetLinePrimitive(line);

                case GlobalPolylinePrimitive polyline:
                    return GetPolylinePrimitive(polyline);

                case GlobalRectanglePrimitive rectangle:
                    return GetRectanglePrimitive(rectangle);

                case GlobalCirclePrimitive circle:
                    return GetCirclePrimitive(circle);

                case GlobalHolePrimitive hole:
                    return GetHolePrimitive(hole);

                case GlobalViaPrimitive via:
                    return GetViaPrimitive(via);

                case GlobalPouredPolygonPrimitive pouredPoly:
                    return GetPolygonPrimitive(pouredPoly);

                case GlobalPolygonPrimitive poly:
                    return GetPolygonPrimitive(poly);

                case GlobalArcPrimitive arc:
                    return GetArcPrimitive(arc);

                case GlobalTextPrimitive text:
                    return GetTextPrimitive(text);

                case GlobalRegionPrimitive region:
                    return GetRegionPrimitive(region);

                case GlobalFigurePrimitive figure:
                    return GetFigurePrimitive(figure);
            }

            return null;
        }

        private PdfPrimitive GetLinePrimitive(GlobalLinePrimitive line)
        {
            return new PdfLinePrimitive
            {
                StartPoint = ToPdfPoint(line.StartPoint),
                EndPoint = ToPdfPoint(line.EndPoint),
                Width = ToPdfSize(line.Width),
            };
        }

        private PdfPrimitive GetPolylinePrimitive(GlobalPolylinePrimitive polyline)
        {
            return new PdfPolylinePrimitive
            {
                Width = ToPdfSize(polyline.Width),
                Points = polyline.Points.Select(p => ToPdfPoint(p)).ToList()
            };
        }

        private PdfPrimitive GetRectanglePrimitive(GlobalRectanglePrimitive item)
        {

            ////case for rounded pads
            //if (item.Width == item.Height && item.CornerRadius == 0.5 * item.Width)
            //{
            //    return new PdfEllipsePrimitive
            //    {
            //        X = ToPdfX(item.X),
            //        Y = ToPdfY(item.Y),
            //        Width = ToPdfSize(item.Width),
            //        Height = ToPdfSize(item.Height),
            //        BorderWidth = ToPdfSize(item.BorderWidth),
            //        FillColor = GetFillColor(item.IsFilled)
            //    };
            //}
            ////todo: implement the case for a pad with rounded corners

            ////a rotated rectangle
            //var t = new XTransformGroup();
            //t.Children.Add(new XRotateTransform(item.Rot));
            //t.Children.Add(new XTranslateTransform(item.X, item.Y));

            //var points = new List<XPoint>();
            //var rect = new XRect(-0.5 * item.Width, -0.5 * item.Height, item.Width, item.Height);
            //points.Add(rect.TopLeft);
            //points.Add(rect.TopRight);
            //points.Add(rect.BottomRight);
            //points.Add(rect.BottomLeft);

            ////transform points
            //for (int i = 0; i < points.Count; i++)
            //{
            //    points[i] = t.Transform(points[i]);
            //}

            //return new PdfRectanglePrimitive
            //{
            //    BorderWidth = ToPdfSize(item.BorderWidth),
            //    Points = points.Select(p => ToPdfPoint(p)).ToList(),
            //    FillColor = item.IsFilled ? darkColor : backgroundColor,
            //};

            var t = new XTransformGroup();
            t.Children.Add(new XRotateTransform(item.Rot));
            t.Children.Add(new XTranslateTransform(item.X, item.Y));

            if (item.Width == item.Height && item.CornerRadius == 0.5 * item.Width)
            {
                return new PdfEllipsePrimitive
                {
                    X = ToPdfX(item.X),
                    Y = ToPdfY(item.Y),
                    Width = ToPdfSize(item.Width),
                    Height = ToPdfSize(item.Height),
                    BorderWidth = ToPdfSize(item.BorderWidth),
                    FillColor = GetFillColor(item.IsFilled)
                };
            }
            if (item.CornerRadius > 0.0d)
            {
                var figure = GetRoundedRectangle(item.Width, item.Height, item.CornerRadius, t);

                return figure;
            }
            else
            {
                var width = item.Width;
                var height = item.Height;
                var rect = new XRect(-0.5 * width, -0.5 * height, width, height);
                var points = GetPointsFromRect(rect, t);

                return new PdfRectanglePrimitive
                {
                    BorderWidth = ToPdfSize(item.BorderWidth),
                    Points = points.Select(p => ToPdfPoint(p)).ToList(),
                    FillColor = GetFillColor(true),
                };
            }

        }

        PdfPrimitive GetRoundedRectangle(double width, double height, double cornerRadius, XTransform t)
        {
            //a figure with 2 crossed rectangles: one horizontal, one vertical
            //four circles in corners ar added

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
                var centerCircle = t.Transform(c);

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

        private IList<XPoint> GetPointsFromRect(XRect rect, XTransform t)
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


        private PdfPrimitive GetCirclePrimitive(GlobalCirclePrimitive item)
        {
            return new PdfEllipsePrimitive
            {
                X = ToPdfX(item.X),
                Y = ToPdfY(item.Y),
                Width = ToPdfSize(item.Diameter),
                Height = ToPdfSize(item.Diameter),
                BorderWidth = ToPdfSize(item.BorderWidth),
                FillColor = GetFillColor(item.IsFilled)
            };
        }

        private PdfPrimitive GetHolePrimitive(GlobalHolePrimitive hole)
        {
            return new PdfEllipsePrimitive
            {
                X = ToPdfX(hole.X),
                Y = ToPdfY(hole.Y),
                Width = ToPdfSize(hole.Drill),
                Height = ToPdfSize(hole.Drill),
                BorderWidth = 0,
                FillColor = GetFillColor(true)
            };
        }

        private PdfPrimitive GetViaPrimitive(GlobalViaPrimitive item)
        {
            var figure = new PdfFigure();

            figure.FigureItems.Add(new PdfEllipsePrimitive
            {
                X = ToPdfX(item.X),
                Y = ToPdfY(item.Y),
                Width = ToPdfSize(item.PadDiameter),
                Height = ToPdfSize(item.PadDiameter),
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

        private PdfPrimitive GetPolygonPrimitive(GlobalPouredPolygonPrimitive poly)
        {
            var figure = new PdfFigure();

            //fill polygon 
            var polyRegion = GetPrimitive(poly.FillPrimitive);

            if (polyRegion != null)
                figure.FigureItems.Add(polyRegion);

            //clear primitives
            foreach (var clearPrimitive in poly.RemovePrimitives)
            {
                AddClearFigure(clearPrimitive, figure);
            }

            //thermals
            foreach (var thermal in poly.Thermals)
            {
                var thermalGerber = GetPrimitive(thermal);
                if (thermalGerber != null)
                {
                    figure.FigureItems.Add(thermalGerber);
                }
            }

            return figure;
        }

        private void AddClearFigure(GlobalPrimitive globalPrimitive, PdfFigure figure)
        {
            var clearFigure = GetPrimitive(globalPrimitive);

            if (clearFigure == null)
                return;

            SetColorRecursive(clearFigure, backgroundColor);

            figure.FigureItems.Add(clearFigure);
        }

        private void SetColorRecursive(PdfPrimitive primitive, XColor color)
        {
            primitive.Color = backgroundColor;
            if (primitive is IFilledPdfPrimitive filled)
                filled.FillColor = backgroundColor;

            if (primitive is PdfFigure gFig)
            {
                foreach (var fi in gFig.FigureItems)
                    SetColorRecursive(fi, color);
            }
        }

        private PdfPrimitive GetPolygonPrimitive(GlobalPolygonPrimitive poly)
        {
            return new PdfPolygonPrimitive
            {
                BorderWidth = ToPdfSize(poly.BorderWidth),
                Points = poly.Points.Select(p => ToPdfPoint(p)).ToList(),
                Color = darkColor,
                FillColor = GetFillColor(poly.IsFilled)
            };
        }

        private PdfPrimitive GetArcPrimitive(GlobalArcPrimitive item)
        {
            var center = Geometry2DHelper.GetArcCenter(item.StartPoint, item.EndPoint, item.SizeDiameter, item.SizeDiameter, item.SweepDirection);

            //arcs in pdf (from designators) are not generated properly
            //we'll make some improvements on fonts anyway, so will fix this later
            //this affects assembly drawings

            return new PdfArcPrimitive
            {
                StartPoint = ToPdfPoint(item.StartPoint),
                EndPoint = ToPdfPoint(item.EndPoint),
                Width = ToPdfSize(item.Width),
                SizeDiameter = ToPdfSize(item.SizeDiameter),
                //Center = ToPdfPoint(item.Center),
                Center = ToPdfPoint(center),
                IsLargeArc = item.IsLargeArc,

                RotationAngle = ToPdfRot(item.RotationAngle),
                SweepDirection = item.SweepDirection,
                //SweepDirection = item.IsMirrored ? (XSweepDirection)( 1 - (int)item.SweepDirection ) : item.SweepDirection,
            };
        }

        private PdfPrimitive GetTextPrimitive(GlobalTextPrimitive text)
        {
            return new PdfTextPrimitive
            {
                Position = ToPdfPoint(new XPoint(text.X, text.Y)),
                Rot = ToPdfRot(text.Rot),
                Text = text.Text,
                FontFamily = text.FontFamily,
                FontSize = text.FontSize,
                FontWeight = text.Bold ? 600 : 0,
            };
        }

        private PdfPrimitive GetFigurePrimitive(GlobalFigurePrimitive item)
        {
            var figure = new PdfFigure();

            foreach (var fi in item.FigureItems)
            {
                var fp = GetPrimitive(fi);
                if (fp != null)
                {
                    figure.FigureItems.Add(fp);
                }
            }

            return figure;
        }

        private PdfPrimitive GetRegionPrimitive(GlobalRegionPrimitive item)
        {
            var figure = new PdfFigure();

            foreach (GlobalPrimitive regionItem in item.Items)
            {
                var primitive = GetPrimitive(regionItem);
                figure.FigureItems.Add(primitive);
            }

            return figure;
        }
    }
}
