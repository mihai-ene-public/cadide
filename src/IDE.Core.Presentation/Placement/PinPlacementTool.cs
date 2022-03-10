using System.Linq;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Placement
{
    public class PinPlacementTool : PlacementTool, IPinPlacementTool
    {
        IPinCanvasItem GetItem() => canvasItem as IPinCanvasItem;

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

                    var newItem = (IPinCanvasItem)canvasItem.Clone();

                    PlacementStatus = PlacementStatus.Ready;
                    canvasItem = newItem;

                    CanvasModel.AddItem(canvasItem);

                    SetupCanvasItem();
                    break;
            }
        }

        public override void SetupCanvasItem()
        {
            base.SetupCanvasItem();

            var item = GetItem();

            var lastPinNumber = CanvasModel.GetItems().OfType<IPinCanvasItem>()
                                        .OrderBy(p => p.Number, new IDE.Documents.Views.IndexedNameComparer())
                                        .Select(p => p.Number)
                                        .LastOrDefault();

            item.Number = lastPinNumber.GetNextIndexedName();
            item.Name = item.Number;
        }
    }
}
