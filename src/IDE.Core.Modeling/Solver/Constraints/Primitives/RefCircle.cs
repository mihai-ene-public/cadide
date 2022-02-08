namespace IDE.Core.Modeling.Solver.Constraints.Primitives
{
    public struct RefCircle
    {
        public RefPoint Center;
        public int Radius;

        public RefCircle(int radius, int x, int y)
        {
            Radius = radius;
            Center = new RefPoint(x, y);
        }
    }
}
