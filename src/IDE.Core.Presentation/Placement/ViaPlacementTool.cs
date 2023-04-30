using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.Presentation.Placement;

public class ViaPlacementTool : PlacementTool, IViaPlacementTool
{
    public ViaPlacementTool()
    {
        GeometryHelper = ServiceProvider.Resolve<IGeometryOutlineHelper>();
    }

    private readonly IGeometryOutlineHelper GeometryHelper;

    ViaCanvasItem GetItem() => canvasItem as ViaCanvasItem;

    BoardObstacleProvider obstaclesProvider;

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
            case PlacementStatus.Ready:
                item.X = mp.X;
                item.Y = mp.Y;

                var hitItems = new List<ISignalPrimitiveCanvasItem>();
                HandleLinePoint(item.Center, hitItems);
                item.IsPlaced = true;
                CommitPlacement();

                //create another
                var newItem = (ViaCanvasItem)canvasItem.Clone();

                PlacementStatus = PlacementStatus.Ready;
                canvasItem = newItem;

                CanvasModel.AddItem(canvasItem);
                break;

        }
    }

    public override void SetupCanvasItem()
    {
        base.SetupCanvasItem();

        if (obstaclesProvider == null)
            obstaclesProvider = new BoardObstacleProvider(CanvasModel);
        obstaclesProvider.BuildObstacles();
    }

    bool HandleLinePoint(XPoint linePointMM, List<ISignalPrimitiveCanvasItem> signalItems)
    {
        var item = GetItem();

        HitTestItems(linePointMM, signalItems);

        //we take the 1st pad; we could show a list with the pads
        var hitPad = signalItems.OfType<IPadCanvasItem>().FirstOrDefault() as BoardCanvasItemViewModel;
        var hitItem = signalItems.FirstOrDefault();
        ////////////////////////////////////////////

        if (hitPad != null)
        {
            var fpItem = hitPad.ParentObject as FootprintBoardCanvasItem;
            var fpInstance = fpItem.FootprintPrimitive;
            var fpInstanceId = fpInstance.Id;
            var padNumber = ((IPadCanvasItem)hitPad).Number;

            var board = fpItem.BoardModel as IBoardDesigner;

            //search a signal that has our pad
            var candidate = (from n in board.NetList
                             from p in n.Pads
                             where p.FootprintInstanceId == fpInstanceId && p.Number == padNumber
                             select p.Signal).FirstOrDefault();

            if (candidate != null)
            {
                if (item.Signal == null)
                    item.Signal = candidate;
                else
                    //we validate if we are on the same signal
                    return item.Signal.Name == candidate.Name;
            }

        }
        else if (hitItem != null)
        {
            if (item.Signal == null)
                item.Signal = hitItem.Signal;
            else
                //we validate if we are on the same signal
                return hitItem.Signal != null && item.Signal.Name == hitItem.Signal.Name;
        }


        return true;
    }

    void HitTestItems(XPoint point, List<ISignalPrimitiveCanvasItem> signalItems)
    {
        var item = GetItem();

        var obstacles = obstaclesProvider.GetNearestObstaclesAllLayers(point, item.Diameter);

        foreach (var obs in obstacles)
        {
            if (obs.CanvasItem is ISignalPrimitiveCanvasItem signalItem)
            {
                if (GeometryHelper.Intersects(item, signalItem))
                {
                    if (signalItem is IPadCanvasItem)
                    {
                        signalItems.Add(signalItem);
                    }
                    else if (signalItem.Signal != null && signalItem.IsPlaced)
                    {
                        signalItems.Add(signalItem);
                    }
                }
            }
        }
    }
}
