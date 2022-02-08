using IDE.Core.Designers;
using IDE.Core.Types.Media;
using Xunit;

namespace IDE.Core.Presentation.Tests
{
    public class EllipseCanvasItemTests
    {
        [Theory]
        [InlineData(0, 0, 2, 1)]
        public void GetBoundingRectangle(double x, double y, double width, double height)
        {
            var item = new EllipseCanvasItem
            {
                X = x,
                Y = y,
                Width = width,
                Height = height
            };

            var expected = new XRect(x - width * 0.5, y - height * 0.5, width, height);

            var actual = item.GetBoundingRectangle();

            Assert.Equal(expected, actual);
        }
    }
}
