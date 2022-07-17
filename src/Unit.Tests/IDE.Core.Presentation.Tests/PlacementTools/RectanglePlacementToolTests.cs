using System.Collections.Generic;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Placement;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Linq;
using Xunit;
using IDE.Core.ViewModels;
using Moq;
using IDE.Core.Collision;


namespace IDE.Core.Presentation.Tests.PlacementTools
{
    public class RectanglePlacementToolTests : PlacementToolTest
    {
        public RectanglePlacementToolTests()
        {
            var dispatcherMock = new Mock<IDispatcherHelper>();
            var schMock = new Mock<ISchematicDesigner>();

            _canvasModel = new DrawingViewModel(schMock.Object, dispatcherMock.Object);
            ((CanvasGrid)_canvasModel.CanvasGrid).GridSizeModel.SelectedItem = new Units.MilUnit(50);

            //todo: maybe we need to separate tests for RectangleBoardCanvasItem
            var canvasItemType = typeof(RectangleCanvasItem);
            placementTool = new RectanglePlacementTool();
            placementTool.CanvasModel = _canvasModel;
            placementTool.StartPlacement(canvasItemType);
        }

        private readonly IDrawingViewModel _canvasModel;


        [Fact]
        public void PlacementReady_MouseMoves()
        {

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Ready;
            //act
            MouseMove(mp.X, mp.Y);

            var item = placementTool.CanvasItem as IRectangleCanvasItem;

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
            var item = placementTool.CanvasItem as IRectangleCanvasItem;

            item.X = 0;
            item.Y = 0;

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Started;
            //act
            MouseMove(mp.X, mp.Y);


            //assert
            Assert.Equal(0, item.X);
            Assert.Equal(0, item.Y);
            var expectedWidth = 2 * (mp.X - item.X);
            var expectedHeight = 2 * (mp.Y - item.Y);
            Assert.Equal(expectedWidth, item.Width);
            Assert.Equal(expectedHeight, item.Height);
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

            var item = placementTool.CanvasItem as IRectangleCanvasItem;

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
            var item = placementTool.CanvasItem as IRectangleCanvasItem;

            item.X = 0;
            item.Y = 0;

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Started;
            //act
            MouseClick(mp.X, mp.Y);


            //assert
            Assert.Equal(0, item.X);
            Assert.Equal(0, item.Y);
            var expectedWidth = 2 * (mp.X - item.X);
            var expectedHeight = 2 * (mp.Y - item.Y);
            Assert.Equal(expectedWidth, item.Width);
            Assert.Equal(expectedHeight, item.Height);
            Assert.True(item.IsPlaced);
            //ensure placement status
            Assert.Equal(PlacementStatus.Ready, placementTool.PlacementStatus);

            //we have a cloned item
            var clonedItem = placementTool.CanvasItem as IRectangleCanvasItem;
            Assert.Equal(clonedItem.X, item.X);
            Assert.Equal(clonedItem.Y, item.Y);
            Assert.Equal(clonedItem.Width, item.Width);
            Assert.Equal(clonedItem.Height, item.Height);
            Assert.Equal(clonedItem.CornerRadius, item.CornerRadius);
            Assert.Equal(clonedItem.BorderWidth, item.BorderWidth);
            Assert.Equal(clonedItem.IsFilled, item.IsFilled);
        }
    }

}
