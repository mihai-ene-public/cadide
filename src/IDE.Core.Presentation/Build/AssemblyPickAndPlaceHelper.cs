using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Documents.Views;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace IDE.Core.Build
{
    public class AssemblyPickAndPlaceHelper
    {
        public List<AssemblyPickAndPlaceItemDisplay> GetPickAndPlaceList(BoardDesignerFileViewModel board)
        {
            var buildOptions = ((BoardBuildOptionsViewModel)board.BuildOptions).Assembly;

            var items = new List<AssemblyPickAndPlaceItemDisplay>();

            var boardRectangle = board.GetBoardRectangle();
            var boardOriginX = boardRectangle.BottomLeft.X;
            var boardOriginY = boardRectangle.BottomLeft.Y;
            var useImperial = buildOptions.PositionUnits == OutputUnits.inch;

            var parts = board.GetBoardFootprints();

            foreach (var p in parts)
            {
                var assemblyItem = new AssemblyPickAndPlaceItemDisplay
                {
                    PartName = p.PartName,
                    Layer = p.Placement.ToString(),
                    Footprint = p.CachedFootprint?.Name,
                    CenterX = GetAssemblyX(p.X, boardOriginX, useImperial),
                    CenterY = GetAssemblyY(p.Y, boardOriginY, useImperial),
                    Rot = GetAssemblyRot(p.Rot)
                };

                items.Add(assemblyItem);
            }

            return items;
        }

        string GetAssemblyX(double x, double brdOriginX, bool useImperial)//x is in mm
        {
            x -= brdOriginX;

            if (useImperial)
                x /= 25.4;

            return x.ToString("0.0000", CultureInfo.InvariantCulture);
        }

        string GetAssemblyY(double y, double brdOriginY, bool useImperial)//y is in mm
        {
            y = brdOriginY - y;

            if (useImperial)
                y /= 25.4;

            return y.ToString("0.0000", CultureInfo.InvariantCulture);
        }

        string GetAssemblyRot(double rot)
        {
            rot = -rot;
            return rot.ToString("0", CultureInfo.InvariantCulture);
        }

        public Task<DynamicList> GetOutputData(BoardDesignerFileViewModel board, IList<AssemblyOutputColumn> columns)
        {
            return Task.Run(() =>
            {
                var src = GetPickAndPlaceList(board);

                var propertyNames = columns.Where(c => c.Show)
                                           .Select(c => new PropertyNameDisplayMapping
                                           {
                                               PropertyName = c.ColumnName,
                                               DisplayName = c.Header
                                           }
                                                  )
                                           .ToList();

                var result = src.Select(b => new CustomType(propertyNames, b))
                                .ToList();

                return new DynamicList(propertyNames, result);
            });
        }


    }
}
