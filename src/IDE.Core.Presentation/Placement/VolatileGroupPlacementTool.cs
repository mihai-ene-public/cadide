using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Placement;

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
                AddItemsFromGroup(item.Items, item);

                CommitPlacement();

                //stop placing other items
                CanvasModel.CancelPlacement();

                break;
        }
    }

    private void AddItemsFromGroup(List<ISelectableItem> items, VolatileGroupCanvasItem volatileGroup)
    {
        items.ForEach(item =>
        {
            var tg = new XTransformGroup();

            var scaleTransform = new XScaleTransform(volatileGroup.ScaleX, volatileGroup.ScaleY);
            tg.Children.Add(scaleTransform);

            var rotateTransform = new XRotateTransform(volatileGroup.Rot);
            tg.Children.Add(rotateTransform);

            tg.Children.Add(new XTranslateTransform(volatileGroup.X, volatileGroup.Y));

            ((BaseCanvasItem)item).TransformBy(tg.Value);
            item.IsPlaced = true;

            if (item is SingleLayerBoardCanvasItem sl)
            {
                sl.Layer.Items.Add(item);
            }
            if (item is ISignalPrimitiveCanvasItem signalPrimitive)
            {
                //we force adding the item to the signal
                signalPrimitive.Signal = signalPrimitive.Signal;
            }
            if (item.ParentObject == volatileGroup)
            {
                item.ParentObject = null;
            }

            CanvasModel.AddItem(item);
        });
    }

    protected override void RegisterUndoActionExecuted()
    {
        var volatileGroup = GetItem();

        var items = volatileGroup.Items.ToList();

        Func<object, object> undo = (i) =>
        {
            CanvasModel.RemoveItems(items);
            return volatileGroup;
        };
        Func<object, object> redo = (i) =>
        {
            //items are transform inverted so that they are not transformed by the group
            foreach(var groupItem in items)
            {
                var tg = new XTransformGroup();

                var scaleTransform = new XScaleTransform(volatileGroup.ScaleX, volatileGroup.ScaleY);
                tg.Children.Add(scaleTransform);

                var rotateTransform = new XRotateTransform(volatileGroup.Rot);
                tg.Children.Add(rotateTransform);

                tg.Children.Add(new XTranslateTransform(volatileGroup.X, volatileGroup.Y));
                var matrix = tg.Value;
                matrix.Invert();

                groupItem.TransformBy(matrix);
            }

            AddItemsFromGroup(items, volatileGroup);
            return volatileGroup;
        };

        CanvasModel.RegisterUndoActionExecuted(undo, redo, volatileGroup);
    }
}
