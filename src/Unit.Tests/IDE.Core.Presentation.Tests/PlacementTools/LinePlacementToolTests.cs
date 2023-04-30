using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Placement;
using IDE.Core.Types.Media;
using Xunit;
using IDE.Core.ViewModels;
using Moq;


namespace IDE.Core.Presentation.Tests.PlacementTools
{
    public class LinePlacementToolTests : PlacementToolTest
    {
        public LinePlacementToolTests()
        {
            var dispatcherMock = new Mock<IDispatcherHelper>();
            var schMock = new Mock<ISchematicDesigner>();

            _canvasModel = CreateCanvasModel();
            _canvasModel.CanvasGrid.SetUnit(new Units.MilUnit(50));

            //todo: maybe we need to separate tests for RectangleBoardCanvasItem
            var canvasItemType = typeof(LineSchematicCanvasItem);
            placementTool = new LinePlacementTool();
            placementTool.CanvasModel = _canvasModel;
            placementTool.StartPlacement(canvasItemType);
        }

        private readonly ICanvasDesignerFileViewModel _canvasModel;


        [Fact]
        public void PlacementReady_MouseMoves()
        {

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Ready;
            //act
            MouseMove(mp.X, mp.Y);

            var item = placementTool.CanvasItem as ILineCanvasItem;

            //assert
            //item position is the same as mouse position
            Assert.Equal(mp.X, item.X1);
            Assert.Equal(mp.Y, item.Y1);
            Assert.Equal(mp.X, item.X2);
            Assert.Equal(mp.Y, item.Y2);
            //ensure placement status
            Assert.Equal(PlacementStatus.Ready, placementTool.PlacementStatus);
        }

        [Fact]
        public void PlacementStarted_MouseMoves()
        {
            var item = placementTool.CanvasItem as ILineCanvasItem;

            item.X1 = 0;
            item.Y1 = 0;

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Started;
            //act
            MouseMove(mp.X, mp.Y);


            //assert
            Assert.Equal(0, item.X1);
            Assert.Equal(0, item.Y1);
            Assert.Equal(mp.X, item.X2);
            Assert.Equal(mp.Y, item.Y2);
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

            var item = placementTool.CanvasItem as ILineCanvasItem;

            //assert
            //item position is the same as mouse position
            Assert.Equal(mp.X, item.X1);
            Assert.Equal(mp.Y, item.Y1);
            //ensure placement status
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);
        }

        [Fact]
        public void PlacementStarted_MouseClick()
        {
            var item = placementTool.CanvasItem as ILineCanvasItem;

            item.X1 = 0;
            item.Y1 = 0;

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Started;
            //act
            MouseClick(mp.X, mp.Y);


            //assert
            Assert.Equal(0, item.X1);
            Assert.Equal(0, item.Y1);
            Assert.Equal(mp.X, item.X2);
            Assert.Equal(mp.Y, item.Y2);

            //ensure placement status
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);

            Assert.True(item.IsPlaced);

            //we have a cloned item
            var clonedItem = placementTool.CanvasItem as ILineCanvasItem;
            Assert.Equal(clonedItem.X1, item.X2);
            Assert.Equal(clonedItem.Y1, item.Y2);
            Assert.Equal(clonedItem.Width, item.Width);
        }
    }
}
