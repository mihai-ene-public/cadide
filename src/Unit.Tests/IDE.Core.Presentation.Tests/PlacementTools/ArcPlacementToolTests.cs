using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Placement;
using IDE.Core.Types.Media;
using Xunit;
using IDE.Core.ViewModels;
using Moq;


namespace IDE.Core.Presentation.Tests.PlacementTools
{
    public class ArcPlacementToolTests : PlacementToolTest
    {
        public ArcPlacementToolTests()
        {
            var dispatcherMock = new Mock<IDispatcherHelper>();
            var schMock = new Mock<ISchematicDesigner>();

            _canvasModel = new DrawingViewModel(schMock.Object, dispatcherMock.Object);
            ((CanvasGrid)_canvasModel.CanvasGrid).GridSizeModel.SelectedItem = new Units.MilUnit(50);

            var canvasItemType = typeof(ArcCanvasItem);
            placementTool = new ArcPlacementTool();//PlacementTool.CreateTool(canvasItemType);
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

            var item = placementTool.CanvasItem as IArcCanvasItem;

            //assert
            //item position is the same as mouse position
            Assert.Equal(mp.X, item.StartPointX);
            Assert.Equal(mp.Y, item.StartPointY);
            Assert.Equal(mp.X, item.EndPointX);
            Assert.Equal(mp.Y, item.EndPointY);
            //ensure placement status
            Assert.Equal(PlacementStatus.Ready, placementTool.PlacementStatus);
        }

        [Fact]
        public void PlacementStarted_MouseMoves()
        {
            var item = placementTool.CanvasItem as IArcCanvasItem;

            item.StartPointX = 0;
            item.StartPointY = 0;

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Started;
            //act
            MouseMove(mp.X, mp.Y);


            //assert
            Assert.Equal(0, item.StartPointX);
            Assert.Equal(0, item.StartPointY);
            Assert.Equal(mp.X, item.EndPointX);
            Assert.Equal(mp.Y, item.EndPointY);
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

            var item = placementTool.CanvasItem as IArcCanvasItem;

            //assert
            //item position is the same as mouse position
            Assert.Equal(mp.X, item.StartPointX);
            Assert.Equal(mp.Y, item.StartPointY);
            //ensure placement status
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);
        }

        [Fact]
        public void PlacementStarted_MouseClick()
        {
            var item = placementTool.CanvasItem as IArcCanvasItem;

            item.StartPointX = 0;
            item.StartPointY = 0;

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Started;
            //act
            MouseClick(mp.X, mp.Y);


            //assert
            Assert.Equal(0, item.StartPointX);
            Assert.Equal(0, item.StartPointY);
            Assert.Equal(mp.X, item.EndPointX);
            Assert.Equal(mp.Y, item.EndPointY);

            //ensure placement status
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);

            Assert.True(item.IsPlaced);

            //we have a cloned item
            var clonedItem = placementTool.CanvasItem as IArcCanvasItem;
            Assert.Equal(clonedItem.StartPointX, item.EndPointX);
            Assert.Equal(clonedItem.StartPointY, item.EndPointY);
            Assert.Equal(clonedItem.BorderWidth, item.BorderWidth);
        }
    }

}
