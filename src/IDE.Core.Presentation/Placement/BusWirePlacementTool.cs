using System;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Storage;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Placement
{
    public class BusWirePlacementTool : PlacementTool
    {
        BusWireCanvasItem GetItem() => canvasItem as BusWireCanvasItem;

        static NetPlacementMode placementMode = NetPlacementMode.Single;

        BusWireCanvasItem commitedPolyline = new BusWireCanvasItem();

        public BusWireCanvasItem CommitedPolyline => commitedPolyline;

        IBusManager BusManager
        {
            get
            {
                return CanvasModel?.GetBusManager();
            }
        }

        public override void SetupCanvasItem()
        {
            base.SetupCanvasItem();

            //var item = GetItem();
            EnsurePoints();
        }

        public override void PlacementMouseMove(XPoint mousePosition)
        {
            var mp = CanvasModel.SnapToGrid(mousePosition);
            var item = GetItem();

            switch (PlacementStatus)
            {
                case PlacementStatus.Ready:
                    {
                        for (int i = 0; i < item.Points.Count; i++)
                        {
                            item.Points[i] = mp;
                        }
                        break;
                    }

                case PlacementStatus.Started:
                    {
                        SetWirebyMouse(mp, item);

                        break;
                    }
            }

            item.OnPropertyChanged(nameof(item.Points));
        }

        public override void PlacementMouseUp(XPoint mousePosition)
        {
            var mp = CanvasModel.SnapToGrid(mousePosition);

            var item = GetItem();

            switch (PlacementStatus)
            {
                //1st click
                case PlacementStatus.Ready:
                    {
                        if (commitedPolyline.Points.Count > 0)
                        {
                            commitedPolyline = new BusWireCanvasItem();
                        }
                        item.Bus = null;
                        item.StartPoint = mp;
                        PlacementStatus = PlacementStatus.Started;

                        HandleLinePoint(mp);

                        //unhighlight
                        CanvasModel.ClearSelectedItems();
                        break;
                    }
                //2nd click
                case PlacementStatus.Started:
                    SetWirebyMouse(mp, item);

                    HandleLinePoint(mp);
                    if (item.Bus == null)
                    {
                        //item.Bus = new SchematicBus
                        //{
                        //    Name = $"Bus{LibraryItem.GetNextId()}",
                        //    CanvasModel = CanvasModel
                        //};
                        var bus = (SchematicBus)BusManager.AddNew();
                        item.Bus = bus;
                    }

                    commitedPolyline.Points.AddRange(item.Points);
                    SimplifyPoints(commitedPolyline.Points);
                    commitedPolyline.IsPlaced = true;
                    commitedPolyline.Bus = item.Bus;
                    if (!CanvasModel.Items.Contains(commitedPolyline))
                        CanvasModel.AddItem(commitedPolyline);

                    commitedPolyline.HighlightOwnedBus(true);

                    item.Points.Clear();

                    SetupCanvasItem();

                    item.StartPoint = mp;
                    SetWirebyMouse(mp, item);

                    item.OnPropertyChanged(nameof(item.Points));
                    commitedPolyline.OnPropertyChanged(nameof(commitedPolyline.Points));

                    PlacementStatus = PlacementStatus.Started;


                    break;
            }
        }

        private void SetWirebyMouse(XPoint mp, BusWireCanvasItem item)
        {
            switch (placementMode)
            {
                case NetPlacementMode.Single:
                    item.EndPoint = mp;
                    break;

                case NetPlacementMode.HorizontalVertical:
                    {
                        var pCount = item.Points.Count;
                        var p2 = new XPoint(mp.X, item.Points[pCount - 3].Y);
                        var p3 = mp;
                        item.Points[pCount - 2] = p2;
                        item.EndPoint = p3;
                        break;
                    }

                case NetPlacementMode.VerticalHorizontal:
                    {
                        var pCount = item.Points.Count;
                        var p2 = new XPoint(item.Points[pCount - 3].X, mp.Y);
                        var p3 = mp;
                        item.Points[pCount - 2] = p2;
                        item.EndPoint = p3;
                        break;

                    }
            }
        }

        void SimplifyPoints(IList<XPoint> points)
        {
            var simplifiedPoints = new List<XPoint>();
            var simplified = Geometry2DHelper.SimplifyPolyline(points, simplifiedPoints);
            if (simplified)
            {
                points.Clear();
                points.AddRange(simplifiedPoints);
            }
        }

        void HandleLinePoint(XPoint linePointMM)
        {
            var item = GetItem();

            var busItems = CanvasModel.Items.OfType<BusWireCanvasItem>().Where(b => b.IsPlaced).ToList();

            foreach (var busItem in busItems)
            {
                var intersects = GeometryHelper.ItemIntersectsPoint(busItem, linePointMM, item.Width);
                if (!intersects)
                    continue;

                //assign this bus to be that intersected bus
                SchematicBus sourceBus = item.Bus;//old
                SchematicBus destBus = null; //new

                if (busItem.Bus != null)
                {
                    destBus = busItem.Bus;
                }
                if (sourceBus != null && destBus != null)
                {
                    //the new net will be from the source if source net is named; otherwise we take it from the destination
                    if (sourceBus.IsNamed() && !destBus.IsNamed())
                    {
                        ChangeBus(destBus, sourceBus);
                    }
                    else
                    {
                        ChangeBus(sourceBus, destBus);
                    }
                }
                else
                {
                    item.Bus = destBus;
                }

            }
        }

        private void ChangeBus(SchematicBus sourceBus, SchematicBus destBus)
        {
            if (sourceBus == null || destBus == null)
                return;

            var busItems = sourceBus.BusItems.ToList();

            //just change the reference
            busItems.ForEach(n => n.Bus = destBus);
        }

        private void EnsurePoints()
        {

            switch (placementMode)
            {
                case NetPlacementMode.Single:
                    EnsurePointsCount(2);
                    break;

                case NetPlacementMode.HorizontalVertical:
                case NetPlacementMode.VerticalHorizontal:
                    EnsurePointsCount(3);
                    break;
            }
        }

        void EnsurePointsCount(int pointsCount)
        {
            var item = GetItem();
            while (item.Points.Count < pointsCount)
                item.Points.Add(new XPoint());
            while (item.Points.Count > pointsCount)
                item.Points.RemoveAt(item.Points.Count - 1);
        }

        public override void CyclePlacement()
        {
            if (PlacementStatus == PlacementStatus.Ready)
                return;

            var pm = (int)placementMode;
            pm++;
            SetPlacementMode((NetPlacementMode)((pm) % (int)NetPlacementMode.Count));
        }

        public void SetPlacementMode(NetPlacementMode netPlacementMode)
        {
            placementMode = netPlacementMode;
            EnsurePoints();
        }
    }
}
