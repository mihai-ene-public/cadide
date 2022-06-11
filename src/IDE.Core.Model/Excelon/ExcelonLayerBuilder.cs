using System.Threading.Tasks;
using IDE.Core.Build;
using IDE.Core.Interfaces;
using IDE.Core.Model.GlobalRepresentation.Primitives;
using IDE.Core.Types.Media;

namespace IDE.Core.Excelon
{
    public class ExcelonLayerBuilder
    {
        private double boardOriginX;
        private double boardOriginY;

        private List<NCTool> tools = new List<NCTool>();

        #region BRD.Options

        private int formatStatementDigitsBeforeDecimal = 2;
        private int formatStatementDigitsAfterDecimal = 4;
        private NCUnits units = NCUnits.Millimeters;

        #endregion

        public Task<BuildResult> Build(IBoardDesigner board, BoardGlobalDrillPairOutput drillPair, string savePath)
        {
            var drills = new List<GlobalPrimitive>();
            var mills = new List<GlobalPrimitive>();

            var drillPrimitives = drillPair.DrillPrimitives;
            var millPrimitives = drillPair.MillingPrimitives;

            if (drillPrimitives != null)
            {
                //we only want the holes, not the slots
                drills.AddRange(drillPrimitives.OfType<GlobalHolePrimitive>());
            }
            if (millPrimitives != null)
            {
                mills.AddRange(millPrimitives);
            }
            //add the drill slots
            mills.AddRange(drillPrimitives.Except(drills));

            LoadOptions(board);

            var buildPlanDrillPrimitives = new List<ExcelonPrimitive>();
            var buildPlanMillingPrimitives = new List<ExcelonPrimitive>();

            PrepareBoardOutline(board);
            BuildExecutionPlan(drills, mills, buildPlanDrillPrimitives, buildPlanMillingPrimitives);
            CreateOutput(savePath, buildPlanDrillPrimitives, buildPlanMillingPrimitives);

            var result = new BuildResult { Success = true };
            result.OutputFiles.Add(savePath);

            return Task.FromResult(result);
        }

        private void LoadOptions(IBoardDesigner board)
        {
            formatStatementDigitsBeforeDecimal = board.BuildOptions.NCDrillFormatBeforeDecimal;
            formatStatementDigitsAfterDecimal = board.BuildOptions.NCDrillFormatAfterDecimal;
            units = board.BuildOptions.NCDrillUnits == OutputUnits.mm ? NCUnits.Millimeters : NCUnits.Inches;
        }

        private void PrepareBoardOutline(IBoardDesigner board)
        {
            //calculate Excelon board origin
            if (board.BoardOutline == null)
                return;
            var boardRectangle = board.GetBoardRectangle();

            boardOriginX = boardRectangle.BottomLeft.X;
            boardOriginY = boardRectangle.BottomLeft.Y;
        }

        private void BuildExecutionPlan(IList<GlobalPrimitive> drillItems, IList<GlobalPrimitive> millingItems,
                                        IList<ExcelonPrimitive> buildPlanDrillPrimitives, IList<ExcelonPrimitive> buildPlanMillingPrimitives)
        {
            //we must have a build plan to optimize the NC file size
            //for now this is dumb

            //drill items
            AddToPlan(buildPlanDrillPrimitives, drillItems);

            //milling items
            AddToPlan(buildPlanMillingPrimitives, millingItems);
        }

        private void AddToPlan(IList<ExcelonPrimitive> buildPlanPrimitives, IList<GlobalPrimitive> itemsToAdd)
        {
            if (itemsToAdd != null)
            {
                var primitives = new List<ExcelonPrimitive>();
                foreach (var item in itemsToAdd)
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
                    buildPlanPrimitives.AddRange(g);
                }
            }
        }

