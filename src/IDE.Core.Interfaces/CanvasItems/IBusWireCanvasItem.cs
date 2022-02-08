using IDE.Core.Types.Media;


namespace IDE.Core.Interfaces
{
    public interface IBusWireCanvasItem : ISegmentedPolylineSelectableCanvasItem
    {
        XColor LineColor { get; set; }

    }
}
