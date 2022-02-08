using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;

namespace IDE.Core.Presentation.Placement;

public class VolatileGroup3DPlacementTool : PlacementTool
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

                item.Items.ForEach(c =>
                {
                    var tg = new XTransform3DGroup();
                    var rotateTransform = new XRotateTransform3D
                    {
                        Rotation = new XAxisAngleRotation3D { Angle = item.RotationZ, Axis = new XVector3D(0, 0, 1) }
                    };
                    tg.Children.Add(rotateTransform);
                    tg.Children.Add(new XTranslateTransform3D(item.X, item.Y, item.Z));

                    ((BaseMeshItem)c).TransformBy(tg.Value);
                    CanvasModel.AddItem(c);
                });

                //CanvasModel.RemoveItem(canvasItem);
                //stop placing other items
                CanvasModel.CancelPlacement();

                CanvasModel.OnDrawingChanged(DrawingChangedReason.ItemPlacementFinished);
                break;


        }
    }
}
