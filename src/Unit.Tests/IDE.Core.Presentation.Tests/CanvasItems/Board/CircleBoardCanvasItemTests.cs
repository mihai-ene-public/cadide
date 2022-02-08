using IDE.Core.Designers;
using IDE.Core.Types.Media;
using Xunit;

namespace IDE.Core.Presentation.Tests.CanvasItems.Board
{
    public class CircleBoardCanvasItemTests
    {
        [Theory]
        [InlineData(0, 0, 10, -5, -5, 10, 10)]
        public void GetBoundingRectangle(double x, double y, double diameter,
                                        double expectedRectX, double expectedRectY, double expectedRectWidth, double expectedRectHeight)
        {
            var item = new CircleBoardCanvasItem
            {
                X = x,
                Y = y,
                Diameter = diameter,
                BorderWidth = 0.5
            };

            var expected = new XRect(expectedRectX, expectedRectY, expectedRectWidth, expectedRectHeight);

            var actual = item.GetBoundingRectangle();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0, 10, 1, 1)]
        public void Translate(double x, double y, double diameter, double dx, double dy)
        {
            var item = new CircleBoardCanvasItem
            {
                X = x,
                Y = y,
                Diameter = diameter,
                BorderWidth = 0.5
            };

            item.Translate(dx, dy);

            Assert.Equal(dx, item.X - x);
            Assert.Equal(dy, item.Y - y);
        }

        [Theory]
        //translate only
        [InlineData(0, 0, 10, 1, 1, 0, 1, 1, 1, 1)]
        public void TransformBy(double x, double y, double diameter,
                                double scaleX, double scaleY, double rot, double tx, double ty,
                                double expectedX, double expectedY)
        {
            var item = new CircleBoardCanvasItem
            {
                X = x,
                Y = y,
                Diameter = diameter,
                BorderWidth = 0.5
            };

            var tg = new XTransformGroup();

            var scaleTransform = new XScaleTransform(scaleX, scaleY);
            tg.Children.Add(scaleTransform);

            var rotateTransform = new XRotateTransform(rot);
            tg.Children.Add(rotateTransform);

            tg.Children.Add(new XTranslateTransform(tx, ty));

            item.TransformBy(tg.Value);

            Assert.Equal(expectedX, item.X);
            Assert.Equal(expectedY, item.Y);
        }

        [Fact]
        public void MirrorX()
        {
            var item = new CircleBoardCanvasItem
            {
                X = 0,
                Y = 0,
                Diameter = 10,
                BorderWidth = 0.5
            };

            item.MirrorX();

            Assert.Equal(1, item.ScaleX);
        }

        [Fact]
        public void MirrorY()
        {
            var item = new CircleBoardCanvasItem
            {
                X = 0,
                Y = 0,
                Diameter = 10,
                BorderWidth = 0.5
            };

            item.MirrorY();

            Assert.Equal(1, item.ScaleY);
        }

        [Theory]
        [InlineData(0, 0, 10, 0, 0)]   //E
        public void Rotate(double x, double y, double diameter,
                           double expectedX, double expectedY)
        {
            var item = new CircleBoardCanvasItem
            {
                X = x,
                Y = y,
                Diameter = diameter,
                BorderWidth = 0.5,
                IsPlaced = true,
            };

            item.Rotate();

            Assert.Equal(expectedX, item.X);
            Assert.Equal(expectedY, item.Y);
        }
    }
}
