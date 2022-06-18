using System.Threading.Tasks;
using IDE.Core.Build;
using IDE.Core.Interfaces;
using IDE.Core.Model.GlobalRepresentation.Primitives;
using IDE.Core.Types.Media;

namespace IDE.Core.Gerber
{
    public class GerberLayerBuilder
    {
        public GerberLayerBuilder()
        {
            _geometryHelper = ServiceProvider.Resolve<IGeometryHelper>();
            _meshHelper = ServiceProvider.Resolve<IMeshHelper>();
        }

        private readonly IGeometryHelper _geometryHelper;
        private readonly IMeshHelper _meshHelper;

        private double boardOriginX;
        private double boardOriginY;

        //private IBoardDesigner _board;

        private List<ApertureDefinitionBase> apertures = new List<ApertureDefinitionBase>();

        #region BRD.Options

        private int formatStatementDigitsBeforeDecimal = 2;
        private int formatStatementDigitsAfterDecimal = 4;
        private Modes units = Modes.Millimeters;
        private bool drawBoardOutline = true;

        #endregion

        public Task<BuildResult> Build(IBoardDesigner board, BoardGlobalLayerOutput layer, string gerberFilePath)
        {
           // _board = board;

            LoadOptions(board);

            PrepareBoardOutline(board);
            var buildPlanPrimitives = BuildExecutionPlan(board, layer);

            CreateGerberOutput(gerberFilePath, buildPlanPrimitives);

            var result = new BuildResult { Success = true };
            result.OutputFiles.Add(gerberFilePath);

            return Task.FromResult(result);
        }

        private void LoadOptions(IBoardDesigner board)
        {
            var buildOptions = board.BuildOptions;
            formatStatementDigitsBeforeDecimal = buildOptions.GerberFormatBeforeDecimal;
            formatStatementDigitsAfterDecimal = buildOptions.GerberFormatAfterDecimal;
            units = buildOptions.GerberUnits == OutputUnits.mm ? Modes.Millimeters : Modes.Inches;
            drawBoardOutline = buildOptions.GerberPlotBoardOutlineOnAllLayers;
        }

        private void PrepareBoardOutline(IBoardDesigner board)
        {
            var brdOutline = board.BoardOutline;
            //calculate gerber board origin
            if (brdOutline == null)
                return;
            var upperLeft = brdOutline.StartPoint;
            var lowerRight = upperLeft;
            var startPoint = brdOutline.StartPoint;
            foreach (var item in brdOutline.Items)
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

            var boardRectangle = new XRect(upperLeft, lowerRight);
            boardOriginX = boardRectangle.BottomLeft.X;
            boardOriginY = boardRectangle.BottomLeft.Y;
        }

        private IList<GerberPrimitive> BuildExecutionPlan(IBoardDesigner board, BoardGlobalLayerOutput layer)
        {
            //we must have a build plan to optimize the gerber file size
            //for now this is dumb

            var buildPlanPrimitives = new List<GerberPrimitive>();

            //dark items
            var itemsToAdd = new List<GlobalPrimitive>();
            if (layer.AddItems != null)
            {
                if (drawBoardOutline)
                    itemsToAdd.Add(layer.BoardOutline);

                //add poured polygons first (so that it won't clear wanted traces)
                var pouredPolys = layer.AddItems.OfType<GlobalPouredPolygonPrimitive>().ToList();
                itemsToAdd.AddRange(pouredPolys);
                itemsToAdd.AddRange(layer.AddItems.Except(pouredPolys));

                AddToPlan(buildPlanPrimitives, itemsToAdd, Polarity.Dark);
            }

            //clear items
            if (layer.ExtractItems != null)
            {
                AddToPlan(buildPlanPrimitives, layer.ExtractItems, Polarity.Clear);
            }

            return buildPlanPrimitives;
        }

