using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Modeling.Solver.Constraints.Primitives
{
    public class Arc
    {
        public Point Center;
        public DoubleRefObject EndAngle;
        public DoubleRefObject Radius;
        public DoubleRefObject StartAngle;

        public double StartX
        {
            get { return Center.X.Value + Radius.Value * Math.Cos(StartAngle.Value); }
        }

        public double StartY
        {
            get { return Center.Y.Value + Radius.Value * Math.Sin(StartAngle.Value); }
        }

        public double EndX
        {
            get { return Center.X.Value + Radius.Value * Math.Cos(EndAngle.Value); }
        }

        public double EndY
        {
            get { return Center.Y.Value + Radius.Value * Math.Sin(EndAngle.Value); }
        }
    }
}
