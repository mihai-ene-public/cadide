using System;
using System.Collections.Generic;
using System.Linq;
using Eagle;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Core.ViewModels;

namespace IDE.Core.Presentation.Placement
{
    public class NetWirePlacementTool : PlacementTool, INetWirePlacementTool
    {
        public NetWirePlacementTool()
        {
            GeometryHelper = ServiceProvider.Resolve<IGeometryOutlineHelper>();
        }

        private readonly IGeometryOutlineHelper GeometryHelper;

        NetWireCanvasItem GetItem() => canvasItem as NetWireCanvasItem;

        static NetPlacementMode placementMode = NetPlacementMode.Single;

        NetWireCanvasItem commitedPolyline = new NetWireCanvasItem();

        List<JunctionCanvasItem> _junctionItems = new List<JunctionCanvasItem>();

        private List<NetChanges> _netChanges = new List<NetChanges>();

        public NetWireCanvasItem CommitedPolyline => commitedPolyline;

        INetManager NetManager
        {
            get
            {
                return CanvasModel?.GetNetManager();
            }
        }

        public override void SetupCanvasItem()
        {
            base.SetupCanvasItem();

            _junctionItems.Clear();
            EnsurePoints();
        }

        public override void PlacementMouseMove(XPoint mousePosition)
        {
            var mp = CanvasModel.SnapToGrid(mousePosition);

            var item = GetItem();

            switch (PlacementStatus)
            {
                case PlacementStatus.Ready:
                    for (int i = 0; i < item.Points.Count; i++)
                    {
                        item.Points[i] = mp;
                    }
                    item.Net = null;
                    break;
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
                            commitedPolyline = new NetWireCanvasItem();
                        }
                        item.Net = null;
                        item.StartPoint = mp;
                        PlacementStatus = PlacementStatus.Started;

                        //if we clicked on a net segment (pin/port/wire/junction)
                        HandleLinePoint(mp);
                        //unhighlight
                        CanvasModel.ClearSelectedItems();
                        break;
                    }
                //2nd click
                case PlacementStatus.Started:
                    SetWirebyMouse(mp, item);

                    HandleLinePoint(mp);

                    if (item.Net == null)
                    {
                        var net = NetManager.AddNew();
                        item.Net = (SchematicNet)net;
                    }

                    commitedPolyline.Points.AddRange(item.Points);
                    SimplifyPoints(commitedPolyline.Points);
                    commitedPolyline.IsPlaced = true;
                    commitedPolyline.Net = item.Net;
                    if (!CanvasModel.Items.Contains(commitedPolyline))
                        CanvasModel.AddItem(commitedPolyline);

                    commitedPolyline.HighlightOwnedNet(true);

                    item.Points.Clear();

                    CommitPlacement();

                    SetupCanvasItem();

                    item.StartPoint = mp;
                    SetWirebyMouse(mp, item);

                    item.OnPropertyChanged(nameof(item.Points));
                    commitedPolyline.OnPropertyChanged(nameof(commitedPolyline.Points));

                    PlacementStatus = PlacementStatus.Started;

