using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Placement;
using IDE.Core.Types.Media;
using Xunit;
using Moq;
using IDE.Core.ViewModels;

namespace IDE.Core.Presentation.Tests.PlacementTools;

public class ExtrudedPolyMeshItemPlacementToolTests : PlacementToolTest
{
    public ExtrudedPolyMeshItemPlacementToolTests()
    {
        var dispatcherMock = new Mock<IDispatcherHelper>();
        var schMock = new Mock<ISchematicDesigner>();

        _canvasModel = new DrawingViewModel(schMock.Object, dispatcherMock.Object);
        ((CanvasGrid)_canvasModel.CanvasGrid).GridSizeModel.SelectedItem = new Units.MilUnit(50);

        var canvasItemType = typeof(ExtrudedPolyMeshItem);
        placementTool = new ExtrudedPolyMeshItemPlacementTool();
        placementTool.CanvasModel = _canvasModel;
        placementTool.StartPlacement(canvasItemType);
    }

    private readonly IDrawingViewModel _canvasModel;


    [Fact]
    public void PlacementReady_MouseMoves()
    {
        var item = placementTool.CanvasItem as ExtrudedPolyMeshItem;
        item.Points.Add(new XPoint());
        item.Points.Add(new XPoint());


        var mp = new XPoint(10.16, 10.16);

        placementTool.PlacementStatus = PlacementStatus.Ready;
        //act
        MouseMove(mp.X, mp.Y);


        //assert
        //last 2 points are the same as mouse position
        Assert.Equal(mp, item.Points[item.Points.Count - 1]);
        Assert.Equal(mp, item.Points[item.Points.Count - 2]);
        //ensure placement status
        Assert.Equal(PlacementStatus.Ready, placementTool.PlacementStatus);
    }

    [Fact]
    public void PlacementStarted_MouseMoves()
    {
        var item = placementTool.CanvasItem as ExtrudedPolyMeshItem;
        item.Points.Add(new XPoint());
        item.Points.Add(new XPoint());

        var mp = new XPoint(10.16, 10.16);

        placementTool.PlacementStatus = PlacementStatus.Started;
        //act
        MouseMove(mp.X, mp.Y);

        //assert

        //last point the same as mouse position
        Assert.Equal(mp, item.Points[item.Points.Count - 1]);
        //ensure placement status
        Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);
    }


    [Fact]
    public void PlacementReady_MouseClick()
    {
        var item = placementTool.CanvasItem as ExtrudedPolyMeshItem;
        item.Points.Add(new XPoint());
        item.Points.Add(new XPoint());

        var mp = new XPoint(10.16, 10.16);

        placementTool.PlacementStatus = PlacementStatus.Ready;
        //act
        MouseClick(mp.X, mp.Y);


        //assert
        //last 2 points are the same as mouse position
        Assert.Equal(mp, item.Points[item.Points.Count - 1]);
        Assert.Equal(mp, item.Points[item.Points.Count - 2]);
        //ensure placement status
        Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);
    }

    [Fact]
    public void PlacementStarted_MouseClick()
    {
        //todo: we need to split the tests when mouse clicks on the first point or not
        var item = placementTool.CanvasItem as ExtrudedPolyMeshItem;
        item.Points.Add(new XPoint());
        item.Points.Add(new XPoint());

        var mp = new XPoint(10.16, 10.16);

        placementTool.PlacementStatus = PlacementStatus.Started;
        //act
        MouseClick(mp.X, mp.Y);


        //assert
        Assert.Equal(mp, item.Points[item.Points.Count - 1]);
        //Assert.True(item.IsPlaced);
        //ensure placement status
        Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);

        ////we have a cloned item
        //var clonedItem = placementTool.CanvasItem as ExtrudedPolyMeshItem;
        //Assert.True(clonedItem.Points.Count == 0);
    }

    [Fact]
    public void PlacementStarted_MouseClickOnFirstPoint()
    {
        //todo: we need to split the tests when mouse clicks on the first point or not
        var item = placementTool.CanvasItem as ExtrudedPolyMeshItem;
        
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
        Assert.Equal(new XPoint(), item.Points[item.Points.Count - 1]);
        Assert.True(item.IsPlaced);
        //ensure placement status
        Assert.Equal(PlacementStatus.Ready, placementTool.PlacementStatus);

        //we have a cloned item
        var clonedItem = placementTool.CanvasItem as ExtrudedPolyMeshItem;
        Assert.True(clonedItem.Points.Count == 0);
    }
}
