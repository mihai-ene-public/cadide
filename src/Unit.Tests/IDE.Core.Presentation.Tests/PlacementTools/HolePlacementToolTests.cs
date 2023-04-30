using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Placement;
using IDE.Core.Types.Media;
using Xunit;
using IDE.Core.ViewModels;
using Moq;

namespace IDE.Core.Presentation.Tests.PlacementTools
{
    public class HolePlacementToolTests : PlacementToolTest
    {
        public HolePlacementToolTests()
        {
            var dispatcherMock = new Mock<IDispatcherHelper>();
            var fileModelMock = new Mock<IFileBaseViewModel>();

            _canvasModel = CreateCanvasModel();
            _canvasModel.CanvasGrid.SetUnit(new Units.MilUnit(50));


            var canvasItemType = typeof(HoleCanvasItem);
            placementTool = new HolePlacementTool();
            placementTool.CanvasModel = _canvasModel;
            placementTool.StartPlacement(canvasItemType);
        }

        private readonly ICanvasDesignerFileViewModel _canvasModel;

        [Fact]
        public void PlacementStarted_MouseMoves()
        {

            var mp = new XPoint(10.16, 10.16);

            //act
            MouseMove(mp.X, mp.Y);

            var item = placementTool.CanvasItem as IHoleCanvasItem;

            //assert
            //item position is the same as mouse position
            Assert.Equal(mp.X, item.X);
            Assert.Equal(mp.Y, item.Y);
            //ensure placement status
            Assert.Equal(PlacementStatus.Ready, placementTool.PlacementStatus);
        }

        [Fact]
        public void Placement_FirstMouseClick()
        {
            var item = placementTool.CanvasItem as IHoleCanvasItem;

            var mp = new XPoint(10.16, 10.16);
            MouseMove(mp.X, mp.Y);
            MouseClick(mp.X, mp.Y);

            Assert.Equal(mp.X, item.X);
            Assert.Equal(mp.Y, item.Y);
            Assert.True(item.IsPlaced);

            Assert.Equal(PlacementStatus.Ready, placementTool.PlacementStatus);
            //placement not finished
            Assert.NotNull(_canvasModel.PlacementTool);
        }
    }

}
