using IDE.Core.Common.Geometries;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IDE.Core.Common.Tests
{
    public class PolylineGeometryOutlineTests
    {
        [Fact]
        public void PointsCountPass()
        {
            var points = new List<XPoint>();
            points.Add(new XPoint());
            points.Add(new XPoint(0, 10));
            var p = new PolylineGeometryOutline(points, 1);

            var actualPoints = p.GetOutline();

            Assert.True(actualPoints.Count > 0);
        }
    }
}
