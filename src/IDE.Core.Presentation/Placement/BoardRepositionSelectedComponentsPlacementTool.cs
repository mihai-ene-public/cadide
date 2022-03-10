using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Documents.Views;

namespace IDE.Core.Presentation.Placement
{
    public class BoardRepositionSelectedComponentsPlacementTool : PlacementTool, IBoardRepositionSelectedComponentsPlacementTool
    {

        public BoardRepositionSelectedComponentsPlacementTool(IList<FootprintBoardCanvasItem> selectedItems)
        {
            selectedParts = selectedItems;
        }

        FootprintBoardCanvasItem GetItem() => canvasItem as FootprintBoardCanvasItem;

        IList<FootprintBoardCanvasItem> selectedParts;

        public override void SetupCanvasItem()
        {
            if (selectedParts != null && selectedParts.Count > 0)
            {
                canvasItem = selectedParts[0];
                canvasItem.IsPlaced = false;
                canvasItem.IsSelected = false;
            }
            else
                canvasItem = null;
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
                    CanvasModel.OnDrawingChanged(DrawingChangedReason.ItemPlacementFinished);

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
    }
}
