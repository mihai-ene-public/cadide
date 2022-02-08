using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Placement
{
    public class LinePlacementTool : PlacementTool
    {
        ILineCanvasItem GetItem() => canvasItem as ILineCanvasItem;

        public override void PlacementMouseMove(XPoint mousePosition)
        {
            var mp = CanvasModel.SnapToGrid(mousePosition);

            var item = GetItem();


            switch (PlacementStatus)
            {
                case PlacementStatus.Ready:
                    item.X1 = item.X2 = mp.X;
                    item.Y1 = item.Y2 = mp.Y;
                    break;
                case PlacementStatus.Started:
                    item.X2 = mp.X;
                    item.Y2 = mp.Y;
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
                    item.X1 = mp.X;
                    item.Y1 = mp.Y;
                    PlacementStatus = PlacementStatus.Started;
                    break;
                //2nd click
                case PlacementStatus.Started:
                    item.X2 = mp.X;
                    item.Y2 = mp.Y;
                    item.IsPlaced = true;
                    CanvasModel.OnDrawingChanged(DrawingChangedReason.ItemPlacementFinished);

                    var newItem = (ILineCanvasItem)canvasItem.Clone();
                    newItem.X1 = item.X2;
                    newItem.Y1 = item.Y2;


                    PlacementStatus = PlacementStatus.Started;
                    canvasItem = newItem;

                    SetupCanvasItem();
                    CanvasModel.AddItem(canvasItem);

                    break;
            }
        }
    }
}
