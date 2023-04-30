using IDE.Core.Types.Media;
using System;

namespace IDE.Core;

public static class SnapHelper
{

    public static XPoint SnapToGrid(XPoint position, double gridSize)
    {
        var absX = Math.Abs(position.X);
        var signX = Math.Sign(position.X);

        var absY = Math.Abs(position.Y);
        var signY = Math.Sign(position.Y);

        var countsX = Math.Truncate(absX / gridSize);
        var deltaX = absX - countsX * gridSize;

        var countsY = Math.Truncate(absY / gridSize);
        var deltaY = absY - countsY * gridSize;

        double snapX = 0;
        double snapY = 0;

        if (deltaX <= (gridSize * 0.5))
            snapX = countsX * gridSize;
        else
            snapX = (countsX + 1) * gridSize;

        if (deltaY <= (gridSize * 0.5))
            snapY = countsY * gridSize;
        else
            snapY = (countsY + 1) * gridSize;

        return new XPoint(signX * snapX, signY * snapY);
    }
}
