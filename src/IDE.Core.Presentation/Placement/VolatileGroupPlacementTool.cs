using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Placement
{
    public class VolatileGroupPlacementTool : PlacementTool, IVolatileGroupPlacementTool
    {
        VolatileGroupCanvasItem GetItem() => canvasItem as VolatileGroupCanvasItem;

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
                case PlacementStatus.Ready:
                    //place all items from this group to the canvas translated by mouse position
                    item.Items.ForEach(c =>
                     {
                         var tg = new XTransformGroup();

                         var scaleTransform = new XScaleTransform(item.ScaleX, item.ScaleY);
                         tg.Children.Add(scaleTransform);

                         var rotateTransform = new XRotateTransform(item.Rot);
                         tg.Children.Add(rotateTransform);

                         tg.Children.Add(new XTranslateTransform(item.X, item.Y));

                         ((BaseCanvasItem)c).TransformBy(tg.Value);
                         c.IsPlaced = true;

                         if (c is SingleLayerBoardCanvasItem sl)
                         {
                             sl.Layer.Items.Add(c);
                         }
                         if(c is ISignalPrimitiveCanvasItem signalPrimitive)
                         {
                             //we force adding the item to the signal
                             signalPrimitive.Signal = signalPrimitive.Signal;
                         }
                         if (c.ParentObject == item)
                         {
                             c.ParentObject = null;
                         }

                         CanvasModel.AddItem(c);
                     });
                    //stop placing other items
                    CanvasModel.CancelPlacement();

                    CanvasModel.OnDrawingChanged(DrawingChangedReason.ItemPlacementFinished);
                    break;
            }
        }
    }
}
