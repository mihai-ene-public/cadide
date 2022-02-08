using IDE.Core.Types.Media;
using Xunit;

namespace IDE.Core.Presentation.Tests
{
    public class XLineSegmentTests
    {
        public static object[][] PerpendicularTestPointData =
        {
            new object[]{new XPoint(0, 0), new XPoint(4, 0), new XPoint(2, 1), new XPoint(2, 0) },
            new object[]{new XPoint(0, 0), new XPoint(8, 0), new XPoint(4, 1), new XPoint(4, 0) },
        };

        [Theory]
        [MemberData(nameof(PerpendicularTestPointData))]
        public void PerpendicularTest(XPoint sp, XPoint ep, XPoint p, XPoint expectedPerpPoint)
        {
            var seg = new XLineSegment(sp, ep);

            var actual = seg.GetPointDistanceToSegment(p);

            Assert.Equal(expectedPerpPoint, actual);
        }
    }
}
