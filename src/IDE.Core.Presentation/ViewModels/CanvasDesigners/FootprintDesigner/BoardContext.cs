using System.Collections.Generic;
using IDE.Core.Designers;
using IDE.Core.Interfaces;

namespace IDE.Documents.Views;

internal class BoardContext
{
    public ICanvasItem PcbBody { get; set; }

    public IList<ViaCanvasItem> ViaItems { get; set; } = new List<ViaCanvasItem>();
    public IList<IPadCanvasItem> PadItems { get; set; } = new List<IPadCanvasItem>();
    public IList<IPadCanvasItem> PadItemsOnTop { get; set; } = new List<IPadCanvasItem>();
    public IList<IPadCanvasItem> PadItemsOnBottom { get; set; } = new List<IPadCanvasItem>();

    public IList<ICanvasItem> MillingItems { get; set; } = new List<ICanvasItem>();
    public IList<IHoleCanvasItem> DrillItems { get; set; } = new List<IHoleCanvasItem>();
    public IList<ICanvasItem> CanvasItems { get; set; } = new List<ICanvasItem>();
    public IList<ICanvasItem> FootprintItems { get; set; } = new List<ICanvasItem>();
}
