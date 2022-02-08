using IDE.Core.Designers;
using IDE.Core.Types.Media;
using Xunit;

namespace IDE.Core.Presentation.Tests
{
    public class ShematicArcCanvasItemTests
    {
        [Theory]
        [InlineData(-5, 0, 5, 0, XSweepDirection.Clockwise, -5.25, -5.25, 10 + 0.5, 5 + 0.5)]   //semi top
        [InlineData(5, 0, -5, 0, XSweepDirection.Clockwise, -5.25, -0.25, 10 + 0.5, 5 + 0.5)]   //semi-bottom
        [InlineData(0, 5, 0, -5, XSweepDirection.Clockwise, -5.25, -5.25, 5 + 0.5, 10 + 0.5)]   //semi-left
        [InlineData(0, -5, 0, 5, XSweepDirection.Clockwise, -0.25, -5.25, 5 + 0.5, 10 + 0.5)]   //semi-right
        [InlineData(-5, 0, 0, -5, XSweepDirection.Clockwise, -5.25, -5.25, 5 + 0.5, 5 + 0.5)]   //quad-top-left
        [InlineData(0, -5, 5, 0, XSweepDirection.Clockwise, -0.25, -5.25, 5 + 0.5, 5 + 0.5)]    //quad-top-right
        [InlineData(0, 5, -5, 0, XSweepDirection.Clockwise, -5.25, -0.25, 5 + 0.5, 5 + 0.5)]    //quad-bottom-left
        [InlineData(5, 0, 0, 5, XSweepDirection.Clockwise, -0.25, -0.25, 5 + 0.5, 5 + 0.5)]     //quad-bottom-right
        public void GetBoundingRectangle(double spX, double spY, double epX, double epY, XSweepDirection xSweep,
                                        double expectedRectX, double expectedRectY, double expectedRectWidth, double expectedRectHeight)
        {
            var arc = new ArcCanvasItem
            {
                StartPointX = spX,
                StartPointY = spY,
                EndPointX = epX,
                EndPointY = epY,
                Radius = 5,
                BorderWidth = 0.5,
                SweepDirection = xSweep
            };

            var expected = new XRect(expectedRectX, expectedRectY, expectedRectWidth, expectedRectHeight);

            var actual = arc.GetBoundingRectangle();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0, 10, 0, 1, 1)]
        public void Translate(double spX, double spY, double epX, double epY, double dx, double dy)
        {
            var arc = new ArcCanvasItem
            {
                StartPointX = spX,
                StartPointY = spY,
                EndPointX = epX,
                EndPointY = epY,
                Radius = 5,
                BorderWidth = 0.5
            };

            arc.Translate(dx, dy);

            Assert.Equal(dx, arc.StartPointX - spX);
            Assert.Equal(dy, arc.StartPointY - spY);
            Assert.Equal(dx, arc.EndPointX - epX);
            Assert.Equal(dy, arc.EndPointY - epY);
        }

        [Theory]
        //translate only
        [InlineData(-5, 0, 5, 0, XSweepDirection.Clockwise, 1, 1, 0, 1, 1, -4, 1, 6, 1)]   //semi top
        [InlineData(5, 0, -5, 0, XSweepDirection.Clockwise, 1, 1, 0, 1, 1, 6, 1, -4, 1)]   //semi-bottom
        [InlineData(0, 5, 0, -5, XSweepDirection.Clockwise, 1, 1, 0, 1, 1, 1, 6, 1, -4)]   //semi-left
        [InlineData(0, -5, 0, 5, XSweepDirection.Clockwise, 1, 1, 0, 1, 1, 1, -4, 1, 6)]   //semi-right
        [InlineData(-5, 0, 0, -5, XSweepDirection.Clockwise, 1, 1, 0, 1, 1, -4, 1, 1, -4)]  //quad-top-left
        [InlineData(0, -5, 5, 0, XSweepDirection.Clockwise, 1, 1, 0, 1, 1, 1, -4, 6, 1)]    //quad-top-right
        [InlineData(0, 5, -5, 0, XSweepDirection.Clockwise, 1, 1, 0, 1, 1, 1, 6, -4, 1)]  //quad-bottom-left
        [InlineData(5, 0, 0, 5, XSweepDirection.Clockwise, 1, 1, 0, 1, 1, 6, 1, 1, 6)]    //quad-bottom-right
        public void TransformBy(double spX, double spY, double epX, double epY, XSweepDirection xSweep,
                                double scaleX, double scaleY, double rot, double tx, double ty,
                                double expectedSpX, double expectedSpY, double expectedEpX, double expectedEpY)
        {
            var arc = new ArcCanvasItem
            {
                StartPointX = spX,
                StartPointY = spY,
                EndPointX = epX,
                EndPointY = epY,
                Radius = 5,
                BorderWidth = 0.5,
                SweepDirection = xSweep
            };

            var tg = new XTransformGroup();

            var scaleTransform = new XScaleTransform(scaleX, scaleY);
            tg.Children.Add(scaleTransform);

            var rotateTransform = new XRotateTransform(rot);
            tg.Children.Add(rotateTransform);

            tg.Children.Add(new XTranslateTransform(tx, ty));

            arc.TransformBy(tg.Value);

            Assert.Equal(expectedSpX, arc.StartPointX);
            Assert.Equal(expectedSpY, arc.StartPointY);
            Assert.Equal(expectedEpX, arc.EndPointX);
            Assert.Equal(expectedEpY, arc.EndPointY);
        }

