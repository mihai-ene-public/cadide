using System.Collections.Generic;

namespace IDE.Core.Modeling.Solver.Constraints
{
    public class PointOnPointRef : ConstraintRefBase
    {
        public override double Calc(List<double> parameters)
        {
            return (parameters[P2.X] - parameters[P1.X]) * (parameters[P2.X] - parameters[P1.X]) +
                   (parameters[P2.Y] - parameters[P1.Y]) * (parameters[P2.Y] - parameters[P1.Y]);
        }
        public override double Grad(List<double> parameters, int index)
        {
            var dx = parameters[P2.X] - parameters[P1.X];
            var dy = parameters[P2.Y] - parameters[P1.Y];
            var calc = 2;

            if (index == P2.X)
                return calc * dx;
            if (index == P2.Y)
                return calc * dy;
            if (index == P1.X)
                return -calc * dx;
            if (index == P1.Y)
                return -calc * dy;
            return 0;
        }
    }
}
