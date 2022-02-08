using IDE.Core.Designers;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IDE.Core.Presentation.Tests;

public class PolygonCanvasItemTests
{
    private PolygonCanvasItem CreateItem(double borderWidth = 0.5)
    {
        var points = new List<XPoint>
            {
                new XPoint(10,0),
                new XPoint(10,5),
                new XPoint(0,5),
                new XPoint(0,0),
                new XPoint(10,0),
            };

        var item = new PolygonCanvasItem
        {
            PolygonPoints = points,
            BorderWidth = borderWidth
        };

        return item;
    }

    [Fact]
    public void GetBoundingRectangle()
    {

        var item = CreateItem();

        var expected = new XRect(new XPoint(0, 0), new XPoint(10, 5));

        var actual = item.GetBoundingRectangle();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1, 1)]
    public void Translate(double dx, double dy)
    {
        var item = CreateItem();

        var oldPoints = item.PolygonPoints.ToList();

        item.Translate(dx, dy);

        for (int i = 0; i < item.PolygonPoints.Count; i++)
        {
            var p = item.PolygonPoints[i];
            var op = oldPoints[i];
            Assert.Equal(dx, p.X - op.X);
            Assert.Equal(dy, p.Y - op.Y);
        }

    }

    [Theory]
    //translate only
    [InlineData(1, 1, 0, 1, 1, 11, 1)]
    public void TransformBy(
                            double scaleX, double scaleY, double rot, double tx, double ty,
                            double expectedX, double expectedY)
    {
        var item = CreateItem();

        var tg = new XTransformGroup();

        var scaleTransform = new XScaleTransform(scaleX, scaleY);
        tg.Children.Add(scaleTransform);

        var rotateTransform = new XRotateTransform(rot);
        tg.Children.Add(rotateTransform);

        tg.Children.Add(new XTranslateTransform(tx, ty));

        item.TransformBy(tg.Value);

        //only the startpoint
        Assert.Equal(expectedX, item.PolygonPoints[0].X);
        Assert.Equal(expectedY, item.PolygonPoints[0].Y);
    }

    [Fact]
    public void MirrorX()
    {
        var item = CreateItem();

        item.MirrorX();

        Assert.Equal(-1, item.ScaleX);
    }

    [Fact]
    public void MirrorY()
    {
        var item = CreateItem();

        item.MirrorY();

        Assert.Equal(-1, item.ScaleY);
    }

    [Theory]
    [InlineData(7.5, 7.5)]
    public void Rotate(double expectedX, double expectedY)
    {
        var item = CreateItem();

        item.Rotate();

        //only the startpoint
        Assert.Equal(expectedX, item.PolygonPoints[0].X);
        Assert.Equal(expectedY, item.PolygonPoints[0].Y);
    }
}
