using IDE.Core.Types.Media;
using System;
using Xunit;

namespace IDE.Core.Presentation.Tests
{
    public class LineDirectionTests
    {
        public static object[][] PointData =
        {
            new object[]{new XPoint(), new XPoint(0,1), MapDirection.S },
            new object[]{new XPoint(), new XPoint(0,-1), MapDirection.N },
            new object[]{new XPoint(), new XPoint(1,0), MapDirection.E },
            new object[]{new XPoint(), new XPoint(-1,0), MapDirection.W },
            new object[]{new XPoint(), new XPoint(1,-1), MapDirection.NE },
            new object[]{new XPoint(), new XPoint(1,1), MapDirection.SE },
            new object[]{new XPoint(), new XPoint(-1,1), MapDirection.SW },
            new object[]{new XPoint(), new XPoint(-1,-1), MapDirection.NW },
        };

        public static object[][] VectorData =
        {
            new object[]{new XPoint(), new XPoint(0,1),   new XVector(0,1) },
            new object[]{new XPoint(), new XPoint(0,-1),  new XVector(0,-1)},
            new object[]{new XPoint(), new XPoint(1,0),   new XVector(1,0) },
            new object[]{new XPoint(), new XPoint(-1,0),  new XVector(-1,0)},
            new object[]{new XPoint(), new XPoint(1,-1),  new XVector(1,-1) },
            new object[]{new XPoint(), new XPoint(1,1),   new XVector(1,1)  },
            new object[]{new XPoint(), new XPoint(-1,1),  new XVector(-1,1) },
            new object[]{new XPoint(), new XPoint(-1,-1), new XVector(-1,-1) },
        };

        [Theory]
        [MemberData(nameof(PointData))]
        public void LineDirectionConstructorShouldSetDirection(XPoint startPoint, XPoint endPoint, MapDirection expected)
        {
            var direction = new XLineDirection(startPoint, endPoint);

            Assert.Equal(expected, direction.Direction);
        }

        [Theory]
        [MemberData(nameof(VectorData))]
        public void LineDirectionShouldGetVector(XPoint startPoint, XPoint endPoint, XVector expected)
        {
            var direction = new XLineDirection(startPoint, endPoint);

            var actual = direction.ToVector();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(MapDirection.N, MapDirection.NW)]
        [InlineData(MapDirection.NW, MapDirection.W)]
        [InlineData(MapDirection.W, MapDirection.SW)]
        [InlineData(MapDirection.SW, MapDirection.S)]
        [InlineData(MapDirection.S, MapDirection.SE)]
        [InlineData(MapDirection.SE, MapDirection.E)]
        [InlineData(MapDirection.E, MapDirection.NE)]
        [InlineData(MapDirection.NE, MapDirection.N)]
        public void Left_ShouldCalculate(MapDirection direction, MapDirection expected)
        {
            var dir = new XLineDirection(direction);

            var actual = dir.Left().Direction;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(MapDirection.N, MapDirection.NE)]
        [InlineData(MapDirection.NE, MapDirection.E)]
        [InlineData(MapDirection.E, MapDirection.SE)]
        [InlineData(MapDirection.SE, MapDirection.S)]
        [InlineData(MapDirection.S, MapDirection.SW)]
        [InlineData(MapDirection.SW, MapDirection.W)]
        [InlineData(MapDirection.W, MapDirection.NW)]
        [InlineData(MapDirection.NW, MapDirection.N)]
        public void Right_ShouldCalculate(MapDirection direction, MapDirection expected)
        {
            var dir = new XLineDirection(direction);

            var actual = dir.Right().Direction;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(MapDirection.N, MapDirection.N, AngleType.STRAIGHT)]
        [InlineData(MapDirection.N, MapDirection.NE, AngleType.ACUTE)]
        [InlineData(MapDirection.N, MapDirection.E, AngleType.RIGHT)]
        [InlineData(MapDirection.N, MapDirection.SE, AngleType.OBTUSE)]
        [InlineData(MapDirection.N, MapDirection.S, AngleType.HALF_FULL)]
        [InlineData(MapDirection.N, MapDirection.SW, AngleType.OBTUSE)]
        [InlineData(MapDirection.N, MapDirection.W, AngleType.RIGHT)]
        [InlineData(MapDirection.N, MapDirection.NW, AngleType.ACUTE)]

        [InlineData(MapDirection.NE, MapDirection.N, AngleType.ACUTE)]
        [InlineData(MapDirection.NE, MapDirection.NE, AngleType.STRAIGHT)]
        [InlineData(MapDirection.NE, MapDirection.E, AngleType.ACUTE)]
        [InlineData(MapDirection.NE, MapDirection.SE, AngleType.RIGHT)]
        [InlineData(MapDirection.NE, MapDirection.S, AngleType.OBTUSE)]
        [InlineData(MapDirection.NE, MapDirection.SW, AngleType.HALF_FULL)]
        [InlineData(MapDirection.NE, MapDirection.W, AngleType.OBTUSE)]
        [InlineData(MapDirection.NE, MapDirection.NW, AngleType.RIGHT)]

