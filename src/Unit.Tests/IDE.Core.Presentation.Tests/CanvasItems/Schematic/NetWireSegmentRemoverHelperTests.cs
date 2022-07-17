using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Core.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using System;

namespace IDE.Core.Presentation.Tests
{
    public class NetWireSegmentRemoverHelperTests
    {
        public NetWireSegmentRemoverHelperTests()
        {
            var debounceMock = new Mock<IDebounceDispatcher>();
            var dispatcherMock = new Mock<IDispatcherHelper>();
            dispatcherMock.Setup(x => x.RunOnDispatcher(It.IsAny<Action>()))
                           .Callback((Action action) =>
                           {
                               action();
                           });


            _dispatcher = dispatcherMock.Object;

            ServiceProvider.RegisterResolver(t =>
            {
                if (t == typeof(IDebounceDispatcher))
                    return debounceMock.Object;
                //if (t == typeof(IDispatcherHelper))
                //    return dispatcherMock.Object;

                throw new NotImplementedException();
            });

            var schMock = new Mock<ISchematicDesigner>();
            schMock.SetupGet(x => x.NetManager)
                    .Returns(new SchematicNetManager());//mock net manager?

            _canvasModel = new DrawingViewModel(schMock.Object, _dispatcher);
            ((CanvasGrid)_canvasModel.CanvasGrid).GridSizeModel.SelectedItem = new Units.MilUnit(50);
        }

        private readonly IDrawingViewModel _canvasModel;

        private readonly IDispatcherHelper _dispatcher;

        [Fact]
        public void RemoveSelectedSegments_WhenNetWithSingleSegmentRemovesConnectionBetweenPins()
        {
            var vccSymbol = SchematicTestsHelper.CreateVccSymbol();
            var part1 = PlacePart(vccSymbol, 25.4, 25.4);
            var part2 = PlacePart(vccSymbol, 25.4 * 2, 25.4);

            var netManager = (_canvasModel.FileDocument as ISchematicDesigner).NetManager;

            var net = new SchematicNet
            {
                Id = 1,
                Name = "Net$1"
            };
            netManager.Add(net);


            var netWire = new NetWireCanvasItem
            {
                Net = net,
                Points = new List<XPoint>
                {
                    new XPoint(25.4, 26.67),
                    new XPoint(50.8, 26.67)
                }
            };
            var pin1 = part1.Pins.FirstOrDefault();
            var pin2 = part2.Pins.FirstOrDefault();

            pin1.Net = net;
            pin2.Net = net;

            netWire.SelectSegment(0);

            //act
            var segmentRemover = new SegmentRemoverHelper(_canvasModel, netWire, _dispatcher);
            segmentRemover.RemoveSelectedSegments();

            //assert

            //no net wires
            Assert.Empty(_canvasModel.Items.OfType<NetWireCanvasItem>());
            //nothing is left on this net
            Assert.Empty(net.NetItems);

            //nets are removed from net manager
            var existingNet = netManager.Get(net.Name);
            Assert.Null(existingNet);

            //pins are no longer connected
            Assert.Null(pin1.Net);
            Assert.Null(pin2.Net);
        }

        [Fact]
        public void RemoveSelectedSegments_WhenNetWith2SegmentsRemovesConnectionBetweenPins()
        {
            var vccSymbol = SchematicTestsHelper.CreateVccSymbol();
            var part1 = PlacePart(vccSymbol, 25.4, 25.4);
            var part2 = PlacePart(vccSymbol, 25.4 * 2, 25.4 * 2);

            var netManager = (_canvasModel.FileDocument as ISchematicDesigner).NetManager;

            var net = new SchematicNet
            {
                Id = 1,
                Name = "Net$1"
            };
            netManager.Add(net);

            var netWire = new NetWireCanvasItem
            {
                Net = net,
                Points = new List<XPoint>
                {
                    new XPoint(25.4, 26.67),
                    new XPoint(25.4, 52.07),
                    new XPoint(50.8, 52.07)
                }
            };
            var pin1 = part1.Pins.FirstOrDefault();
            var pin2 = part2.Pins.FirstOrDefault();

            pin1.Net = net;
            pin2.Net = net;

            //select all segments
            netWire.SelectSegmentAppend(0);
            netWire.SelectSegmentAppend(1);

            //act
            var segmentRemover = new SegmentRemoverHelper(_canvasModel, netWire, _dispatcher);
            segmentRemover.RemoveSelectedSegments();

            //assert

            //no net wires
            Assert.Empty(_canvasModel.Items.OfType<NetWireCanvasItem>());
            //nothing is left on this net
            Assert.Empty(net.NetItems);

            //nets are removed from net manager
            var existingNet = netManager.Get(net.Name);
            Assert.Null(existingNet);

            //pins are no longer connected
            Assert.Null(pin1.Net);
            Assert.Null(pin2.Net);
        }

