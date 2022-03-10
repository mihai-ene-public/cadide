using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.Linq;

namespace IDE.Core.Presentation.Placement;

public class ExtrudedPolyMeshItemPlacementTool : PlacementTool, IExtrudedPolyMeshItemPlacementTool
{
    ExtrudedPolyMeshItem GetItem() => canvasItem as ExtrudedPolyMeshItem;

    public override void PlacementMouseMove(XPoint mousePosition)
    {
        var mp = CanvasModel.SnapToGrid(mousePosition);

        var item = GetItem();


        switch (PlacementStatus)
        {
            case PlacementStatus.Ready:
                {
                    if (item.Points.Count > 1)
                    {
                        //we update our 2 points at once
                        item.Points[item.Points.Count - 1] = new XPoint(mp.X, mp.Y);
                        item.Points[item.Points.Count - 2] = new XPoint(mp.X, mp.Y);
                        item.OnPropertyChanged(nameof(item.Points));
                    }

                }
                break;

            case PlacementStatus.Started:
                {
                    if (item.Points.Count > 0)
                    {
                        item.Points[item.Points.Count - 1] = new XPoint(mp.X, mp.Y);
                        item.OnPropertyChanged(nameof(item.Points));
                    }
                }
                break;
        }
    }

    public override void PlacementMouseUp(XPoint mousePosition)
    {
        var mp = CanvasModel.SnapToGrid(mousePosition);

        var item = GetItem();

        switch (PlacementStatus)
        {
            //1st click
            case PlacementStatus.Ready:
                //add 2 points
                item.Points.Add(mp);
                item.Points.Add(mp);
                PlacementStatus = PlacementStatus.Started;
                break;

            //n-th click
            case PlacementStatus.Started:

                if (item.Points.Count < 1)
                    break;

                var firstPoint = item.Points[0];

                double eps = CanvasModel.GridSize * 0.5;//this could be the grid size

                var distance = (mp - firstPoint).Length;
                if (item.Points.Count >= 3 && distance <= eps)
                {
                    //remove last point
                    item.Points.RemoveAt(item.Points.Count - 1);
                    item.IsPlaced = true;
                    CanvasModel.OnDrawingChanged(DrawingChangedReason.ItemPlacementFinished);

                    //create another object
                    var newItem = (ExtrudedPolyMeshItem)canvasItem.Clone();
                    newItem.IsPlaced = false;
                    newItem.Points.Clear();

                    PlacementStatus = PlacementStatus.Ready;
                    canvasItem = newItem;

                    CanvasModel.AddItem(canvasItem);
                }
                else
                {
                    item.Points.Add(mp);
                }

                break;
        }
    }
}
