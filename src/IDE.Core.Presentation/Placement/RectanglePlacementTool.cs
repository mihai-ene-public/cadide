using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Placement
{
    public class RectanglePlacementTool : PlacementTool
    {

        IRectangleCanvasItem GetItem() => canvasItem as IRectangleCanvasItem;

        const double minWidth = 0.1;
        const double minHeight = 0.1;

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
                    var w = 2 * (mp.X - item.X);
                    var h = 2 * (mp.Y - item.Y);
                    if (w < minWidth)
                        w = minWidth;
                    if (h < minHeight)
                        h = minHeight;
                    item.Width = w;
                    item.Height = h;
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
                    var w = 2 * (mp.X - item.X);
                    var h = 2 * (mp.Y - item.Y);
                    if (w > minWidth && h > minHeight)
                    {
                        item.Width = w;
                        item.Height = h;
                        item.IsPlaced = true;
                        CanvasModel.OnDrawingChanged(DrawingChangedReason.ItemPlacementFinished);


                        var newItem = (IRectangleCanvasItem)canvasItem.Clone();

                        PlacementStatus = PlacementStatus.Ready;
                        canvasItem = newItem;

                        SetupCanvasItem();
                        CanvasModel.AddItem(canvasItem);
                    }
                    break;
            }
        }
    }
}
