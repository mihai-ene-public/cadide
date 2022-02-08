using IDE.Core.Designers;
using IDE.Core.Types.Media;
using Xunit;

namespace IDE.Core.Presentation.Tests.CanvasItems.Board
{
    public class TextBoardCanvasItemTests
    {
        [Theory]
        [InlineData(0, 0, 10, "Text", false, 0, 0, 4.9155, 2.8553)]
        [InlineData(0, 0, 20, "Text", true, 0, 0, 20, 2.8553)]
        public void GetBoundingRectangle(double x, double y, double width, string text, bool wordWrap,
             double expectedRectX, double expectedRectY, double expectedRectWidth, double expectedRectHeight)
        {
            var item = new TextBoardCanvasItem
            {
                X = x,
                Y = y,
                Text = text,
                Width = width,
                FontSize = 8,
                WordWrap = wordWrap,
            };

            var expected = new XRect(expectedRectX, expectedRectY, expectedRectWidth, expectedRectHeight);

            var actual = item.GetBoundingRectangle();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0, 5, 1, 1)]
        public void Translate(double x, double y, double width, double dx, double dy)
        {
            var item = new TextBoardCanvasItem
            {
                X = x,
                Y = y,
                Width = width,
            };

            item.Translate(dx, dy);

            Assert.Equal(dx, item.X - x);
            Assert.Equal(dy, item.Y - y);
        }

        [Theory]
        //translate only
        [InlineData(0, 0, 5, 1, 1, 0, 1, 1, 1, 1)]
        public void TransformBy(double x, double y, double width,
                                double scaleX, double scaleY, double rot, double tx, double ty,
                                double expectedX, double expectedY)
        {
            var item = new TextBoardCanvasItem
            {
                X = x,
                Y = y,
                Width = width,
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
            var item = new TextBoardCanvasItem
            {
                X = 0,
                Y = 0,
            };

            item.MirrorX();

            Assert.Equal(-1, item.ScaleX);
        }

        [Fact]
        public void MirrorY()
        {
            var item = new TextBoardCanvasItem
            {
                X = 0,
                Y = 0,
            };

            item.MirrorY();

            Assert.Equal(-1, item.ScaleY);
        }

        [Theory]
        [InlineData(0, 0, 5, 0, 0, 0, 90)]
        [InlineData(0, 0, 5, 90, 0, 0, 180)]
        [InlineData(0, 0, 5, 180, 0, 0, 270)]
        [InlineData(0, 0, 5, 270, 0, 0, 0)]
        public void Rotate(double x, double y, double width, double rot,
                           double expectedX, double expectedY, double expectedRot)
        {
            var item = new TextBoardCanvasItem
            {
                X = x,
                Y = y,
                Width = width,
                Rot = rot,
            };

            item.Rotate();

            Assert.Equal(expectedX, item.X);
            Assert.Equal(expectedY, item.Y);
            Assert.Equal(expectedRot, item.Rot);
        }

    }
}
