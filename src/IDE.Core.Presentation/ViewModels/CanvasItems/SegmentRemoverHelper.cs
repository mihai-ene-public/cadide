using System;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Presentation.Utilities;
using System.Threading.Tasks;

namespace IDE.Core.Designers
{
    public class SegmentRemoverHelper
    {
        private readonly IDrawingViewModel _canvasModel;
        private readonly ISegmentedPolylineSelectableCanvasItem _canvasItem;
        private readonly IDispatcherHelper _dispatcher;

        public SegmentRemoverHelper(IDrawingViewModel canvasModel, ISegmentedPolylineSelectableCanvasItem canvasItem, IDispatcherHelper dispatcher)
        {
            _canvasModel = canvasModel;
            _canvasItem = canvasItem;
            _dispatcher = dispatcher;
        }

        private ISegmentedPolylineSelectableCanvasItem CreateAnother()
        {
            switch (_canvasItem)
            {
                case NetWireCanvasItem netWire:
                    return new NetWireCanvasItem
                    {
                        ParentObject = netWire.ParentObject,
                        Width = netWire.Width,
                        LineColor = netWire.LineColor,
                        ZIndex = netWire.ZIndex,
                        IsPlaced = true
                    };

                case BusWireCanvasItem busWire:
                    return new BusWireCanvasItem()
                    {
                        ParentObject = busWire.ParentObject,
                        Width = busWire.Width,
                        LineColor = busWire.LineColor,
                        Nets = busWire.Nets,
                        ZIndex = busWire.ZIndex,
                        IsPlaced = true
                    };

                case TrackBoardCanvasItem trackWire:
                    return new TrackBoardCanvasItem()
                    {
                        LayerDocument = trackWire.LayerDocument,
                        ParentObject = trackWire.ParentObject,
                        Layer = trackWire.Layer,
                        Signal = trackWire.Signal,//same signal (this is intended)
                        Width = trackWire.Width,
                        IsPlaced = true
                    };

            }

            throw new NotImplementedException();
        }

        private void RunRemoveBehavior(IList<ISegmentedPolylineSelectableCanvasItem> newTracks)
        {
            switch (_canvasItem)
            {
                case NetWireCanvasItem netWire:
                    RunRemoveNetWireBehavior(netWire, newTracks.Cast<NetWireCanvasItem>().ToList());
                    break;

                case BusWireCanvasItem busWire:
                    RunRemoveBusWireBehavior(busWire, newTracks.Cast<BusWireCanvasItem>().ToList());
                    break;

                case TrackBoardCanvasItem trackWire:
                    RunRemoveTrackWireBehavior(trackWire, newTracks.Cast<TrackBoardCanvasItem>().ToList());
                    break;
            }
        }



        private void RunRemoveNetWireBehavior(NetWireCanvasItem canvasItem, IList<NetWireCanvasItem> newTracks)
        {
            //assumptions:
            //one wire net endpoint (start, end) connect to a pin
            //a pin connects to a single net wire endpoint

            var net = canvasItem.Net;

            if (net == null)
                return;

            var netManager = _canvasModel.GetNetManager();

            canvasItem.Net = null;

            foreach (var pin in net.NetItems.OfType<PinCanvasItem>().ToList())
            {
                if (PinConnectsWithWire(pin, canvasItem))
                {
                    pin.Net = null;

                    foreach (var track in newTracks)
                    {
                        if (PinConnectsWithWire(pin, track))
                        {
                            var newNet = (SchematicNet)netManager.AddNew();
                            pin.Net = newNet;
                            track.Net = newNet;

                            break;
                        }
                    }
                }
            }

            foreach (var track in newTracks)
            {
                if (track.Net == null)
                {
                    var newNet = (SchematicNet)netManager.AddNew();
                    track.Net = newNet;
                }
            }

            if (net.NetItems?.Count == 0)
            {
                netManager.Remove(net);
            }
        }

        private void RunRemoveBusWireBehavior(BusWireCanvasItem canvasItem, IList<BusWireCanvasItem> newTracks)
        {
            //assumptions:
            //one wire net endpoint (start, end) connect to a pin
            //a pin connects to a single net wire endpoint

            var bus = canvasItem.Bus;

            var busManager = _canvasModel.GetBusManager();

            if (bus == null)
                return;

            canvasItem.Bus = null;

            foreach (var track in newTracks)
            {
                if (track.Points.Count > 1)
                {
                    var newBus = (SchematicBus)busManager.AddNew();
                    track.Bus = newBus;
                }
            }

            if (bus.BusItems?.Count == 0)
            {
                busManager.Remove(bus);
            }
        }

        private void RunRemoveTrackWireBehavior(TrackBoardCanvasItem canvasItem, IList<TrackBoardCanvasItem> newTracks)
        {
            //there is no behavior at the moment; see if we need to do anything

        }

        private bool PinConnectsWithWire(PinCanvasItem pin, NetWireCanvasItem netWire)
        {
            var t = pin.GetTransform();
            var pinPos = t.Transform(new XPoint()).Round();

            //maybe improve this with GeometryHelper.CirclesIntersect

            var sp = netWire.StartPoint.Round();
            var ep = netWire.EndPoint.Round();

            return pinPos == sp || pinPos == ep;
        }

        public void RemoveSelectedSegments()
        {
            if (_canvasItem.HasSelectedSegments())
            {
                //consider we have to tracks and rebuild points
                //if we have a track from the points then we build another item
                var track1Points = new List<XPoint>();
                for (int pIndex = 0; pIndex <= _canvasItem.SelectedSegmentStart; pIndex++)// pIndex <=? canvasItem.SelectedSegmentStart
                {
                    track1Points.Add(_canvasItem.Points[pIndex]);
                }

                var track2Points = new List<XPoint>();
                for (int pIndex = _canvasItem.SelectedSegmentEnd + 1; pIndex < _canvasItem.Points.Count; pIndex++)
                {
                    track2Points.Add(_canvasItem.Points[pIndex]);
                }

                var newTracks = new List<ISegmentedPolylineSelectableCanvasItem>();

                if (track1Points.Count > 1)
                {
                    var track1 = CreateAnother();
                    track1.Points = track1Points;
                    newTracks.Add(track1);
                }

                if (track2Points.Count > 1)
                {
                    var track2 = CreateAnother();
                    track2.Points = track2Points;
                    newTracks.Add(track2);
                }

                RunRemoveBehavior(newTracks);

                _canvasModel.ClearSelectedItems();
                _canvasModel.UpdateSelection();

                //this is an workaround for a refresh issue
                //it happens that when you delete a segment in the middle the first portion is not visible; only the second part of the segment is
                //it exists on the canvas since if you save then reload all segments are where they're supposed to be
                //var dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();

                _dispatcher.RunOnDispatcher(async () =>
                {
                    foreach (ISelectableItem track in newTracks)
                    {
                        _canvasModel.AddItem(track);
                        await Task.Delay(10);
                    }
                    _canvasModel.RemoveItem((ISelectableItem)_canvasItem);
                });
            }
        }
    }
}
