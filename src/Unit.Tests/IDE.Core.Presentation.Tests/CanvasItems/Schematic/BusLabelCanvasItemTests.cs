using IDE.Core.Designers;
using IDE.Core.Types.Media;
using Xunit;

namespace IDE.Core.Presentation.Tests
{
    public class BusLabelCanvasItemTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(10, 10)]
        public void GetBoundingRectangle(double x, double y)
        {
            //default busName: "Not assigned"
            var item = new BusLabelCanvasItem
            {
                X = x,
                Y = y
            };

            var expected = new XRect(x, y, 16.4055, 3.4864);
            var actual = item.GetBoundingRectangle();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0, 1, 1)]
        [InlineData(0, 0, -1, -1)]
        public void Translate(double x, double y, double dx, double dy)
        {
            var item = new BusLabelCanvasItem
            {
                X = x,
                Y = y
            };

            item.Translate(dx, dy);

            Assert.Equal(dx, item.X - x);
            Assert.Equal(dy, item.Y - y);
        }

        [Theory]
        [InlineData(0, 0, 0, 1, 1, 0, 1, 1, 1, 1, 0)]
        public void TransformBy(double x, double y, double rot,
                                double scaleX, double scaleY, double tRot, double tx, double ty,
                                double expectedX, double expectedY, double expectedRot)
        {
            var item = new BusLabelCanvasItem
            {
                X = x,
                Y = y,
                Rot = rot
            };

            var tg = new XTransformGroup();

            var scaleTransform = new XScaleTransform(scaleX, scaleY);
            tg.Children.Add(scaleTransform);

            var rotateTransform = new XRotateTransform(tRot);
            tg.Children.Add(rotateTransform);

            tg.Children.Add(new XTranslateTransform(tx, ty));

            item.TransformBy(tg.Value);

            Assert.Equal(expectedX, item.X);
            Assert.Equal(expectedY, item.Y);
            Assert.Equal(expectedRot, item.Rot);
        }

        [Fact]
        public void MirrorX()
        {
            var item = new BusLabelCanvasItem
            {
                X = 0,
                Y = 0,
                Rot = 0
            };

            item.MirrorX();

            Assert.Equal(-1, item.ScaleX);
        }

        [Fact]
        public void MirrorY()
        {
            var item = new BusLabelCanvasItem
            {
                X = 0,
                Y = 0,
                Rot = 0
            };

            item.MirrorY();

            Assert.Equal(-1, item.ScaleY);
        }

        [Theory]
        [InlineData(0, 90)]
        [InlineData(90, 180)]
        [InlineData(180, 270)]
        [InlineData(270, 0)]
        public void Rotate(double rot, double expectedRot)
        {
            var item = new BusLabelCanvasItem
            {
                X = 0,
                Y = 0,
                Rot = rot
            };

            item.Rotate();

            Assert.Equal(expectedRot, item.Rot);
        }
    }
}
