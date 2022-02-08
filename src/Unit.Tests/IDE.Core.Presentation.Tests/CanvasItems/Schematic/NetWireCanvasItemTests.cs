using IDE.Core.Designers;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using Xunit;

namespace IDE.Core.Presentation.Tests
{
    public class NetWireCanvasItemTests
    {
        [Theory]
        [InlineData(0, 10)]
        [InlineData(10, 0)]
        [InlineData(10, 10)]
        [InlineData(5, 10)]
        public void GetBoundingRectangle(double epx, double epy)
        {
            var item = new NetWireCanvasItem
            {
                Points = new List<XPoint>
                {
                    new XPoint(0,0),
                    new XPoint(epx,epy)
                }
            };

            var expected = new XRect(new XPoint(), new XPoint(epx, epy));
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
            var item = new NetWireCanvasItem
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
            var item = new NetWireCanvasItem
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
            var item = new NetWireCanvasItem
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
            var item = new NetWireCanvasItem
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
            var item = new NetWireCanvasItem
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

        [Fact]
        public void LoadFromPrimitive_LoadsOlderVersions()
        {
            //has x1,y1, x2,y2 only; points.Count=0
            //has both specified
            //points.count>0, no x1,y1...

            var dataWire = new NetWire
            {
                X1 = 0,
                Y1 = 0,
                X2 = 10,
                Y2 = 10,

                Width = 1
            };

            var netWire = new NetWireCanvasItem();
            netWire.LoadFromPrimitive(dataWire);

            Assert.True(netWire.IsPlaced);
            Assert.Equal(new XPoint(0, 0), netWire.StartPoint);
            Assert.Equal(new XPoint(10, 10), netWire.EndPoint);
        }

        [Fact]
        public void LoadFromPrimitive_LoadsCurrentVersions()
        {
            //has x1,y1, x2,y2 only; points.Count=0
            //has both specified
            //points.count>0, no x1,y1...

            var dataWire = new NetWire
            {
                Points = new List<Vertex>
                {
                    new Vertex{ x = 0, y = 0 },
                    new Vertex{ x = 10, y = 10 },
                },

                Width = 1
            };

            var netWire = new NetWireCanvasItem();
            netWire.LoadFromPrimitive(dataWire);

            Assert.True(netWire.IsPlaced);
            Assert.Equal(new XPoint(0, 0), netWire.StartPoint);
            Assert.Equal(new XPoint(10, 10), netWire.EndPoint);

        }

        //net wire placement completed

        //net wire segment removed from canvas

        //select one segment
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void SelectSegment_SelectsPoints(int segmentIndex)
        {
            var item = new NetWireCanvasItem
            { // -|_|-
                Points = new List<XPoint>
                {
                    new XPoint(10, 10),
                    new XPoint(20,10),
                    new XPoint(20,20),
                    new XPoint(30,20),
                    new XPoint(30,10),//4
                    new XPoint(40,10),
                }
            };

            item.SelectSegment(segmentIndex);

            var selectedPoints = item.SelectedPoints;

            Assert.Equal(2, selectedPoints.Count);
            Assert.Equal(selectedPoints[0], item.Points[segmentIndex]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(5)]
        [InlineData(6)]
        public void SelectSegment_NoSelection(int segmentIndex)
        {
            var item = new NetWireCanvasItem
            { // -|_|-
                Points = new List<XPoint>
                {
                    new XPoint(10, 10),
                    new XPoint(20,10),
                    new XPoint(20,20),
                    new XPoint(30,20),
                    new XPoint(30,10),//4
                    new XPoint(40,10),
                }
            };

            item.SelectSegment(segmentIndex);

            var selectedPoints = item.SelectedPoints;

            Assert.Equal(0, selectedPoints.Count);
        }

        //select one segment with existing selected segments
        [Theory]
        [InlineData(0, 0, 2)] //(abs(start - second) + 1) +1
        [InlineData(0, 1, 3)]
        [InlineData(0, 2, 4)]
        [InlineData(0, 3, 5)]
        [InlineData(0, 4, 6)]

        [InlineData(1, 0, 3)]
        [InlineData(1, 1, 2)]
        [InlineData(1, 2, 3)]
        [InlineData(1, 3, 4)]
        [InlineData(1, 4, 5)]

        [InlineData(2, 0, 4)]
        [InlineData(2, 1, 3)]
        [InlineData(2, 2, 2)]
        [InlineData(2, 3, 3)]
        [InlineData(2, 4, 4)]

        [InlineData(3, 0, 5)]
        [InlineData(3, 1, 4)]
        [InlineData(3, 2, 3)]
        [InlineData(3, 3, 2)]
        [InlineData(3, 4, 3)]

        [InlineData(4, 0, 6)]
        [InlineData(4, 1, 5)]
        [InlineData(4, 2, 4)]
        [InlineData(4, 3, 3)]
        [InlineData(4, 4, 2)]
        public void SelectSegmentAppend(int startIndex, int secondIndex, int expectedSelectedPoints)
        {
            var item = new NetWireCanvasItem
            { // -|_|-
                Points = new List<XPoint>
                {
                    new XPoint(10, 10),
                    new XPoint(20, 10),
                    new XPoint(20, 20),
                    new XPoint(30, 20),
                    new XPoint(30, 10),//4
                    new XPoint(40, 10),
                }
            };

            item.SelectSegment(startIndex);

            item.SelectSegmentAppend(secondIndex);

            var selectedPoints = item.SelectedPoints;

            Assert.Equal(expectedSelectedPoints, selectedPoints.Count);
            Assert.Equal(selectedPoints[0], item.Points[Math.Min(startIndex, secondIndex)]);
        }

        [Theory]
        [InlineData(0, 0, 2)] //(abs(start - second) + 1) +1
        [InlineData(0, 1, 3)]
        [InlineData(0, 2, 4)]
        [InlineData(0, 3, 5)]
        [InlineData(0, 4, 6)]

        [InlineData(1, 0, 3)]
        [InlineData(1, 1, 2)]
        [InlineData(1, 2, 3)]
        [InlineData(1, 3, 4)]
        [InlineData(1, 4, 5)]

        [InlineData(2, 0, 4)]
        [InlineData(2, 1, 3)]
        [InlineData(2, 2, 2)]
        [InlineData(2, 3, 3)]
        [InlineData(2, 4, 4)]

        [InlineData(3, 0, 5)]
        [InlineData(3, 1, 4)]
        [InlineData(3, 2, 3)]
        [InlineData(3, 3, 2)]
        [InlineData(3, 4, 3)]

        [InlineData(4, 0, 6)]
        [InlineData(4, 1, 5)]
        [InlineData(4, 2, 4)]
        [InlineData(4, 3, 3)]
        [InlineData(4, 4, 2)]
        public void SelectSegmentAppend_SelectSegmentAppend(int startIndex, int secondIndex, int expectedSelectedPoints)
        {
            var item = new NetWireCanvasItem
            { // -|_|-
                Points = new List<XPoint>
                {
                    new XPoint(10, 10),
                    new XPoint(20, 10),
                    new XPoint(20, 20),
                    new XPoint(30, 20),
                    new XPoint(30, 10),//4
                    new XPoint(40, 10),
                }
            };

            item.SelectSegmentAppend(startIndex);

            item.SelectSegmentAppend(secondIndex);

            var selectedPoints = item.SelectedPoints;

            Assert.Equal(expectedSelectedPoints, selectedPoints.Count);
            Assert.Equal(selectedPoints[0], item.Points[Math.Min(startIndex, secondIndex)]);
        }

        //clear selected segments
        [Fact]
        public void ClearSelection()
        {
            var item = new NetWireCanvasItem
            { // -|_|-
                Points = new List<XPoint>
                {
                    new XPoint(10, 10),
                    new XPoint(20,10),
                    new XPoint(20,20),
                    new XPoint(30,20),
                    new XPoint(30,10),//4
                    new XPoint(40,10),
                }
            };

            item.SelectSegment(1);

            item.ClearSelection();
            var selectedPoints = item.SelectedPoints;

            Assert.Equal(0, selectedPoints.Count);
        }
    }
}
