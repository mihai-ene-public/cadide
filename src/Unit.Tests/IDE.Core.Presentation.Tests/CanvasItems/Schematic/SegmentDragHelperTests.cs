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
    public class SegmentDragHelperTests
    {
        public SegmentDragHelperTests()
        {
            var c = new PrimitiveToCanvasItemMapper();//it register itself

            var debounceMock = new Mock<IDebounceDispatcher>();

            var dispatcherMock = new Mock<IDispatcherHelper>();
            dispatcherMock.Setup(x => x.RunOnDispatcher(It.IsAny<Action>()))
                           .Callback((Action action) =>
                           {
                               action();
                           });



            ServiceProvider.RegisterResolver(t =>
            {
                //if (t == typeof(IGeometryHelper))
                //    return new GeometryHelper();
                if (t == typeof(IDebounceDispatcher))
                    return debounceMock.Object;

                throw new NotImplementedException();
            });

            var schMock = new Mock<ISchematicDesigner>();
            schMock.SetupGet(x => x.NetManager)
                    .Returns(new SchematicNetManager());//mock net manager?

            _canvasModel = new DrawingViewModel(schMock.Object, dispatcherMock.Object);
            //_canvasModel.FileDocument = schMock.Object;
            ((CanvasGrid)_canvasModel.CanvasGrid).GridSizeModel.SelectedItem = new Units.MilUnit(50);
        }

        private readonly IDrawingViewModel _canvasModel;

        [Theory]
        [InlineData(36.83, 38.1)]
        [InlineData(39.37, 38.1)]
        public void ThreeSegments_DragVerticalMiddleSegment(double mpX, double mpY)
        {
            var mousePos = new XPoint(mpX, mpY);
            var segmentIndex = 1;

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

            netWire.SelectSegment(segmentIndex);

            var dragHelper = new SegmentDragHelper(_canvasModel, netWire);
            dragHelper.DragSegment(mousePos, segmentIndex);

            var expectedPoints = new List<XPoint>
            {
                 new XPoint(25.4, 26.67),
                 new XPoint(mousePos.X, 26.67),
                 new XPoint(mousePos.X, 52.07),
                 new XPoint(50.8, 52.07)
            };

            Assert.Equal(expectedPoints, netWire.Points);
        }

        [Theory]
        [InlineData(38.1, 38.1)]
        [InlineData(40.64, 38.1)]
        public void ThreeSegments_DragHorizontalMiddleSegment(double mpX, double mpY)
        {
            var mousePos = new XPoint(mpX, mpY);
            var segmentIndex = 1;

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
                    new XPoint(25.4, 39.37),
                    new XPoint(50.8, 39.37),
                    new XPoint(50.8, 52.07),
                }
            };
            var pin1 = part1.Pins.FirstOrDefault();
            var pin2 = part2.Pins.FirstOrDefault();

            pin1.Net = net;
            pin2.Net = net;

            netWire.SelectSegment(segmentIndex);

            var dragHelper = new SegmentDragHelper(_canvasModel, netWire);
            dragHelper.DragSegment(mousePos, segmentIndex);

            var expectedPoints = new List<XPoint>
            {
                 new XPoint(25.4, 26.67),
                 new XPoint(25.4, mousePos.Y),
                 new XPoint(50.8, mousePos.Y),
                 new XPoint(50.8, 52.07),
            };

            Assert.Equal(expectedPoints, netWire.Points);
        }

        [Fact]
        public void ThreeSegments_DragDiagonalMiddleSegment()
        {
            var mousePos = new XPoint(6.35, 13.97);
            var segmentIndex = 1;

            var netWire = new NetWireCanvasItem
            {
                Points = new List<XPoint>
                {
                    new XPoint(3.81, 2.54),
                    new XPoint(3.81, 8.89),
                    new XPoint(11.43, 16.51),
                    new XPoint(17.78, 16.51),
                }
            };

            netWire.SelectSegment(segmentIndex);

            var dragHelper = new SegmentDragHelper(_canvasModel, netWire);
            dragHelper.DragSegment(mousePos, segmentIndex);

            var expectedPoints = new List<XPoint>
            {
                 new XPoint(3.81, 2.54),
                 new XPoint(3.81, 10.16),
                 new XPoint(10.16, 16.51),
                 new XPoint(17.78, 16.51),
            };

            Assert.Equal(expectedPoints, netWire.Points);
        }


        private SchematicSymbolCanvasItem PlacePart(Symbol symbol, double x, double y)
        {
            var part = SchematicTestsHelper.CreatePart(symbol, x, y);

            _canvasModel.AddItem(part);

            return part;
        }
    }
}
