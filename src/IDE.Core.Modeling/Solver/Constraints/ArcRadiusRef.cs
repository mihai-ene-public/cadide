using System.Collections.Generic;

namespace IDE.Core.Modeling.Solver.Constraints
{
    public class ArcRadiusRef : ConstraintRefBase
    {
        public override double Calc(List<double> parameters)
        {
            var rad1 = Utils.Hypot(parameters[Arc1.Center.X] - parameters[Arc1.Start.X], parameters[Arc1.Center.Y] - parameters[Arc1.Start.Y]);
            var temp = rad1 - Radius;
            return temp * temp;
        }
        public override double Grad(List<double> parameters, int index)
        {
            return 0;
        }
    }
}
