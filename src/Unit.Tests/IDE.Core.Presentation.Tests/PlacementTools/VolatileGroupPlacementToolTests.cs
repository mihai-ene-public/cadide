using System;
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
    public class VolatileGroupPlacementToolTests : PlacementToolTest
    {
        public VolatileGroupPlacementToolTests()
        {
            var dispatcherMock = new Mock<IDispatcherHelper>();
            var schMock = new Mock<ISchematicDesigner>();

            _canvasModel = CreateCanvasModel();
            _canvasModel.CanvasGrid.SetUnit(new Units.MilUnit(50));

            var canvasItemType = typeof(VolatileGroupCanvasItem);
            var canvasItem = new VolatileGroupCanvasItem();
            placementTool = new VolatileGroupPlacementTool();
            placementTool.CanvasModel = _canvasModel;
            placementTool.StartPlacement(canvasItem);
        }
        private readonly ICanvasDesignerFileViewModel _canvasModel;

        [Fact]
        public void PlacementStarted_MouseMoves()
        {

            var mp = new XPoint(10.16, 10.16);

            //act
            MouseMove(mp.X, mp.Y);

            var item = placementTool.CanvasItem as VolatileGroupCanvasItem;

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
            var item = placementTool.CanvasItem as VolatileGroupCanvasItem;
            
            var mp = new XPoint(10.16, 10.16);
            MouseMove(mp.X, mp.Y);
            MouseClick(mp.X, mp.Y);

            Assert.Equal(PlacementStatus.Ready, placementTool.PlacementStatus);
            //placement finished
            Assert.Null(_canvasModel.PlacementTool);

        }
    }

}
