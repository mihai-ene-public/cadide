using System;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Placement
{
    public class ArcPlacementTool : PlacementTool, IArcPlacementTool
    {
        IArcCanvasItem GetArcItem() => canvasItem as IArcCanvasItem;

        bool sweepChangedDuringPlacement = false;

        public override void PlacementMouseMove(XPoint mousePosition)
        {
            var mp = CanvasModel.SnapToGrid(mousePosition);
            var item = GetArcItem();

            switch (PlacementStatus)
            {
                case PlacementStatus.Ready:
                    item.StartPointX = item.EndPointX = mp.X;
                    item.StartPointY = item.EndPointY = mp.Y;
                    break;
                case PlacementStatus.Started:
                    item.EndPointX = mp.X;
                    item.EndPointY = mp.Y;

                    var dx = item.EndPointX - item.StartPointX;
                    var dy = item.EndPointY - item.StartPointY;

                    var rx = dx == 0 ? 0.5 * dy : dx;
                    var ry = dy == 0 ? 0.5 * dx : dy;
                    rx = Math.Abs(rx);
                    ry = Math.Abs(ry);
                    rx = Math.Max(rx, ry);
                    if (rx == 0)
                        rx = 2;
                    item.Radius = rx;

                     if (!sweepChangedDuringPlacement)//?
                    {
                        if (dx == 0)
                        {
                            item.SweepDirection = dy > 0 ? XSweepDirection.Counterclockwise : XSweepDirection.Clockwise;
                        }
                        else if (dy == 0)
                        {
                            item.SweepDirection = dx > 0 ? XSweepDirection.Counterclockwise : XSweepDirection.Clockwise;
                        }
                        else
                        {
                            if (dx > 0)
                            {
                                item.SweepDirection = dy > 0 ? XSweepDirection.Counterclockwise : XSweepDirection.Clockwise;
                            }
                            else
                            {
                                item.SweepDirection = dy > 0 ? XSweepDirection.Clockwise : XSweepDirection.Counterclockwise;
                            }
                        }
                    }
                    break;
            }
        }

        public override void PlacementMouseUp(XPoint mousePosition)
        {
            var mp = CanvasModel.SnapToGrid(mousePosition);
            var item = GetArcItem();

            switch (PlacementStatus)
            {
                //1st click
                case PlacementStatus.Ready:
                    item.StartPointX = mp.X;
                    item.StartPointY = mp.Y;
                    PlacementStatus = PlacementStatus.Started;
                    break;
                //2nd click
                case PlacementStatus.Started:
                    item.EndPointX = mp.X;
                    item.EndPointY = mp.Y;
                    item.IsPlaced = true;
                    CanvasModel.OnDrawingChanged(DrawingChangedReason.ItemPlacementFinished);

                    var newItem = (IArcCanvasItem)canvasItem.Clone();
                    newItem.StartPointX = item.EndPointX;
                    newItem.StartPointY = item.EndPointY;

                    PlacementStatus = PlacementStatus.Started;
                    canvasItem = newItem;

                    SetupCanvasItem();
                    CanvasModel.AddItem(canvasItem);

                    break;
            }
        }

        public override void SetupCanvasItem()
        {
            base.SetupCanvasItem();

            sweepChangedDuringPlacement = false;
        }

        public override void CyclePlacement()
        {
            var item = GetArcItem();
            item.SweepDirection = (XSweepDirection)(1 - (int)item.SweepDirection);
            sweepChangedDuringPlacement = true;
        }
    }
}
