using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Placement;
using IDE.Core.Types.Media;
using Xunit;
using IDE.Core.ViewModels;
using Moq;

namespace IDE.Core.Presentation.Tests.PlacementTools
{
    public class PolygonPlacementToolTests : PlacementToolTest
    {
        public PolygonPlacementToolTests()
        {
            var dispatcherMock = new Mock<IDispatcherHelper>();
            var schMock = new Mock<ISchematicDesigner>();

            var c = new PrimitiveToCanvasItemMapper();//it registers itself

            _canvasModel = new DrawingViewModel(schMock.Object, dispatcherMock.Object);
            ((CanvasGrid)_canvasModel.CanvasGrid).GridSizeModel.SelectedItem = new Units.MilUnit(50);

            var canvasItemType = typeof(PolygonCanvasItem);
            placementTool = PlacementTool.CreateTool(canvasItemType);
            placementTool.CanvasModel = _canvasModel;
            placementTool.StartPlacement(canvasItemType);
        }

        private readonly IDrawingViewModel _canvasModel;


        [Fact]
        public void PlacementReady_MouseMoves()
        {
            var item = placementTool.CanvasItem as IPolygonCanvasItem;
            //item.Points.Add(new XPoint());
            //item.Points.Add(new XPoint());


            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Ready;
            //act
            MouseMove(mp.X, mp.Y);


            //assert
            //last 2 points are the same as mouse position
            Assert.Equal(mp, item.PolygonPoints[item.PolygonPoints.Count - 1]);
            Assert.Equal(mp, item.PolygonPoints[item.PolygonPoints.Count - 2]);
            Assert.Equal(mp, item.PolygonPoints[item.PolygonPoints.Count - 3]);
            //ensure placement status
            Assert.Equal(PlacementStatus.Ready, placementTool.PlacementStatus);
        }

        [Fact]
        public void PlacementStarted_MouseMoves()
        {
            var item = placementTool.CanvasItem as IPolygonCanvasItem;
            //item.Points.Add(new XPoint());
            //item.Points.Add(new XPoint());

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Started;
            //act
            MouseMove(mp.X, mp.Y);

            //assert

            //last point the same as mouse position
            Assert.Equal(mp, item.PolygonPoints[item.PolygonPoints.Count - 1]);
            //ensure placement status
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);
        }


        [Fact]
        public void PlacementReady_MouseClick()
        {
            var item = placementTool.CanvasItem as IPolygonCanvasItem;
            //item.Points.Add(new XPoint());
            //item.Points.Add(new XPoint());

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Ready;
            //act
            MouseClick(mp.X, mp.Y);


            //assert
            
            //ensure placement status
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);
        }

        [Fact]
        public void PlacementStarted_MouseClick()
        {
            //todo: we need to split the tests when mouse clicks on the first point or not
            var item = placementTool.CanvasItem as IPolygonCanvasItem;
            //item.PolygonPoints.Add(new XPoint());
            //item.PolygonPoints.Add(new XPoint());

            var mp = new XPoint(10.16, 10.16);

            placementTool.PlacementStatus = PlacementStatus.Started;
            //act
            MouseClick(mp.X, mp.Y);


            //assert
            Assert.Equal(mp, item.PolygonPoints[item.PolygonPoints.Count - 1]);
            //Assert.True(item.IsPlaced);
            //ensure placement status
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);

        }

        [Fact]
        public void PlacementStarted_MouseClickOnFirstPoint()
        {
            //todo: we need to split the tests when mouse clicks on the first point or not
            var item = placementTool.CanvasItem as IPolygonCanvasItem;

            var mp = new XPoint(10.16, 10.16);

            //item.Points.Add(mp);
            //item.Points.Add(new XPoint());
            //item.Points.Add(new XPoint());

            //we need the other status points
            placementTool.PlacementStatus = PlacementStatus.Ready;
            //1st point
            MouseMove(mp.X, mp.Y);
            MouseClick(mp.X, mp.Y);

            //2nd point
            MouseMove(0, 0);
            MouseClick(0, 0);

            //placementTool.PlacementStatus = PlacementStatus.Started;
            //act
            MouseMove(mp.X, mp.Y);
            MouseClick(mp.X, mp.Y);


            //assert
            //last point is the 2nd point
            Assert.Equal(new XPoint(), item.PolygonPoints[item.PolygonPoints.Count - 1]);
            Assert.True(item.IsPlaced);
            //ensure placement status
            Assert.Equal(PlacementStatus.Ready, placementTool.PlacementStatus);

            //we have a cloned item
            var clonedItem = placementTool.CanvasItem as IPolygonCanvasItem;
            Assert.True(clonedItem.PolygonPoints.Count == 3);
        }
    }

}
