using System.Collections.Generic;

namespace IDE.Core.Modeling.Solver.Constraints
{
    public class P2PDistanceRef : ConstraintRefBase
    {
        public override double Calc(List<double> parameters)
        {
            return (parameters[P1.X] - parameters[P2.X]) * (parameters[P1.X] - parameters[P2.X]) +
                    (parameters[P1.Y] - parameters[P2.Y]) * (parameters[P1.Y] - parameters[P2.Y]) - Distance * Distance;
        }

        public override double Grad(List<double> parameters, int index)
        {
            return 0;
        }
    }
}