        [InlineData(MapDirection.E, MapDirection.N, AngleType.RIGHT)]
        [InlineData(MapDirection.E, MapDirection.NE, AngleType.ACUTE)]
        [InlineData(MapDirection.E, MapDirection.E, AngleType.STRAIGHT)]
        [InlineData(MapDirection.E, MapDirection.SE, AngleType.ACUTE)]
        [InlineData(MapDirection.E, MapDirection.S, AngleType.RIGHT)]
        [InlineData(MapDirection.E, MapDirection.SW, AngleType.OBTUSE)]
        [InlineData(MapDirection.E, MapDirection.W, AngleType.HALF_FULL)]
        [InlineData(MapDirection.E, MapDirection.NW, AngleType.OBTUSE)]

        [InlineData(MapDirection.SE, MapDirection.N, AngleType.OBTUSE)]
        [InlineData(MapDirection.SE, MapDirection.NE, AngleType.RIGHT)]
        [InlineData(MapDirection.SE, MapDirection.E, AngleType.ACUTE)]
        [InlineData(MapDirection.SE, MapDirection.SE, AngleType.STRAIGHT)]
        [InlineData(MapDirection.SE, MapDirection.S, AngleType.ACUTE)]
        [InlineData(MapDirection.SE, MapDirection.SW, AngleType.RIGHT)]
        [InlineData(MapDirection.SE, MapDirection.W, AngleType.OBTUSE)]
        [InlineData(MapDirection.SE, MapDirection.NW, AngleType.HALF_FULL)]

        [InlineData(MapDirection.S, MapDirection.N, AngleType.HALF_FULL)]
        [InlineData(MapDirection.S, MapDirection.NE, AngleType.OBTUSE)]
        [InlineData(MapDirection.S, MapDirection.E, AngleType.RIGHT)]
        [InlineData(MapDirection.S, MapDirection.SE, AngleType.ACUTE)]
        [InlineData(MapDirection.S, MapDirection.S, AngleType.STRAIGHT)]
        [InlineData(MapDirection.S, MapDirection.SW, AngleType.ACUTE)]
        [InlineData(MapDirection.S, MapDirection.W, AngleType.RIGHT)]
        [InlineData(MapDirection.S, MapDirection.NW, AngleType.OBTUSE)]

        [InlineData(MapDirection.SW, MapDirection.N, AngleType.OBTUSE)]
        [InlineData(MapDirection.SW, MapDirection.NE, AngleType.HALF_FULL)]
        [InlineData(MapDirection.SW, MapDirection.E, AngleType.OBTUSE)]
        [InlineData(MapDirection.SW, MapDirection.SE, AngleType.RIGHT)]
        [InlineData(MapDirection.SW, MapDirection.S, AngleType.ACUTE)]
        [InlineData(MapDirection.SW, MapDirection.SW, AngleType.STRAIGHT)]
        [InlineData(MapDirection.SW, MapDirection.W, AngleType.ACUTE)]
        [InlineData(MapDirection.SW, MapDirection.NW, AngleType.RIGHT)]

        [InlineData(MapDirection.W, MapDirection.N, AngleType.RIGHT)]
        [InlineData(MapDirection.W, MapDirection.NE, AngleType.OBTUSE)]
        [InlineData(MapDirection.W, MapDirection.E, AngleType.HALF_FULL)]
        [InlineData(MapDirection.W, MapDirection.SE, AngleType.OBTUSE)]
        [InlineData(MapDirection.W, MapDirection.S, AngleType.RIGHT)]
        [InlineData(MapDirection.W, MapDirection.SW, AngleType.ACUTE)]
        [InlineData(MapDirection.W, MapDirection.W, AngleType.STRAIGHT)]
        [InlineData(MapDirection.W, MapDirection.NW, AngleType.ACUTE)]

        [InlineData(MapDirection.NW, MapDirection.N, AngleType.ACUTE)]
        [InlineData(MapDirection.NW, MapDirection.NE, AngleType.RIGHT)]
        [InlineData(MapDirection.NW, MapDirection.E, AngleType.OBTUSE)]
        [InlineData(MapDirection.NW, MapDirection.SE, AngleType.HALF_FULL)]
        [InlineData(MapDirection.NW, MapDirection.S, AngleType.OBTUSE)]
        [InlineData(MapDirection.NW, MapDirection.SW, AngleType.RIGHT)]
        [InlineData(MapDirection.NW, MapDirection.W, AngleType.ACUTE)]
        [InlineData(MapDirection.NW, MapDirection.NW, AngleType.STRAIGHT)]
        public void Angle_ShouldCalculate(MapDirection first, MapDirection second, AngleType expected)
        {
            var dir1 = new XLineDirection(first);
            var dir2 = new XLineDirection(second);

            var actual = dir1.Angle(dir2);

            Assert.Equal(expected, actual);
        }
    }
}
