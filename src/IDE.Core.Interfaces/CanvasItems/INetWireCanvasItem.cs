using IDE.Core.Types.Media;


namespace IDE.Core.Interfaces
{
    public interface INetWireCanvasItem : ISegmentedPolylineSelectableCanvasItem
    {
        XColor LineColor { get; set; }

        LineStyle LineStyle { get; set; }
    }
}
