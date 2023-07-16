using System.Linq;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Placement
{
    public class PolygonPlacementTool : PlacementTool<IPolygonCanvasItem>, IPolygonPlacementTool
    {
        public override void PlacementMouseMove(XPoint mousePosition)
        {
            var mp = CanvasModel.SnapToGrid(mousePosition);

            var item = GetItem();

            switch (PlacementStatus)
            {
                case PlacementStatus.Ready:
                    {
                        if (item.PolygonPoints.Count > 1)
                        {
                            //we update our 2 points at once
                            item.PolygonPoints[item.PolygonPoints.Count - 1] = new XPoint(mp.X, mp.Y);
                            item.PolygonPoints[item.PolygonPoints.Count - 2] = new XPoint(mp.X, mp.Y);
                            item.PolygonPoints[item.PolygonPoints.Count - 3] = new XPoint(mp.X, mp.Y);
                            item.OnPropertyChanged(nameof(item.PolygonPoints));
                        }

                    }
                    break;
                case PlacementStatus.Started:
                    {
                        if (item.PolygonPoints.Count > 0)
                        {
                            item.PolygonPoints[item.PolygonPoints.Count - 1] = new XPoint(mp.X, mp.Y);
                            item.OnPropertyChanged(nameof(item.PolygonPoints));
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
                    //item.PolygonPoints.Add(mp);
                    //item.PolygonPoints.Add(mp);
                    //item.PolygonPoints.Add(mp);
                    PlacementStatus = PlacementStatus.Started;
                    break;
                //n-th click
                case PlacementStatus.Started:

                    if (item.PolygonPoints.Count < 1)
                        break;

                    var firstPoint = item.PolygonPoints.FirstOrDefault();

                    double eps = CanvasModel.GridSize * 0.5;//this could be the grid size


                    var distance = (mp - firstPoint).Length;
                    if (item.PolygonPoints.Count >= 3 && distance <= eps)
                    {
                        //remove last point
                        item.PolygonPoints.RemoveAt(item.PolygonPoints.Count - 1);
                        item.IsPlaced = true;
                        CommitPlacement();

                        var newItem = (IPolygonCanvasItem)canvasItem.Clone();
                        newItem.PolygonPoints.Clear();

                        PlacementStatus = PlacementStatus.Ready;
                        canvasItem = newItem;

                        SetupCanvasItem();
                        CanvasModel.AddItem(canvasItem);
                    }
                    else
                    {
                        item.PolygonPoints.Add(mp);
                    }

                    break;
            }
        }

        public override void SetupCanvasItem()
        {
            base.SetupCanvasItem();

            var item = GetItem();
            item.PolygonPoints.Add(new XPoint());
            item.PolygonPoints.Add(new XPoint());
            item.PolygonPoints.Add(new XPoint());
        }
    }
}
