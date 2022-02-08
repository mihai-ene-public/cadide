using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using IDE.Core.Designers;
using IDE.Core.Utilities;
using IDE.Core.Adorners;
using IDE.Core.Converters;
using IDE.Core.Units;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Controls
{
    public class DrawingCanvas : ZoomableCanvas
    {

        public DrawingCanvas()
        {
            ClipToBounds = false;
            AllowDrop = true;
            Focusable = true;
            DataContextChanged += DrawingCanvas_DataContextChanged;
        }

        void DrawingCanvas_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var cm = GetCanvasModel();
            if (cm != null)
            {
                var actualWidth = ActualWidth;
                var actualHeight = ActualHeight;

                var scrollViewer = this.FindParent<ScrollViewer>();
                if (scrollViewer != null)
                {
                    actualHeight = scrollViewer.ActualHeight;
                    actualWidth = scrollViewer.ActualWidth;
                }

                cm.SetViewportSize(() => new XSize(actualWidth, actualHeight));
                cm.SetRequestMousePosition(() => Mouse.GetPosition(this).ToXPoint());

                cm.ZoomToFit();
            }

        }

        Point? rubberbandSelectionStartPoint = null;

        Point? measureDistanceStartPoint = null;

        Point? dragStart;

        IDrawingViewModel _canvasModel;
        IDrawingViewModel GetCanvasModel()
        {
            //if (DataContext is IDrawingViewModel)
            //    _canvasModel = DataContext as IDrawingViewModel;

            if (_canvasModel == null)
                _canvasModel = this.FindDataContext<IDrawingViewModel>();

            return _canvasModel;
        }

        private Point GetMousePosition()
        {
            var thisOrigin = Origin;
            var mp = Mouse.GetPosition(this);
            mp.Offset(-thisOrigin.X, -thisOrigin.Y);

            return mp;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    {
                        if (e.LeftButton == MouseButtonState.Pressed)
                        {
                            //if we are source of event, we are rubberband selecting or placing the component
                            if (e.Source == this)
                            {
                                var canvasModel = GetCanvasModel();
                                if (canvasModel == null)
                                    return;

                                if (canvasModel.PlacementTool != null)
                                {
                                    //we will show an adorner maybe or some visual feedback (or init values for that)
                                    //we won't add it here because we could add it multiple times
                                }
                                else
                                {
                                    // in case that this click is the start for a 
                                    // drag operation we cache the start point
                                    rubberbandSelectionStartPoint = e.GetPosition(this);

                                    if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                                    {
                                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                                        {
                                            canvasModel.ClearHighlightedItems();
                                        }
                                        else
                                        {
                                            canvasModel.ClearSelectedItems();
                                        }
                                    }

                                }
                                //if this e.Handled is commented solves the autofocus problem
                                //but let's see if causes the old hiccup
                                //e.Handled = true;
                            }
                        }
                        break;
                    }

                case MouseButton.Right:
                    {
                        if (e.RightButton == MouseButtonState.Pressed)
                        {

                            var canvasModel = GetCanvasModel();
                            if (canvasModel == null)
                                return;


                            if (canvasModel.IsPlacingItem() == false)
                            {
                                measureDistanceStartPoint = e.GetPosition(this);
                            }

                        }
                        break;
                    }
            }



        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            //don't call Focus() here; it will cause a hicup (jump) of offset and it is annoying
            //if (!IsFocused)
            //    Focus();

            switch (e.ChangedButton)
            {
                case MouseButton.Right:
                    {
                        if (e.RightButton == MouseButtonState.Pressed)
                        {

                            var canvasModel = GetCanvasModel();
                            if (canvasModel == null)
                                return;


                            if (canvasModel.IsPlacingItem())
                            {
                                if (canvasModel.PlacementTool.PlacementStatus == PlacementStatus.Started)
                                {
                                    //we will give the chance to create another item in reset state
                                    canvasModel.PlacementTool.PlacementStatus = PlacementStatus.Ready;
                                }
                                else if (canvasModel.PlacementTool.PlacementStatus == PlacementStatus.Ready)
                                {
                                    //remove the object if we had one placing
                                    canvasModel.CancelPlacement();
                                }

                                e.Handled = true;//?
                            }
                            else
                            {
                                measureDistanceStartPoint = e.GetPosition(this);
                            }
                        }
                        break;
                    }

                case MouseButton.Middle:
                    {
                        EnsureFocus();

                        if (e.MiddleButton == MouseButtonState.Pressed)
                        {
                            dragStart = e.GetPosition(this);
                            CaptureMouse();
                            e.Handled = true;
                        }
                        else
                        {
                            dragStart = null;
                            if (IsMouseCaptured)
                                ReleaseMouseCapture();
                            e.Handled = true;
                        }
                        break;
                    }
            }
        }

        void EnsureFocus()
        {
            //when losing focus, command bindindgs don't execute, so we need to call Focus()
            //this have the disadvantage for causing an offset

            var parent = this.FindParent<ScrollViewer>();
            var isFocused = IsFocused;
            if (parent != null)
                isFocused = isFocused || parent.IsFocused;

            if (!isFocused)
                if (parent != null)
                    parent.Focus();//focusable?
                else
                    Focus();
        }



        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            var canvasModel = GetCanvasModel();
            if (canvasModel == null)
                return;

            //a copy of grid unit
            var canvasGrid = canvasModel.CanvasGrid as CanvasGrid;
            var gridUnit = (AbstractUnit)Activator.CreateInstance(canvasGrid.GridSizeModel.SelectedItem.GetType());

            var mousePosition = GetMousePosition();

            if (e.LeftButton != MouseButtonState.Pressed)
            {

                //canvasModel.Canvas = this;
                if (canvasModel != null)
                {
                    if (canvasModel.PlacementTool != null && canvasModel.PlacementTool.CanvasItem != null)
                    {
                        var mousePos = mousePosition.ToXPoint();//e.GetPosition(this).ToXPoint();
                        var posMM = MilimetersToDpiHelper.ConvertToMM(mousePos);
                        canvasModel.PlacementTool.PlacementMouseMove(posMM);
                    }
                }
            }

            if (e.MiddleButton == MouseButtonState.Pressed
                && !(e.OriginalSource is Thumb)) // Don't block the scrollbars.
            {
                if (dragStart.HasValue)
                {
                    var position = e.GetPosition(this);
                    var d = position - dragStart.Value;
                    var pp = MilimetersToDpiHelper.ConvertToMM(new XPoint(d.X, d.Y));
                    canvasModel.Offset -= (XVector)pp;
                    InvalidateArrange();
                    e.Handled = true;
                }
            }

            var p = mousePosition.ToXPoint();
            p = MilimetersToDpiHelper.ConvertToMM(p);

            //mouse pos in mm
            var snappedMousePos = canvasModel.SnapToGrid(p);


            canvasModel.X = gridUnit.ConvertFrom(new MillimeterUnit(snappedMousePos.X));
            canvasModel.Y = gridUnit.ConvertFrom(new MillimeterUnit(snappedMousePos.Y));
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    {
                        var canvasModel = GetCanvasModel();
                        if (canvasModel != null)
                        {
                            if (canvasModel.PlacementTool != null && canvasModel.PlacementTool.CanvasItem != null)
                            {
                                var mousePos = GetMousePosition().ToXPoint();
                                var posMM = MilimetersToDpiHelper.ConvertToMM(mousePos);
                                canvasModel.PlacementTool.PlacementMouseUp(posMM);

                                e.Handled = true;
                            }
                            canvasModel.UpdateSelection();
                        }

                        break;
                    }
                case MouseButton.Middle:
                    {
                        dragStart = null;
                        ReleaseMouseCapture();
                        e.Handled = true;
                        break;
                    }
            }


            //if (!IsFocused)
            //    Focus();
        }


        double CoerceRange(double value, double min, double max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;
            return value;
        }

        //!
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);


            if (e.MiddleButton == MouseButtonState.Released)
            {
                var parent = this.FindParent<ScrollViewer>();
                var cm = GetCanvasModel();
                if (parent != null)
                {
                    var scaleBefore = cm.Scale;
                    var x = Math.Pow(2, e.Delta / 3.0 / Mouse.MouseWheelDeltaForOneLine);

                    var newScale = CoerceRange(cm.Scale * x, DrawingViewModel.minScale, DrawingViewModel.maxScale);

                    if (scaleBefore != newScale)
                    {
                        var delta = scaleBefore - newScale;
                        // Adjust the offset to make the point under the mouse stay still.
                        var position = e.GetPosition(this).ToXPoint();//in dpi
                        var posMM = (XVector)MilimetersToDpiHelper.ConvertToMM(position);

                        var newOffset = new XPoint(cm.Offset.X - posMM.X * delta,
                                                  cm.Offset.Y - posMM.Y * delta);

                        cm.Offset = newOffset;
                        cm.Scale = newScale;
                        InvalidateArrange();
                        e.Handled = true;
                    }

                }
            }


        }


        public void RaisePreviewMouseWheel(MouseWheelEventArgs e)
        {
            OnPreviewMouseWheel(e);
        }

        bool IsCanvasModel()
        {
            return DataContext is IDrawingViewModel;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!IsCanvasModel())
                return;

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton == MouseButtonState.Released)
            {
                rubberbandSelectionStartPoint = null;
            }
            if (e.RightButton == MouseButtonState.Released)
            {
                measureDistanceStartPoint = null;
            }

            // ... but if mouse button is pressed and start
            // point value is set we do have one
            if (rubberbandSelectionStartPoint.HasValue)
            {
                // create rubberband adorner
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    var adorner = new RubberbandAdorner(this, rubberbandSelectionStartPoint);
                    adornerLayer.Add(adorner);
                }
            }
            else if (measureDistanceStartPoint.HasValue)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    var adorner = new MeasureDistance2DAdorner(this, measureDistanceStartPoint);
                    adornerLayer.Add(adorner);
                }
            }
        }

        public void RaisePreviewMouseMove(MouseEventArgs e)
        {
            OnPreviewMouseMove(e);
        }

    }
}