        private void CreateGerberOutput(string gerberPath, IList<GerberPrimitive> buildPlanPrimitives)
        {

            using (var gw = new Gerber274XWriter(gerberPath))
            {
                gw.FormatStatement(formatStatementDigitsBeforeDecimal, formatStatementDigitsAfterDecimal);
                gw.SetMode(units);

                CreateAperturesOutput(gw);
                CreatePrimitivesOutput(gw, buildPlanPrimitives);

                gw.EndOfProgram();
                gw.Close();
            }
        }

        void CreateAperturesOutput(Gerber274XWriter gw)
        {
            foreach (var aperture in apertures)
            {
                var adCode = 0;

                switch (aperture)
                {
                    case ApertureDefinitionCircle ac:
                        adCode = gw.AddApertureDefinitionCircle(ac.Diameter);
                        break;

                    case ApertureDefinitionRectangle rect:
                        adCode = gw.AddApertureDefinitionRectangle(rect.Width, rect.Height);
                        break;

                    case ApertureDefinitionRotatedRoundedRectangle arr:
                        adCode = gw.AddApertureDefinitionRotatedRoundedRectangle(arr);
                        break;
                }

                if (adCode != 0)
                    aperture.Number = adCode;
            }
        }

        void CreatePrimitivesOutput(Gerber274XWriter gw, IList<GerberPrimitive> buildPlanPrimitives)
        {
            //output from the plan
            foreach (var gp in buildPlanPrimitives)
            {
                gp.WriteGerber(gw);
            }
        }

        private void AddToPlan(List<GerberPrimitive> buildPlanPrimitives, IList<GlobalPrimitive> itemsToAdd, Polarity polarity)
        {
            foreach (var item in itemsToAdd)
            {
                var gbrPrimitive = GetGerberPrimitive(item);
                if (gbrPrimitive != null)
                {
                    HandleApertures(gbrPrimitive);

                    gbrPrimitive.Polarity = polarity;
                    buildPlanPrimitives.Add(gbrPrimitive);
                }
            }
        }

        void HandleApertures(GerberPrimitive primitive)
        {
            //dark and clearance apertures
            if (primitive == null)
                return;
            foreach (var aperture in primitive.GetApertures())
            {
                //if (aperture != null)
                {
                    var existingAperture = apertures.FirstOrDefault(a => a.Equals(aperture));
                    if (existingAperture == null)
                    {
                        aperture.Number = GetNextApertureId();
                        apertures.Add(aperture);
                    }
                    else
                    {
                        aperture.Number = existingAperture.Number;
                    }
                }
            }

            foreach (var clearanceAperture in primitive.GetClearanceAperture())
            {
                // if (clearanceAperture != null)
                {
                    var existingAperture = apertures.FirstOrDefault(a => a.Equals(clearanceAperture));
                    if (existingAperture == null)
                    {
                        clearanceAperture.Number = GetNextApertureId();
                        apertures.Add(clearanceAperture);
                    }
                    else
                    {
                        clearanceAperture.Number = existingAperture.Number;
                    }
                }
            }


        }

        int nextApertureId = 10;
        int GetNextApertureId()
        {
            var crt = nextApertureId;
            nextApertureId++;
            return crt;
        }

        private double ToGerberX(double x)//x is in mm
        {
            if (units == Modes.Inches)
                return ( x - boardOriginX ) / 25.4;

            return x - boardOriginX;
        }

        private double ToGerberY(double y)//y is in mm
        {
            if (units == Modes.Inches)
                return ( boardOriginY - y ) / 25.4;

            return boardOriginY - y;
        }

        private XPoint ToGerberPoint(XPoint p)
        {
            return new XPoint(ToGerberX(p.X), ToGerberY(p.Y));
        }

        private double ToGerberRot(double rot)
        {
            return -rot;
        }

        private GerberPrimitive GetGerberPrimitive(GlobalPrimitive item)
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

        private GerberPrimitive GetLinePrimitive(GlobalLinePrimitive line)
        {
            return new GerberLinePrimitive
            {
                StartPoint = ToGerberPoint(line.StartPoint),
                EndPoint = ToGerberPoint(line.EndPoint),
                Width = line.Width
            };
        }

