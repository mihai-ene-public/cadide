using IDE.Core.Designers;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.PlacementRouting
{
    public class SingleTrackRoutingMode : TrackRoutingMode
    {
        TrackBoardCanvasItem GetItem() => PlacementTool.CanvasItem as TrackBoardCanvasItem;

        public SingleTrackRoutingMode()
        {

        }

        public override void PlacementMouseUp(XPoint mousePosition)
        {
            var mp = canvasModel.SnapToGrid(mousePosition);

            var item = GetItem();

            //code similar from TracePlacementTool

            var hitItems = new List<ISignalPrimitiveCanvasItem>();

            switch (PlacementStatus)
            {
                //1st click
                case PlacementStatus.Ready:
                    item.StartPoint = mp;

                    //if we clicked on a net segment (pad/via/trace)
                    HandleLinePoint(item.StartPoint, hitItems);
                    if (item.Signal != null)
                    {
                        if (hitLayer != null)
                        {
                            currentRoutingBehavior.SetStartLayer(hitLayer, mp);
                        }

                        if (hitItems.Count > 0)
                            SplitAdjacentSegments(hitItems, item.StartPoint);

                        PlacementStatus = PlacementStatus.Started;
                    }

                    break;
                //2nd click
                case PlacementStatus.Started:
                    if (HandleLinePoint(item.EndPoint, hitItems))
                    {
                        if (hitLayer != null)
                            currentRoutingBehavior.SetEndLayer(hitLayer, mp);

                        if (hitItems.Count > 0)
                            SplitAdjacentSegments(hitItems, item.EndPoint);
                        //removeLoops()
                        //updateLeadingRatLine()

                        currentRoutingBehavior.CommitRouting();

                        var toRemove = item.StartPoint == item.EndPoint;
                        if (item.StartPoint == item.EndPoint)
                        {
                            // Parent.RemoveItem(this);
                        }
                        else
                        {
                            item.IsPlaced = true;
                        }

                        RemoveLoops();

                        //if (IsPlaced)
                        CanvasModel.OnDrawingChanged(DrawingChangedReason.ItemPlacementFinished);

                        //create another line
                        var newItem = (TrackBoardCanvasItem)item.Clone();
                        newItem.StartPoint = mp;
                        newItem.Signal = item.Signal;

                        //todo: check the layer should be on a copper layer

                        PlacementStatus = PlacementStatus.Started;


                        CanvasModel.AddItem(newItem);

                        //if (currentRoutingBehavior.canvasModel == null)
                        //    currentRoutingBehavior.canvasModel = CanvasModel;

                        currentRoutingBehavior.CurrentSegment = newItem;
                        currentRoutingBehavior.SetStartLayer(item.Layer, mp);

                        if (toRemove)
                            CanvasModel.RemoveItem(item);

                        PlacementTool.CanvasItem = newItem;

                        //?
                        PlacementTool.SetupCanvasItem();
                    }

                    break;
            }
        }

        ILayerDesignerItem hitLayer = null;
        //returns true if we validate for next operation
        bool HandleLinePoint(XPoint linePointMM, List<ISignalPrimitiveCanvasItem> signalItems)
        {
            hitLayer = null;
            var item = GetItem();

            //if (Parent == null)
            //    return false;

            //check if we intersect a point of this trace with other pads or signal items
            //var hitPads = new List<IPadCanvasItem>();
            // var signalItems = new List<ISignalPrimitiveCanvasItem>();

            /*
            HitTestResultCallback resultCallback = delegate (HitTestResult result)
            {
                var fwk = result.VisualHit as FrameworkElement;
                if (fwk != null && fwk.DataContext != this)
                {
                    if (fwk.DataContext is IPadCanvasItem)
                    {
                        var pad = fwk.DataContext as IPadCanvasItem;

                        if (!hitPads.Contains(pad))
                            hitPads.Add(pad);
                    }
                    else if (fwk.DataContext is ISignalPrimitiveCanvasItem)
                    {
                        var signalItem = fwk.DataContext as ISignalPrimitiveCanvasItem;
                        if (signalItem.Signal != null && ((BaseCanvasItem)signalItem).IsPlaced)
                            signalItems.Add(signalItem);
                    }
                }
                return HitTestResultBehavior.Continue;
            };

            var center = MilimetersToDpiHelper.ConvertToDpi(linePointMM);
            var radius = MilimetersToDpiHelper.ConvertToDpi(item.Width / 2);
            VisualTreeHelper.HitTest(CanvasModel.Canvas, null,
                                    resultCallback,
                                    new GeometryHitTestParameters(new EllipseGeometry(center, radius, radius))
                                    );
            */
            HitTestItems(linePointMM, signalItems);


            //we take the 1st pad; we could show a list with the pads
            var hitPad = signalItems.OfType<IPadCanvasItem>().FirstOrDefault() as BoardCanvasItemViewModel;
            //we want to have non-polygons first in our collision
            var polygons = signalItems.OfType<IPolygonBoardCanvasItem>().Cast<ISignalPrimitiveCanvasItem>();
            var notPolygons = signalItems.Except(polygons);
            var hitItems = notPolygons.Union(polygons);
            var hitItem = hitItems.FirstOrDefault();
            ////////////////////////////////////////////

            if (hitPad != null)
            {
                //var fpItem = hitPad.ParentObject as FootprintBoardCanvasItem;
                //var fpInstance = fpItem.FootprintPrimitive;
                //var fpInstanceId = fpInstance.Id;
                //var padNumber = ((IPadCanvasItem)hitPad).Number;

                // var board = fpItem.BoardModel as IBoardModel;
                var board = hitPad.LayerDocument as IBoardDesigner;

                ////search a signal that has our pad
                //var candidate = (from n in board.NetList
                //                 from p in n.Pads
                //                 where p.FootprintInstanceId == fpInstanceId && p.Number == padNumber
                //                 select p.Signal).FirstOrDefault();

                var candidate = ((IPadCanvasItem)hitPad).Signal;

                if (candidate != null)
                {
                    //options:
                    //      we have a hit layer based on placement of footprint
                    //  (*) the layer is the current layer
                    hitLayer = board.SelectedLayer;
                    //there is a bug 

                    if (item.Signal == null)
                        item.Signal = candidate;
                    else
                        //we validate if we are on the same signal
                        return item.Signal.Name == candidate.Name;
                }

            }
            else if (hitItem != null)
            {
                if (hitItem is SingleLayerBoardCanvasItem)
                {
                    hitLayer = (hitItem as SingleLayerBoardCanvasItem).Layer;
                }

                if (item.Signal == null)
                    item.Signal = hitItem.Signal;
                else
                {
                    if (hitItem is IPolygonBoardCanvasItem)
                        return true;

                    //we validate if we are on the same signal
                    return hitItem.Signal != null && item.Signal.Name == hitItem.Signal.Name;
                }

            }


            return true;
        }

        void HitTestItems(XPoint point, List<ISignalPrimitiveCanvasItem> signalItems)
        {
            var item = GetItem();

            var obstacles = obstaclesProvider.GetNearestObstacles(point, item.Width);

            foreach (var obs in obstacles)
            {
                if (obs.CanvasItem is ISignalPrimitiveCanvasItem signalItem)
                {
                    if (GeometryHelper.Intersects(item, signalItem))
                    {
                        if (signalItem is IPadCanvasItem)
                        {
                            signalItems.Add(signalItem);
                        }
                        else if (signalItem.Signal != null && signalItem.IsPlaced)
                        {
                            signalItems.Add(signalItem);
                        }
                    }
                }
            }
        }

        //for a track on the same net we've hit, split it in 2 tracks at the splitPoint
        void SplitAdjacentSegments(IList<ISignalPrimitiveCanvasItem> hitItems,XPoint splitPoint)
        {
            //  return;

            //we want the 1st track on the same layer that doesn't end or start with splitPoint
            var trackItem = hitItems.OfType<TrackBoardCanvasItem>()
                                  .Where(s => s.LayerId == currentRoutingBehavior.CurrentSegment.LayerId
                                           && s.Signal == currentRoutingBehavior.CurrentSegment.Signal
                                           && s.StartPoint != splitPoint && s.EndPoint != splitPoint)
                                  .FirstOrDefault();

            if (trackItem != null)
            {
                //2 new tracks

                //build segments
                for (int i = 0; i < trackItem.Points.Count - 1; i++)
                {
                    var segment = new XLineSegment(trackItem.Points[i], trackItem.Points[i + 1]);

                    if (segment.IsPointOnLine(splitPoint))
                    {
                        var track1 = new TrackBoardCanvasItem()
                        {
                            LayerDocument = trackItem.LayerDocument,
                            ParentObject = trackItem.ParentObject,
                            Layer = trackItem.Layer,
                            Signal = trackItem.Signal,
                            Width = trackItem.Width,
                            IsPlaced = true
                        };
                        track1.Points.Clear();
                        for (int pIndex = 0; pIndex <= i; pIndex++)
                        {
                            track1.Points.Add(trackItem.Points[pIndex]);
                        }
                        track1.Points.Add(splitPoint);


                        var track2 = new TrackBoardCanvasItem()
                        {
                            LayerDocument = trackItem.LayerDocument,
                            ParentObject = trackItem.ParentObject,
                            Layer = trackItem.Layer,
                            Signal = trackItem.Signal,
                            Width = trackItem.Width,
                            IsPlaced = true
                        };
                        track2.Points.Clear();

                        track2.Points.Add(splitPoint);

                        for (int pIndex = i + 1; pIndex < trackItem.Points.Count; pIndex++)
                        {
                            track2.Points.Add(trackItem.Points[pIndex]);
                        }

                        canvasModel.AddItem(track1);
                        canvasModel.AddItem(track2);

                        //remove the track
                        canvasModel.RemoveItem(trackItem);

                        break;
                    }
                }

            }
        }

        void RemoveLoops()
        {
            //todo: currently the net is not optimized properly, so we remove this feature for now
            //we need to have a better architecture

            /*
            var net = GetItem().Signal;

            net.Optimize();

            net.RemoveLoops();
            */
        }
    }
}
