using System;
using System.Collections.Generic;

namespace IDE.Core.Modeling.Solver.Constraints
{
    public class PointOnCircleRef : ConstraintRefBase
    {
        public override double Calc(List<double> parameters)
        {
            var rad1 = Utils.Hypot(parameters[Circle1.Center.X] - parameters[P1.X], parameters[Circle1.Center.Y] - parameters[P1.Y]);

            //Compare this radius to the radius of the circle, return the error squared
            var temp = rad1 - parameters[Circle1.Radius];
            return Math.Abs(temp);// *temp;
        }
        public override double Grad(List<double> parameters, int index)
        {
            var dx = parameters[Circle1.Center.X] - parameters[P1.X];
            var dy = parameters[Circle1.Center.Y] - parameters[P1.Y];
            var hypotCalc = Utils.Hypot(parameters[Circle1.Center.X] - parameters[P1.X],
                                        parameters[Circle1.Center.Y] - parameters[P1.Y]);

            //Compare this radius to the radius of the circle, return the error squared
            var temp = hypotCalc - parameters[Circle1.Radius];
            var calc = temp >= 0 ? 1 : -1; ;

            if (index == P1.X)
                return calc * (-dx) / hypotCalc;
            if (index == P1.Y)
                return calc * (-dy) / hypotCalc;
            if (index == Circle1.Center.X)
                return calc * dx / hypotCalc;
            if (index == Circle1.Center.Y)
                return calc * dy / hypotCalc;
            if (index == Circle1.Radius)
                return -calc;

            return 0;
        }
    }
}
