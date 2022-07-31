using System.Threading.Tasks;
using IDE.Core.Build;
using IDE.Core.Interfaces;
using IDE.Core.Model.Gerber.Primitives.Apertures;
using IDE.Core.Model.Gerber.Primitives.Attributes;
using IDE.Core.Model.GlobalRepresentation;
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
        private Dictionary<string, FileGerberAttribute> fileAttributes = new Dictionary<string, FileGerberAttribute>();

        #region BRD.Options

        private int formatStatementDigitsBeforeDecimal = 2;
        private int formatStatementDigitsAfterDecimal = 4;
        private Modes units = Modes.Millimeters;
        private bool drawBoardOutline = true;

        //global switch to write gerber attributes
        private bool writeGerberMetadata = false;
        //private bool writeFileAttributes = true;
        private bool writeNetListAttributes = false;

        /*
        //todo: future feature
        //we cannot do it here, we need to have same netnames accross all layers
        //best to obfuscate names in the global helper
        private bool obfuscateNetNames = false;
        //todo: should be done on global builder
        private bool writeComponentInfo = false;//for assembly
        */

        #endregion

        public Task<BuildResult> Build(IBoardDesigner board, BoardGlobalLayerOutput layer, string gerberFilePath)
        {
            // _board = board;

            LoadOptions(board);

            BuildFileAttributes(board, layer.Layer);

            PrepareBoardOutline(board);
            var buildPlanPrimitives = BuildExecutionPlan(board, layer);

            WriteGerberOutput(gerberFilePath, buildPlanPrimitives);

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
            writeGerberMetadata = buildOptions.GerberWriteGerberMetadata;
            writeNetListAttributes = buildOptions.GerberWriteNetListAttributes;
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

                AddToPlan(buildPlanPrimitives, itemsToAdd, Polarity.Dark, layer.Layer);
            }

            //clear items
            if (layer.ExtractItems != null)
            {
                AddToPlan(buildPlanPrimitives, layer.ExtractItems, Polarity.Clear, layer.Layer);
            }

            return buildPlanPrimitives;
        }

        private void WriteGerberOutput(string gerberPath, IList<GerberPrimitive> buildPlanPrimitives)
        {

            using (var gw = new Gerber274XWriter(gerberPath))
            {
                gw.FormatStatement(formatStatementDigitsBeforeDecimal, formatStatementDigitsAfterDecimal);
                gw.SetMode(units);

                if (writeGerberMetadata)
                {
                    WriteFileAttributes(gw);
                }

                WriteAperturesOutput(gw);
                WritePrimitivesOutput(gw, buildPlanPrimitives);

                gw.EndOfProgram();
                gw.Close();
            }
        }

        private void WriteAperturesOutput(Gerber274XWriter gw)
        {
            foreach (var aperture in apertures)
            {
                if (writeGerberMetadata)
                {
                    foreach (var apertureAttr in aperture.ApertureAttributes.Values)
                    {
                        gw.WriteApertureAttribute(apertureAttr.ToString());
                    }
                }

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
                {
                    aperture.Number = adCode;

                    if (writeGerberMetadata && aperture.ApertureAttributes.Count > 0)
                    {
                        gw.DeleteAllAttributes();
                    }
                }
            }
        }

        private void WritePrimitivesOutput(Gerber274XWriter gw, IList<GerberPrimitive> buildPlanPrimitives)
        {
            //output from the plan
            foreach (var gp in buildPlanPrimitives)
            {
                if (writeGerberMetadata)
                {
                    foreach (var objAttr in gp.ObjectAttributes.Values)
                    {
                        gw.WriteObjectAttribute(objAttr.ToString());
                    }
                }

                gp.WriteGerber(gw);
            }
        }

        private void BuildFileAttributes(IBoardDesigner board, ILayerDesignerItem layer)
        {
            fileAttributes[nameof(StandardFileAttributes.Part)] = new PartFileGerberAttribute { PartFileGerberAttributeValue = PartFileGerberAttributeValue.Single };

            var fileFunction = BuildFileFunctionAttribute(board, layer);
            if (fileFunction != null)
            {
                fileAttributes[nameof(StandardFileAttributes.FileFunction)] = fileFunction;
            }


            var filePolarity = new FilePolarityFileGerberAttribute { Value = FilePolarityValue.Positive };
            if (layer.LayerType == LayerType.SolderMask)
                filePolarity.Value = FilePolarityValue.Negative;
            fileAttributes[nameof(StandardFileAttributes.FilePolarity)] = filePolarity;

            fileAttributes[nameof(StandardFileAttributes.GenerationSoftware)] = new GenerationSoftwareFileGerberAttribute();
            fileAttributes[nameof(StandardFileAttributes.CreationDate)] = new CreationDateFileGerberAttribute { Value = DateTime.Now };
        }

        private void WriteFileAttributes(Gerber274XWriter gw)
        {
            foreach (var fileAttribute in fileAttributes)
            {
                gw.WriteFileAttribute(fileAttribute.Value.ToString());
            }
        }

        private FileFunctionFileGerberAttribute BuildFileFunctionAttribute(IBoardDesigner board, ILayerDesignerItem layer)
        {
            var fileFunctionAttribute = new FileFunctionFileGerberAttribute();

            switch (layer.LayerType)
            {
                case LayerType.Plane:
                case LayerType.Signal:
                    fileFunctionAttribute.FileFunctionType = FileFunctionType.Copper;
                    var copperValue = new FileFunctionTypeCopperValue();
                    copperValue.LayerNumber = GetCopperLayerNumber(board, layer);
                    copperValue.CopperLayerSide = FileFunctionTypeCopperLayerSide.Inr;
                    if (layer.IsTopLayer)
                        copperValue.CopperLayerSide = FileFunctionTypeCopperLayerSide.Top;
                    if (layer.IsBottomLayer)
                        copperValue.CopperLayerSide = FileFunctionTypeCopperLayerSide.Bot;
                    fileFunctionAttribute.Value = copperValue;
                    break;

                case LayerType.SilkScreen:
                    fileFunctionAttribute.FileFunctionType = FileFunctionType.Legend;
                    fileFunctionAttribute.Value = FileFunctionTypeLayerSide.Top;
                    if (layer.IsBottomLayer)
                        fileFunctionAttribute.Value = FileFunctionTypeLayerSide.Bot;
                    break;

                case LayerType.SolderMask:
                    fileFunctionAttribute.FileFunctionType = FileFunctionType.Soldermask;
                    fileFunctionAttribute.Value = FileFunctionTypeLayerSide.Top;
                    if (layer.IsBottomLayer)
                        fileFunctionAttribute.Value = FileFunctionTypeLayerSide.Bot;
                    break;

                case LayerType.PasteMask:
                    fileFunctionAttribute.FileFunctionType = FileFunctionType.Paste;
                    fileFunctionAttribute.Value = FileFunctionTypeLayerSide.Top;
                    if (layer.IsBottomLayer)
                        fileFunctionAttribute.Value = FileFunctionTypeLayerSide.Bot;
                    break;

                case LayerType.BoardOutline:
                    fileFunctionAttribute.FileFunctionType = FileFunctionType.Profile;
                    fileFunctionAttribute.Value = FileFunctionProfileValue.NP;
                    break;

                default:
                    return null;
            }

            return fileFunctionAttribute;
        }

        private int GetCopperLayerNumber(IBoardDesigner board, ILayerDesignerItem layer)
        {
            var layerTypes = new[] { LayerType.Plane, LayerType.Signal };
            var copperLayers = board.LayerItems.Where(l => layerTypes.Contains(l.LayerType)).ToList();

            var layerIndex = copperLayers.IndexOf(layer);
            return layerIndex + 1;
        }

        private void AddToPlan(List<GerberPrimitive> buildPlanPrimitives, IList<GlobalPrimitive> itemsToAdd, Polarity polarity, ILayerDesignerItem layer)
        {
            //we transfer attributes only if enabled, if we add items and we are on a copper layer
            var transferAttributes = writeGerberMetadata && polarity == Polarity.Dark
                                   && ( layer.LayerType == LayerType.Signal || layer.LayerType == LayerType.Plane );

            foreach (var item in itemsToAdd)
            {
                var gbrPrimitive = GetGerberPrimitive(item);
                if (gbrPrimitive != null)
                {

                    if (transferAttributes)
                    {
                        TransferGerberAttributes(item, gbrPrimitive);
                    }

                    HandleApertures(gbrPrimitive);

                    gbrPrimitive.Polarity = polarity;
                    buildPlanPrimitives.Add(gbrPrimitive);
                }
            }
        }

        private void HandleApertures(GerberPrimitive primitive)
        {
            //dark and clearance apertures
            if (primitive == null)
                return;
            foreach (var aperture in primitive.GetApertures())
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

            foreach (var clearanceAperture in primitive.GetClearanceAperture())
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

        private void TransferGerberAttributes(GlobalPrimitive globalPrimitive, GerberPrimitive gerberPrimitive)
        {
            TransferApertureGerberAttributes(globalPrimitive, gerberPrimitive);
            TransferObjectGerberAttributes(globalPrimitive, gerberPrimitive);
        }

        private void TransferApertureGerberAttributes(GlobalPrimitive globalPrimitive, GerberPrimitive gerberPrimitive)
        {
            //aperture function
            var roleFound = globalPrimitive.Tags.TryGetValue(nameof(GlobalStandardPrimitiveTag.Role), out var roleObject);

            //we have the role, then we have an aperture function
            if (roleFound)
            {
                var role = (GlobalStandardPrimitiveRole)roleObject;
                var mapping = new Dictionary<GlobalStandardPrimitiveRole, AperFunctionType>
                {
                    { GlobalStandardPrimitiveRole.PadTht, AperFunctionType.ComponentPad },
                    { GlobalStandardPrimitiveRole.PadSmd, AperFunctionType.SMDPad },
                    { GlobalStandardPrimitiveRole.Via, AperFunctionType.ViaPad },
                    { GlobalStandardPrimitiveRole.Track, AperFunctionType.Conductor },
                    { GlobalStandardPrimitiveRole.Text, AperFunctionType.NonConductor },
                    { GlobalStandardPrimitiveRole.BoardOutline, AperFunctionType.Profile },
                };

                var functionTypeFound = mapping.TryGetValue(role, out var functionType);

                if (functionTypeFound)
                {
                    var apertureFunctionAttribute = new AperFunctionApertureGerberAttribute();

                    apertureFunctionAttribute.AperFunctionType = functionType;

                    switch (functionType)
                    {

                        case AperFunctionType.ComponentPad:
                        case AperFunctionType.ViaPad:
                        case AperFunctionType.Conductor:
                        case AperFunctionType.NonConductor:
                        case AperFunctionType.Profile:
                            apertureFunctionAttribute.Value = null;
                            break;
                        case AperFunctionType.SMDPad:
                            apertureFunctionAttribute.Value = AperFunctionTypePadValue.CuDef;
                            break;
                    }

                    foreach (var aperture in gerberPrimitive.GetApertures())
                    {
                        aperture.ApertureAttributes[nameof(StandardApertureAttributes.AperFunction)] = apertureFunctionAttribute;
                    }
                }
            }
        }

        private void TransferObjectGerberAttributes(GlobalPrimitive globalPrimitive, GerberPrimitive gerberPrimitive)
        {
            if (writeNetListAttributes)
            {
                var netName = GetDictionaryValue<string>(globalPrimitive.Tags, nameof(GlobalStandardPrimitiveTag.NetName));
                if (!string.IsNullOrEmpty(netName))
                {
                    gerberPrimitive.ObjectAttributes[nameof(StandardObjectAttributes.Net)] = new NetObjectGerberAttribute { NetName = netName };
                }
            }

            var partName = GetDictionaryValue<string>(globalPrimitive.Tags, nameof(GlobalStandardPrimitiveTag.PartName));
            if (!string.IsNullOrEmpty(partName))
            {
                gerberPrimitive.ObjectAttributes[nameof(StandardObjectAttributes.PartName)] = new PartNameObjectGerberAttribute { PartName = partName };
            }

            var pinNumber = GetDictionaryValue<string>(globalPrimitive.Tags, nameof(GlobalStandardPrimitiveTag.PinNumber));
            if (!string.IsNullOrEmpty(pinNumber) && !string.IsNullOrEmpty(partName))
            {
                gerberPrimitive.ObjectAttributes[nameof(StandardObjectAttributes.Pin)] = new PinObjectGerberAttribute
                {
                    PartName = partName,
                    PinNumber = pinNumber
                };
            }
        }

        private T GetDictionaryValue<T>(Dictionary<string, object> dictionary, string key) where T : class
        {
            var keyFound = dictionary.TryGetValue(key, out var valueObj);

            if (keyFound && valueObj != null)
                return valueObj as T;

            return null;
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
                return RoundGerberValue(( x - boardOriginX ) / 25.4);

            return RoundGerberValue(x - boardOriginX);
        }

        private double ToGerberY(double y)//y is in mm
        {
            if (units == Modes.Inches)
                return RoundGerberValue(( boardOriginY - y ) / 25.4);

            return RoundGerberValue(boardOriginY - y);
        }

        private double RoundGerberValue(double x)
        {
            return Math.Round(x, formatStatementDigitsAfterDecimal);
        }

        private XPoint ToGerberPoint(XPoint p)
        {
            return new XPoint(ToGerberX(p.X), ToGerberY(p.Y));
        }

        private double ToGerberRot(double rot)
        {
            rot = Math.Round(rot, 2);
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
