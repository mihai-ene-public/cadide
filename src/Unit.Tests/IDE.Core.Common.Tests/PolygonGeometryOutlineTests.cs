using IDE.Core.Common.Geometries;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using Xunit;

namespace IDE.Core.Common.Tests
{
    public class PolygonGeometryOutlineTests
    {
        [Fact]
        public void PointsPass()
        {
            var points = new List<XPoint>();
            points.Add(new XPoint());
            points.Add(new XPoint(0, 10));
            points.Add(new XPoint(10, 10));
            points.Add(new XPoint(0, 0));//last point to close the loop
            var p = new PolygonGeometryOutline(points);

            var actualPoints = p.GetOutline();

            //last point is equal first point to close the loop
            Assert.Equal(actualPoints[0], actualPoints[actualPoints.Count - 1]);

            //actual points should be equal the first given points
            for (int i = 0; i < points.Count; i++)
            {
                Assert.Equal(actualPoints[i], points[i]);
            }

        }
    }
}
