using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;

namespace IDE.Core.Presentation.Placement;

public class VolatileGroup3DPlacementTool : PlacementTool, IVolatileGroup3DPlacementTool
{
    VolatileGroup3DCanvasItem GetItem() => canvasItem as VolatileGroup3DCanvasItem;

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

    private void AddItemsFromGroup(List<ISelectableItem> items, VolatileGroup3DCanvasItem volatileGroup)
    {
        items.ForEach(c =>
        {
            var tg = new XTransform3DGroup();
            var rotateTransform = new XRotateTransform3D
            {
                Rotation = new XAxisAngleRotation3D { Angle = volatileGroup.RotationZ, Axis = new XVector3D(0, 0, 1) }
            };
            tg.Children.Add(rotateTransform);
            tg.Children.Add(new XTranslateTransform3D(volatileGroup.X, volatileGroup.Y, volatileGroup.Z));

            ((BaseMeshItem)c).TransformBy(tg.Value);
            CanvasModel.AddItem(c);
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
            foreach (var groupItem in items)
            {
                var tg = new XTransform3DGroup();
                var rotateTransform = new XRotateTransform3D
                {
                    Rotation = new XAxisAngleRotation3D { Angle = volatileGroup.RotationZ, Axis = new XVector3D(0, 0, 1) }
                };
                tg.Children.Add(rotateTransform);
                tg.Children.Add(new XTranslateTransform3D(volatileGroup.X, volatileGroup.Y, volatileGroup.Z));
                var matrix = tg.Value;
                matrix.Invert();

                ((BaseMeshItem)groupItem).TransformBy(matrix);
            }

            AddItemsFromGroup(items, volatileGroup);
            return volatileGroup;
        };

        CanvasModel.RegisterUndoActionExecuted(undo, redo, volatileGroup);
    }
}
