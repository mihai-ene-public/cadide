using System.Collections.Generic;
using IDE.Core.Designers;
using IDE.Core.Interfaces;

namespace IDE.Documents.Views
{
    internal class BoardContext
    {
        public ICanvasItem PcbBody { get; set; }

        public IList<ViaCanvasItem> ViaItems { get; set; } = new List<ViaCanvasItem>();
        public IList<IPadCanvasItem> PadItems { get; set; }
        public IList<IPadCanvasItem> PadItemsOnTop { get; set; }
        public IList<IPadCanvasItem> PadItemsOnBottom { get; set; }

        public IList<ICanvasItem> MillingItems { get; set; }
        public IList<ICanvasItem> DrillItems { get; set; }
    }
}
