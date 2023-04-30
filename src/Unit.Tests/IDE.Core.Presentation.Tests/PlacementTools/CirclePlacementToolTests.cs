using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Placement;
using IDE.Core.Types.Media;
using Xunit;
using IDE.Core.ViewModels;
using Moq;
using System;

namespace IDE.Core.Presentation.Tests.PlacementTools
{
    public class CirclePlacementToolTests : PlacementToolTest
    {
        public CirclePlacementToolTests()
        {
            var dispatcherMock = new Mock<IDispatcherHelper>();
            var schMock = new Mock<ISchematicDesigner>();

            var _canvasModel = CreateCanvasModel();
            _canvasModel.CanvasGrid.SetUnit(new Units.MilUnit(50));

            var canvasItemType = typeof(CircleCanvasItem);
            placementTool = new CirclePlacementTool();//PlacementTool.CreateTool(canvasItemType);
            placementTool.CanvasModel = _canvasModel;
            placementTool.StartPlacement(canvasItemType);
        }

        [Fact]
        public void PlacementReady_MouseMoves()
        {

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Ready;
            //act
            MouseMove(mp.X, mp.Y);

            var item = placementTool.CanvasItem as ICircleCanvasItem;

            //assert
            //item position is the same as mouse position
            Assert.Equal(mp.X, item.X);
            Assert.Equal(mp.Y, item.Y);
            //ensure placement status
            Assert.Equal(PlacementStatus.Ready, placementTool.PlacementStatus);
        }

        [Fact]
        public void PlacementStarted_MouseMoves()
        {
            var item = placementTool.CanvasItem as ICircleCanvasItem;

            item.X = 0;
            item.Y = 0;

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Started;
            //act
            MouseMove(mp.X, mp.Y);


            //assert

            var w = (mp.X - item.X) * 2;
            var h = (mp.Y - item.Y) * 2;

            var d = Math.Sqrt(w * w + h * h);

            Assert.Equal(0, item.X);
            Assert.Equal(0, item.Y);
            var expectedWidth = d;
            var expectedHeight = d;
            Assert.Equal(expectedWidth, item.Diameter);
            //ensure placement status
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);
        }


        [Fact]
        public void PlacementReady_MouseClick()
        {

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Ready;
            //act
            MouseClick(mp.X, mp.Y);

            var item = placementTool.CanvasItem as ICircleCanvasItem;

            //assert
            //item position is the same as mouse position
            Assert.Equal(mp.X, item.X);
            Assert.Equal(mp.Y, item.Y);
            //ensure placement status
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);
        }

        [Fact]
        public void PlacementStarted_MouseClick()
        {
            var item = placementTool.CanvasItem as ICircleCanvasItem;

            item.X = 0;
            item.Y = 0;

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Started;
            //act
            MouseClick(mp.X, mp.Y);


            //assert
            var w = (mp.X - item.X) * 2;
            var h = (mp.Y - item.Y) * 2;

            var d = Math.Sqrt(w * w + h * h);

            Assert.Equal(0, item.X);
            Assert.Equal(0, item.Y);
            var expectedWidth = d;
            var expectedHeight = d;
            Assert.Equal(expectedWidth, item.Diameter);
            Assert.True(item.IsPlaced);
            //ensure placement status
            Assert.Equal(PlacementStatus.Ready, placementTool.PlacementStatus);

            //we have a cloned item
            var clonedItem = placementTool.CanvasItem as ICircleCanvasItem;
            Assert.Equal(clonedItem.X, item.X);
            Assert.Equal(clonedItem.Y, item.Y);
            Assert.Equal(clonedItem.Diameter, item.Diameter);
            Assert.Equal(clonedItem.BorderWidth, item.BorderWidth);
        }
    }

}
