using System.Linq;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Placement
{
    public class PadPlacementTool : PlacementTool, IPadPlacementTool
    {
        IPadCanvasItem GetItem() => canvasItem as IPadCanvasItem;

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
                    CanvasModel.OnDrawingChanged(DrawingChangedReason.ItemPlacementFinished);

                    var newItem = (ISelectableItem)canvasItem.Clone();

                    PlacementStatus = PlacementStatus.Ready;
                    canvasItem = newItem;

                    SetupCanvasItem();
                    CanvasModel.AddItem(canvasItem);
                    break;
            }
        }

        public override void SetupCanvasItem()
        {
            base.SetupCanvasItem();

            var item = GetItem();

            var lastPadNumber = CanvasModel.GetItems().OfType<IPadCanvasItem>()
                                        .OrderBy(p => p.Number)
                                        .Select(p => p.Number)
                                        .LastOrDefault();

            item.Number = lastPadNumber.GetNextIndexedName();
        }
    }

}
