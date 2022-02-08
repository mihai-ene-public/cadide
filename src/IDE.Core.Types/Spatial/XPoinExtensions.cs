using System;

namespace IDE.Core.Types.Media
{
    public static class XPoinExtensions
    {
        public static bool PointsAreAlmostEqual(XPoint point1, XPoint point2, double tolerance = 1e-4)
        {
            var v = point2 - point1;

            return Math.Abs(v.X) <= tolerance && Math.Abs(v.Y) <= tolerance;
        }
    }
}
