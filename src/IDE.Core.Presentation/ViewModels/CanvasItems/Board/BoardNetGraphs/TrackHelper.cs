using IDE.Core.Interfaces;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.Designers
{
    /// <summary>
    /// represents a continous link of trace items that are on the same layer, and between 2 joints
    /// </summary>
    class TrackHelper
    {
        public TrackHelper(IBoardDesigner brd)
        {
            board = brd;
            GeometryHelper = ServiceProvider.Resolve<IGeometryHelper>();
        }

        IBoardDesigner board;

        IGeometryHelper GeometryHelper;

        public List<XPoint> trackPoints = new List<XPoint>();

        public XPoint StartPoint => trackPoints[0];

        public XPoint EndPoint => trackPoints[trackPoints.Count - 1];

        public double Width { get; set; }

        public void AppendPoints(IList<XPoint> points)
        {
            if (points.Count == 0)
                return;

            if (trackPoints.Count > 0)
            {
                if (EndPoint == points[0])
                {
                    //append normal
                    trackPoints.AddRange(points);
                }
                else if (EndPoint == points[points.Count - 1])
                {
                    //append reversed
                    trackPoints.AddRange(points.Reverse());
                }
            }
            else
            {
                trackPoints.AddRange(points);
            }
        }

        public void PrependPoints(IList<XPoint> points)
        {
            if (points.Count == 0)
                return;

            if (trackPoints.Count > 0)
            {
                if (StartPoint == points[points.Count - 1])
                {
                    //append normal
                    //trackPoints.AddRange(points);
                    // for(int i=0;i<points.Count;i++)
                    trackPoints.InsertRange(0, points);
                }
                else if (StartPoint == points[0])
                {
                    //append reversed
                    // trackPoints.AddRange(points.Reverse());
                    trackPoints.InsertRange(0, points.Reverse());
                }
            }
            else
            {
                trackPoints.AddRange(points);
            }
        }

        public void Simplify()
        {
            var simplifiedPoints = new List<XPoint>();
            var simplified = Geometry2DHelper.SimplifyPolyline(trackPoints, simplifiedPoints);
            if (simplified)
            {
                trackPoints.Clear();
                trackPoints.AddRange(simplifiedPoints);
            }
        }
    }
}
