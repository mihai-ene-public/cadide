using IDE.Core.Common.Variables;
using IDE.Core.Documents;
using IDE.Core.Interfaces;
using IDE.Core.Model.GlobalRepresentation;
using IDE.Core.Model.GlobalRepresentation.Primitives;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Gerber
{
    public class GerberLayer
    {
        //info needed:
        //  - board, project
        //  - output options

        public GerberLayer(IBoardDesigner boardModel, List<ICanvasItem> addItems, List<ICanvasItem> extractItems = null)
        {
            itemsToAdd = addItems;
            itemsToExtract = extractItems;

            board = boardModel;

            GeometryHelper = ServiceProvider.Resolve<IGeometryHelper>();
            meshHelper = ServiceProvider.Resolve<IMeshHelper>();

            LoadOptions();
        }



        IBoardDesigner board;

        List<ICanvasItem> itemsToAdd;
        List<ICanvasItem> itemsToExtract;

        List<GerberPrimitive> addPrimitives = new List<GerberPrimitive>();
        List<GerberPrimitive> clearPrimitives = new List<GerberPrimitive>();
        List<GerberPrimitive> buildPlanPrimitives = new List<GerberPrimitive>();
        List<ApertureDefinitionBase> apertures = new List<ApertureDefinitionBase>();
        List<GerberPrimitive> boardOutlinePrimitives = new List<GerberPrimitive>();

        IGeometryHelper GeometryHelper;
        IMeshHelper meshHelper;


        ILayerDesignerItem _layer = null;

        XRect boardRectangle;
        double boardOriginX;
        double boardOriginY;

        public List<string> OutputFiles { get; private set; } = new List<string>();

        #region BRD.Options

        int formatStatementDigitsBeforeDecimal = 2;
        int formatStatementDigitsAfterDecimal = 4;
        Modes units = Modes.Millimeters;
        bool drawBoardOutline = true;

        #endregion

        public void PrepareLayer(ILayerDesignerItem layer)
        {
            _layer = layer;
            addPrimitives.Clear();
            clearPrimitives.Clear();
            apertures.Clear();

            PrepareBoardOutline();
            BuildExecutionPlan();

        }

        public Task BuildLayer()
        {
            return Task.Run(() =>
                    CreateGerberOutput()
            );
        }

        void LoadOptions()
        {
            formatStatementDigitsBeforeDecimal = board.BuildOptions.GerberFormatBeforeDecimal;
            formatStatementDigitsAfterDecimal = board.BuildOptions.GerberFormatAfterDecimal;
            units = board.BuildOptions.GerberUnits == OutputUnits.mm ? Modes.Millimeters : Modes.Inches;
            drawBoardOutline = board.BuildOptions.GerberPlotBoardOutlineOnAllLayers;
        }

        void PrepareBoardOutline()
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

            boardRectangle = new XRect(upperLeft, lowerRight);
            boardOriginX = boardRectangle.BottomLeft.X;
            boardOriginY = boardRectangle.BottomLeft.Y;
        }

        double ToGerberX(double x)//x is in mm
        {
            if (units == Modes.Inches)
                return ( x - boardOriginX ) / 25.4;

            return x - boardOriginX;
        }

        double ToGerberY(double y)//y is in mm
        {
            if (units == Modes.Inches)
                return ( boardOriginY - y ) / 25.4;

            return boardOriginY - y;
        }

        XPoint ToGerberPoint(XPoint p)
        {
            return new XPoint(ToGerberX(p.X), ToGerberY(p.Y));
        }

        double ToGerberRot(double rot)
        {
            return -rot;
        }

        void BuildExecutionPlan()
        {
            //we must have a build plan to optimize the gerber file size
            //for now this is dumb

            //dark items
            if (itemsToAdd != null)
            {
                if (drawBoardOutline)
                    itemsToAdd.Add(board.BoardOutline);

                foreach (var item in itemsToAdd)
                {
                    var gbrPrimitive = GetGerberPrimitive(item);
                    if (gbrPrimitive != null)
                    {
                        HandleApertures(gbrPrimitive);

                        gbrPrimitive.Polarity = Polarity.Dark;
                        addPrimitives.Add(gbrPrimitive);
                    }
                }
                //add to plan
                buildPlanPrimitives.AddRange(addPrimitives);
            }


            //clear items
            if (itemsToExtract != null)
            {
                foreach (var item in itemsToExtract)
                {
                    var gbrPrimitive = GetGerberPrimitive(item);
                    if (gbrPrimitive != null)
                    {
                        HandleApertures(gbrPrimitive);
                        gbrPrimitive.Polarity = Polarity.Clear;
                        clearPrimitives.Add(gbrPrimitive);
                    }
                }

                //add to plan
                buildPlanPrimitives.AddRange(clearPrimitives);
            }

        }

        string GetExtension(string extension)
        {
            if (extension.StartsWith("."))
                return extension;
            return "." + extension.ToUpper();
        }
        void CreateGerberOutput()
        {
            //project/!Output
            //save output
            var f = board as IFileBaseViewModel;
            if (f == null)
                return;
            var project = f.ProjectNode;
            if (project == null)
                return;
            var savePath = Path.Combine(project.GetItemFolderFullPath(), "!Output");//folder
            Directory.CreateDirectory(savePath);
            var boardName = Path.GetFileNameWithoutExtension(f.FilePath);
            var layerName = _layer.LayerName;

            var gerberFileName = $"{boardName}-{layerName}";
            if (!string.IsNullOrWhiteSpace(_layer.GerberFileName))
            {
                var variables = new VariablesContext();
                variables.Add(new Variable("boardName", boardName));
                variables.Add(new Variable("layerName", layerName));

                gerberFileName = variables.Replace(_layer.GerberFileName);
            }

            savePath = Path.Combine(savePath, $"{gerberFileName}{GetExtension(_layer.GerberExtension)}");
            using (var gw = new Gerber274XWriter(savePath))
            {
                gw.FormatStatement(formatStatementDigitsBeforeDecimal, formatStatementDigitsAfterDecimal);
                gw.SetMode(units);

                CreateAperturesOutput(gw);
                CreatePrimitivesOutput(gw);

                gw.EndOfProgram();
                gw.Close();
            }
            OutputFiles.Clear();
            OutputFiles.Add(savePath);
        }

        void CreateAperturesOutput(Gerber274XWriter gw)
        {
            foreach (var aperture in apertures)
            {
                var adCode = 0;
                if (aperture is ApertureDefinitionCircle)
                {
                    var ac = aperture as ApertureDefinitionCircle;
                    adCode = gw.AddApertureDefinitionCircle(ac.Diameter);

                }
                else if (aperture is ApertureDefinitionRectangle)
                {
                    var ac = aperture as ApertureDefinitionRectangle;
                    adCode = gw.AddApertureDefinitionRectangle(ac.Width, ac.Height);
                }
                else if (aperture is ApertureDefinitionRotatedRoundedRectangle)
                {
                    var ac = aperture as ApertureDefinitionRotatedRoundedRectangle;
                    adCode = gw.AddApertureDefinitionRotatedRoundedRectangle(ac);//ac.Width, ac.Height, ac.Rot, false);
                }

                if (adCode != 0)
                    aperture.Number = adCode;
            }
        }

        void CreatePrimitivesOutput(Gerber274XWriter gw)
        {
            //output from the plan
            foreach (var gp in buildPlanPrimitives)
            {
                gp.WriteGerber(gw);
            }
        }

        int nextApertureId = 10;
        int GetNextApertureId()
        {
            var crt = nextApertureId;
            nextApertureId++;
            return crt;
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

        GerberPrimitive GetGerberPrimitive(ICanvasItem item, bool transformed = true, double clearance = 0.0)
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
                        //return GetPolygonPrimitive(poly, clearance);
                        return GetPolygonPrimitive(poly);

                    case IArcCanvasItem arc:
                        return GetArcPrimitive(arc, transformed, clearance);

                    case ITextCanvasItem text:
                        return GetTextPrimitive(text);

                    case ITextMonoLineCanvasItem textMono:
                        return GetTextPrimitive(textMono);

                    case IRegionCanvasItem region:
                        return GetRegionPrimitive(region);

                    case IPlaneBoardCanvasItem plane:
                        return GetPlanePrimitive(plane);
                }

            }

            return null;
        }



        GerberPrimitive GetGerberPrimitive(IRegionItem item, XPoint startPoint, double width)
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

        private GerberPrimitive GetGerberPrimitive(GlobalPrimitive globalPrimitive)
        {
            if (globalPrimitive == null)
                return null;

            switch (globalPrimitive)
            {
                case GlobalPolygonPrimitive poly:
                    return GetPolygonPrimitive(poly);

                case GlobalLinePrimitive line:
                    return GetLinePrimitive(line);

                case GlobalPolylinePrimitive polyline:
                    return GetPolylinePrimitive(polyline);

                case GlobalViaPrimitive via:
                    return GetViaPrimitive(via);

                case GlobalArcPrimitive arc:
                    return GetArcPrimitive(arc);

                case GlobalRectanglePrimitive rect:
                    return GetRectanglePrimitive(rect);

                case GlobalCirclePrimitive circle:
                    return GetCirclePrimitive(circle);

                case GlobalHolePrimitive hole:
                    return GetHolePrimitive(hole);

                case GlobalFigure figure:
                    return GetFigurePrimitive(figure);

                case GlobalTextPrimitive text:
                    return GetTextPrimitive(text);
            }

            return null;
        }

        XPoint GetPointTransform(XTransform transform, double x, double y)
        {
            var p = new XPoint(x, y);
            p = transform.Transform(p);
            return p;
        }
        XPoint GetPointTransform(XTransform transform, XPoint p)
        {
            p = transform.Transform(p);
            return p;
        }

        double GetRotationTransform(XTransform transform)
        {
            return Geometry2DHelper.GetRotationAngleFromMatrix(transform.Value);
        }

        private GerberPrimitive GetFigurePrimitive(GlobalFigure item)
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

        GerberPrimitive GetCirclePrimitive(ICircleCanvasItem item, double clearance = 0.0)
        {
            // var position = GetPointTransform(item.GetTransform(), item.X, item.Y);
            var t = item.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);

            if (item.IsFilled)
            {
                return new GerberCirclePrimitive
                {
                    X = ToGerberX(position.X),
                    Y = ToGerberY(position.Y),
                    Diameter = item.Diameter + item.BorderWidth + 2 * clearance
                };
            }
            else if (item.BorderWidth > 0.0)
            {
                //big circle, substract hole not filled
                var figure = new GerberFigure();
                figure.FigureItems.Add(new GerberCirclePrimitive
                {
                    X = ToGerberX(position.X),
                    Y = ToGerberY(position.Y),
                    Diameter = item.Diameter + item.BorderWidth + 2 * clearance
                });
                figure.FigureItems.Add(new GerberCirclePrimitive
                {
                    X = ToGerberX(position.X),
                    Y = ToGerberY(position.Y),
                    Diameter = item.Diameter - item.BorderWidth,
                    Polarity = Polarity.Clear
                });

                return figure;
            }

            return null;
        }

        private GerberPrimitive GetCirclePrimitive(GlobalCirclePrimitive item)
        {
            if (item.IsFilled)
            {
                return new GerberCirclePrimitive
                {
                    X = ToGerberX(item.X),
                    Y = ToGerberY(item.Y),
                    Diameter = item.Diameter + item.BorderWidth
                };
            }
            else if (item.BorderWidth > 0.0)
            {
                //big circle, substract hole not filled
                var figure = new GerberFigure();
                figure.FigureItems.Add(new GerberCirclePrimitive
                {
                    X = ToGerberX(item.X),
                    Y = ToGerberY(item.Y),
                    Diameter = item.Diameter + item.BorderWidth
                });
                figure.FigureItems.Add(new GerberCirclePrimitive
                {
                    X = ToGerberX(item.X),
                    Y = ToGerberY(item.Y),
                    Diameter = item.Diameter - item.BorderWidth,
                    Polarity = Polarity.Clear
                });

                return figure;
            }

            return null;
        }

        GerberPrimitive GetHolePrimitive(IHoleCanvasItem item, double clearance = 0)
        {
            var t = item.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);

            if (item.DrillType == DrillType.Drill)
            {
                return new GerberCirclePrimitive
                {
                    X = ToGerberX(position.X),
                    Y = ToGerberY(position.Y),
                    Diameter = item.Drill + 2 * clearance
                };
            }
            else
            {
                var placement = FootprintPlacement.Top;
                var fp = item.ParentObject as IFootprintBoardCanvasItem;
                if (fp != null)
                    placement = fp.Placement;

                var rot = GetWorldRotation(t, placement);

                return new GerberRectanglePrimitive
                {
                    X = ToGerberX(position.X),
                    Y = ToGerberY(position.Y),
                    Width = item.Drill + 2 * clearance,
                    Height = item.Height + 2 * clearance,
                    CornerRadius = item.Drill * 0.5 + clearance,
                    Rot = ToGerberRot(rot)
                };
            }
        }

        private GerberPrimitive GetHolePrimitive(GlobalHolePrimitive item)
        {
            return new GerberCirclePrimitive
            {
                X = ToGerberX(item.X),
                Y = ToGerberY(item.Y),
                Diameter = item.Drill,
                Polarity = Polarity.Clear
            };
        }

        GerberPrimitive GetViaPrimitive(IViaCanvasItem item, double clearance = 0)
        {
            var position = new XPoint(item.X, item.Y);//GetPointTransform(item.GetTransform(), item.X, item.Y);
            var figure = new GerberFigure();
            figure.FigureItems.Add(new GerberCirclePrimitive
            {
                X = ToGerberX(position.X),
                Y = ToGerberY(position.Y),
                Diameter = item.Diameter + 2 * clearance
            });

            if (item.Drill > 0.0)
            {
                figure.FigureItems.Add(new GerberCirclePrimitive
                {
                    X = ToGerberX(position.X),
                    Y = ToGerberY(position.Y),
                    Diameter = item.Drill,
                    Polarity = Polarity.Clear
                });
            }
            return figure;
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

        double GetWorldRotation(/*double rot, */XTransform transform, FootprintPlacement placement)
        {
            var rotAngle = GetRotationTransform(transform);
            var myRot = rotAngle;

            if (placement == FootprintPlacement.Bottom)
            {
                myRot = 180.0d - myRot;
            }

            return myRot;
        }

        GerberPrimitive GetPadPrimitive(IPadCanvasItem item, double clearance = 0)
        {
            var t = item.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);
            var placement = FootprintPlacement.Top;
            if (item.ParentObject is IFootprintBoardCanvasItem fp)
                placement = fp.Placement;
            var rot = GetWorldRotation(t, placement);

            return new GerberRectanglePrimitive
            {
                X = ToGerberX(position.X),
                Y = ToGerberY(position.Y),
                Width = item.Width + 2 * clearance,
                Height = item.Height + 2 * clearance,
                CornerRadius = item.CornerRadius + clearance,
                Rot = ToGerberRot(rot)
            };
        }

        GerberPrimitive GetPolygonPrimitive(IPolygonBoardCanvasItem item, double clearance)
        {
            var figure = new GerberFigure();
            var t = item.GetTransform();


            //border
            var borderWidth = item.BorderWidth + 2 * clearance;
            if (borderWidth > 0.0)
            {
                for (int i = 1; i < item.PolygonPoints.Count; i++)
                {
                    var sp = GetPointTransform(t, item.PolygonPoints[i - 1]);
                    var ep = GetPointTransform(t, item.PolygonPoints[i]);

                    figure.FigureItems.Add(new GerberLinePrimitive
                    {
                        StartPoint = ToGerberPoint(sp),
                        EndPoint = ToGerberPoint(ep),
                        Width = borderWidth
                    });
                }
                //add start point to close
                figure.FigureItems.Add(new GerberLinePrimitive
                {
                    StartPoint = ToGerberPoint(GetPointTransform(t, item.PolygonPoints[item.PolygonPoints.Count - 1])),
                    EndPoint = ToGerberPoint(GetPointTransform(t, item.PolygonPoints[0])),
                    Width = item.BorderWidth + 2 * clearance
                });
            }
            if (item.IsFilled)
            {
                var polyRegion = new GerberRegionPrimitive();
                for (int i = 1; i < item.PolygonPoints.Count; i++)
                {
                    var sp = GetPointTransform(t, item.PolygonPoints[i - 1]);
                    var ep = GetPointTransform(t, item.PolygonPoints[i]);

                    polyRegion.RegionItems.Add(new GerberLinePrimitive
                    {
                        StartPoint = ToGerberPoint(sp),
                        EndPoint = ToGerberPoint(ep),
                        Width = item.BorderWidth
                    });
                }
                //add start point to close
                polyRegion.RegionItems.Add(new GerberLinePrimitive
                {
                    StartPoint = ToGerberPoint(GetPointTransform(t, item.PolygonPoints[item.PolygonPoints.Count - 1])),
                    EndPoint = ToGerberPoint(GetPointTransform(t, item.PolygonPoints[0])),
                    Width = item.BorderWidth
                });
                figure.FigureItems.Add(polyRegion);

                //substract the other geometries on the same layer (polygons and tracks with its clearances)
                var excludedItems = item.GetExcludedItems();

                var thermalPrimitives = new List<GerberPrimitive>();

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
                            //BuildThermalsForPad(inflatedItem, thermalItems);
                            padsOfThisSignal.Add(excludedItem);
                            if (item.GenerateThermals)
                            {
                                var thermal = GetThermalsForPad(excludedItem, item.ThermalWidth);
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

        private GerberPrimitive GetPolygonPrimitive(GlobalPolygonPrimitive globalPoly)
        {
            var polyRegion = new GerberRegionPrimitive();
            for (int i = 1; i < globalPoly.Points.Count; i++)
            {
                var sp = globalPoly.Points[i - 1];
                var ep = globalPoly.Points[i];

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
                StartPoint = ToGerberPoint(globalPoly.Points[globalPoly.Points.Count - 1]),
                EndPoint = ToGerberPoint(globalPoly.Points[0]),
                //Width = item.BorderWidth
            });

            return polyRegion;
        }

        private GerberPrimitive GetPolygonPrimitive(GlobalPouredPolygonPrimitive globalPoly)
        {
            var figure = new GerberFigure();

            //fill polygon
            var polyRegion = GetGerberPrimitive(globalPoly.FillPrimitive);

            if (polyRegion != null)
                figure.FigureItems.Add(polyRegion);

            //clear primitives
            foreach (var clearPrimitive in globalPoly.RemovePrimitives)
            {
                AddClearFigure(clearPrimitive, figure);
            }

            //thermals
            foreach (var thermal in globalPoly.Thermals)
            {
                var thermalGerber = GetGerberPrimitive(thermal);
                if (thermalGerber != null)
                {
                    figure.FigureItems.Add(thermalGerber);
                }
            }

            return figure;
        }
        private GerberPrimitive GetPolygonPrimitive(IPolygonBoardCanvasItem item)
        {
            var proc = new GlobalPrimitivePourProcessor();
            var globalPrimitive = proc.GetPrimitive(item) as GlobalPouredPolygonPrimitive;

            var primitive = GetPolygonPrimitive(globalPrimitive);
            return primitive;
        }
        void AddClearFigure(IItemWithClearance clearanceItem, GerberFigure figure)
        {
            var clearFigure = GetGerberPrimitive(clearanceItem.CanvasItem, true, clearanceItem.Clearance);
            clearFigure.Polarity = Polarity.Clear;
            if (clearFigure is GerberFigure gFig)
            {
                foreach (var fi in gFig.FigureItems)
                    fi.Polarity = Polarity.Clear;
            }

            figure.FigureItems.Add(clearFigure);
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

        GerberPrimitive GetThermalsForPad(IItemWithClearance padItem, double thermalWidth)
        {
            var pad = padItem.CanvasItem as IPadCanvasItem;
            var clearance = padItem.Clearance;

            var figure = new GerberFigure();

            //2 rectangles in cross rotated by pad and in center of the pad

            var t = pad.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);
            var placement = FootprintPlacement.Top;
            if (pad.ParentObject is IFootprintBoardCanvasItem fp)
                placement = fp.Placement;
            var rot = GetWorldRotation(t, placement);

            //we add some tolerance so that when thermal geometry intersects with the final geometry will create some outlines
            const double tolerance = 0.01d;

            ////horizontal
            //var tFigure = new GerberRectanglePrimitive
            //{
            //    X = ToGerberX(position.X),
            //    Y = ToGerberY(position.Y),
            //    Width = pad.Width + 2 * clearance,
            //    Height = thermalWidth,
            //    Rot = ToGerberRot(rot)
            //};
            //figure.FigureItems.Add(tFigure);

            ////vertical
            //tFigure = new GerberRectanglePrimitive
            //{
            //    X = ToGerberX(position.X),
            //    Y = ToGerberY(position.Y),
            //    Width = thermalWidth,
            //    Height = pad.Height + 2 * clearance,
            //    Rot = ToGerberRot(rot)
            //};
            //figure.FigureItems.Add(tFigure);


            //east
            var posEast = new XPoint(0.5 * ( 0.5 * pad.Width + clearance ), 0);
            posEast = t.Transform(posEast);
            figure.FigureItems.Add(new GerberRectanglePrimitive
            {
                X = ToGerberX(posEast.X),
                Y = ToGerberY(posEast.Y),
                Width = 0.5 * pad.Width + clearance + tolerance,
                Height = thermalWidth,
                Rot = ToGerberRot(rot),
                CornerRadius = 0
            });
            //west
            var posWest = new XPoint(-0.5 * ( 0.5 * pad.Width + clearance ), 0);
            posWest = t.Transform(posWest);
            figure.FigureItems.Add(new GerberRectanglePrimitive
            {
                X = ToGerberX(posWest.X),
                Y = ToGerberY(posWest.Y),
                Width = 0.5 * pad.Width + clearance + tolerance,
                Height = thermalWidth,
                Rot = ToGerberRot(rot),
                CornerRadius = 0
            });

            //north
            var posNorth = new XPoint(0, -0.5 * ( 0.5 * pad.Height + clearance ));
            posNorth = t.Transform(posNorth);
            figure.FigureItems.Add(new GerberRectanglePrimitive
            {
                X = ToGerberX(posNorth.X),
                Y = ToGerberY(posNorth.Y),
                Width = thermalWidth,
                Height = 0.5 * pad.Height + clearance + tolerance,
                Rot = ToGerberRot(rot),
                CornerRadius = 0
            });
            //south
            var posSouth = new XPoint(0, 0.5 * ( 0.5 * pad.Height + clearance ));
            posSouth = t.Transform(posSouth);
            figure.FigureItems.Add(new GerberRectanglePrimitive
            {
                X = ToGerberX(posSouth.X),
                Y = ToGerberY(posSouth.Y),
                Width = thermalWidth,
                Height = 0.5 * pad.Height + clearance + tolerance,
                Rot = ToGerberRot(rot),
                CornerRadius = 0
            });

            return figure;
        }

        GerberPrimitive GetPolylinePrimitive(IPolylineCanvasItem item, double clearance = 0)
        {
            //not needed to be transformed...
            return new GerberPolylinePrimitive
            {
                Width = item.Width + 2 * clearance,
                Points = item.Points.Select(p => ToGerberPoint(p)).ToList()
            };
        }

        private GerberPrimitive GetPolylinePrimitive(GlobalPolylinePrimitive item)
        {
            //not needed to be transformed...
            return new GerberPolylinePrimitive
            {
                Width = item.Width,
                Points = item.Points.Select(p => ToGerberPoint(p)).ToList()
            };
        }

        GerberPrimitive GetArcPrimitive(IArcCanvasItem item, bool transformed = true, double clearance = 0)
        {
            var sp = new XPoint(item.StartPointX, item.StartPointY);
            var ep = new XPoint(item.EndPointX, item.EndPointY);
            if (transformed)
            {
                var t = item.GetTransform();
                sp = GetPointTransform(t, sp);
                ep = GetPointTransform(t, ep);
            }

            return new GerberArcPrimitive
            {
                StartPoint = ToGerberPoint(sp),
                EndPoint = ToGerberPoint(ep),
                Width = item.BorderWidth + 2 * clearance,
                SizeDiameter = 2 * item.Radius,
                SweepDirection = item.IsMirrored() ? (XSweepDirection)( 1 - (int)item.SweepDirection ) : item.SweepDirection
            };
        }

        private GerberPrimitive GetArcPrimitive(GlobalArcPrimitive item)
        {
            return new GerberArcPrimitive
            {
                StartPoint = ToGerberPoint(item.StartPoint),
                EndPoint = ToGerberPoint(item.EndPoint),
                Width = item.Width,
                SizeDiameter = item.SizeDiameter,
                SweepDirection = item.SweepDirection
            };
        }

        GerberPrimitive GetArcPrimitive(IArcRegionItem item, XPoint startPoint, double width)
        {
            return new GerberArcPrimitive
            {
                StartPoint = ToGerberPoint(startPoint),
                EndPoint = ToGerberPoint(item.EndPoint),
                Width = width,
                SizeDiameter = 2 * item.SizeDiameter,
                SweepDirection = item.SweepDirection
            };
        }

        GerberPrimitive GetTextPrimitive(ITextCanvasItem text)
        {
            var t = text.GetTransform();
            
            var figure = new GerberFigure();

            var outlineList = new List<List<XPoint[]>>();
            GeometryHelper.GetTextOutlines(text, outlineList);

            var darkRegions = new List<GerberRegionPrimitive>();
            var clearRegions = new List<GerberRegionPrimitive>();

            foreach (var outlines in outlineList)
            {
                var outerOutline = outlines.OrderBy(x => Geometry2DHelper.AreaOfSegment(x)).Last();

                for (int i = 0; i < outlines.Count; i++)
                {
                    var outline = outlines[i];
                    var isHole = i != outlines.Count - 1 && meshHelper.IsPointInPolygon(outerOutline, outline[0]);

                    var polyRegion = GetRegionFromPoints(outline.Select(p => GetPointTransform(t, new XPoint(p.X, p.Y))).ToArray());
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

        private GerberPrimitive GetTextPrimitive(GlobalTextPrimitive text)
        {
            var figure = new GerberFigure();

            //todo: uncomment
            //var outlineList = new List<List<XPoint[]>>();
            //GeometryHelper.GetTextOutlines(text, outlineList);

            //var darkRegions = new List<GerberRegionPrimitive>();
            //var clearRegions = new List<GerberRegionPrimitive>();

            //foreach (var outlines in outlineList)
            //{
            //    var outerOutline = outlines.OrderBy(x => Geometry2DHelper.AreaOfSegment(x)).Last();

            //    for (int i = 0; i < outlines.Count; i++)
            //    {
            //        var outline = outlines[i];
            //        var isHole = i != outlines.Count - 1 && meshHelper.IsPointInPolygon(outerOutline, outline[0]);

            //        var polyRegion = GetRegionFromPoints(outline.Select(p => GetPointTransform(t, new XPoint(p.X, p.Y))).ToArray());
            //        if (isHole)
            //        {
            //            polyRegion.Polarity = Polarity.Clear;
            //            clearRegions.Add(polyRegion);
            //        }
            //        else
            //        {
            //            darkRegions.Add(polyRegion);
            //        }
            //    }
            //}

            //figure.FigureItems.AddRange(darkRegions);
            //figure.FigureItems.AddRange(clearRegions);

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
                    //Width = item.BorderWidth
                });
            }
            ////add start point to close
            polyRegion.RegionItems.Add(new GerberLinePrimitive
            {
                StartPoint = ToGerberPoint(points[points.Length - 1]),
                EndPoint = ToGerberPoint(points[0]),
                //  Width = item.BorderWidth
            });

            return polyRegion;
        }

        GerberPrimitive GetRegionPrimitive(IRegionCanvasItem item)
        {
            //we shouldn't convert to Gerber coordinates here
            var figure = new GerberFigure();

            var startPoint = item.StartPoint;

            foreach (var regionItem in item.Items)
            {
                var primitive = GetGerberPrimitive(regionItem, startPoint, item.Width);
                startPoint = regionItem.EndPoint;
                figure.FigureItems.Add(primitive);
            }

            return figure;
        }

        GerberPrimitive GetPlanePrimitive(IPlaneBoardCanvasItem plane)
        {
            var figure = new GerberFigure();

            var fillRegion = new GerberRegionPrimitive();

            dynamic brdItem = plane;
            var brd = brdItem.LayerDocument as IBoardDesigner;
            if (brd != null)
            {
                var brdOutline = brd.BoardOutline;


                var startPoint = brdOutline.StartPoint;

                foreach (var regionItem in brdOutline.Items)
                {
                    var primitive = GetGerberPrimitive(regionItem, startPoint, brdOutline.Width);
                    startPoint = regionItem.EndPoint;
                    fillRegion.RegionItems.Add(primitive);
                }

                figure.FigureItems.Add(fillRegion);

                //substract the other geometries on the same layer (polygons and tracks with its clearances)
                var excludedItems = plane.GetExcludedItems();

                var thermalPrimitives = new List<GerberPrimitive>();

                var padsOfThisSignal = new List<IItemWithClearance>();
                var restOfItems = new List<IItemWithClearance>();

                foreach (var inflatedItem in excludedItems)
                {
                    var clearanceItem = inflatedItem.CanvasItem;

                    if (clearanceItem is IPadCanvasItem pad)
                    {
                        var polySignal = plane as ISignalPrimitiveCanvasItem;
                        if (polySignal.Signal != null && pad.Signal != null &&
                            polySignal.Signal.Name == pad.Signal.Name)
                        {
                            //BuildThermalsForPad(inflatedItem, thermalItems);
                            padsOfThisSignal.Add(inflatedItem);
                            if (plane.GenerateThermals)
                            {
                                var thermal = GetThermalsForPad(inflatedItem, plane.ThermalWidth);
                                thermalPrimitives.Add(thermal);
                            }
                        }
                        else
                        {
                            restOfItems.Add(inflatedItem);
                        }
                    }
                    else
                    {
                        restOfItems.Add(inflatedItem);
                    }
                }

                foreach (var clearedpad in padsOfThisSignal)
                {
                    AddClearFigure(clearedpad, figure);
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

        GerberPrimitive GetTextPrimitive(ITextMonoLineCanvasItem item)
        {
            var t = item.GetTransform();

            var figure = new GerberFigure();
            double lx = 0;
            foreach (var letter in item.LetterItems)
            {
                var local = new XTranslateTransform(lx, 0);
                foreach (var li in letter.Items.Select(l => (ISelectableItem)l.Clone()))
                {
                    li.TransformBy(local.Value * t.Value);

                    var primitive = GetGerberPrimitive(li, false);
                    figure.FigureItems.Add(primitive);
                }

                lx += item.FontSize;
            }


            return figure;
        }

        GerberPrimitive GetRectanglePrimitive(IRectangleCanvasItem item, double clearance = 0)
        {
            //var t = new TransformGroup();
            //t.Children.Add(new RotateTransform(item.Rot));
            //t.Children.Add(new TranslateTransform(item.X, item.Y));
            var t = item.GetTransform();



            if (item.CornerRadius > 0.0d)
            {
                var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);
                var placement = FootprintPlacement.Top;
                if (item.ParentObject is IFootprintBoardCanvasItem fp)
                    placement = fp.Placement;
                var rot = GetWorldRotation(t, placement);


                return new GerberRectanglePrimitive
                {
                    X = ToGerberX(position.X),
                    Y = ToGerberY(position.Y),
                    Width = item.Width,
                    Height = item.Height,
                    CornerRadius = item.CornerRadius,
                    Rot = ToGerberRot(rot)
                };
            }
            else
            {
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


                var figure = new GerberFigure();

                //border
                if (item.BorderWidth > 0.0)
                {
                    for (int i = 1; i < points.Count; i++)
                    {
                        var sp = points[i - 1];
                        var ep = points[i];

                        figure.FigureItems.Add(new GerberLinePrimitive
                        {
                            StartPoint = ToGerberPoint(sp),
                            EndPoint = ToGerberPoint(ep),
                            Width = item.BorderWidth,
                            // LineCap = GerberLineCap.Square
                        });
                    }
                    //add start point to close
                    figure.FigureItems.Add(new GerberLinePrimitive
                    {
                        StartPoint = ToGerberPoint(points[points.Count - 1]),
                        EndPoint = ToGerberPoint(points[0]),
                        Width = item.BorderWidth,
                        //LineCap = GerberLineCap.Square
                    });

                    ////todo: rotation
                    //figure.FigureItems.Add(new GerberLinePrimitive
                    //{
                    //    StartPoint = ToGerberPoint(new Point(item.X, item.Y)),
                    //    EndPoint = ToGerberPoint(new Point(item.X, item.Y + item.Height)),
                    //    Width = item.BorderWidth
                    //});
                    //figure.FigureItems.Add(new GerberLinePrimitive
                    //{
                    //    StartPoint = ToGerberPoint(new Point(item.X, item.Y + item.Height)),
                    //    EndPoint = ToGerberPoint(new Point(item.X + item.Width, item.Y + item.Height)),
                    //    Width = item.BorderWidth
                    //});
                    //figure.FigureItems.Add(new GerberLinePrimitive
                    //{
                    //    StartPoint = ToGerberPoint(new Point(item.X + item.Width, item.Y + item.Height)),
                    //    EndPoint = ToGerberPoint(new Point(item.X + item.Width, item.Y)),
                    //    Width = item.BorderWidth
                    //});
                    //figure.FigureItems.Add(new GerberLinePrimitive
                    //{
                    //    StartPoint = ToGerberPoint(new Point(item.X + item.Width, item.Y)),
                    //    EndPoint = ToGerberPoint(new Point(item.X, item.Y)),
                    //    Width = item.BorderWidth
                    //});
                }
                if (item.IsFilled)
                {
                    var polyRegion = new GerberRegionPrimitive();
                    for (int i = 1; i < points.Count; i++)
                    {
                        var sp = points[i - 1];
                        var ep = points[i];

                        polyRegion.RegionItems.Add(new GerberLinePrimitive
                        {
                            StartPoint = ToGerberPoint(sp),
                            EndPoint = ToGerberPoint(ep),
                            Width = item.BorderWidth
                        });
                    }
                    //add start point to close
                    polyRegion.RegionItems.Add(new GerberLinePrimitive
                    {
                        StartPoint = ToGerberPoint(points[points.Count - 1]),
                        EndPoint = ToGerberPoint(points[0]),
                        Width = item.BorderWidth
                    });
                    figure.FigureItems.Add(polyRegion);

                    ////an aperture is centered
                    //figure.FigureItems.Add(new GerberRectanglePrimitive
                    //{
                    //    X = ToGerberX(item.X),
                    //    Y = ToGerberY(item.Y + item.Height),
                    //    Width = item.Width,
                    //    Height = item.Height,
                    //    Rot = item.Rot
                    //});
                }

                return figure;
            }
        }
        private GerberPrimitive GetRectanglePrimitive(GlobalRectanglePrimitive item)
        {
            return new GerberRectanglePrimitive
            {
                X = ToGerberX(item.X),
                Y = ToGerberY(item.Y),
                Width = item.Width,
                Height = item.Height,
                CornerRadius = item.CornerRadius,
                Rot = ToGerberRot(item.Rot)
            };

            //if (item.CornerRadius > 0.0d)
            //{
            //    return new GerberRectanglePrimitive
            //    {
            //        X = ToGerberX(item.X),
            //        Y = ToGerberY(item.Y),
            //        Width = item.Width,
            //        Height = item.Height,
            //        CornerRadius = item.CornerRadius,
            //        Rot = ToGerberRot(item.Rot)
            //    };
            //}
            //else
            //{
            //    var t = new XTransformGroup();
            //    t.Children.Add(new XRotateTransform(item.Rot));//?

            //    var points = new List<XPoint>();
            //    var rect = new XRect(-0.5 * item.Width, -0.5 * item.Height, item.Width, item.Height);
            //    points.Add(rect.TopLeft);
            //    points.Add(rect.TopRight);
            //    points.Add(rect.BottomRight);
            //    points.Add(rect.BottomLeft);

            //    //transform points
            //    for (int i = 0; i < points.Count; i++)
            //    {
            //        points[i] = t.Transform(points[i]);
            //    }


            //    var figure = new GerberFigure();

            //    //border
            //    if (item.IsFilled)
            //    {
            //        var polyRegion = new GerberRegionPrimitive();
            //        for (int i = 1; i < points.Count; i++)
            //        {
            //            var sp = points[i - 1];
            //            var ep = points[i];

            //            polyRegion.RegionItems.Add(new GerberLinePrimitive
            //            {
            //                StartPoint = ToGerberPoint(sp),
            //                EndPoint = ToGerberPoint(ep),
            //                //Width = item.BorderWidth
            //            });
            //        }
            //        //add start point to close
            //        polyRegion.RegionItems.Add(new GerberLinePrimitive
            //        {
            //            StartPoint = ToGerberPoint(points[points.Count - 1]),
            //            EndPoint = ToGerberPoint(points[0]),
            //            //Width = item.BorderWidth
            //        });
            //        figure.FigureItems.Add(polyRegion);

            //    }

            //    return figure;
            //}

        }
        GerberPrimitive GetLinePrimitive(ILineCanvasItem item, bool transformed = true, double clearance = 0)
        {
            var sp = new XPoint(item.X1, item.Y1);
            var ep = new XPoint(item.X2, item.Y2);
            if (transformed)
            {
                var t = item.GetTransform();
                sp = GetPointTransform(t, sp);
                ep = GetPointTransform(t, ep);
            }

            return new GerberLinePrimitive
            {
                StartPoint = ToGerberPoint(sp),
                EndPoint = ToGerberPoint(ep),
                Width = item.Width + 2 * clearance
            };
        }

        private GerberPrimitive GetLinePrimitive(GlobalLinePrimitive item)
        {
            return new GerberLinePrimitive
            {
                StartPoint = ToGerberPoint(item.StartPoint),
                EndPoint = ToGerberPoint(item.EndPoint),
                Width = item.Width
            };
        }

        GerberPrimitive GetLinePrimitive(ILineRegionItem item, XPoint startPoint, double width)
        {
            return new GerberLinePrimitive
            {
                StartPoint = ToGerberPoint(startPoint),
                EndPoint = ToGerberPoint(item.EndPoint),
                Width = width
            };
        }





    }

    //public static class GeometryExtensions
    //{
    //    public static void AppendOutlines(this Geometry geometry, List<List<Point[]>> outlines)
    //    {
    //        var group = geometry as GeometryGroup;
    //        if (group != null)
    //        {
    //            foreach (var g in group.Children)
    //            {
    //                AppendOutlines(g, outlines);
    //            }

    //            return;
    //        }

    //        var pathGeometry = (geometry is PathGeometry) ? geometry as PathGeometry : geometry.GetOutlinedPathGeometry();
    //        if (pathGeometry != null)
    //        {
    //            var figures = pathGeometry.Figures.Select(figure => figure.ToPolyLine()).ToList();
    //            outlines.Add(figures);
    //            return;
    //        }

    //        throw new NotImplementedException();
    //    }

    //    public static Point[] ToPolyLine(this PathFigure figure)
    //    {
    //        var outline = new List<Point> { figure.StartPoint };
    //        var previousPoint = figure.StartPoint;
    //        foreach (var segment in figure.Segments)
    //        {
    //            var polyline = segment as PolyLineSegment;
    //            if (polyline != null)
    //            {
    //                outline.AddRange(polyline.Points);
    //                previousPoint = polyline.Points.Last();
    //                continue;
    //            }

    //            var polybezier = segment as PolyBezierSegment;
    //            if (polybezier != null)
    //            {
    //                for (int i = -1; i + 3 < polybezier.Points.Count; i += 3)
    //                {
    //                    var p1 = i == -1 ? previousPoint : polybezier.Points[i];
    //                    outline.AddRange(FlattenBezier(p1, polybezier.Points[i + 1], polybezier.Points[i + 2], polybezier.Points[i + 3], 20));
    //                }

    //                previousPoint = polybezier.Points.Last();
    //                continue;
    //            }

    //            var lineSegment = segment as LineSegment;
    //            if (lineSegment != null)
    //            {
    //                outline.Add(lineSegment.Point);
    //                previousPoint = lineSegment.Point;
    //                continue;
    //            }

    //            var bezierSegment = segment as BezierSegment;
    //            if (bezierSegment != null)
    //            {
    //                outline.AddRange(FlattenBezier(previousPoint, bezierSegment.Point1, bezierSegment.Point2, bezierSegment.Point3, 20));
    //                previousPoint = bezierSegment.Point3;
    //                continue;
    //            }

    //            throw new NotImplementedException();
    //        }

    //        return outline.ToArray();
    //    }

    //    static IEnumerable<Point> FlattenBezier(Point p1, Point p2, Point p3, Point p4, int n)
    //    {
    //        // http://tsunami.cis.usouthal.edu/~hain/general/Publications/Bezier/bezier%20cccg04%20paper.pdf
    //        // http://en.wikipedia.org/wiki/De_Casteljau's_algorithm
    //        for (int i = 1; i <= n; i++)
    //        {
    //            var t = (double)i / n;
    //            var u = 1 - t;
    //            yield return new Point(
    //                (u * u * u * p1.X) + (3 * t * u * u * p2.X) + (3 * t * t * u * p3.X) + (t * t * t * p4.X),
    //                (u * u * u * p1.Y) + (3 * t * u * u * p2.Y) + (3 * t * t * u * p3.Y) + (t * t * t * p4.Y));
    //        }
    //    }

    //    public static double AreaOfSegment(this Point[] segment)
    //    {
    //        return Math.Abs(segment.Take(segment.Length - 1)
    //            .Select((p, i) => (segment[i + 1].X - p.X) * (segment[i + 1].Y + p.Y))
    //            .Sum() / 2);
    //    }

    //    public static bool IsPointInPolygon(this IList<XPoint> polygon, XPoint testPoint)
    //    {
    //        bool result = false;
    //        int j = polygon.Count - 1;
    //        for (int i = 0; i < polygon.Count; i++)
    //        {
    //            if ((polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y) || (polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y))
    //            {
    //                if (polygon[i].X + ((testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X)) < testPoint.X)
    //                {
    //                    result = !result;
    //                }
    //            }

    //            j = i;
    //        }

    //        return result;
    //    }


    //}
}
