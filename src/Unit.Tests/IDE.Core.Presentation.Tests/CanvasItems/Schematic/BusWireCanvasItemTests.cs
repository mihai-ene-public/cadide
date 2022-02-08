using IDE.Core.Designers;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using Xunit;

namespace IDE.Core.Presentation.Tests
{
    public class BusWireCanvasItemTests
    {
        [Fact]
        public void GetBoundingRectangle()
        {
            var item = new BusWireCanvasItem
            {
                Points = new List<XPoint>
                {
                    new XPoint(0,0),
                    new XPoint(0,5),
                    new XPoint(1,6),
                    new XPoint(5,6),
                },
                Width = 0.5
            };

            var expected = new XRect(new XPoint(), new XPoint(5, 6));
            expected.Inflate(0.5 * item.Width);

            var actual = item.GetBoundingRectangle();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1, 1, 1)]
        public void Translate(double x, double y, double dx, double dy)
        {
            var sp = new XPoint(0, 0);
            var ep = new XPoint(x, y);
            var item = new BusWireCanvasItem
            {
                Points = new List<XPoint>
                {
                    sp,
                    ep,
                },
                Width = 0.5
            };

            item.Translate(dx, dy);

            Assert.Equal(dx, item.StartPoint.X - sp.X);
            Assert.Equal(dy, item.StartPoint.Y - sp.Y);
            Assert.Equal(dx, item.EndPoint.X - ep.X);
            Assert.Equal(dy, item.EndPoint.Y - ep.Y);
        }

        [Theory]
        [InlineData(1, 0, 1, 1, 0, 1, 1, 1, 1, 2, 1)]//E
        public void TransformBy(double x, double y,
                               double scaleX, double scaleY, double tRot, double tx, double ty,
                               double expectedSpX, double expectedSpY, double expectedEpX, double expectedEpY)
        {

            var sp = new XPoint(0, 0);
            var ep = new XPoint(x, y);
            var item = new BusWireCanvasItem
            {
                Points = new List<XPoint>
                {
                    sp,
                    ep,
                },
                Width = 0.5
            };

            var tg = new XTransformGroup();

            var scaleTransform = new XScaleTransform(scaleX, scaleY);
            tg.Children.Add(scaleTransform);

            var rotateTransform = new XRotateTransform(tRot);
            tg.Children.Add(rotateTransform);

            tg.Children.Add(new XTranslateTransform(tx, ty));

            item.TransformBy(tg.Value);

            Assert.Equal(expectedSpX, item.StartPoint.X);
            Assert.Equal(expectedSpY, item.StartPoint.Y);
            Assert.Equal(expectedEpX, item.EndPoint.X);
            Assert.Equal(expectedEpY, item.EndPoint.Y);
        }

        [Fact]
        public void MirrorX()
        {
            var sp = new XPoint(0, 0);
            var ep = new XPoint(1, 1);
            var item = new BusWireCanvasItem
            {
                Points = new List<XPoint>
                {
                    sp,
                    ep,
                },
                Width = 0.5
            };

            item.MirrorX();

            Assert.Equal(1, item.ScaleX);
            Assert.Equal(0, item.StartPoint.X);
            Assert.Equal(0, item.StartPoint.Y);
            Assert.Equal(1, item.EndPoint.X);
            Assert.Equal(1, item.EndPoint.Y);
        }

        [Fact]
        public void MirrorY()
        {
            var sp = new XPoint(0, 0);
            var ep = new XPoint(1, 1);
            var item = new BusWireCanvasItem
            {
                Points = new List<XPoint>
                {
                    sp,
                    ep,
                },
                Width = 0.5
            };

            item.MirrorY();

            Assert.Equal(1, item.ScaleY);
            Assert.Equal(0, item.StartPoint.X);
            Assert.Equal(0, item.StartPoint.Y);
            Assert.Equal(1, item.EndPoint.X);
            Assert.Equal(1, item.EndPoint.Y);
        }

        [Fact]
        public void Rotate()
        {
            var sp = new XPoint(0, 0);
            var ep = new XPoint(1, 1);
            var item = new BusWireCanvasItem
            {
                Points = new List<XPoint>
                {
                    sp,
                    ep,
                },
                Width = 0.5
            };

            item.Rotate();

            Assert.Equal(1, item.ScaleY);
            Assert.Equal(0, item.StartPoint.X);
            Assert.Equal(0, item.StartPoint.Y);
            Assert.Equal(1, item.EndPoint.X);
            Assert.Equal(1, item.EndPoint.Y);
        }
    }
}
