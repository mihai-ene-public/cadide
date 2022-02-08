using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using Xunit;

namespace IDE.Core.Presentation.Tests
{
    public class PinCanvasItemTests
    {
        [Theory]
        [InlineData(0, 0, pinOrientation.Left, -0.25, -0.25, 2.54 + 0.5, 0.5)]
        [InlineData(0, 0, pinOrientation.Right, -2.79, -0.25, 2.54 + 0.5, 0.5)]
        [InlineData(0, 0, pinOrientation.Up, -0.25, -0.25, 0.5, 2.54 + 0.5)]
        [InlineData(0, 0, pinOrientation.Down, -0.25, -2.79, 0.5, 2.54 + 0.5)]

        [InlineData(5, 5, pinOrientation.Left, 5 - 0.25, 5 - 0.25, 2.54 + 0.5, 0.5)]
        [InlineData(5, 5, pinOrientation.Right, 5 - 2.79, 5 - 0.25, 2.54 + 0.5, 0.5)]
        [InlineData(5, 5, pinOrientation.Up, 5 - 0.25, 5 - 0.25, 0.5, 2.54 + 0.5)]
        [InlineData(5, 5, pinOrientation.Down, 5 - 0.25, 5 - 2.79, 0.5, 2.54 + 0.5)]
        public void GetBoundingRectangle(double x, double y, pinOrientation orientation,
                                         double expectedRectX, double expectedRectY, double expectedRectWidth, double expectedRectHeight)
        {
            var item = new PinCanvasItem
            {
                X = x,
                Y = y,
                Orientation = orientation,
                Width = 0.5,
                PinLength = 2.54
            };

            var expected = new XRect(expectedRectX, expectedRectY, expectedRectWidth, expectedRectHeight);

            var actual = item.GetBoundingRectangle();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0, 1, 1)]
        public void Translate(double x, double y, double dx, double dy)
        {
            var item = new PinCanvasItem
            {
                X = x,
                Y = y,
                Width = 0.5,
                PinLength = 2.54
            };

            item.Translate(dx, dy);

            Assert.Equal(dx, item.X - x);
            Assert.Equal(dy, item.Y - y);
        }

        [Theory]
        //translate only
        [InlineData(0, 0, pinOrientation.Left, 1, 1, 0, 1, 1, 1, 1, pinOrientation.Left)]
        public void TransformBy(double x, double y, pinOrientation orientation,
                                double scaleX, double scaleY, double rot, double tx, double ty,
                                double expectedX, double expectedY, pinOrientation expectedOrientation)
        {
            var item = new PinCanvasItem
            {
                X = x,
                Y = y,
                Orientation = orientation,
                Width = 0.5,
                PinLength = 2.54
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
            Assert.Equal(expectedOrientation, item.Orientation);
        }

        [Theory]
        [InlineData(pinOrientation.Left, pinOrientation.Right)]
        [InlineData(pinOrientation.Right, pinOrientation.Left)]
        [InlineData(pinOrientation.Up, pinOrientation.Up)]
        [InlineData(pinOrientation.Down, pinOrientation.Down)]
        public void MirrorX(pinOrientation orientation, pinOrientation expectedOrientation)
        {
            var item = new PinCanvasItem
            {
                X = 0,
                Y = 0,
                Orientation = orientation,
                Width = 0.5,
                PinLength = 2.54
            };

            item.MirrorX();

            Assert.Equal(1, item.ScaleX);
            Assert.Equal(expectedOrientation, item.Orientation);
        }

        [Theory]
        [InlineData(pinOrientation.Left, pinOrientation.Left)]
        [InlineData(pinOrientation.Right, pinOrientation.Right)]
        [InlineData(pinOrientation.Up, pinOrientation.Down)]
        [InlineData(pinOrientation.Down, pinOrientation.Up)]
        public void MirrorY(pinOrientation orientation, pinOrientation expectedOrientation)
        {
            var item = new PinCanvasItem
            {
                X = 0,
                Y = 0,
                Orientation = orientation,
                Width = 0.5,
                PinLength = 2.54
            };

            item.MirrorY();

            Assert.Equal(1, item.ScaleY);
            Assert.Equal(expectedOrientation, item.Orientation);
        }

        [Theory]
        [InlineData(0, 0, pinOrientation.Left, 0, 0, pinOrientation.Up)]
        [InlineData(0, 0, pinOrientation.Up, 0, 0, pinOrientation.Right)]
        [InlineData(0, 0, pinOrientation.Right, 0, 0, pinOrientation.Down)]
        [InlineData(0, 0, pinOrientation.Down, 0, 0, pinOrientation.Left)]
        public void Rotate(double x, double y, pinOrientation orientation,
                           double expectedX, double expectedY, pinOrientation expectedOrientation)
        {
            var item = new PinCanvasItem
            {
                X = x,
                Y = y,
                Orientation = orientation,
                Width = 0.5,
                PinLength = 2.54
            };

            item.Rotate();

            Assert.Equal(expectedX, item.X);
            Assert.Equal(expectedY, item.Y);
            Assert.Equal(expectedOrientation, item.Orientation);
        }
    }
}
