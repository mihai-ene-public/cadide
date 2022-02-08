using IDE.Core.Designers;
using IDE.Core.Types.Media;
using Xunit;

namespace IDE.Core.Presentation.Tests.CanvasItems.Board
{
    public class LineBoardCanvasItemTests
    {
        [Theory]
        [InlineData(-5, 0, 5, 0, -5.25, -0.25, 10 + 0.5, 0.5)]   //E
        [InlineData(5, 0, -5, 0, -5.25, -0.25, 10 + 0.5, 0.5)]   //W
        [InlineData(0, 5, 0, -5, -0.25, -5.25, 0.5, 10 + 0.5)]   //N
        [InlineData(0, -5, 0, 5, -0.25, -5.25, 0.5, 10 + 0.5)]   //S
        public void GetBoundingRectangle(double spX, double spY, double epX, double epY,
                                       double expectedRectX, double expectedRectY, double expectedRectWidth, double expectedRectHeight)
        {
            var item = new LineBoardCanvasItem
            {
                X1 = spX,
                Y1 = spY,
                X2 = epX,
                Y2 = epY,
                Width = 0.5
            };

            var expected = new XRect(expectedRectX, expectedRectY, expectedRectWidth, expectedRectHeight);

            var actual = item.GetBoundingRectangle();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0, 10, 0, 1, 1)]
        public void Translate(double spX, double spY, double epX, double epY, double dx, double dy)
        {
            var item = new LineBoardCanvasItem
            {
                X1 = spX,
                Y1 = spY,
                X2 = epX,
                Y2 = epY,
                Width = 0.5
            };

            item.Translate(dx, dy);

            Assert.Equal(dx, item.X1 - spX);
            Assert.Equal(dy, item.Y1 - spY);
            Assert.Equal(dx, item.X2 - epX);
            Assert.Equal(dy, item.Y2 - epY);
        }

        [Theory]
        //translate only
        [InlineData(-5, 0, 5, 0, 1, 1, 0, 1, 1, -4, 1, 6, 1)]   //semi top
        [InlineData(5, 0, -5, 0, 1, 1, 0, 1, 1, 6, 1, -4, 1)]   //semi-bottom
        [InlineData(0, 5, 0, -5, 1, 1, 0, 1, 1, 1, 6, 1, -4)]   //semi-left
        [InlineData(0, -5, 0, 5, 1, 1, 0, 1, 1, 1, -4, 1, 6)]   //semi-right
        [InlineData(-5, 0, 0, -5, 1, 1, 0, 1, 1, -4, 1, 1, -4)]  //quad-top-left
        [InlineData(0, -5, 5, 0, 1, 1, 0, 1, 1, 1, -4, 6, 1)]    //quad-top-right
        [InlineData(0, 5, -5, 0, 1, 1, 0, 1, 1, 1, 6, -4, 1)]  //quad-bottom-left
        [InlineData(5, 0, 0, 5, 1, 1, 0, 1, 1, 6, 1, 1, 6)]    //quad-bottom-right
        public void TransformBy(double spX, double spY, double epX, double epY,
                                double scaleX, double scaleY, double rot, double tx, double ty,
                                double expectedSpX, double expectedSpY, double expectedEpX, double expectedEpY)
        {
            var item = new LineBoardCanvasItem
            {
                X1 = spX,
                Y1 = spY,
                X2 = epX,
                Y2 = epY,
                Width = 0.5
            };

            var tg = new XTransformGroup();

            var scaleTransform = new XScaleTransform(scaleX, scaleY);
            tg.Children.Add(scaleTransform);

            var rotateTransform = new XRotateTransform(rot);
            tg.Children.Add(rotateTransform);

            tg.Children.Add(new XTranslateTransform(tx, ty));

            item.TransformBy(tg.Value);

            Assert.Equal(expectedSpX, item.X1);
            Assert.Equal(expectedSpY, item.Y1);
            Assert.Equal(expectedEpX, item.X2);
            Assert.Equal(expectedEpY, item.Y2);
        }

        [Fact]
        public void MirrorX()
        {
            var item = new LineBoardCanvasItem
            {
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = 2,
                Width = 0.5
            };

            item.MirrorX();

            Assert.Equal(-1, item.ScaleX);
        }

        [Fact]
        public void MirrorY()
        {
            var item = new LineBoardCanvasItem
            {
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = 2,
                Width = 0.5
            };

            item.MirrorY();

            Assert.Equal(-1, item.ScaleY);
        }

        [Theory]
        [InlineData(-5, 0, 5, 0, 0, -5, 0, 5)]   //E
        [InlineData(5, 0, -5, 0, 0, 5, 0, -5)]   //W
        [InlineData(0, 5, 0, -5, -5, 0, 5, 0)]   //N
        [InlineData(0, -5, 0, 5, 5, 0, -5, 0)]   //S
        public void Rotate(double spX, double spY, double epX, double epY,
                           double expectedSpX, double expectedSpY, double expectedEpX, double expectedEpY)
        {
            var item = new LineBoardCanvasItem
            {
                X1 = spX,
                Y1 = spY,
                X2 = epX,
                Y2 = epY,
                Width = 0.5,
                IsPlaced = true,
            };

            item.Rotate();

            Assert.Equal(expectedSpX, item.X1);
            Assert.Equal(expectedSpY, item.Y1);
            Assert.Equal(expectedEpX, item.X2);
            Assert.Equal(expectedEpY, item.Y2);
        }
    }
}
