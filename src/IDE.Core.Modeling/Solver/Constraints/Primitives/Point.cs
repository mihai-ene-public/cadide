using System;

namespace IDE.Core.Modeling.Solver.Constraints.Primitives
{
    public class Point
    {
        public Point(DoubleRefObject x, DoubleRefObject y)
        {
            X = x;
            Y = y;
        }

        public DoubleRefObject X { get; set; }
        public DoubleRefObject Y { get; set; }

        public double Distance(Point p)
        {
            return Math.Sqrt((X.Value - p.X.Value) * (X.Value - p.X.Value) + (Y.Value - p.Y.Value) * (Y.Value - p.Y.Value));
        }
    }
}
