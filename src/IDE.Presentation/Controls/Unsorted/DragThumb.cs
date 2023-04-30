using IDE.Core.Designers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using IDE.Core.Utilities;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.Converters;
using IDE.Core.Presentation.Adorners;
using IDE.Controls;
using IDE.Core.Presentation.Utilities;

namespace IDE.Core.Controls
{
    public class DragThumb : Thumb
    {
        public DragThumb()
        {
            Cursor = Cursors.SizeAll;
        }

        private XPoint? dragStart;

        private double deltaXSum = 0;
        private double deltaYSum = 0;

        private List<XPoint> originalWirePoints;
        //private int originalSelectionStart;

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (e.ChangedButton != MouseButton.Left)
                return;
            var designerItem = DataContext as BaseCanvasItem;

            if (designerItem == null || !designerItem.IsPlaced || e.Source is PositionThumb)
                return;

            if (designerItem.IsEditing)
                return;

            if (Mouse.Captured != null)
                return;

            HandleSelectionMouseDown(e);

            var drawingCanvas = this.FindParent<DrawingCanvas>();
            var canvasModel = this.FindParentDataContext<ICanvasViewModel>();

            if (canvasModel == null)
                return;

            var posDpi = e.GetPosition(drawingCanvas).ToXPoint();
            var pos = MilimetersToDpiHelper.ConvertToMM(posDpi);
            dragStart = SnapHelper.SnapToGrid(pos, canvasModel.GridSize);
            CaptureMouse();
            e.Handled = true;
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            var designerItem = DataContext as BaseCanvasItem;

            if (designerItem == null || !designerItem.IsPlaced)
                return;
            if (designerItem.IsEditing || designerItem.IsLocked)
                return;

            var canvasModel = this.FindParentDataContext<ICanvasDesignerFileViewModel>();
            if (deltaXSum != 0 && deltaYSum != 0)
            {

                if (canvasModel != null && !canvasModel.IsPlacingItem())
                {
                    var designerItems = canvasModel.SelectedItems.Where(c => c.IsLocked == false)
                                                                 .Cast<BaseCanvasItem>()
                                                                 .ToList();

                    canvasModel.RegisterUndoActionExecuted(
                        undo: o =>
                        {
                            foreach (var item in designerItems)
                            {
                                item.Translate(-deltaXSum, -deltaYSum);
                            }
                            return null;
                        },
                        redo: o =>
                        {
                            foreach (var item in designerItems)
                            {
                                item.Translate(deltaXSum, deltaYSum);
                            }
                            return null;
                        }
                        , null
                        );
                }
            }
            else if (originalWirePoints != null && designerItem is ISegmentedPolylineSelectableCanvasItem wire)
            {
                if (canvasModel != null && !canvasModel.IsPlacingItem())
                {
                    var currentPoints = wire.Points.ToList();
                    var oldPoints = originalWirePoints.ToList();
                    originalWirePoints = null;
                    //var currentSelectionStart = wire.SelectedSegmentStart;
                    //var oldSelectionStart = originalSelectionStart;

                    if (!SamePoints(currentPoints, oldPoints))
                    {
                        canvasModel.RegisterUndoActionExecuted(
                                               undo: o =>
                                               {
                                                   wire.Points = oldPoints;
                                                   wire.ClearSelection();
                                                   return null;
                                               },
                                               redo: o =>
                                               {
                                                   wire.Points = currentPoints;
                                                   wire.ClearSelection();
                                                   return null;
                                               }
                                               , null
                                               );
                    }

                }
            }

            dragStart = null;
            ReleaseMouseCapture();
        }

        private bool SamePoints(IList<XPoint> points1, IList<XPoint> points2)
        {
            if (points1.Count != points2.Count)
            {
                return false;
            }

            for (int i = 0; i < points1.Count; i++)
            {
                if (points1[i] != points2[i])
                    return false;
            }

            return true;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            var designerItem = DataContext as BaseCanvasItem;
            if (designerItem == null || !designerItem.IsPlaced)
                return;
            if (designerItem.IsEditing || designerItem.IsLocked)
                return;

            if (dragStart.HasValue && e.LeftButton == MouseButtonState.Pressed)
            {
                var drawingCanvas = this.FindParent<DrawingCanvas>();
                var canvasModel = this.FindParentDataContext<ICanvasDesignerFileViewModel>();
                if (canvasModel == null)
                {
                    return;
                }

                var currentPos = canvasModel.SnapToGridFromDpi(e.GetPosition(drawingCanvas).ToXPoint());
                var deltaX = currentPos.X - dragStart.Value.X;
                var deltaY = currentPos.Y - dragStart.Value.Y;

                if (deltaX == 0 && deltaY == 0)
                    return;

                dragStart = currentPos;

                if (designerItem != null && designerItem.IsSelected && designerItem.CanEdit)
                {
                    // we only move DesignerItems that are not locked
                    var designerItems = canvasModel.SelectedItems.Where(c => c.IsLocked == false).ToList();

                    if (designerItems.Count == 0)
                        return;

                    if (designerItems.Count == 1 && designerItems[0] is ISegmentedPolylineSelectableCanvasItem wire)
                    {
                        if (wire.HasSelectedSegments() && wire.SelectedSegmentStart == wire.SelectedSegmentEnd)
                        {
                            if (originalWirePoints == null)
                            {
                                originalWirePoints = wire.Points.ToList();
                                //originalSelectionStart = wire.SelectedSegmentStart;
                            }

                            //drag the first segment
                            var segmentIndex = wire.SelectedSegmentStart;
                            var dragHelper = new SegmentDragHelper(canvasModel, wire);
                            dragHelper.DragSegment(currentPos, segmentIndex);
                        }
                    }
                    else
                    {
                        foreach (var item in designerItems.OfType<BaseCanvasItem>())
                        {
                            item.Translate(deltaX, deltaY);
                        }

                        deltaXSum += deltaX;
                        deltaYSum += deltaY;
                    }

                    e.Handled = true;//?
                }
            }
        }

