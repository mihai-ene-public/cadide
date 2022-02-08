using HelixToolkit.Wpf.SharpDX;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Documents.Views;
using IDE.Presentation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IDE.Controls
{
    public partial class Viewport3DXControl : UserControl
    {
        public Viewport3DXControl()
        {
            InitializeComponent();
        }

        XPoint? dragStart;

        Viewport3DX helixViewport3D;

        RectangleSelectionBehavior rectangleSelection;

        IDrawingViewModel GetCanvasModel()
        {
            var canvas = DataContext as FootprintDesignerFileViewModel;
            if (canvas != null)
                return canvas.BoardPreview3DViewModel.Model3DCanvasModel;

            return null;
        }

        XPoint CursorPosition
        {
            get
            {
                if (helixViewport3D == null)
                    return new XPoint();

                //var p = helixViewport3D.CurrentPosition;
                var mousePos = Mouse.GetPosition(helixViewport3D).ToVector2();
                helixViewport3D.UnProjectOnPlane(mousePos, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(0, 0, 1), out var p);

                var cursorPosition = new XPoint(p.X, p.Y);
                return cursorPosition;
            }
        }

        private void HelixViewport3D_Loaded(object sender, RoutedEventArgs e)
        {
            helixViewport3D = sender as Viewport3DX;
            rectangleSelection = new RectangleSelectionBehavior(helixViewport3D, (r) => helixViewport3D.FindHitsInRectangle(r));
        }

        private void HelixViewport3D_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var canvasModel = GetCanvasModel();
            if (canvasModel == null)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //if we are source of event, we are rubberband selecting or placing the component
                if (e.Source == sender)
                {


                    if (canvasModel.PlacementTool != null)
                    {
                        //we will show an adorner maybe or some visual feedback (or init values for that)
                        //we won't add it here because we could add it multiple times
                    }
                    else
                    {

                        //hit test
                        #region Hit test

                        //if we are during placing an object ignore hit test
                        if (canvasModel.PlacementTool != null)
                            return;

                        var selectionHandled = false;
                        var hitItem = helixViewport3D.FindHits(e.GetPosition(helixViewport3D)).FirstOrDefault();
                        if (hitItem != null && hitItem.ModelHit != null)
                        {
                            //var itemsControl = hitItem.Visual.FindParent<ItemsModel3D>();
                            //if (itemsControl != null)
                            {
                                var selectableItem = (hitItem.ModelHit as Element3D).DataContext as ISelectableItem;//itemsControl.GetItemFromVisual(hitItem.Visual) as ISelectableItem;
                                if (selectableItem != null)
                                {
                                    var kbModifiers = Keyboard.Modifiers;
                                    if (kbModifiers.HasFlag(ModifierKeys.Control) || kbModifiers.HasFlag(ModifierKeys.Shift))
                                    {
                                        selectableItem.IsSelected = !selectableItem.IsSelected;
                                    }
                                    else if (selectableItem.IsSelected)
                                    {
                                        //if it is already selected, we show the adorner on the second mouse down
                                        //we must have one item selected to show the adorner
                                    }
                                    else if (!selectableItem.IsSelected)
                                    {
                                        //deselect all items and select this one
                                        canvasModel.ClearSelectedItems();
                                        selectableItem.IsSelected = true;
                                    }

                                    selectionHandled = true;
                                    canvasModel.UpdateSelection();

                                    dragStart = canvasModel.SnapToGrid(CursorPosition);
                                    //CaptureMouse();

                                }
                            }
                        }
                        else
                        {
                            rectangleSelection?.Execute();
                        }

                        #endregion Hit test

                        if (!selectionHandled)
                        {
                            // if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                            {
                                canvasModel.ClearSelectedItems();
                            }
                        }

                    }

                    e.Handled = true;
                    return;
                }
                else
                {



                }
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
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
                }
            }
        }

        private void HelixViewport3D_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var canvasModel = GetCanvasModel();
            if (helixViewport3D == null || canvasModel == null)
                return;

            var cursorPosition = CursorPosition;

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                if (canvasModel != null)
                {
                    if (canvasModel.IsPlacingItem())
                    {
                        canvasModel.PlacementTool.PlacementMouseMove(cursorPosition);
                    }
                }
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (dragStart.HasValue)
                {
                    var currentPos = canvasModel.SnapToGrid(cursorPosition);
                    var deltaX = currentPos.X - dragStart.Value.X;
                    var deltaY = currentPos.Y - dragStart.Value.Y;

                    if (deltaX != 0 || deltaY != 0)
                    {
                        dragStart = currentPos;

                        foreach (var item in canvasModel.SelectedItems.OfType<BaseMeshItem>())
                        {
                            item.Translate(deltaX, deltaY, 0);
                        }
                    }
                }
            }

            //mouse pos in mm
            var snappedMousePos = canvasModel.SnapToGrid(cursorPosition);

            canvasModel.X = snappedMousePos.X;
            canvasModel.Y = snappedMousePos.Y;
        }

        private void HelixViewport3D_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            dragStart = null;

            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    {
                        var canvasModel = GetCanvasModel();
                        if (canvasModel != null)
                        {
                            if (canvasModel.IsPlacingItem())
                            {
                                canvasModel.PlacementTool.PlacementMouseUp(CursorPosition);
                            }
                            canvasModel.UpdateSelection();
                        }

                        break;
                    }
            }
        }

    }
}
