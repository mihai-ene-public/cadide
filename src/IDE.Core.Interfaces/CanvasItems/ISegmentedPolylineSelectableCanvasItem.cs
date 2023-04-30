using IDE.Core.Types.Media;
using System.Collections.Generic;

namespace IDE.Core.Interfaces
{
    public interface ISegmentedPolylineSelectableCanvasItem : IPolylineCanvasItem
    {
        int SelectedSegmentStart { get; }
        int SelectedSegmentEnd { get; }

        IList<XPoint> SelectedPoints { get; }

        bool HasSelectedSegments();
        void ClearSelection();

        void SelectSegment(int segmentIndex);

        void SelectSegmentAppend(int segmentIndex);

        void ToggleSelectSegmentAppendAtPosition(XPoint mousePositionMM);

        void SelectSegmentAtPosition(XPoint mousePositionMM);
    }
}
