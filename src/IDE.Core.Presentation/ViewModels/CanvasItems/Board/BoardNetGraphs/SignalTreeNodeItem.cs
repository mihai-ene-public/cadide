using IDE.Core.Interfaces;
using IDE.Core.Spatial2D;

namespace IDE.Core.Designers
{
    class SignalTreeNodeItem : ISpatialData
    {
        public ISignalPrimitiveCanvasItem CanvasItem { get; set; }

        //?
        public object ItemGeometry { get; set; }

        /// <summary>
        /// the tree rectangle it was registered with
        /// </summary>
        public Envelope Envelope { get; set; }

        public bool Walked { get; set; }
    }

}