        [Theory]
        [InlineData(0, false, true)]
        [InlineData(1, true, false)]
        public void RemoveSelectedSegments_WhenWireTwoSegmentsAndOneSegmentSelectedThenOnePinIsRemovedFromNet(int selectedSegment, bool expectedPin1Connected, bool expectedPin2Connected)
        {
            var vccSymbol = SchematicTestsHelper.CreateVccSymbol();
            var part1 = PlacePart(vccSymbol, 25.4, 25.4);
            var part2 = PlacePart(vccSymbol, 25.4 * 2, 25.4 * 2);

            var netManager = (_canvasModel.FileDocument as ISchematicDesigner).NetManager;

            var net = new SchematicNet
            {
                Id = 1,
                Name = "Net$1"
            };
            netManager.Add(net);

            var netWire = new NetWireCanvasItem
            {
                Net = net,
                Points = new List<XPoint>
                {
                    new XPoint(25.4, 26.67),
                    new XPoint(25.4, 52.07),
                    new XPoint(50.8, 52.07)
                }
            };
            var pin1 = part1.Pins.FirstOrDefault();
            var pin2 = part2.Pins.FirstOrDefault();

            pin1.Net = net;
            pin2.Net = net;

            //select all segments
            netWire.SelectSegmentAppend(selectedSegment);

            //act
            var segmentRemover = new SegmentRemoverHelper(_canvasModel, netWire, _dispatcher);
            segmentRemover.RemoveSelectedSegments();

            //assert
            //no net wires
            Assert.NotEmpty(_canvasModel.Items.OfType<NetWireCanvasItem>());
            //nothing is left on this net
            Assert.Empty(net.NetItems);//?

            //nets are removed from net manager
            var existingNet = netManager.Get(net.Name);
            //Assert.NotNull(existingNet);

            //pins are no longer connected
            Assert.Equal(expectedPin1Connected, pin1.Net != null);
            Assert.Equal(expectedPin2Connected, pin2.Net != null);
        }

        [Fact]
        public void RemoveSelectedSegments_WireThreeSegmentsRemovesMiddleSegment()
        {
            var vccSymbol = SchematicTestsHelper.CreateVccSymbol();
            var part1 = PlacePart(vccSymbol, 25.4, 25.4);
            var part2 = PlacePart(vccSymbol, 25.4 * 2, 25.4 * 2);

            var netManager = (_canvasModel.FileDocument as ISchematicDesigner).NetManager;

            var net = new SchematicNet
            {
                Id = 1,
                Name = "Net$1"
            };
            netManager.Add(net);

            var netWire = new NetWireCanvasItem
            {
                Net = net,
                Points = new List<XPoint>
                {
                    new XPoint(25.4, 26.67),
                    new XPoint(38.1, 26.67),
                    new XPoint(38.1, 52.07),
                    new XPoint(50.8, 52.07)
                }
            };
            var pin1 = part1.Pins.FirstOrDefault();
            var pin2 = part2.Pins.FirstOrDefault();

            pin1.Net = net;
            pin2.Net = net;

            //select segments
            netWire.SelectSegmentAppend(1);

            //act
            var segmentRemover = new SegmentRemoverHelper(_canvasModel, netWire, _dispatcher);
            segmentRemover.RemoveSelectedSegments();

            //assert
            //no net wires
            Assert.NotEmpty(_canvasModel.Items.OfType<NetWireCanvasItem>());
            //nothing is left on this net
            Assert.Empty(net.NetItems);//?

            //nets on net manager
            var existingNet = netManager.Get(net.Name);
            Assert.Null(existingNet);

            //pins are connected
            Assert.NotNull(pin1.Net);
            Assert.NotNull(pin2.Net);

            //nets on pins are different
            Assert.True(pin1.Net.Name != pin2.Net.Name);

            existingNet = netManager.Get(pin1.Net.Name);
            Assert.NotNull(existingNet);

            existingNet = netManager.Get(pin2.Net.Name);
            Assert.NotNull(existingNet);
        }



        private SchematicSymbolCanvasItem PlacePart(Symbol symbol, double x, double y)
        {
            var part = SchematicTestsHelper.CreatePart(symbol, x, y);

            _canvasModel.AddItem(part);

            return part;
        }
    }
}
