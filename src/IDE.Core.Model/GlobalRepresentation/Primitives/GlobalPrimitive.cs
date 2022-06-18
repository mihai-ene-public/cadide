using IDE.Core.Interfaces;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public abstract class GlobalPrimitive : IShape
    {
        public abstract void AddClearance(double clearance);
    }
}