        private GerberPrimitive GetPolylinePrimitive(GlobalPolylinePrimitive polyline)
        {
            return new GerberPolylinePrimitive
            {
                Width = polyline.Width,
                Points = polyline.Points.Select(p => ToGerberPoint(p)).ToList()
            };
        }

        private GerberPrimitive GetRectanglePrimitive(GlobalRectanglePrimitive rect)
        {
            return new GerberRectanglePrimitive
            {
                X = ToGerberX(rect.X),
                Y = ToGerberY(rect.Y),
                Width = rect.Width,
                Height = rect.Height,
                CornerRadius = rect.CornerRadius,
                Rot = ToGerberRot(rect.Rot)
            };
        }

        private GerberPrimitive GetCirclePrimitive(GlobalCirclePrimitive circle)
        {
            if (circle.IsFilled)
            {
                return new GerberCirclePrimitive
                {
                    X = ToGerberX(circle.X),
                    Y = ToGerberY(circle.Y),
                    Diameter = circle.Diameter + circle.BorderWidth
                };
            }
            else if (circle.BorderWidth > 0.0)
            {
                //big circle, substract hole not filled
                var figure = new GerberFigure();
                figure.FigureItems.Add(new GerberCirclePrimitive
                {
                    X = ToGerberX(circle.X),
                    Y = ToGerberY(circle.Y),
                    Diameter = circle.Diameter + circle.BorderWidth
                });
                figure.FigureItems.Add(new GerberCirclePrimitive
                {
                    X = ToGerberX(circle.X),
                    Y = ToGerberY(circle.Y),
                    Diameter = circle.Diameter - circle.BorderWidth,
                    Polarity = Polarity.Clear
                });

                return figure;
            }

            return null;
        }

        private GerberPrimitive GetHolePrimitive(GlobalHolePrimitive hole)
        {
            return new GerberCirclePrimitive
            {
                X = ToGerberX(hole.X),
                Y = ToGerberY(hole.Y),
                Diameter = hole.Drill,
                Polarity = Polarity.Clear
            };
        }

        private GerberPrimitive GetViaPrimitive(GlobalViaPrimitive item)
        {
            var figure = new GerberFigure();
            figure.FigureItems.Add(new GerberCirclePrimitive
            {
                X = ToGerberX(item.X),
                Y = ToGerberY(item.Y),
                Diameter = item.PadDiameter
            });

            if (item.Drill > 0.0)
            {
                figure.FigureItems.Add(new GerberCirclePrimitive
                {
                    X = ToGerberX(item.X),
                    Y = ToGerberY(item.Y),
                    Diameter = item.Drill,
                    Polarity = Polarity.Clear
                });
            }
            return figure;
        }

        private GerberPrimitive GetPolygonPrimitive(GlobalPouredPolygonPrimitive poly)
        {
            var figure = new GerberFigure();

            //fill polygon 
            var polyRegion = GetGerberPrimitive(poly.FillPrimitive);

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
                var thermalGerber = GetGerberPrimitive(thermal);
                if (thermalGerber != null)
                {
                    figure.FigureItems.Add(thermalGerber);
                }
            }

            return figure;
        }

        private void AddClearFigure(GlobalPrimitive globalPrimitive, GerberFigure figure)
        {
            var clearFigure = GetGerberPrimitive(globalPrimitive);

            if (clearFigure == null)
                return;

            SetPolarityRecursive(clearFigure, Polarity.Clear);

            figure.FigureItems.Add(clearFigure);
        }

        private void SetPolarityRecursive(GerberPrimitive primitive, Polarity polarity)
        {
            primitive.Polarity = polarity;
            if (primitive is GerberFigure gFig)
            {
                foreach (var fi in gFig.FigureItems)
                    SetPolarityRecursive(fi, polarity);
            }
        }

