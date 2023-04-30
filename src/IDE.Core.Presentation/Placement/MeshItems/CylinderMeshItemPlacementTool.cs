using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Placement;

public class CylinderMeshItemPlacementTool : PlacementTool, ICylinderMeshItemPlacementTool
{
    CylinderMeshItem GetItem() => canvasItem as CylinderMeshItem;

    public override void PlacementMouseMove(XPoint mousePosition)
    {
        var mp = CanvasModel.SnapToGrid(mousePosition);

        var item = GetItem();

        switch (PlacementStatus)
        {
            case PlacementStatus.Ready:
                item.X = mp.X;
                item.Y = mp.Y;
                break;
        }
    }

    public override void PlacementMouseUp(XPoint mousePosition)
    {
        var mp = CanvasModel.SnapToGrid(mousePosition);

        var item = GetItem();

        switch (PlacementStatus)
        {
            //1st click
            case PlacementStatus.Ready:
                item.X = mp.X;
                item.Y = mp.Y;
                item.IsPlaced = true;
                CommitPlacement();

                var newItem = (ISelectableItem)canvasItem.Clone();
                newItem.IsPlaced = false;
                PlacementStatus = PlacementStatus.Ready;
                canvasItem = newItem;

                CanvasModel.AddItem(canvasItem);
                break;
        }
    }
}
