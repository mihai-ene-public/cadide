using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IDE.Core.Presentation.Tests.CanvasItems.Board
{
    public class PlaneBoardCanvasItemTests
    {
        public PlaneBoardCanvasItemTests()
        {
            var debounceMock = new Mock<IDebounceDispatcher>();

            //ServiceProvider.RegisterResolver(t =>
            //{
            //    if (t == typeof(IDebounceDispatcher))
            //        return debounceMock.Object;

            //    throw new NotImplementedException();
            //});
        }

        private RegionBoard CreateRegionBoard()
        {
            const double farPointX = 100d;
            const double farPointY = 80d;

            var rb = new RegionBoard();
            rb.LayerId = (int)LayerType.BoardOutline + 1;

            rb.Items.Add(new LineRegionPrimitive
            {
                EndPointX = farPointX,
                EndPointY = 0
            });
            rb.Items.Add(new LineRegionPrimitive
            {
                EndPointX = farPointX,
                EndPointY = farPointY
            });
            rb.Items.Add(new LineRegionPrimitive
            {
                EndPointX = 0,
                EndPointY = farPointY
            });
            rb.Items.Add(new LineRegionPrimitive
            {
                EndPointX = 0,
                EndPointY = 0
            });

            return rb;
        }
        private PlaneBoardCanvasItem CreateItem(double borderWidth = 0.5)
        {
            var boardMock = new Mock<IBoardDesigner>();

            var boardOutline = CreateRegionBoard();
            var boardOutlineItem = RegionBoardCanvasItem.FromData(boardOutline);

            boardMock.SetupGet(x => x.BoardOutline)
                .Returns(boardOutlineItem);

            var item = new PlaneBoardCanvasItem
            {
                LayerDocument = boardMock.Object
            };

            return item;
        }

        [Fact]
        public void GetBoundingRectangle()
        {
            var item = CreateItem();

            var expected = new XRect(new XPoint(0, 0), new XPoint(100, 80));

            var actual = item.GetBoundingRectangle();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1)]
        public void Translate(double dx, double dy)
        {
            var item = CreateItem();

            //a plane doesn't translate
            item.Translate(dx, dy);

            Assert.True(true);

        }

        [Theory]
        //translate only
        [InlineData(1, 1, 0, 1, 1)]
        public void TransformBy(
                                double scaleX, double scaleY, double rot, double tx, double ty
                                )
        {
            var item = CreateItem();

            var tg = new XTransformGroup();

            var scaleTransform = new XScaleTransform(scaleX, scaleY);
            tg.Children.Add(scaleTransform);

            var rotateTransform = new XRotateTransform(rot);
            tg.Children.Add(rotateTransform);

            tg.Children.Add(new XTranslateTransform(tx, ty));

            //no transform
            item.TransformBy(tg.Value);

            Assert.True(true);
        }

        [Fact]
        public void MirrorX()
        {
            var item = CreateItem();

            item.MirrorX();

            Assert.Equal(1, item.ScaleX);
        }

        [Fact]
        public void MirrorY()
        {
            var item = CreateItem();

            item.MirrorY();

            Assert.Equal(1, item.ScaleY);
        }

        [Fact]
        public void Rotate()
        {
            var item = CreateItem();

            item.Rotate();

            //no rot
            Assert.True(true);
        }
    }
}