        bool HandleSelectionMouseDown(MouseButtonEventArgs e)
        {
            var originalElement = e.OriginalSource as FrameworkElement;
            var selectableItem = DataContext as ISelectableItem;
            var originalItem = originalElement?.DataContext as ISelectableItem;
            if (selectableItem is LayerDesignerItem)
                return false;

            if (selectableItem != null)
            {
                var canvasModel = this.FindParentDataContext<ICanvasViewModel>();

                if (canvasModel == null)
                    return false;

                //if we are during placing an object ignore all these
                if (canvasModel is ICanvasDesignerFileViewModel cm && cm.PlacementTool != null)
                    return false;

                var kbModifiers = Keyboard.Modifiers;
                //Ctrl will select; Shift will highlight
                if (kbModifiers.HasFlag(ModifierKeys.Control) || kbModifiers.HasFlag(ModifierKeys.Shift))
                {
                    //var toggleSelected
                    if (kbModifiers.HasFlag(ModifierKeys.Shift))
                    {
                        ToggleHighlightForPinsOrPads(selectableItem, originalItem);
                    }
                    else
                    {
                        //toggle select
                        ToggleSelect(selectableItem);
                    }
                    //don't tunnel
                    e.Handled = true;
                }
                else if (selectableItem.IsSelected)
                {
                    //if it is already selected, we show the adorner on the second mouse down
                    //we must have one item selected to show the adorner
                    if (canvasModel.SelectedItems.Count == 1)
                    {
                        //to improve experience, select single segment
                        SelectWireSegment(selectableItem);
                    }

                    //if already selected, then we exit, without updating selection
                    return false;
                }
                else if (!selectableItem.IsSelected) //deselect all and select item
                {
                    //this can be a pad inside a footprint
                    if (selectableItem.ParentObject != null)
                        return false;

                    SelectSingle(selectableItem, canvasModel);

                    //don't tunnel; this is important because for a poly is always selected and not something under it or above
                    e.Handled = true;

                }

                canvasModel.UpdateSelection();

                ShowAdorner(selectableItem, canvasModel);

                return true;

                //!! never put e.Handled = true here; DragThumb will not work anymore
                // e.Handled = false;
            }

            return false;
        }

        private XPoint GetCanvasPosition()
        {
            var drawingCanvas = this.FindParent<DrawingCanvas>();
            var mousePos = Mouse.GetPosition(drawingCanvas);
            var posMM = MilimetersToDpiHelper.ConvertToMM(mousePos.ToXPoint());
            return posMM;
        }

        private static void ToggleHighlightForPinsOrPads(ISelectableItem selectableItem, ISelectableItem originalItem)
        {
            if (originalItem != null && selectableItem is IContainerSelectableItem)
            {
                //toggle highlight for pins or pads

                //highlight only pins and pads
                if (originalItem is PinCanvasItem pin)
                {
                    pin.Net.HighlightNet(!pin.Net.IsHighlighted);
                }
                else if (originalItem is IPadCanvasItem pad)//(originalItem.ParentObject is IPadCanvasItem pad)
                {
                    if (pad != null && pad.Signal != null)
                        pad.Signal.HighlightNet(!pad.Signal.IsHighlighted);
                }
            }
            else if (selectableItem is ISignalPrimitiveCanvasItem signalItem)
            {
                //toggle highlight for board net items (tracks, vias, pads)
                signalItem?.Signal?.HighlightNet(!signalItem.Signal.IsHighlighted);
            }
        }

        private void ToggleSelect(ISelectableItem selectableItem)
        {
            if (selectableItem is ISegmentedPolylineSelectableCanvasItem wire)
            {
                var pos = GetCanvasPosition();

                wire.ToggleSelectSegmentAppendAtPosition(pos);
            }
            else
            {
                selectableItem.ToggleSelect();
            }
        }

        private void SelectSingle(ISelectableItem selectableItem, ICanvasViewModel canvasModel)
        {
            //deselect all items and select this one
            foreach (var item in canvasModel.GetItems())
            {
                item.IsSelected = false;
            }

            selectableItem.IsSelected = true;

            SelectWireSegment(selectableItem);
        }

        private void SelectWireSegment(ISelectableItem selectableItem)
        {
            if (selectableItem is ISegmentedPolylineSelectableCanvasItem wire)
            {
                var pos = GetCanvasPosition();

                wire.SelectSegmentAtPosition(pos);
            }
        }

        private void ShowAdorner(ISelectableItem selectableItem, ICanvasViewModel canvasModel)
        {
            if (canvasModel is ICanvasDesignerFileViewModel && selectableItem.IsSelected)
            {
                if (canvasModel.SelectedItems.Count == 1)
                {
                    CanvasItemToAdornerMapper.ShowAdorner(selectableItem, this);
                }
            }
        }
    }
}
