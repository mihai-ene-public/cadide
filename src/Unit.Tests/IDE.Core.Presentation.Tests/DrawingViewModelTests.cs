using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IDE.Core.Presentation.Tests
{
    public class DrawingViewModelTests
    {
        [Theory]
        [InlineData(0.00, -2.00, 0.00, -2.00)]
        [InlineData(0.00, -1.75, 0.00, -2.00)]
        [InlineData(0.00, -1.50, 0.00, -1.00)]
        [InlineData(0.00, -1.25, 0.00, -1.00)]
        [InlineData(0.00, -1.00, 0.00, -1.00)]
        [InlineData(0.00, -0.75, 0.00, -1.00)]
        [InlineData(0.00, -0.50, 0.00, 0.00)]
        [InlineData(0.00, -0.25, 0.00, 0.00)]
        [InlineData(0.00, 0.00, 0.00, 0.00)]
        [InlineData(0.00, 0.25, 0.00, 0.00)]
        [InlineData(0.00, 0.50, 0.00, 0.00)]
        [InlineData(0.00, 0.75, 0.00, 1.00)]
        [InlineData(0.00, 1.00, 0.00, 1.00)]
        [InlineData(0.00, 1.25, 0.00, 1.00)]
        [InlineData(0.00, 1.50, 0.00, 1.00)]
        [InlineData(0.00, 1.75, 0.00, 2.00)]
        [InlineData(0.00, 2.00, 0.00, 2.00)]

        [InlineData(-2.00, 0.00, -2.00, 0.00)]
        [InlineData(-1.75, 0.00, -2.00, 0.00)]
        [InlineData(-1.50, 0.00, -1.00, 0.00)]
        [InlineData(-1.25, 0.00, -1.00, 0.00)]
        [InlineData(-1.00, 0.00, -1.00, 0.00)]
        [InlineData(-0.75, 0.00, -1.00, 0.00)]
        [InlineData(-0.50, 0.00, 0.00, 0.00)]
        [InlineData(-0.25, 0.00, 0.00, 0.00)]
        [InlineData(0.25, 0.00, 0.00, 0.00)]
        [InlineData(0.50, 0.00, 0.00, 0.00)]
        [InlineData(0.75, 0.00, 1.00, 0.00)]
        [InlineData(1.00, 0.00, 1.00, 0.00)]
        [InlineData(1.25, 0.00, 1.00, 0.00)]
        [InlineData(1.50, 0.00, 1.00, 0.00)]
        [InlineData(1.75, 0.00, 2.00, 0.00)]
        [InlineData(2.00, 0.00, 2.00, 0.00)]
        public void SnapToGridReturnsSnapPoint(double x, double y, double expectedX, double expectedY)
        {
            var dispatcherMock = new Mock<IDispatcherHelper>();
            dispatcherMock.Setup(x => x.RunOnDispatcher(It.IsAny<Action>()))
                           .Callback((Action action) =>
                           {
                               action();
                           });



            var canvasModel = new DrawingViewModel(null, dispatcherMock.Object);

            var actual = canvasModel.SnapToGrid(new XPoint(x, y));

            Assert.Equal(expectedX, actual.X);
            Assert.Equal(expectedY, actual.Y);
        }
    }
}
