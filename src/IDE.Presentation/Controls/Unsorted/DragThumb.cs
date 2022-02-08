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

        XPoint? dragStart;
        bool isSelecting = false;

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (e.ChangedButton != MouseButton.Left)
                return;
            var designerItem = DataContext as BaseCanvasItem;

            if (designerItem == null || !designerItem.IsPlaced || e.Source is PositionThumb)
                return;

            if (designerItem.IsEditing)// || designerItem.IsLocked)
                return;

            if (Mouse.Captured != null)
                return;

            HandleSelectionMouseDown(e);

            //if (e.Handled)
            //    return;

            var drawingCanvas = this.FindParent<DrawingCanvas>();
            var canvasModel = this.FindParentDataContext<IDrawingViewModel>();
            dragStart = canvasModel.SnapToGridFromDpi(e.GetPosition(drawingCanvas).ToXPoint());
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

            dragStart = null;
            ReleaseMouseCapture();
            //e.Handled = true;

            HandleSelectionMouseUp(e);
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
                var canvasModel = this.FindParentDataContext<IDrawingViewModel>();
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

                    if (designerItems == null)
                        return;//how about dragStart? should be set to null?

                    if (designerItems.Count == 1 && designerItems[0] is ISegmentedPolylineSelectableCanvasItem wire)
                    {
                        if (wire.HasSelectedSegments() && wire.SelectedSegmentStart == wire.SelectedSegmentEnd)
                        {
                            //drag the first segment
                            var segmentIndex = wire.SelectedSegmentStart;//wire.GetSegmentAtMousePosition(currentPos);
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
                    }

                    e.Handled = true;//?
                }

            }


        }

        /// <summary>
        /// returns true if we want to skip mouse capture
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        bool HandleSelectionMouseDown(MouseButtonEventArgs e)
        {
            var originalElement = e.OriginalSource as FrameworkElement;
            var selectableItem = DataContext as ISelectableItem;
            var originalItem = originalElement?.DataContext as ISelectableItem;
            if (selectableItem is LayerDesignerItem)
                return false;

            if (selectableItem != null)
            {
                var canvasModel = this.FindParentDataContext<IDrawingViewModel>();

                if (canvasModel == null)
                    return false;

                //if we are during placing an object ignore all these
                if (canvasModel.PlacementTool != null)
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
                        //selectableItem.ToggleSelect();
                        isSelecting = true;
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
                    //return true;

                    isSelecting = true;
                }


                canvasModel.UpdateSelection();

                ShowAdorner(selectableItem, canvasModel);

                return true;

                //!! never put e.Handled = true here; DragThumb will not work anymore
                // e.Handled = false;
            }

            return false;
        }

        private XPoint GetCanvasPosition()//MouseButtonEventArgs e)
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

        private void SelectSingle(ISelectableItem selectableItem, IDrawingViewModel canvasModel)
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
            if (selectableItem is ISegmentedPolylineSelectableCanvasItem netWire)
            {
                var pos = GetCanvasPosition();

                netWire.SelectSegmentAtPosition(pos);
            }
        }

        void HandleSelectionMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            if (e.Source is PositionThumb)
                return;

            var selectableItem = DataContext as ISelectableItem;
            if (selectableItem is LayerDesignerItem)
                return;

            if (selectableItem != null)
            {
                if (isSelecting)
                {
                    isSelecting = false;
                    return;
                }
                if (selectableItem.IsEditing)
                    return;

                var canvasModel = this.FindParentDataContext<IDrawingViewModel>();

                if (canvasModel == null)
                    return;

                //if we are during placing an object ignore all these
                if (canvasModel.PlacementTool != null)
                    return;

                //ShowAdorner(selectableItem, canvasModel);
            }

        }

        private void ShowAdorner(ISelectableItem selectableItem, IDrawingViewModel canvasModel)
        {
            if (selectableItem.IsSelected)
            {
                if (canvasModel.SelectedItems.Count == 1)
                {
                    var adorner = CanvasItemToAdornerMapper.ShowAdorner(selectableItem, this);
                }
            }
        }
    }
}