        [Fact]
        public void MirrorX()
        {
            var arc = new ArcCanvasItem
            {
                StartPointX = 0,
                StartPointY = 0,
                EndPointX = 0,
                EndPointY = 2,
                Radius = 1,
                BorderWidth = 0.5
            };

            arc.MirrorX();

            //in fact an arc is not mirrored
            Assert.Equal(1, arc.ScaleX);
        }

        [Fact]
        public void MirrorY()
        {
            var arc = new ArcCanvasItem
            {
                StartPointX = 0,
                StartPointY = 0,
                EndPointX = 0,
                EndPointY = 2,
                Radius = 1,
                BorderWidth = 0.5
            };

            arc.MirrorY();

            //in fact an arc is not mirrored
            Assert.Equal(1, arc.ScaleY);
        }

        [Theory]
        [InlineData(-5, 0, 5, 0, XSweepDirection.Clockwise, 0, -5, 0, 5)]   //semi top
        [InlineData(5, 0, -5, 0, XSweepDirection.Clockwise, 0, 5, 0, -5)]   //semi-bottom
        [InlineData(0, 5, 0, -5, XSweepDirection.Clockwise, -5, 0, 5, 0)]   //semi-left
        [InlineData(0, -5, 0, 5, XSweepDirection.Clockwise, 5, 0, -5, 0)]   //semi-right
        [InlineData(-5, 0, 0, -5, XSweepDirection.Clockwise, 0, -5, 5, 0)]  //quad-top-left
        [InlineData(0, -5, 5, 0, XSweepDirection.Clockwise, 5, 0, 0, 5)]    //quad-top-right
        [InlineData(0, 5, -5, 0, XSweepDirection.Clockwise, -5, 0, 0, -5)]  //quad-bottom-left
        [InlineData(5, 0, 0, 5, XSweepDirection.Clockwise, 0, 5, -5, 0)]    //quad-bottom-right
        public void Rotate(double spX, double spY, double epX, double epY, XSweepDirection xSweep,
                           double expectedSpX, double expectedSpY, double expectedEpX, double expectedEpY)
        {
            var arc = new ArcCanvasItem
            {
                StartPointX = spX,
                StartPointY = spY,
                EndPointX = epX,
                EndPointY = epY,
                Radius = 5,
                BorderWidth = 0.5,
                SweepDirection = xSweep
            };

            arc.Rotate();

            Assert.Equal(expectedSpX, arc.StartPointX);
            Assert.Equal(expectedSpY, arc.StartPointY);
            Assert.Equal(expectedEpX, arc.EndPointX);
            Assert.Equal(expectedEpY, arc.EndPointY);
        }

        [Theory]
        [InlineData(-5, 0, 5, 0, XSweepDirection.Clockwise, 0, 0)]   //semi top
        [InlineData(5, 0, -5, 0, XSweepDirection.Clockwise, 0, 0)]   //semi-bottom
        [InlineData(0, 5, 0, -5, XSweepDirection.Clockwise, 0, 0)]   //semi-left
        [InlineData(0, -5, 0, 5, XSweepDirection.Clockwise, 0, 0)]   //semi-right
        [InlineData(-5, 0, 0, -5, XSweepDirection.Clockwise, 0, 0)]   //quad-top-left
        [InlineData(0, -5, 5, 0, XSweepDirection.Clockwise, 0, 0)]    //quad-top-right
        [InlineData(0, 5, -5, 0, XSweepDirection.Clockwise, 0, 0)]    //quad-bottom-left
        [InlineData(5, 0, 0, 5, XSweepDirection.Clockwise, 0, 0)]     //quad-bottom-right
        public void GetCenter(double spX, double spY, double epX, double epY, XSweepDirection xSweep,
                              double expectedCenterX, double expectedCenterY)
        {
            var arc = new ArcCanvasItem
            {
                StartPointX = spX,
                StartPointY = spY,
                EndPointX = epX,
                EndPointY = epY,
                Radius = 5,
                BorderWidth = 0.5,
                SweepDirection = xSweep
            };


            var actual = arc.GetCenter();

            Assert.Equal(expectedCenterX, actual.X);
            Assert.Equal(expectedCenterY, actual.Y);
        }
    }
}
