using System;
using System.Collections.Generic;

namespace IDE.Core.Modeling.Solver.Constraints
{
    public class PointOnArcMidPointRef : ConstraintRefBase
    {
        public override double Calc(List<double> parameters)
        {
            var rad1 = Utils.Hypot(parameters[Arc1.Center.X] - parameters[Arc1.Start.X], parameters[Arc1.Center.Y] - parameters[Arc1.Start.Y]);
            var temp = Math.Atan2(parameters[Arc1.Start.Y] - parameters[Arc1.Center.Y], parameters[Arc1.Start.X] - parameters[Arc1.Center.X]);
            var temp2 = Math.Atan2(parameters[Arc1.End.Y] - parameters[Arc1.Center.Y], parameters[Arc1.End.X] - parameters[Arc1.Center.X]);
            var ex = parameters[Arc1.Center.X] + rad1 * Math.Cos((temp2 + temp) / 2);
            var ey = parameters[Arc1.Center.Y] + rad1 * Math.Sin((temp2 + temp) / 2);
            temp = (ex - parameters[P1.X]);
            temp2 = (ey - parameters[P1.Y]);
            return temp * temp + temp2 * temp2;
        }

        public override double Grad(List<double> parameters, int index)
        {
            return 0;
        }
    }
}
