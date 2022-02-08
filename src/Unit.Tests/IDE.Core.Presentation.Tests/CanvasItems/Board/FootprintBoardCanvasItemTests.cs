using IDE.Core.Designers;
using IDE.Core.Types.Media;
using Xunit;

namespace IDE.Core.Presentation.Tests.CanvasItems.Board
{
    public class FootprintBoardCanvasItemTests
    {
        [Theory]
        [InlineData(0, 0, 5, 10, -2.5, -5, 5, 10)]
        public void GetBoundingRectangle(double x, double y, double width, double height,
                                    double expectedRectX, double expectedRectY, double expectedRectWidth, double expectedRectHeight)
        {
            var item = new FootprintBoardCanvasItem
            {
                X = x,
                Y = y,
                DisplayWidth = width,
                DisplayHeight = height
            };

            var expected = new XRect(expectedRectX, expectedRectY, expectedRectWidth, expectedRectHeight);

            var actual = item.GetBoundingRectangle();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0, 5, 10, 1, 1)]
        public void Translate(double x, double y, double width, double height, double dx, double dy)
        {
            var item = new FootprintBoardCanvasItem
            {
                X = x,
                Y = y,
                DisplayWidth = width,
                DisplayHeight = height
            };

            item.Translate(dx, dy);

            Assert.Equal(dx, item.X - x);
            Assert.Equal(dy, item.Y - y);
        }

        [Theory]
        //translate only
        [InlineData(0, 0, 5, 10, 1, 1, 0, 1, 1, 1, 1)]
        public void TransformBy(double x, double y, double width, double height,
                                double scaleX, double scaleY, double rot, double tx, double ty,
                                double expectedX, double expectedY)
        {
            var item = new FootprintBoardCanvasItem
            {
                X = x,
                Y = y,
                DisplayWidth = width,
                DisplayHeight = height
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
            var item = new FootprintBoardCanvasItem
            {
            };

            item.MirrorX();

            Assert.Equal(1, item.ScaleX);
        }

        [Fact]
        public void MirrorY()
        {
            var item = new FootprintBoardCanvasItem
            {
            };

            item.MirrorY();

            Assert.Equal(1, item.ScaleY);
        }

        [Theory]
        [InlineData(0, 0, 5, 10, 0, 0, 0, 90)]
        [InlineData(0, 0, 5, 10, 90, 0, 0, 180)]
        [InlineData(0, 0, 5, 10, 180, 0, 0, 270)]
        [InlineData(0, 0, 5, 10, 270, 0, 0, 0)]
        public void Rotate(double x, double y, double width, double height, double rot,
                           double expectedX, double expectedY, double expectedRot)
        {
            var item = new FootprintBoardCanvasItem
            {
                X = x,
                Y = y,
                DisplayWidth = width,
                DisplayHeight = height,
                Rot = rot,
            };

            item.Rotate();

            Assert.Equal(expectedX, item.X);
            Assert.Equal(expectedY, item.Y);
            Assert.Equal(expectedRot, item.Rot);
        }
    }
}
