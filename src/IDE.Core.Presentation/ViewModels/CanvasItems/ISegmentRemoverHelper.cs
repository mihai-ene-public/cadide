using IDE.Core.Interfaces;

namespace IDE.Core.Designers;
public interface ISegmentRemoverHelper
{
    void RemoveSelectedSegments(ICanvasDesignerFileViewModel canvasModel, ISegmentedPolylineSelectableCanvasItem segmentedItem);
}