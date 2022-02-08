using IDE.Core.Interfaces;

namespace IDE.Core.Designers
{
    public class PlaneJoint : Joint
    {
        public IPlaneBoardCanvasItem Plane => Item as IPlaneBoardCanvasItem;
    }
}
