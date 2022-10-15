namespace IDE.Core.Converters
{
    using IDE.Core.Common.Geometries;
    using IDE.Core.Interfaces;
    using IDE.Core.Interfaces.Geometries;
    using IDE.Core.Types.Media;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class IGeometryOutlineToGeometryConverter : IValueConverter
    {

        static IGeometryOutlineToGeometryConverter instance;
        public static IGeometryOutlineToGeometryConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new IGeometryOutlineToGeometryConverter();

                return instance;
            }
        }

        private void LoadPaths(IGeometryOutline geometry, IList<IList<XPoint>> paths)
        {
            if (geometry is GeometryOutlines go)
            {
                foreach (var g in go.Outlines)
                {
                    LoadPaths(g, paths);
                }
            }
            else
            {
                var outline = geometry.GetOutline();
                paths.Add(outline);
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IGeometryOutline geometryOutline)
            {
                var paths = new List<IList<XPoint>>();

                LoadPaths(geometryOutline, paths);

                var poly = new PathGeometry();

                foreach (var path in paths)
                {
                    if (path.Count == 0)
                        continue;

                    var segments = new List<LineSegment>();
                    var startpoint = MilimetersToDpiHelper.ConvertToDpi(path[0]);
                    for (int i = 1; i < path.Count; i++)
                    {
                        var point = MilimetersToDpiHelper.ConvertToDpi(path[i]);
                        segments.Add(new LineSegment(point.ToPoint(), false));
                    }

                    var figure = new PathFigure(startpoint.ToPoint(), segments, true);
                    poly.Figures.Add(figure);
                }

                return poly;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
