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
    public class NetWirePlacementToolTests : PlacementToolTest
    {

        const string Connector2PinsSymbol =
@"<symbol id='2019012314360765101' name='CONN_02'>
      <line zIndex = '0' x1='53.34' y1='53.34' x2='48.26' y2='53.34' width='0.4064' lineStyle='Solid' lineCap='Round' lineColor='#FF000080' />
      <line zIndex = '0' x1='52.07' y1='48.26' x2='53.34' y2='48.26' width='0.6096' lineStyle='Solid' lineCap='Round' lineColor='#FF000080' />
      <line zIndex = '0' x1='52.07' y1='50.8' x2='53.34' y2='50.8' width='0.6096' lineStyle='Solid' lineCap='Round' lineColor='#FF000080' />
      <line zIndex = '0' x1='48.26' y1='45.72' x2='48.26' y2='53.34' width='0.4064' lineStyle='Solid' lineCap='Round' lineColor='#FF000080' />
      <line zIndex = '0' x1='53.34' y1='53.34' x2='53.34' y2='45.72' width='0.4064' lineStyle='Solid' lineCap='Round' lineColor='#FF000080' />
      <line zIndex = '0' x1='48.26' y1='45.72' x2='53.34' y2='45.72' width='0.4064' lineStyle='Solid' lineCap='Round' lineColor='#FF000080' />
      <pin zIndex = '0' name='1' showName='false' pinNameX='1' pinNameY='-1.45' pinNameRot='0' number='1' showNumber='true' pinNumberX='0.3' pinNumberY='-2.25' pinNumberRot='0' x='58.42' y='50.8' pinLength='5.08' width='0.5' pinType='Passive' orientation='Right' swapLevel='0' pinColor='#FF000080' pinNameColor='#FF000080' pinNumberColor='#FF000080' />
      <pin zIndex = '0' name='2' showName='false' pinNameX='1' pinNameY='-1.45' pinNameRot='0' number='2' showNumber='true' pinNumberX='0.3' pinNumberY='-2.25' pinNumberRot='0' x='58.42' y='48.26' pinLength='5.08' width='0.5' pinType='Passive' orientation='Right' swapLevel='0' pinColor='#FF000080' pinNameColor='#FF000080' pinNumberColor='#FF000080' />
    </symbol>";

        public NetWirePlacementToolTests()
        {
            var debounceMock = new Mock<IDebounceDispatcher>();

            var dispatcherMock = new Mock<IDispatcherHelper>();
            dispatcherMock.Setup(x => x.RunOnDispatcher(It.IsAny<Action>()))
                           .Callback((Action action) =>
                           {
                               action();
                           });



            //ServiceProvider.RegisterResolver(t =>
            //{
            //    if (t == typeof(IGeometryHelper))
            //        return new GeometryHelper();
            //    if (t == typeof(IDebounceDispatcher))
            //        return debounceMock.Object;
            //    if (t == typeof(IDispatcherHelper))
            //        return dispatcherMock.Object;

            //    throw new NotImplementedException();
            //});

            var schMock = new Mock<ISchematicDesigner>();
            schMock.SetupGet(x => x.NetManager)
                    .Returns(new SchematicNetManager());//mock net manager?

            _canvasModel = new DrawingViewModel(schMock.Object, dispatcherMock.Object);
            ((CanvasGrid)_canvasModel.CanvasGrid).GridSizeModel.SelectedItem = new Units.MilUnit(50);

            var canvasItemType = typeof(NetWireCanvasItem);
            placementTool = new NetWirePlacementTool();
            placementTool.CanvasModel = _canvasModel;
            placementTool.StartPlacement(canvasItemType);

            ((NetWirePlacementTool)placementTool).SetPlacementMode(NetPlacementMode.Single);
        }

        private readonly IDrawingViewModel _canvasModel;

        NetWirePlacementTool NetWirePlacementTool => (NetWirePlacementTool)placementTool;

        private Symbol CreateGndSymbol()
        {
            var s = new Symbol
            {
                Items = new List<SchematicPrimitive>
                 {
                     new LineSchematic
                     {
                         x1 = 46.99,
                         y1 = 50.8,
                         x2 = 49.53,
                         y2 = 50.8,
                         width = 0.5,
                     },
                     new Pin
                     {
                         Name = "GND",
                         ShowName = false,
                         Number = "GND",
                         ShowNumber = false,
                         x = 48.26,
                         y = 48.26,
                         PinLength = 2.54,
                         Width = 0.5,
                         pinType = PinType.Power,
                         Orientation = pinOrientation.Up
                     }
                 }
            };

            return s;
        }

        private Symbol CreateVccSymbol()
        {
            var s = new Symbol
            {
                Items = new List<SchematicPrimitive>
                 {
                     new LineSchematic
                     {
                         x1 = 49.022,
                         y1 = 46.736,
                         x2 = 48.26,
                         y2 = 45.466,
                         width = 0.5,
                     },
                     new LineSchematic
                     {
                         x1 = 48.26,
                         y1 = 45.466,
                         x2 = 47.498,
                         y2 = 46.736,
                         width = 0.5,
                     },
                     new Pin
                     {
                         Name = "5V",
                         ShowName = false,
                         Number = "5V",
                         ShowNumber = false,
                         x = 48.26,
                         y = 48.26,
                         PinLength = 2.54,
                         Width = 0.5,
                         pinType = PinType.Power,
                         Orientation = pinOrientation.Down
                     }
                 }
            };

            return s;
        }

        private Symbol LoadSymbolFromString(string symbolString)
        {
            var s = Helpers.GetObjectFromXmlString<Symbol>(symbolString);

            return s;
        }

        [Theory]
        [InlineData(NetPlacementMode.Single)]
        [InlineData(NetPlacementMode.HorizontalVertical)]
        [InlineData(NetPlacementMode.VerticalHorizontal)]
        public void PlacementStarted_MouseMoves(NetPlacementMode placementMode)
        {
            //arrange
            ((NetWirePlacementTool)placementTool).SetPlacementMode(placementMode);

            var mp = new XPoint(10.16, 10.16);

            //act
            MouseMove(mp.X, mp.Y);

            var wire = placementTool.CanvasItem as NetWireCanvasItem;

            //assert
            //all points are same as mouse point
            for (int i = 0; i < wire.Points.Count; i++)
            {
                Assert.Equal(mp, wire.Points[i]);
            }
            //ensure placement status
            Assert.Equal(PlacementStatus.Ready, placementTool.PlacementStatus);
        }

        //1st mouse click

        [Fact]
        public void Placement_FirstMouseClickNoHits()
        {
            var mp = new XPoint(10.16, 10.16);
            MouseMove(mp.X, mp.Y);
            MouseClick(mp.X, mp.Y);

            var wire = placementTool.CanvasItem as NetWireCanvasItem;
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);
            Assert.Null(wire.Net);
            Assert.Equal(mp, wire.StartPoint);
        }

        //hits power pin
        [Fact]
        public void Placement_FirstMouseClickHitsGroundPin()
        {
            var gndSymbol = CreateGndSymbol();
            var partGnd = new SchematicSymbolCanvasItem
            {
                X = 25.4,
                Y = 25.4,
                ShowName = false
            };
            partGnd.LoadSymbol(gndSymbol);

            _canvasModel.AddItem(partGnd);

            var mp = new XPoint(25.4, 24.13);
            MouseMove(mp.X, mp.Y);
            MouseClick(mp.X, mp.Y);

            var wire = placementTool.CanvasItem as NetWireCanvasItem;
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);
            Assert.NotNull(wire.Net);
            Assert.Equal("GND", wire.Net.Name);
            Assert.Equal(mp, wire.StartPoint);
        }

        //hits regular pin
        [Theory]
        [InlineData(27.94, 26.67, "1")]
        [InlineData(27.94, 24.13, "2")]
        public void Placement_FirstMouseClickHitsConnectorPin(double mpX, double mpY, string expectedHitPinNumber)
        {
            var connectorSymbol = LoadSymbolFromString(Connector2PinsSymbol);
            var partConn = new SchematicSymbolCanvasItem
            {
                X = 25.4,
                Y = 25.4,
                ShowName = false
            };
            partConn.LoadSymbol(connectorSymbol);

            _canvasModel.AddItem(partConn);

            var mp = new XPoint(mpX, mpY);
            MouseMove(mp.X, mp.Y);
            MouseClick(mp.X, mp.Y);

            var wire = placementTool.CanvasItem as NetWireCanvasItem;
            var net = wire.Net;
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);
            Assert.NotNull(net);
            Assert.False(net.IsNamed());
            //hits expected pin
            Assert.NotNull(net.NetItems.OfType<PinCanvasItem>().FirstOrDefault(p => p.Number == expectedHitPinNumber));
            Assert.Equal(mp, wire.StartPoint);
        }

        //hits GND net wire
        [Theory]
        [InlineData(1.27, 1.27, 0)]//StartPoint
        [InlineData(7.62, 1.27, 0)]//EndPoint
        [InlineData(5.08, 1.27, 1)]//somewhere in the middle
        public void Placement_FirstMouseClickHitsGndNetWire(double mpX, double mpY, int expectedJunctions)
        {
            var net = new SchematicNet
            {
                Name = "GND",
                Id = 1
            };
            var gndWire = new NetWireCanvasItem
            {
                Points = new List<XPoint>
                 {
                     new XPoint(1.27, 1.27),
                     new XPoint(7.62, 1.27)
                 },
                Net = net,
                IsPlaced = true
            };

            _canvasModel.AddItem(gndWire);

            var mp = new XPoint(mpX, mpY);
            MouseMove(mp.X, mp.Y);
            MouseClick(mp.X, mp.Y);

            var wire = placementTool.CanvasItem as NetWireCanvasItem;
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);
            Assert.NotNull(wire.Net);
            Assert.Equal(net.Name, wire.Net.Name);
            Assert.Equal(mp, wire.StartPoint);

            var junctions = _canvasModel.Items.OfType<JunctionCanvasItem>().Where(j => j.X == mpX && j.Y == mpY);
            Assert.Equal(expectedJunctions, junctions.Count());
        }

        //hits regular net wire
        [Theory]
        [InlineData(1.27, 1.27, 0)]//StartPoint
        [InlineData(7.62, 1.27, 0)]//EndPoint
        [InlineData(5.08, 1.27, 1)]//somewhere in the middle
        public void Placement_FirstMouseClickHitsRegularNetWire(double mpX, double mpY, int expectedJunctions)
        {
            var net = new SchematicNet
            {
                Name = "Net1",
                Id = 1
            };
            var gndWire = new NetWireCanvasItem
            {
                Points = new List<XPoint>
                 {
                     new XPoint(1.27, 1.27),
                     new XPoint(7.62, 1.27)
                 },
                Net = net,
                IsPlaced = true
            };

            _canvasModel.AddItem(gndWire);

            var mp = new XPoint(mpX, mpY);
            MouseMove(mp.X, mp.Y);
            MouseClick(mp.X, mp.Y);

            var wire = placementTool.CanvasItem as NetWireCanvasItem;
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);
            Assert.NotNull(wire.Net);
            Assert.Equal(net.Name, wire.Net.Name);
            Assert.Equal(mp, wire.StartPoint);

            var junctions = _canvasModel.Items.OfType<JunctionCanvasItem>().Where(j => j.X == mpX && j.Y == mpY);
            Assert.Equal(expectedJunctions, junctions.Count());
        }

        //hits net single segment wire in startPoint
        //hits net single segment wire in endPoint
        //hits net single segment wire somewhere in the middle but on the line

        //hits net multi-segment wire in startPoint
        //hits net multi-segment wire in endPoint
        //hits net multi-segment wire a segment somewhere in the middle but on the line

        //hits junction

        //1st mouse click, second mouse move
        [Theory]
        [InlineData(NetPlacementMode.Single)]
        [InlineData(NetPlacementMode.HorizontalVertical)]
        [InlineData(NetPlacementMode.VerticalHorizontal)]
        public void Placement_MouseClickMouseMoveNoHits(NetPlacementMode placementMode)
        {
            ((NetWirePlacementTool)placementTool).SetPlacementMode(placementMode);

            var sp = new XPoint(10.16, 10.16);
            var ep = new XPoint(20.32, 20.32);

            MouseMove(sp.X, sp.Y);
            MouseClick(sp.X, sp.Y);

            MouseMove(ep.X, ep.Y);


            var wire = placementTool.CanvasItem as NetWireCanvasItem;
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);
            Assert.Null(wire.Net);
            Assert.Equal(sp, wire.StartPoint);
            Assert.Equal(ep, wire.EndPoint);

            //assert middle point
            if (placementMode != NetPlacementMode.Single)
            {
                var mp = placementMode == NetPlacementMode.HorizontalVertical ? new XPoint(ep.X, sp.Y) : new XPoint(sp.X, ep.Y);

                Assert.Equal(mp, wire.Points[1]);
            }
        }

        #region Second mouse click
        //second mouse click
        [Fact]
        public void Placement_MouseClickMouseMoveMouseClickNoHits()
        {
            var sp = new XPoint(10.16, 10.16);
            var ep = new XPoint(20.32, 20.32);

            MouseMove(sp.X, sp.Y);
            MouseClick(sp.X, sp.Y);

            MouseMove(ep.X, ep.Y);
            MouseClick(ep.X, ep.Y);

            var wire = NetWirePlacementTool.CommitedPolyline;

            //assert
            Assert.Equal(PlacementStatus.Started, placementTool.PlacementStatus);

            Assert.NotNull(wire.Net);
            Assert.False(wire.Net.IsNamed());
            Assert.True(wire.IsPlaced);

            Assert.Equal(sp, wire.StartPoint);
            Assert.Equal(ep, wire.EndPoint);
        }

        #endregion
    }
}
