namespace IDE.Core.Modeling.Solver.Constraints.Primitives
{
    public class RefPoint
    {
        public RefPoint(int index)
        {
            Index = index;
        }

        public RefPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public RefPoint(int index, int x, int y)
        {
            Index = index;
            X = x;
            Y = y;
        }

        public int Index { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}
