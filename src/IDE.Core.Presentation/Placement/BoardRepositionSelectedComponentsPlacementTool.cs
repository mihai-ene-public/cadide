using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Placement;

public class BoardRepositionSelectedComponentsPlacementTool : PlacementTool<FootprintBoardCanvasItem>, IBoardRepositionSelectedComponentsPlacementTool
{

    public BoardRepositionSelectedComponentsPlacementTool(IList<FootprintBoardCanvasItem> selectedItems)
    {
        selectedParts = selectedItems;
    }

    private IList<FootprintBoardCanvasItem> selectedParts;

    private XPoint originalPosition;

    public override void SetupCanvasItem()
    {
        if (selectedParts != null && selectedParts.Count > 0)
        {
            var fp = selectedParts[0];
            canvasItem = fp;
            canvasItem.IsPlaced = false;
            canvasItem.IsSelected = false;

            originalPosition = new XPoint(fp.X, fp.Y);
        }
        else
            canvasItem = null;
    }

    public override void CancelPlacement()
    {
        var item = GetItem();

        if (item == null)
            return;

        //restore position
        item.X = originalPosition.X;
        item.Y = originalPosition.Y;
    }

    public override void PlacementMouseMove(XPoint mousePosition)
    {
        var item = GetItem();

        if (item == null)
            return;
        var mp = CanvasModel.SnapToGrid(mousePosition);

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
        var item = GetItem();

        if (item == null)
            return;

        var mp = CanvasModel.SnapToGrid(mousePosition);

        switch (PlacementStatus)
        {
            case PlacementStatus.Ready:
                item.X = mp.X;
                item.Y = mp.Y;
                item.IsPlaced = true;

                CommitPlacement();

                if (selectedParts.Count > 0)
                    selectedParts.RemoveAt(0);

                SetupCanvasItem();

                if (canvasItem == null)
                {
                    CanvasModel.CancelPlacement();
                }

                break;
        }
    }

    protected override void RegisterUndoActionExecuted()
    {
        var item = GetItem();
        var oldPosition = originalPosition;
        var newPosition= new XPoint(item.X, item.Y);

        Func<object, object> undo = (i) =>
        {
            item.X = oldPosition.X;
            item.Y = oldPosition.Y;
            return item;
        };
        Func<object, object> redo = (i) =>
        {
            item.X = newPosition.X;
            item.Y = newPosition.Y;
            return item;
        };

        CanvasModel.RegisterUndoActionExecuted(undo, redo, item);
    }
}
