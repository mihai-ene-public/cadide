using System;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Placement;

public class CirclePlacementTool : PlacementTool, ICirclePlacementTool
{
    ICircleCanvasItem GetItem() => canvasItem as ICircleCanvasItem;


    const double minDiameter = 0.1;
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
            case PlacementStatus.Started:

                var w = (mp.X - item.X) * 2;
                var h = (mp.Y - item.Y) * 2;

                var d = Math.Sqrt(w * w + h * h);

                if (d < minDiameter)
                    d = minDiameter;

                item.Diameter = d;

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
                PlacementStatus = PlacementStatus.Started;
                break;
            //2nd click
            case PlacementStatus.Started:

                var w = (mp.X - item.X) * 2;
                var h = (mp.Y - item.Y) * 2;

                var d = Math.Sqrt(w * w + h * h);

                if (d >= minDiameter)
                {

                    item.Diameter = d;

                    item.IsPlaced = true;
                    CommitPlacement();

                    var newItem = (ISelectableItem)canvasItem.Clone();

                    PlacementStatus = PlacementStatus.Ready;
                    canvasItem = newItem;

                    SetupCanvasItem();
                    CanvasModel.AddItem(canvasItem);
                }

                break;
        }
    }
}
