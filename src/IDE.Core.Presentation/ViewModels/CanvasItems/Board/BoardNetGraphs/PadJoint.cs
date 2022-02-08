using IDE.Core.Interfaces;

namespace IDE.Core.Designers
{
    public class PadJoint : Joint
    {
        public IPadCanvasItem Pad => Item as IPadCanvasItem;
    }
}
