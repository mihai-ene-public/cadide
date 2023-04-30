using IDE.Core.Types.Media;
using Xunit;

namespace IDE.Core.Common.Tests;

public class SnapHelperTests
{
    [Theory]
    [InlineData(0.00, -2.00, 0.00, -2.00)]
    [InlineData(0.00, -1.75, 0.00, -2.00)]
    [InlineData(0.00, -1.50, 0.00, -1.00)]
    [InlineData(0.00, -1.25, 0.00, -1.00)]
    [InlineData(0.00, -1.00, 0.00, -1.00)]
    [InlineData(0.00, -0.75, 0.00, -1.00)]
    [InlineData(0.00, -0.50, 0.00, 0.00)]
    [InlineData(0.00, -0.25, 0.00, 0.00)]
    [InlineData(0.00, 0.00, 0.00, 0.00)]
    [InlineData(0.00, 0.25, 0.00, 0.00)]
    [InlineData(0.00, 0.50, 0.00, 0.00)]
    [InlineData(0.00, 0.75, 0.00, 1.00)]
    [InlineData(0.00, 1.00, 0.00, 1.00)]
    [InlineData(0.00, 1.25, 0.00, 1.00)]
    [InlineData(0.00, 1.50, 0.00, 1.00)]
    [InlineData(0.00, 1.75, 0.00, 2.00)]
    [InlineData(0.00, 2.00, 0.00, 2.00)]

    [InlineData(-2.00, 0.00, -2.00, 0.00)]
    [InlineData(-1.75, 0.00, -2.00, 0.00)]
    [InlineData(-1.50, 0.00, -1.00, 0.00)]
    [InlineData(-1.25, 0.00, -1.00, 0.00)]
    [InlineData(-1.00, 0.00, -1.00, 0.00)]
    [InlineData(-0.75, 0.00, -1.00, 0.00)]
    [InlineData(-0.50, 0.00, 0.00, 0.00)]
    [InlineData(-0.25, 0.00, 0.00, 0.00)]
    [InlineData(0.25, 0.00, 0.00, 0.00)]
    [InlineData(0.50, 0.00, 0.00, 0.00)]
    [InlineData(0.75, 0.00, 1.00, 0.00)]
    [InlineData(1.00, 0.00, 1.00, 0.00)]
    [InlineData(1.25, 0.00, 1.00, 0.00)]
    [InlineData(1.50, 0.00, 1.00, 0.00)]
    [InlineData(1.75, 0.00, 2.00, 0.00)]
    [InlineData(2.00, 0.00, 2.00, 0.00)]
    public void SnapToGridReturnsSnapPoint(double x, double y, double expectedX, double expectedY)
    {

        var actual = SnapHelper.SnapToGrid(new XPoint(x, y), 1.00d);

        Assert.Equal(expectedX, actual.X);
        Assert.Equal(expectedY, actual.Y);
    }
}