using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Presentation.Placement;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDE.Core.Presentation.Utilities
{
    public class NetWireEndPointCollisionHelper
    {

        public NetWireEndPointCollisionHelper(NetWireCanvasItem netWire, ISchematicDesigner canvasModel, IGeometryOutlineHelper geometryHelper)
        {
            _netWire = netWire;
            _canvasModel = canvasModel;
            _geometryHelper = geometryHelper;
        }

        private readonly NetWireCanvasItem _netWire;
        private readonly ISchematicDesigner _canvasModel;
        private readonly IGeometryOutlineHelper _geometryHelper;

        INetManager GetNetManager()
        {
            return _canvasModel?.NetManager;
        }

        public void HandleLinePoint(XPoint linePointMM)
        {
            if (TestBusHit(ref linePointMM))
                return;

            TestOtherNetSegments(linePointMM);

            TestPins(linePointMM);
        }

        private bool TestBusHit(ref XPoint point)
        {

            var busItems = _canvasModel.Items.OfType<BusWireCanvasItem>().Where(b => b.IsPlaced).ToList();
            var circle = new CircleCanvasItem { Diameter = _netWire.Width, X = point.X, Y = point.Y };

            foreach (var busItem in busItems)
            {
                var intersects = _geometryHelper.Intersects(busItem, circle);
                if (!intersects)
                    continue;

                if (busItem.Bus != null)
                {
                    if (_netWire.Net != null && _netWire.Net.IsNamed())
                    {
                        //add net to bus
                        busItem.Bus.AddNet(_netWire.Net.Name);
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
                                if (_netWire.Net != null)
                                    ChangeNet(_netWire.Net, newNet);
                                else
                                    _netWire.Net = newNet;


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

        private bool TestOtherNetSegments(XPoint point)
        {
            var circle = new CircleCanvasItem { Diameter = _netWire.Width, X = point.X, Y = point.Y };
            var intersectedWires = new List<NetWireCanvasItem>();
            var addJunction = false;
            var netElements = _canvasModel.Items.OfType<NetSegmentCanvasItem>().Where(ns => ns != _netWire && ns.IsPlaced).ToList();
            foreach (var netSegm in netElements)
            {
                //ignore colision with net labels or pinrefs
                if (netSegm is NetLabelCanvasItem)
                    continue;

                if (netSegm is ICanvasItem geom)
                {
                    if (geom != null)
                    {
                        var intersects = _geometryHelper.Intersects(geom, circle);
                        if (!intersects)
                            continue;

                        //assign this net to be that intersected net
                        SchematicNet sourceNet = _netWire.Net;//oldNet
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
                            _netWire.Net = destNet;
                        }


                        if (netSegm is NetWireCanvasItem)
                        {
                            //if we are on a net wire but not on a start or end, add a junction
                            var otherNetWire = netSegm as NetWireCanvasItem;
                            if (point != otherNetWire.StartPoint && point != otherNetWire.EndPoint)
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
                var junctionExists = _canvasModel.Items.OfType<JunctionCanvasItem>()
                                              .Any(j => j.X == point.X && j.Y == point.Y);

                if (!junctionExists)
                {
                    var junctionItem = new JunctionCanvasItem();
                    junctionItem.X = point.X;
                    junctionItem.Y = point.Y;
                    junctionItem.Net = _netWire.Net;
                    junctionItem.IsPlaced = true;
                    _canvasModel.AddItem(junctionItem);
                }
            }

            return true;
        }

        private void TestPins(XPoint linePointMM)
        {
            var pinHelper = new PinCanvasItemHelper(_canvasModel);
            var hitPins = pinHelper.GetPinsCollision(linePointMM, _netWire.Width);


            //we take the 1st pin; we could show a list with the pins
            var hitPin = hitPins.FirstOrDefault();

            if (hitPin != null)
            {

                //create a net if we don't have one at this point
                if (_netWire.Net == null)
                {
                    _netWire.Net = hitPin.PinType == PinType.Power ? GetExistingNetOrCreateNew(hitPin.Name) : GetExistingNetOrCreateNew();
                }
                else
                {
                    //we are on a named net but we hit a supply pin; ask what net to use
                    if (hitPin.PinType == PinType.Power)
                    {
                        var supplyPinNet = GetExistingNetOrCreateNew(hitPin.Name);

                        if (_netWire.Net.IsNamed())
                        {
                            if (supplyPinNet.Name != _netWire.Net.Name)
                            {
                                var candidateNets = new[] { supplyPinNet, _netWire.Net }.OrderBy(n => n.Name).ToList();

                                var itemPickerDialog = ServiceProvider.Resolve<IItemPickerDialog>();
                                itemPickerDialog.LoadData(candidateNets);
                                var res = itemPickerDialog.ShowDialog();

                                if (res.GetValueOrDefault() == false)
                                    return;

                                var selectedNet = itemPickerDialog.SelectedItem as SchematicNet;

                                ChangeNet(_netWire.Net, selectedNet);
                            }

                        }
                        else
                        {
                            ChangeNet(_netWire.Net, supplyPinNet);
                        }

                    }
                }
                hitPin.Net = _netWire.Net;
            }
        }

        SchematicNet GetExistingNetOrCreateNew(string netName = null)
        {
            var netManager = GetNetManager();
            if (!string.IsNullOrWhiteSpace(netName))
            {
                var existingNet = netManager.Get(netName) as SchematicNet;

                if (existingNet != null)
                    return existingNet;
            }

            var newNet = netManager.AddNew();
            if (!string.IsNullOrWhiteSpace(netName))
                newNet.Name = netName;

            return (SchematicNet)newNet;
        }

        /// <summary>
        /// Change all the items belonging to the sourceNet to the new destination Net
        /// </summary>
        /// <param name="sourceNet"></param>
        /// <param name="destNet"></param>
        void ChangeNet(SchematicNet sourceNet, SchematicNet destNet)
        {
            if (sourceNet == null || destNet == null)
                return;

            var netItems = sourceNet.NetItems.ToList();

            //just change the reference
            netItems.ForEach(n => n.Net = destNet);
        }

    }
}