        void CreateOutput(string savePath, IList<ExcelonPrimitive> buildPlanDrillPrimitives, IList<ExcelonPrimitive> buildPlanMillingPrimitives)
        {
            using (var nc = new NCDrillFileWriter(savePath))
            {
                //header
                nc.StartHeader();
                nc.SetUnits(units);
                nc.SetFormat(formatStatementDigitsBeforeDecimal, formatStatementDigitsAfterDecimal);
                CreateToolsOutput(nc);
                nc.EndHeader();

                //body
                CreatePrimitivesOutput(nc, buildPlanDrillPrimitives, buildPlanMillingPrimitives);

                nc.WriteEndFile();
                nc.Close();
            }
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

        void CreatePrimitivesOutput(NCDrillFileWriter nc, IList<ExcelonPrimitive> buildPlanDrillPrimitives, IList<ExcelonPrimitive> buildPlanMillingPrimitives)
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

        double ToExcelonX(double x)//x is in mm
        {
            if (units == NCUnits.Inches)
                return ( x - boardOriginX ) / 25.4;

            return x - boardOriginX;
        }

        double ToExcelonY(double y)//y is in mm
        {
            if (units == NCUnits.Inches)
                return ( boardOriginY - y ) / 25.4;

            return boardOriginY - y;
        }

        XPoint ToExcelonPoint(XPoint p)
        {
            return new XPoint(ToExcelonX(p.X), ToExcelonY(p.Y));
        }

        private ExcelonPrimitive GetExcelonPrimitive(GlobalPrimitive item)
        {
            switch (item)
            {
                case GlobalLinePrimitive line:
                    return GetLinePrimitive(line);

                case GlobalCirclePrimitive circle:
                    return GetCirclePrimitive(circle);

                case GlobalHolePrimitive hole:
                    return GetHolePrimitive(hole);

                case GlobalRectanglePrimitive rect://slot
                    return GetRectanglePrimitive(rect);

                case GlobalPolygonPrimitive poly:
                    return GetPolygonPrimitive(poly);

                case GlobalArcPrimitive arc:
                    return GetArcPrimitive(arc);

                case GlobalFigurePrimitive figure:
                    return GeFigurePrimitive(figure);
            }

            return null;
        }

        private ExcelonPrimitive GetLinePrimitive(GlobalLinePrimitive line)
        {
            return new ExcelonLinePrimitive
            {
                StartPoint = ToExcelonPoint(line.StartPoint),
                EndPoint = ToExcelonPoint(line.EndPoint),
                Width = line.Width
            };
        }

        private ExcelonPrimitive GetCirclePrimitive(GlobalCirclePrimitive circle)
        {
            return new ExcelonCirclePrimitive
            {
                X = ToExcelonX(circle.X),
                Y = ToExcelonY(circle.Y),
                Diameter = circle.Diameter,
                BorderWidth = circle.BorderWidth
            };
        }

        private ExcelonPrimitive GetHolePrimitive(GlobalHolePrimitive hole)
        {
            return new ExcelonHolePrimitive
            {
                X = ToExcelonX(hole.X),
                Y = ToExcelonY(hole.Y),
                Diameter = hole.Drill
            };
        }

        private ExcelonPrimitive GetRectanglePrimitive(GlobalRectanglePrimitive rect)
        {
            //we should use a poly
            return null;
        }

        private ExcelonPrimitive GetPolygonPrimitive(GlobalPolygonPrimitive item)
        {
            if (item.BorderWidth > 0.0)
            {
                var figure = new ExcelonFigure();
                for (int i = 1; i < item.Points.Count; i++)
                {
                    var sp = item.Points[i - 1];
                    var ep = item.Points[i];

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
                    StartPoint = ToExcelonPoint(item.Points[item.Points.Count - 1]),
                    EndPoint = ToExcelonPoint(item.Points[0]),
                    Width = item.BorderWidth
                });

                return figure;
            }

            return null;
        }

        private ExcelonPrimitive GetArcPrimitive(GlobalArcPrimitive arc)
        {
            return new ExcelonArcPrimitive
            {
                StartPoint = ToExcelonPoint(arc.StartPoint),
                EndPoint = ToExcelonPoint(arc.EndPoint),
                Width = arc.Width,
                SizeDiameter = arc.SizeDiameter,
                SweepDirection = arc.SweepDirection
            };
        }

        private ExcelonPrimitive GeFigurePrimitive(GlobalFigurePrimitive item)
        {
            var figure = new ExcelonFigure();

            foreach (var fi in item.FigureItems)
            {
                var fp = GetExcelonPrimitive(fi);
                if (fp != null)
                {
                    figure.FigureItems.Add(fp);
                }
            }

            return figure;
        }
    }
}
