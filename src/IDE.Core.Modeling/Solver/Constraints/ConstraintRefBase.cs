using IDE.Core.Modeling.Solver.Constraints.Primitives;
using System.Collections.Generic;

namespace IDE.Core.Modeling.Solver.Constraints
{
    public abstract class ConstraintRefBase
    {
        public RefLine L1;
        public RefLine L2;
        public RefCircle Circle1;
        public RefCircle Circle2;
        public RefPoint P1;
        public RefPoint P2;
        public RefPoint P3;
        public RefPoint P4;
        public RefLine SymLine;
        public RefArc Arc1;
        public RefArc Arc2;
        public double Distance
        { get; set; }

        public double Length
        { get; set; }

        public double Radius
        { get; set; }

        public int Index
        { get; set; }

        public double StartAngle;
        public double EndAngle;

        public abstract double Calc(List<double> parameters);

        public abstract double Grad(List<double> parameters, int index);
    }
}
