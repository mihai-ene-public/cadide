using IDE.Core.Interfaces;
using IDE.Core.Presentation.PlacementRouting;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Placement;

public class TrackPlacementTool<TrackRoutingModeType> : PlacementTool<ISelectableItem>, ITrackPlacementTool where TrackRoutingModeType: TrackRoutingMode, new()
{
    public TrackPlacementTool()
    {
        trackRoutingMode = new TrackRoutingModeType();
        //trackRoutingMode.PlacementTool = this;
        //trackRoutingMode.Start();
    }

    TrackRoutingModeType trackRoutingMode;

    public override void PlacementMouseMove(XPoint mousePosition)
    {
        //updateStartItem(true)
        trackRoutingMode.PlacementMouseMove(mousePosition);
    }

    public override void PlacementMouseUp(XPoint mousePosition)
    {
        //updateStartItem(ignorePads: false)
        //performRouting()

        trackRoutingMode.PlacementMouseUp(mousePosition);
    }

    public override void ChangeMode()
    {
        trackRoutingMode.ChangeMode();
    }

    public override void CyclePlacement()
    {
        trackRoutingMode.CyclePlacement();
    }


    public override void SetupCanvasItem()
    {
        base.SetupCanvasItem();

        trackRoutingMode.CanvasModel = CanvasModel;
        trackRoutingMode.Start();
    }
}