        private GerberPrimitive GetPolygonPrimitive(GlobalPolygonPrimitive poly)
        {
            var polyRegion = new GerberRegionPrimitive();
            for (int i = 1; i < poly.Points.Count; i++)
            {
                var sp = poly.Points[i - 1];
                var ep = poly.Points[i];

                polyRegion.RegionItems.Add(new GerberLinePrimitive
                {
                    StartPoint = ToGerberPoint(sp),
                    EndPoint = ToGerberPoint(ep),
                    //Width = item.BorderWidth
                });
            }
            //add start point to close
            polyRegion.RegionItems.Add(new GerberLinePrimitive
            {
                StartPoint = ToGerberPoint(poly.Points[poly.Points.Count - 1]),
                EndPoint = ToGerberPoint(poly.Points[0]),
                //Width = item.BorderWidth
            });

            return polyRegion;
        }

        private GerberPrimitive GetArcPrimitive(GlobalArcPrimitive arc)
        {
            return new GerberArcPrimitive
            {
                StartPoint = ToGerberPoint(arc.StartPoint),
                EndPoint = ToGerberPoint(arc.EndPoint),
                Width = arc.Width,
                SizeDiameter = arc.SizeDiameter,
                SweepDirection = arc.SweepDirection
            };
        }

        private GerberPrimitive GetTextPrimitive(GlobalTextPrimitive text)
        {
            var transform = new XTransformGroup();
            transform.Children.Add(new XRotateTransform(text.Rot));
            transform.Children.Add(new XTranslateTransform(text.X, text.Y));

            var figure = new GerberFigure();

            var outlineList = new List<List<XPoint[]>>();
            _geometryHelper.GetTextOutlines(text, outlineList);

            var darkRegions = new List<GerberRegionPrimitive>();
            var clearRegions = new List<GerberRegionPrimitive>();

            foreach (var outlines in outlineList)
            {
                var outerOutline = outlines.OrderBy(x => Geometry2DHelper.AreaOfSegment(x)).Last();

                for (int i = 0; i < outlines.Count; i++)
                {
                    var outline = outlines[i];
                    var isHole = i != outlines.Count - 1 && _meshHelper.IsPointInPolygon(outerOutline, outline[0]);

                    var polyRegion = GetRegionFromPoints(outline.Select(p => transform.Transform(new XPoint(p.X, p.Y))).ToArray());
                    if (isHole)
                    {
                        polyRegion.Polarity = Polarity.Clear;
                        clearRegions.Add(polyRegion);
                    }
                    else
                    {
                        darkRegions.Add(polyRegion);
                    }
                }
            }

            figure.FigureItems.AddRange(darkRegions);
            figure.FigureItems.AddRange(clearRegions);

            return figure;
        }

        GerberRegionPrimitive GetRegionFromPoints(XPoint[] points)
        {
            //create poly region
            var polyRegion = new GerberRegionPrimitive();
            for (int i = 1; i < points.Length; i++)
            {
                var sp = points[i - 1];
                var ep = points[i];

                polyRegion.RegionItems.Add(new GerberLinePrimitive
                {
                    StartPoint = ToGerberPoint(sp),
                    EndPoint = ToGerberPoint(ep),
                });
            }
            //add start point to close
            polyRegion.RegionItems.Add(new GerberLinePrimitive
            {
                StartPoint = ToGerberPoint(points[points.Length - 1]),
                EndPoint = ToGerberPoint(points[0]),
            });

            return polyRegion;
        }

        private GerberPrimitive GetFigurePrimitive(GlobalFigurePrimitive item)
        {
            var figure = new GerberFigure();

            foreach (var fi in item.FigureItems)
            {
                var fp = GetGerberPrimitive(fi);
                if (fp != null)
                {
                    figure.FigureItems.Add(fp);
                }
            }

            return figure;
        }

        private GerberPrimitive GetRegionPrimitive(GlobalRegionPrimitive item)
        {
            //we shouldn't convert to Gerber coordinates here
            var figure = new GerberFigure();

            foreach (GlobalPrimitive regionItem in item.Items)
            {
                var primitive = GetGerberPrimitive(regionItem);
                figure.FigureItems.Add(primitive);
            }

            return figure;
        }
    }
}
