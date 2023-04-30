using IDE.Core.Converters;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace IDE.Core.Controls
{
    public class PositionThumb : Thumb
    {
        public PositionThumb()
        {
            Cursor = Cursors.SizeAll;
            Focusable = true;
        }

        //Position
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
                                                                    "Position", typeof(IPosition),
                                                                    typeof(PositionThumb), null);

        public IPosition Position
        {
            get
            {
                return (IPosition)GetValue(PositionProperty);
            }
            set
            {
                SetValue(PositionProperty, value);
            }
        }

        ICanvasDesignerFileViewModel canvasModel => this.FindParentDataContext<ICanvasDesignerFileViewModel>();

        XPoint? dragStart;
        private XPoint originalPosition;
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            if (canvasModel == null)
                return;

            var designerItem = DataContext as BaseCanvasItem;
            if (designerItem == null || !designerItem.IsPlaced)// || e.Source is PositionThumb)
                return;
            if (designerItem.IsEditing)
                return;

            if (e.Source is PositionThumb)
            {
                Focus();
                var drawingCanvas = this.FindParent<DrawingCanvas>();
                dragStart = canvasModel.SnapToGridFromDpi(e.GetPosition(drawingCanvas).ToXPoint());
                originalPosition = new XPoint(Position.X, Position.Y);
                CaptureMouse();
            }

        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            var designerItem = DataContext as BaseCanvasItem;
            if (designerItem == null || !designerItem.IsPlaced)
                return;
            if (designerItem.IsEditing)
                return;

            dragStart = null;
            ReleaseMouseCapture();
            var drawingCanvas = this.FindParent<DrawingCanvas>();
            drawingCanvas?.Focus();

            if (canvasModel == null)
                return;

            var currentPosition = new XPoint(Position.X, Position.Y);
            if (originalPosition != currentPosition)
            {
                canvasModel.RegisterUndoActionExecuted(
                    undo: o =>
                    {
                        Position.X = originalPosition.X;
                        Position.Y = originalPosition.Y;
                        return null;
                    },
                    redo: o =>
                    {
                        Position.X = currentPosition.X;
                        Position.Y = currentPosition.Y;
                        return null;
                    }, null
                    );
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (canvasModel == null)
                return;

            var designerItem = DataContext as BaseCanvasItem;
            if (designerItem == null || !designerItem.IsPlaced)
                return;

            if (designerItem.IsEditing)
                return;

            if (dragStart.HasValue && e.LeftButton == MouseButtonState.Pressed && Position != null)
            {
                var drawingCanvas = this.FindParent<DrawingCanvas>();
                var currentPos = canvasModel.SnapToGridFromDpi(e.GetPosition(drawingCanvas).ToXPoint());
                var deltaX = currentPos.X - dragStart.Value.X;
                var deltaY = currentPos.Y - dragStart.Value.Y;

                if (deltaX == 0 && deltaY == 0)
                    return;

                dragStart = currentPos;

                var dir = GetMoveDirection(designerItem, deltaX, deltaY);
                Position.X += dir.X;
                Position.Y += dir.Y;
            }
        }

        Vector GetMoveDirection(BaseCanvasItem item, double deltaX, double deltaY)
        {
            var dir = new Vector(1, 1);
            var aux = 0.0d;
            if (item is PinCanvasItem pin)
            {

                switch (pin.Orientation)
                {
                    case pinOrientation.Left:
                        dir = new Vector(1, 1);
                        break;

                    case pinOrientation.Right:
                        dir = new Vector(-1, -1);
                        break;

                    case pinOrientation.Up:
                        dir = new Vector(1, -1);
                        //switch
                        aux = deltaX;
                        deltaX = deltaY;
                        deltaY = aux;
                        break;

                    case pinOrientation.Down:
                        dir = new Vector(-1, 1);
                        //switch
                        aux = deltaX;
                        deltaX = deltaY;
                        deltaY = aux;
                        break;
                }
            }
            else if (item is SchematicSymbolCanvasItem)
            {
                //keep the same direction
            }
            else if (item.IsMirrored())
            {
                dir = new Vector(-1, 1);
            }

            return new Vector(dir.X * deltaX, dir.Y * deltaY);
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            var designerItem = DataContext as BaseCanvasItem;
            if (designerItem == null || designerItem.IsEditing)
                return;

            if (e.Key == Key.Space && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var rot = Position.Rotation;
                var originalRot = rot;
                rot += 90;
                rot = ((int)rot % 360);
                Position.Rotation = rot;
                var currentRot = rot;
                e.Handled = true;

                if (currentRot != originalRot)
                {
                    canvasModel.RegisterUndoActionExecuted(
                        undo: o =>
                        {
                            Position.Rotation = originalRot;
                            return null;
                        },
                        redo: o =>
                        {
                            Position.Rotation = currentRot;
                            return null;
                        }, null
                        );
                }
            }
        }
    }


}
