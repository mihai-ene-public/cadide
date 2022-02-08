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

        IDrawingViewModel canvasModel => this.FindParentDataContext<IDrawingViewModel>();

        XPoint? dragStart;
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            //base.OnPreviewMouseDown(e);
            if (e.ChangedButton != MouseButton.Left)
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
                CaptureMouse();
                //e.Handled = true;
            }

        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            //base.OnPreviewMouseUp(e);

            var designerItem = DataContext as BaseCanvasItem;
            if (designerItem == null || !designerItem.IsPlaced)
                return;
            if (designerItem.IsEditing)
                return;

            //if (e.Source is PositionThumb)
            {

                dragStart = null;
                ReleaseMouseCapture();
                var drawingCanvas = this.FindParent<DrawingCanvas>();
                drawingCanvas?.Focus();
                //e.Handled = true;
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
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

            // base.OnPreviewKeyUp(e);
            if (e.Key == Key.Space && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var rot = Position.Rotation;
                rot += 90;
                rot = ((int)rot % 360);
                Position.Rotation = rot;
                e.Handled = true;
            }
        }
    }


}
