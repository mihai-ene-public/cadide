using System;

namespace IDE.Core
{
    public static class DoubleHelper
    {
        public static bool IsAlmostEqualTo(double value1, double value2, double maximumDifferenceAllowed = 0.00001d)
        {
            // Handle comparisons of floating point values that may not be exactly the same
            return (Math.Abs(value2 - value1) < maximumDifferenceAllowed);
        }
    }
}
