using System;

namespace IDE.Core.Types.Media
{
    public struct XLineDirection
    {
        public XLineDirection(XVector vector)
        {
            direction = MapDirection.Other;
            Build(vector);
        }

        public XLineDirection(XLineSegment segment)
            : this(segment.EndPoint - segment.StartPoint)
        {

        }

        public XLineDirection(XPoint startPoint, XPoint endPoint)
            : this(endPoint - startPoint)
        {

        }

        public XLineDirection(MapDirection newDir)
        {
            direction = newDir;
        }

        MapDirection direction;

        public MapDirection Direction => direction;

        private void Build(XVector vector)
        {
            direction = MapDirection.Other;

            if (vector.X == 0 && vector.Y == 0)
                return;

            var mag = -180.0 / Math.PI * Math.Atan2(vector.Y, vector.X) - 90;

            if (mag >= 360.0)
                mag -= 360.0;

            if (mag < 0.0)
                mag += 360.0;

            int dir = (int)((mag + 22.5) / 45.0);

            if (dir >= 8)
                dir = dir - 8;

            if (dir < 0)
                dir = dir + 8;

            direction = (MapDirection)dir;
        }

        public XLineDirection Left()
        {
            var newDir = (MapDirection)((int)(direction + 1) % 8);

            return new XLineDirection(newDir);
        }

        public XLineDirection Right()
        {
            var newDir = direction;
            if (newDir == MapDirection.Other)
                newDir = MapDirection.N;

            if (direction == MapDirection.N)
                newDir = MapDirection.NE;
            else
                newDir = (MapDirection)(direction - 1);

            return new XLineDirection(newDir);
        }

        public bool IsObtuse(XLineDirection other)
        {
            return Angle(other) == AngleType.OBTUSE;
        }

        public AngleType Angle(XLineDirection other)
        {
            if (direction == MapDirection.Other || other.direction == MapDirection.Other)
                return AngleType.UNDEFINED;

            int d = (int)Math.Abs(direction - other.direction);

            if (d == 5 || d == 3)
                return AngleType.OBTUSE;
            else if (d == 2 || d == 6)
                return AngleType.RIGHT;
            else if (d == 1 || d == 7)
                return AngleType.ACUTE;
            else if (d == 4)
                return AngleType.HALF_FULL;
            else
                return AngleType.STRAIGHT;
        }


        public XVector ToVector()
        {
            switch (direction)
            {
                case MapDirection.N: return new XVector(0, -1);
                case MapDirection.S: return new XVector(0, 1);
                case MapDirection.E: return new XVector(1, 0);
                case MapDirection.W: return new XVector(-1, 0);
                case MapDirection.NE: return new XVector(1, -1);
                case MapDirection.NW: return new XVector(-1, -1);
                case MapDirection.SE: return new XVector(1, 1);
                case MapDirection.SW: return new XVector(-1, 1);

                default:
                    return new XVector(0, 0);
            }
        }
    }
}
