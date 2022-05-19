using IDE.Core.Build;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Excelon
{
    //represents a file that will be exported
    public class ExcelonLayer
    {
        public ExcelonLayer(IBoardDesigner boardModel, BoardDrillPairOutput drillPair)// IEnumerable<IHoleCanvasItem> _drillItems, IEnumerable<ICanvasItem> _millingItems)
        {
            _drillPair = drillPair.DrillPair;
            var _drillItems = drillPair.DrillItems;
            var _millingItems = drillPair.MillingItems;

            if (_drillItems != null)
                drillItems = _drillItems.Where(d => d.DrillType == DrillType.Drill).ToList();

            if (_millingItems == null)
                _millingItems = new List<ICanvasItem>();
            millingItems = _millingItems.Union(_drillItems.Where(d => d.DrillType == DrillType.Slot));//.ToList();


            board = boardModel;

            LoadOptions();
        }

        IBoardDesigner board;
        ILayerPairModel _drillPair;

        IEnumerable<IHoleCanvasItem> drillItems;
        IEnumerable<ICanvasItem> millingItems;

        List<NCTool> tools = new List<NCTool>();
        List<ExcelonPrimitive> buildPlanDrillPrimitives = new List<ExcelonPrimitive>();
        List<ExcelonPrimitive> buildPlanMillingPrimitives = new List<ExcelonPrimitive>();

        XRect boardRectangle;
        double boardOriginX;
        double boardOriginY;

        public List<string> OutputFiles { get; private set; } = new List<string>();

        #region BRD.Options

        int formatStatementDigitsBeforeDecimal = 2;
        int formatStatementDigitsAfterDecimal = 4;
        NCUnits units = NCUnits.Millimeters;

        #endregion

        public Task Build()
        {
            buildPlanDrillPrimitives.Clear();
            buildPlanMillingPrimitives.Clear();

            return Task.Run(() =>
            {
                PrepareBoardOutline();
                BuildExecutionPlan();
                CreateOutput();
            });
        }

        void LoadOptions()
        {
            formatStatementDigitsBeforeDecimal = board.BuildOptions.NCDrillFormatBeforeDecimal;
            formatStatementDigitsAfterDecimal = board.BuildOptions.NCDrillFormatAfterDecimal;
            units = board.BuildOptions.NCDrillUnits == OutputUnits.mm ? NCUnits.Millimeters : NCUnits.Inches;
        }

        void PrepareBoardOutline()
        {
            //calculate Excelon board origin
            if (board.BoardOutline == null)
                return;
            //var upperLeft = board.BoardOutline.StartPoint;
            //var lowerRight = upperLeft;
            //var startPoint = board.BoardOutline.StartPoint;
            //foreach (var item in board.BoardOutline.Items)
            //{
            //    //upperLeft: Xmin, Ymin
            //    if (upperLeft.X > item.EndPointX)
            //        upperLeft.X = item.EndPointX;
            //    if (upperLeft.Y > item.EndPointY)
            //        upperLeft.Y = item.EndPointY;

            //    //lowerRight: Xmax, Ymax
            //    if (lowerRight.X < item.EndPointX)
            //        lowerRight.X = item.EndPointX;
            //    if (lowerRight.Y < item.EndPointY)
            //        lowerRight.Y = item.EndPointY;
            //}

            //boardRectangle = new Rect(upperLeft, lowerRight);
            boardRectangle = board.GetBoardRectangle();

            boardOriginX = boardRectangle.BottomLeft.X;
            boardOriginY = boardRectangle.BottomLeft.Y;
        }

        double ToExcelonX(double x)//x is in mm
        {
            if (units == NCUnits.Inches)
                return (x - boardOriginX) / 25.4;

            return x - boardOriginX;
        }

        double ToExcelonY(double y)//y is in mm
        {
            if (units == NCUnits.Inches)
                return (boardOriginY - y) / 25.4;

            return boardOriginY - y;
        }

        XPoint ToExcelonPoint(XPoint p)
        {
            return new XPoint(ToExcelonX(p.X), ToExcelonY(p.Y));
        }

        void BuildExecutionPlan()
        {
            //we must have a build plan to optimize the NC file size
            //for now this is dumb

            //drill items
            if (drillItems != null)
            {
                var primitives = new List<ExcelonPrimitive>();
                foreach (ICanvasItem item in drillItems)
                {
                    var ncPrimitive = GetExcelonPrimitive(item);
                    if (ncPrimitive != null)
                    {
                        HandleTools(ncPrimitive);
                        primitives.Add(ncPrimitive);
                    }
                }

                //add grouped by tool (we must have a tool), order by tool.Number
                foreach (var g in primitives.Where(p => p.Tool != null)
                                           .GroupBy(p => p.Tool.Number)
                                           .OrderBy(p => p.Key))
                {
                    buildPlanDrillPrimitives.AddRange(g);
                }
            }


            //milling items
            if (millingItems != null)
            {
                var primitives = new List<ExcelonPrimitive>();
                foreach (var item in millingItems)
                {
                    var ncPrimitive = GetExcelonPrimitive(item);
                    if (ncPrimitive != null)
                    {
                        HandleTools(ncPrimitive);
                        primitives.Add(ncPrimitive);
                    }
                }

                //add grouped by tool (we must have a tool), order by tool.Number
                foreach (var g in primitives.Where(p => p.Tool != null)
                                           .GroupBy(p => p.Tool.Number)
                                           .OrderBy(p => p.Key))
                {
                    buildPlanMillingPrimitives.AddRange(g);
                }
            }

        }

        void CreateOutput()
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
            var brdName = Path.GetFileNameWithoutExtension(f.FilePath);
            var drillPairName = _drillPair.ToString();
            savePath = Path.Combine(savePath, $"{brdName}-{drillPairName}-DrillsAndSlots.txt");
            using (var nc = new NCDrillFileWriter(savePath))
            {
                //header
                nc.StartHeader();
                nc.SetUnits(units);
                nc.SetFormat(formatStatementDigitsBeforeDecimal, formatStatementDigitsAfterDecimal);
                CreateToolsOutput(nc);
                nc.EndHeader();

                //body
                CreatePrimitivesOutput(nc);

                nc.WriteEndFile();
                nc.Close();
            }

            OutputFiles.Clear();
            OutputFiles.Add(savePath);
        }

        void CreateToolsOutput(NCDrillFileWriter nc)
        {
            var platedTools = tools.Where(t => t.Plating == DrillPlating.Plated).ToList();
            var nonPlatedTools = tools.Where(t => t.Plating == DrillPlating.NonPlated).ToList();

            //PTH - plated through hole
            if (platedTools.Count > 0)
                nc.WriteComment("Type = PTH");
            foreach (var tool in platedTools)
            {
                var adCode = nc.AddTool(tool.Diameter);

                if (adCode != 0)
                    tool.Number = adCode;
            }

            //NPTH - non plated
            if (nonPlatedTools.Count > 0)
                nc.WriteComment("Type = NPTH");
            foreach (var tool in nonPlatedTools)
            {
                var adCode = nc.AddTool(tool.Diameter);

                if (adCode != 0)
                    tool.Number = adCode;
            }
        }

        void CreatePrimitivesOutput(NCDrillFileWriter nc)
        {
            //output from the plan

            //drills
            if (buildPlanDrillPrimitives.Count > 0)
                nc.WriteComment("Drill holes...");
            foreach (var np in buildPlanDrillPrimitives)
            {
                np.WriteExcelon(nc);
            }

            //milling
            if (buildPlanMillingPrimitives.Count > 0)
            {
                nc.WriteComment("Slots...");
                foreach (var np in buildPlanMillingPrimitives)
                {
                    np.WriteExcelon(nc);
                }

                nc.RetractWithoutClamping();
            }
        }

        int nextToolId = 1;
        int GetNextToolId()
        {
            var crt = nextToolId;
            nextToolId++;
            return crt;
        }

        void HandleTools(ExcelonPrimitive primitive)
        {
            if (primitive == null)
                return;
            foreach (var tool in primitive.GetTools())
            {
                var existingTool = tools.FirstOrDefault(a => a.Equals(tool));
                if (existingTool == null)
                {
                    tool.Number = GetNextToolId();
                    tools.Add(tool);
                }
                else
                {
                    tool.Number = existingTool.Number;
                }
            }
        }

        ExcelonPrimitive GetExcelonPrimitive(ICanvasItem item)
        {
            if (item != null)
            {
                if (item is ILineCanvasItem)
                    return GetLinePrimitive((ILineCanvasItem)item);
                //else if (item is IRectangleCanvasItem)
                //    return GetRectanglePrimitive((IRectangleCanvasItem)item);
                else if (item is ICircleCanvasItem)
                    return GetCirclePrimitive((ICircleCanvasItem)item);
                else if (item is IHoleCanvasItem)
                    return GetHolePrimitive((IHoleCanvasItem)item);
                //else if (item is IPadCanvasItem)
                //    return GetPadPrimitive((IPadCanvasItem)item);
                else if (item is IPolygonCanvasItem)
                    return GetPolygonPrimitive((IPolygonCanvasItem)item);
                else if (item is IArcCanvasItem)
                    return GetArcPrimitive((IArcCanvasItem)item);
                //else if (item is ITextCanvasItem)
                //    return GetTextPrimitive((ITextCanvasItem)item);
                else if (item is IRegionCanvasItem)
                    return GetRegionPrimitive((IRegionCanvasItem)item);
            }

            return null;
        }
        ExcelonPrimitive GetExcelonPrimitive(IRegionItem item, XPoint startPoint, double width)
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

        ExcelonPrimitive GetCirclePrimitive(ICircleCanvasItem item)
        {
            if (item.BorderWidth > 0.0)
            {
                var t = item.GetTransform();
                var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);

                return new ExcelonCirclePrimitive
                {
                    X = ToExcelonX(position.X),
                    Y = ToExcelonY(position.Y),
                    Diameter = item.Diameter,
                    BorderWidth = item.BorderWidth
                };
            }

            return null;
        }

        ExcelonPrimitive GetHolePrimitive(IHoleCanvasItem item)
        {
            var t = item.GetTransform();
            var position = new XPoint(t.Value.OffsetX, t.Value.OffsetY);

            if (item.DrillType == DrillType.Drill)
            {
                return new ExcelonHolePrimitive
                {
                    X = ToExcelonX(position.X),
                    Y = ToExcelonY(position.Y),
                    Diameter = item.Drill
                };
            }
            else
            {
                var placement = FootprintPlacement.Top;
                var fp = item.ParentObject as IFootprintBoardCanvasItem;
                if (fp != null)
                    placement = fp.Placement;

                var rot = GetWorldRotation(t, placement);

                //return new GerberRectanglePrimitive
                //{
                //    X = ToGerberX(position.X),
                //    Y = ToGerberY(position.Y),
                //    Width = item.Drill,
                //    Height = item.Height,
                //    CornerRadius = item.Drill * 0.5,
                //    Rot = ToGerberRot(rot)
                //};

                var startPoint = new XPoint(0, -0.5 * item.Height + 0.5 * item.Drill);
                var endPoint = new XPoint(0, 0.5 * item.Height - 0.5 * item.Drill);
                startPoint = t.Transform(startPoint);
                endPoint = t.Transform(endPoint);

                return new ExcelonLinePrimitive
                {
                    StartPoint = ToExcelonPoint(startPoint),
                    EndPoint = ToExcelonPoint(endPoint),
                    Width = item.Drill
                };
            }
        }

        double GetRotationTransform(XTransform transform)
        {
            return Geometry2DHelper.GetRotationAngleFromMatrix(transform.Value);
        }

        //todo: make this an extension
        double GetWorldRotation(/*double rot, */XTransform transform, FootprintPlacement placement)
        {
            var rotAngle = GetRotationTransform(transform);
            var myRot = rotAngle;

            if (placement == FootprintPlacement.Bottom)
            {
                myRot = 180.0d - myRot;

                var sign = Math.Sign(rotAngle);
                rotAngle = sign * (180 - Math.Abs(rotAngle));
                // myRot += rotAngle;
            }

            //myRot += rotAngle;

            return myRot;
        }

        ExcelonPrimitive GetPolygonPrimitive(IPolygonCanvasItem item)
        {
            //border
            if (item.BorderWidth > 0.0)
            {
                var t = item.GetTransform();


                var figure = new ExcelonFigure();
                for (int i = 1; i < item.PolygonPoints.Count; i++)
                {
                    var sp = GetPointTransform(t, item.PolygonPoints[i - 1]);
                    var ep = GetPointTransform(t, item.PolygonPoints[i]);

                    figure.FigureItems.Add(new ExcelonLinePrimitive
                    {
                        StartPoint = ToExcelonPoint(sp),
                        EndPoint = ToExcelonPoint(ep),
                        Width = item.BorderWidth
                    });
                }
                //add start point to close
                figure.FigureItems.Add(new ExcelonLinePrimitive
                {
                    StartPoint = ToExcelonPoint(GetPointTransform(t, item.PolygonPoints[item.PolygonPoints.Count - 1])),
                    EndPoint = ToExcelonPoint(GetPointTransform(t, item.PolygonPoints[0])),
                    Width = item.BorderWidth
                });

                return figure;
            }

            return null;
        }

        ExcelonPrimitive GetArcPrimitive(IArcCanvasItem item)
        {
            var sp = new XPoint(item.StartPointX, item.StartPointY);
            var ep = new XPoint(item.EndPointX, item.EndPointY);

            var t = item.GetTransform();
            sp = GetPointTransform(t, sp);
            ep = GetPointTransform(t, ep);

            return new ExcelonArcPrimitive
            {
                StartPoint = ToExcelonPoint(sp),
                EndPoint = ToExcelonPoint(ep),
                Width = item.BorderWidth,
                SizeDiameter = item.Size.Width,
                SweepDirection = item.SweepDirection
            };
        }

        ExcelonPrimitive GetArcPrimitive(IArcRegionItem item, XPoint startPoint, double width)
        {
            return new ExcelonArcPrimitive
            {
                StartPoint = ToExcelonPoint(startPoint),
                EndPoint = ToExcelonPoint(item.EndPoint),
                Width = width,
                SizeDiameter = item.SizeDiameter,
                SweepDirection = item.SweepDirection
            };
        }

        ExcelonPrimitive GetRegionPrimitive(IRegionCanvasItem item)
        {
            //we shouldn't convert to Gerber coordinates here
            var figure = new ExcelonFigure();

            var startPoint = item.StartPoint;

            foreach (var regionItem in item.Items)
            {
                var primitive = GetExcelonPrimitive(regionItem, startPoint, item.Width);
                startPoint = regionItem.EndPoint;
                figure.FigureItems.Add(primitive);
            }

            return figure;
        }

        ExcelonPrimitive GetRectanglePrimitive(IRectangleCanvasItem item)
        {
            if (item.CornerRadius > 0.0d)
            {
                //we must generate a region with lines and arcs
                return null;
            }
            else
            {
                //border
                if (item.BorderWidth > 0.0)
                {
                    var figure = new ExcelonFigure();
                    figure.FigureItems.Add(new ExcelonLinePrimitive
                    {
                        StartPoint = ToExcelonPoint(new XPoint(item.X, item.Y)),
                        EndPoint = ToExcelonPoint(new XPoint(item.X, item.Y + item.Height)),
                        Width = item.BorderWidth
                    });
                    figure.FigureItems.Add(new ExcelonLinePrimitive
                    {
                        StartPoint = ToExcelonPoint(new XPoint(item.X, item.Y + item.Height)),
                        EndPoint = ToExcelonPoint(new XPoint(item.X + item.Width, item.Y + item.Height)),
                        Width = item.BorderWidth
                    });
                    figure.FigureItems.Add(new ExcelonLinePrimitive
                    {
                        StartPoint = ToExcelonPoint(new XPoint(item.X + item.Width, item.Y + item.Height)),
                        EndPoint = ToExcelonPoint(new XPoint(item.X + item.Width, item.Y)),
                        Width = item.BorderWidth
                    });
                    figure.FigureItems.Add(new ExcelonLinePrimitive
                    {
                        StartPoint = ToExcelonPoint(new XPoint(item.X + item.Width, item.Y)),
                        EndPoint = ToExcelonPoint(new XPoint(item.X, item.Y)),
                        Width = item.BorderWidth
                    });

                    return figure;
                }
            }

            return null;
        }

        XPoint GetPointTransform(XTransform transform, XPoint p)
        {
            p = transform.Transform(p);
            return p;
        }

        ExcelonPrimitive GetLinePrimitive(ILineCanvasItem item)
        {
            var sp = new XPoint(item.X1, item.Y1);
            var ep = new XPoint(item.X2, item.Y2);

            var t = item.GetTransform();
            sp = GetPointTransform(t, sp);
            ep = GetPointTransform(t, ep);

            return new ExcelonLinePrimitive
            {
                StartPoint = ToExcelonPoint(sp),
                EndPoint = ToExcelonPoint(ep),
                Width = item.Width
            };
        }

        ExcelonPrimitive GetLinePrimitive(ILineRegionItem item, XPoint startPoint, double width)
        {
            return new ExcelonLinePrimitive
            {
                StartPoint = ToExcelonPoint(startPoint),
                EndPoint = ToExcelonPoint(item.EndPoint),
                Width = width
            };
        }
    }
}
