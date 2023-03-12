using IDE.Core.Types.Media;
using System;

namespace IDE.Core.Spatial2D
{
    public class Envelope : IEquatable<Envelope>
    {
        public double MinX;
        public double MinY;
        public double MaxX;
        public double MaxY;

        public double Area
        {
            get
            {
                return Math.Max(MaxX - MinX, 0) * Math.Max(MaxY - MinY, 0);
            }
        }

        public double Margin
        {
            get
            {
                return Math.Max(MaxX - MinX, 0) + Math.Max(MaxY - MinY, 0);
            }
        }

        public Envelope() { }
        public Envelope(double minX, double minY, double maxX, double maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        public Envelope(double x, double y)
        {
            MinX = x;
            MaxX = x;
            MinY = y;
            MaxY = y;
        }

        public void Extend(Envelope other)
        {
            MinX = Math.Min(MinX, other.MinX);
            MinY = Math.Min(MinY, other.MinY);
            MaxX = Math.Max(MaxX, other.MaxX);
            MaxY = Math.Max(MaxY, other.MaxY);
        }

        public Envelope Clone()
        {
            return new Envelope
            {
                MinX = MinX,
                MinY = MinY,
                MaxX = MaxX,
                MaxY = MaxY,
            };
        }

        public Envelope Intersection(Envelope other)
        {
            return new Envelope
            {
                MinX = Math.Max(MinX, other.MinX),
                MinY = Math.Max(MinY, other.MinY),
                MaxX = Math.Min(MaxX, other.MaxX),
                MaxY = Math.Min(MaxY, other.MaxY),
            };
        }

        public Envelope Enlargement(Envelope other)
        {
            var clone = Clone();
            clone.Extend(other);
            return clone;
        }

        public bool Contains(Envelope other)
        {
            return
                MinX <= other.MinX &&
                MinY <= other.MinY &&
                MaxX >= other.MaxX &&
                MaxY >= other.MaxY;
        }

        public bool Contains(XPoint p)
        {
            return p.X >= MinX && p.X <= MaxX && p.Y >= MinY && p.Y <= MaxY;
        }

        public bool Intersects(Envelope other)
        {
            return
                MinX <= other.MaxX &&
                MinY <= other.MaxY &&
                MaxX >= other.MinX &&
                MaxY >= other.MinY;
        }

        public static Envelope InfiniteBounds
        {
            get
            {
                return new Envelope
                {
                    MinX = double.NegativeInfinity,
                    MinY = double.NegativeInfinity,
                    MaxX = double.PositiveInfinity,
                    MaxY = double.PositiveInfinity,
                };
            }
        }


        public static Envelope EmptyBounds
        {
            get
            {
                return new Envelope
                {
                    MinX = double.PositiveInfinity,
                    MinY = double.PositiveInfinity,
                    MaxX = double.NegativeInfinity,
                    MaxY = double.NegativeInfinity,
                };
            }
        }


        //public static double Distance(Envelope a, Envelope b)
        //{
        //    var aLength = Math.Sqrt(Math.Pow(a.CenterX, 2) + Math.Pow(a.CenterY, 2));
        //    var bLength = Math.Sqrt(Math.Pow(b.CenterX, 2) + Math.Pow(b.CenterY, 2));
        //    return Math.Abs(aLength - bLength);
        //}
        public double Distance(XPoint point)
        {
            if (Contains(point))
                return 0.0d;

            double distanceSquared = 0;

            {
                double greatestMinX = Math.Max(MinX, point.X);
                double leastMaxX = Math.Min(MaxX, point.X);
                if (greatestMinX > leastMaxX)
                {
                    distanceSquared += ((greatestMinX - leastMaxX) * (greatestMinX - leastMaxX));
                }
            }

            {
                double greatestMinY = Math.Max(MinY, point.Y);
                double leastMaxY = Math.Min(MaxY, point.Y);
                if (greatestMinY > leastMaxY)
                {
                    distanceSquared += ((greatestMinY - leastMaxY) * (greatestMinY - leastMaxY));
                }
            }
            return (double)Math.Sqrt(distanceSquared);
        }

        public bool Equals(Envelope other)
        {
            return this == other;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Envelope left, Envelope right)
        {
            if (left == null && right == null)
                return true;
            if (left == null || right == null)
                return false;

            return left.MinX == right.MinX &&
                   left.MinY == right.MinY &&
                   left.MaxX == right.MaxX &&
                   left.MaxY == right.MaxY;
        }


        public static bool operator !=(Envelope left, Envelope right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return string.Format("Envelope: MinX:{0}, MinY:{1}, MaxX:{2}, MaxY:{3}", MinX, MinY, MaxX, MaxY);
        }

        public static Envelope FromRect(XRect rect)
        {
            return new Envelope(rect.Left, rect.Top, rect.BottomRight.X, rect.BottomRight.Y);
        }
    }
}
