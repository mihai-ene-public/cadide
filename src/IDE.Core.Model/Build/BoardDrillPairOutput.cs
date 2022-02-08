using IDE.Core.Interfaces;
using System.Collections.Generic;

namespace IDE.Core.Build
{
    public class BoardDrillPairOutput
    {
        public ILayerPairModel DrillPair { get; set; }

        public IEnumerable<ICanvasItem> MillingItems { get; set; }
        public IEnumerable<IHoleCanvasItem> DrillItems { get; set; }
    }
}