                    break;
            }
        }

        private void SetWirebyMouse(XPoint mp, NetWireCanvasItem item)
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

        protected override void RegisterUndoActionExecuted()
        {
            var item = commitedPolyline;
            var itemNet = item.Net;
            var currentChanges = _netChanges.ToList();
            var junctions = _junctionItems.ToList();

            Func<object, object> undo = (i) =>
            {
                CanvasModel.RemoveItem(item);
                CanvasModel.RemoveItems(junctions);

                foreach (var netChange in currentChanges)
                {
                    netChange.OldNetItems.ForEach(n => n.Net = netChange.OldNet);
                }
                return item;
            };
            Func<object, object> redo = (i) =>
            {
                item.Net = itemNet;
                CanvasModel.AddItem(item);

                junctions.ForEach(j => j.Net = itemNet);
                CanvasModel.AddItems(junctions);

                foreach (var netChange in currentChanges)
                {
                    netChange.OldNetItems.ForEach(n => n.Net = netChange.NewNet);
                }
                return item;
            };

            CanvasModel.RegisterUndoActionExecuted(undo, redo, item);
        }

        private void SimplifyPoints(IList<XPoint> points)
        {
            var simplifiedPoints = new List<XPoint>();
            var simplified = Geometry2DHelper.SimplifyPolyline(points, simplifiedPoints);
            if (simplified)
            {
                points.Clear();
                points.AddRange(simplifiedPoints);
            }
        }

        public override void CyclePlacement()
        {
            if (PlacementStatus == PlacementStatus.Ready)
                return;

            var pm = (int)placementMode;
            pm++;
            SetPlacementMode((NetPlacementMode)((pm) % (int)NetPlacementMode.Count));

            //EnsurePoints();
        }

        public void SetPlacementMode(NetPlacementMode netPlacementMode)
        {
            placementMode = netPlacementMode;
            EnsurePoints();
        }

        public void HandleLinePoint(XPoint linePointMM)
        {
            var item = GetItem();

            if (TestBusHit(ref linePointMM))
                return;

            //check if we intersect a point of this wire with other net segments
            #region Wire with other net segments

            var circle = new CircleCanvasItem
            {
                Diameter = item.Width,
                X = linePointMM.X,
                Y = linePointMM.Y,
                BorderWidth = 0.00d
            };
            var intersectedWires = new List<NetWireCanvasItem>();
            var addJunction = false;
            var netElements = CanvasModel.Items.OfType<NetSegmentCanvasItem>().Where(ns => ns != item && ns.IsPlaced).ToList();
            foreach (var netSegm in netElements)
            {
                //ignore colision with net labels or pinrefs
                if (netSegm is NetLabelCanvasItem)
                    continue;

                if (netSegm is ICanvasItem)
                {
                    var geom = netSegm as ICanvasItem;
                    if (geom != null)
                    {

                        var intersects = GeometryHelper.Intersects(geom, circle);
                        if (!intersects)
                            continue;

                        //assign this net to be that intersected net
                        SchematicNet sourceNet = item.Net;//oldNet
                        SchematicNet destNet = null; //newNet
                        if (netSegm.Net != null)
                        {
                            destNet = netSegm.Net;
                        }
                        if (sourceNet != null && destNet != null)
                        {
                            //the new net will be from the source if source net is named; otherwise we take it from the destination
                            if (sourceNet.IsNamed() && !destNet.IsNamed())
                            {
                                ChangeNet(destNet, sourceNet);
                            }
                            else
                            {
                                ChangeNet(sourceNet, destNet);
                            }
                        }
                        else
                        {
                            item.Net = destNet;
                        }



                        if (netSegm is NetWireCanvasItem)
                        {
                            //if we are on a net wire but not on a start or end, add a junction
                            var otherNetWire = netSegm as NetWireCanvasItem;
                            if (linePointMM != otherNetWire.StartPoint && linePointMM != otherNetWire.EndPoint)
                            {
                                addJunction = true;
                            }
                            else
                            {
                                intersectedWires.Add(otherNetWire);
                            }
                        }
                    }

                }
            }
            if (intersectedWires.Count > 1)
            {
                addJunction = true;
            }
            if (addJunction)
            {
                var junctionExists = CanvasModel.Items.OfType<JunctionCanvasItem>()
                                              .Any(j => j.X == linePointMM.X && j.Y == linePointMM.Y);

                if (!junctionExists)
                {
                    var junctionItem = (JunctionCanvasItem)Activator.CreateInstance(typeof(JunctionCanvasItem));
                    junctionItem.X = linePointMM.X;
                    junctionItem.Y = linePointMM.Y;
                    junctionItem.Net = item.Net;
                    junctionItem.IsPlaced = true;
                    CanvasModel.AddItem(junctionItem);

                    _junctionItems.Add(junctionItem);
                }
            }

            #endregion Wire with other net segments

            //check if we intersect a point of this wire with other pins
            #region Wire with pins

            var pinHelper = new PinCanvasItemHelper(CanvasModel);
            var hitPins = pinHelper.GetPinsCollision(linePointMM, item.Width);


            //we take the 1st pin; we could show a list with the pins
            var hitPin = hitPins.FirstOrDefault();
            ////////////////////////////////////////////

            if (hitPin != null)
            {

                //create a net if we don't have one at this point
                if (item.Net == null)
                {
                    item.Net = hitPin.PinType == PinType.Power ? GetExistingNetOrCreateNew(hitPin.Name) : GetExistingNetOrCreateNew();
                }
                else
                {
                    //we are on a named net but we hit a supply pin; ask what net to use
                    if (hitPin.PinType == PinType.Power)
                    {
                        var supplyPinNet = GetExistingNetOrCreateNew(hitPin.Name);

                        if (item.Net.IsNamed())
                        {
                            if (supplyPinNet.Name != item.Net.Name)
                            {
                                var candidateNets = new[] { supplyPinNet, item.Net }.OrderBy(n => n.Name).ToList();

                                var itemPickerDialog = ServiceProvider.Resolve<IItemPickerDialog>();
                                itemPickerDialog.LoadData(candidateNets);
                                var res = itemPickerDialog.ShowDialog();

                                if (res.GetValueOrDefault() == false)
                                    return;

                                var selectedNet = itemPickerDialog.SelectedItem as SchematicNet;

                                ChangeNet(item.Net, selectedNet);
                            }

                        }
                        else
                        {
                            ChangeNet(item.Net, supplyPinNet);
                        }

                    }
                }
                hitPin.Net = item.Net;
            }

            #endregion Wire with pins
        }

        private bool TestBusHit(ref XPoint point)
        {
            var item = GetItem();
            var circle = new CircleCanvasItem { Diameter = item.Width, X = point.X, Y = point.Y };
            var busItems = CanvasModel.Items.OfType<BusWireCanvasItem>().Where(b => b.IsPlaced).ToList();

            foreach (var busItem in busItems)
            {
                var intersects = GeometryHelper.Intersects(busItem, circle);
                if (!intersects)
                    continue;

                if (busItem.Bus != null)
                {
                    if (item.Net != null && item.Net.IsNamed())
                    {
                        //add net to bus
                        busItem.Bus.AddNet(item.Net.Name);
                        return true;
                    }
                    else
                    {
                        //choose a net from the bus
                        var candidateNets = busItem.Bus.Nets.OrderBy(n => n).ToList();

                        if (candidateNets.Count > 0)
                        {
                            var itemPickerDialog = ServiceProvider.Resolve<IItemPickerDialog>();
                            itemPickerDialog.LoadData(candidateNets);
                            var res = itemPickerDialog.ShowDialog();

                            if (res.GetValueOrDefault() == false)
                                return false;//cancelled

                            var selectedNet = itemPickerDialog.SelectedItem as string;

                            if (selectedNet == null)
                                return false;

                            //assign the new net from the bus
                            var newNet = GetExistingNetOrCreateNew(selectedNet);
                            if (newNet != null && newNet.IsNamed())
                            {
                                if (item.Net != null)
                                    ChangeNet(item.Net, newNet);
                                else
                                    item.Net = newNet;
                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }

                    //no matter what, we break from the loop if we intersect a bus wire
                    break;//foreach var busItem
                }
            }

            return false;
        }

        SchematicNet GetExistingNetOrCreateNew(string netName = null)
        {
            if (!string.IsNullOrWhiteSpace(netName))
            {
                var existingNet = NetManager.Get(netName) as SchematicNet;

                if (existingNet != null)
                    return existingNet;
            }

            var newNet = NetManager.AddNew();
            if (!string.IsNullOrWhiteSpace(netName))
                newNet.Name = netName;

            return (SchematicNet)newNet;
        }

        /// <summary>
        /// Change all the items belonging to the sourceNet to the new destination Net
        /// </summary>
        /// <param name="sourceNet"></param>
        /// <param name="destNet"></param>
        private void ChangeNet(SchematicNet sourceNet, SchematicNet destNet)
        {
            if (sourceNet == null || destNet == null)
                return;

            var netItems = sourceNet.NetItems.ToList();

            //just change the reference
            netItems.ForEach(n => n.Net = destNet);

            _netChanges.Add(new NetChanges
            {
                OldNet = sourceNet,
                NewNet = destNet,
                OldNetItems = netItems
            });
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
    }
}
