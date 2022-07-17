using IDE.Core.Types.Media;
using System.Windows;

namespace IDE.Core.Interfaces
{
    /*
    Placement of object from a toolbox works as following

        From the toolbox, user selects a primitive or component; PlacementData.PlacingObject = instance of selected type in toolbox (Status = Ready)
            - add the item to the canvas to be rendered
            - on mouse move we might show the object at mouse position (update the placing object position)
        For a Line:
            - when status is Ready we don't have to show anything
            - on first mouse click (mouseup) we add the 1st point; Status = Started; on mouse move update the second point with the mouse position
            - on 2nd click we add the 2nd point; create a new line and add it to canvas; 1st point is the last point; the second updates with mouse position; Status = Started

    */

    public interface IPlacementTool
    {
        ISelectableItem CanvasItem { get; }

        PlacementStatus PlacementStatus { get; set; }

        void ChangeMode();

        void CyclePlacement();

        void PlacementMouseMove(XPoint mousePosition);

        void PlacementMouseUp(XPoint mousePosition);
    }
}
