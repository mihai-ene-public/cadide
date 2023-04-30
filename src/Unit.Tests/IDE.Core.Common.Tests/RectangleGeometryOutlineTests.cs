using System;
using Xunit;
using IDE.Core.Common.Geometries;
using IDE.Core.Types.Media;
using System.Net.WebSockets;

namespace IDE.Core.Common.Tests;

public class RectangleGeometryOutlineTests
{
    [Theory]
    [InlineData(0, 0, 10, 10, 5)]
    public void NumberPointsNoCornersPass(double centerX, double centerY, double width, double height, int expectedPoints)
    {
        var rect = new RectangleGeometryOutline(centerX, centerY, width, height);

        var actualPoints = rect.GetOutline();

        Assert.Equal(expectedPoints, actualPoints.Count);
    }

    [Theory]
    [InlineData(0, 0, 10, 10, 1, 16)]//StandardNumberOfSegmentsPerCircle + 1
    public void NumberPointsCornersPass(double centerX, double centerY, double width, double height, double cornerRadius, int segmentsPerCircle)
    {
        var rect = new RectangleGeometryOutline(centerX, centerY, width, height, cornerRadius, segmentsPerCircle);

        var actualPoints = rect.GetOutline();
        var expected = segmentsPerCircle
                        + 4 //4 corners
                        + 1 //1 point added last to close the loop 
                        ;

        //check points on the radius at the 45 deg angle

        //top-left corner
        var cornerPointsNumber = segmentsPerCircle / 4 + 1;
        var cornerPointIndex = 2;
        var dx = 0.5 * width - cornerRadius * (1.0 - Math.Sin(0.25 * Math.PI));
        var dy = 0.5 * height - cornerRadius * (1.0 - Math.Cos(0.25 * Math.PI));
        var topLeftCornerPoint = new XPoint(centerX - dx, centerY - dy);
        Assert.Equal(topLeftCornerPoint, actualPoints[cornerPointIndex]);

        //top-right corner
        cornerPointIndex += cornerPointsNumber;
        var topRightCorner = new XPoint(centerX + dx, centerY - dy);
        Assert.Equal(topRightCorner, actualPoints[cornerPointIndex]);

        //bottom-right corner
        cornerPointIndex += cornerPointsNumber;
        var bottomRightCorner = new XPoint(centerX + dx, centerY + dy);
        Assert.Equal(bottomRightCorner, actualPoints[cornerPointIndex]);

        //bottom-left corner
        cornerPointIndex += cornerPointsNumber;
        var bottomLeftCorner = new XPoint(centerX - dx, centerY + dy);
        Assert.Equal(bottomLeftCorner, actualPoints[cornerPointIndex]);

        Assert.Equal(expected, actualPoints.Count);
    }

    [Theory]
    [InlineData(0, 0, 10, 10)]
    public void CheckPointsNoCornersPass(double centerX, double centerY, double width, double height)
    {
        //check the corners as key points

        var rect = new RectangleGeometryOutline(centerX, centerY, width, height);

        var actualPoints = rect.GetOutline();

        //first point equals last point
        Assert.True(actualPoints[0] == actualPoints[actualPoints.Count - 1]);

        //top-left corner
        var topLeftCornerPoint = new XPoint(centerX - 0.5 * width, centerY - 0.5 * height);
        Assert.Equal(topLeftCornerPoint, actualPoints[0]);

        //top-right corner
        var topRightCorner = new XPoint(centerX + 0.5 * width, centerY - 0.5 * height);
        Assert.Equal(topRightCorner, actualPoints[1]);

        //bottom-right corner
        var bottomRightCorner = new XPoint(centerX + 0.5 * width, centerY + 0.5 * height);
        Assert.Equal(bottomRightCorner, actualPoints[2]);

        //bottom-left corner
        var bottomLeftCorner = new XPoint(centerX - 0.5 * width, centerY + 0.5 * height);
        Assert.Equal(bottomLeftCorner, actualPoints[3]);
    }


}
