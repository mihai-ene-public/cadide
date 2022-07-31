using IDE.Core.Interfaces;

namespace IDE.Core.Model.GlobalRepresentation.Primitives
{
    public abstract class GlobalPrimitive : IShape
    {
        public Dictionary<string, object> Tags { get; set; } = new Dictionary<string, object>();
        public abstract void AddClearance(double clearance);
    }
}
