using IDE.Core.Types.Media;
using System;

namespace IDE.Core
{
    public static class SnapHelper
    {
        public static double SnapToGrid(double position, double gridSize)
        {
            var countsX = Math.Truncate(position / gridSize);
            var deltaX = Math.Abs(position - countsX * gridSize);

            double snapX = 0;

            if (deltaX <= (gridSize / 2))
                snapX = countsX * gridSize;
            else
                snapX = (countsX + 1) * gridSize;

            return snapX;
        }

        public static XPoint SnapToGrid(XPoint position, double gridSize)
        {
            return new XPoint(SnapToGrid(position.X, gridSize), SnapToGrid(position.Y, gridSize));
        }
    }
}
