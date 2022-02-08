using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
//using IDE.Core.Routing;
using System;
using System.ComponentModel;
using System.Linq;

namespace IDE.Core.Presentation.PlacementRouting
{
    public class RoutingBehavior
    {
        public RoutingBehavior(TrackRoutingMode routingMode)
        {
            trackRoutingMode = routingMode;
        }

        protected static TracePlacementMode placementMode = TracePlacementMode.DiagonalDirect;

        protected TrackRoutingMode trackRoutingMode;

        protected ILayerDesignerItem startLayer;

        protected ILayerDesignerItem endLayer;

        ViaCanvasItem startVia;
        //bool startViaIsVisible;

        ViaCanvasItem endVia;
        // bool endViaIsVisible;

        protected TrackBoardCanvasItem currentSegment;
        public TrackBoardCanvasItem CurrentSegment
        {
            get
            {
                return currentSegment;
            }
            set
            {
                if (currentSegment != value)
                {
                    if (currentSegment != null)
                        currentSegment.PropertyChanged -= CurrentSegment_PropertyChanged;

                    currentSegment = value;

                    if (currentSegment != null)
                        currentSegment.PropertyChanged += CurrentSegment_PropertyChanged;
                }
            }
        }

        public TrackRoutingMode TrackRoutingMode
        {
            get { return trackRoutingMode; }
            set { trackRoutingMode = value; }
        }

        protected bool segmentChanging;
        protected virtual void CurrentSegment_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (segmentChanging)
                return;

            try
            {
                segmentChanging = true;

                switch (e.PropertyName)
                {
                    case nameof(TrackBoardCanvasItem.Layer):
                        HandleVia(startVia, startLayer);
                        break;
                }

            }
            finally
            {
                segmentChanging = false;
            }
        }

        protected void EnsureMiddlePoint()
        {
            EnsureMiddlePoint(currentSegment);
        }

        protected void EnsureMiddlePoint(TrackBoardCanvasItem track)
        {
            if (track.Points.Count == 3)
                return;

            if (track.Points.Count == 2)
            {
                track.Points.Insert(1, new XPoint());
                return;
            }

            throw new InvalidOperationException($"Our line has {track.Points.Count} points");
        }

        protected void RemoveMiddlePoint()
        {
            if (currentSegment.Points.Count == 3)
            {
                currentSegment.Points.RemoveAt(1);
            }
        }

        protected void PointsChanged()
        {
            currentSegment.OnPropertyChanged(nameof(currentSegment.Points));
        }

        protected virtual void Start()
        { }

        public void SetStartLayer(ILayerDesignerItem layer, XPoint mp)
        {
            startLayer = layer;

            startVia = CreateVia(mp);

            HandleVia(startVia, startLayer);

            Start();
        }
        //protected void HandleStartVia()
        //{
        //    var canvasModel = trackRoutingMode.CanvasModel;

        //    if (canvasModel == null || currentSegment.Layer == null)
        //        return;

        //    if (startLayer != null && currentSegment.Layer != null && startLayer != currentSegment.Layer)
        //    {
        //        //check if we already have a via placed
        //        //we should check if vias collide
        //        var otherVia = canvasModel.Items.OfType<ViaCanvasItem>().Where(v => v.IsPlaced //&& v != startVia
        //                                                                         && v.X == startVia.X && v.Y == startVia.Y)
        //                        .FirstOrDefault();

        //        if (otherVia == null)
        //        {
        //            if (canvasModel.Items.Contains(startVia))
        //                return;
        //            startVia.Signal = currentSegment.Signal;
        //            canvasModel.AddItem(startVia);
        //        }
        //    }
        //    else
        //    {
        //        //remove start via
        //        if (startVia != null)
        //        {
        //            canvasModel.RemoveItem(startVia);
        //            startVia.Signal = null;
        //        }

        //    }
        //}

        //protected void HandleEndVia()
        //{
        //    var canvasModel = trackRoutingMode.CanvasModel;

        //    if (canvasModel == null || currentSegment.Layer == null)
        //        return;

        //    if (endLayer != null && currentSegment.Layer != null && endLayer != currentSegment.Layer)
        //    {
        //        //check if we already have a via placed
        //        //we should check if vias collide
        //        var otherVia = canvasModel.Items.OfType<ViaCanvasItem>().Where(v => v.IsPlaced //&& v != startVia
        //                                                                         && v.X == endVia.X && v.Y == endVia.Y)
        //                        .FirstOrDefault();

        //        if (otherVia == null)
        //        {
        //            if (canvasModel.Items.Contains(endVia))
        //                return;
        //            endVia.Signal = currentSegment.Signal;
        //            canvasModel.AddItem(endVia);
        //        }
        //    }
        //    else
        //    {
        //        canvasModel.RemoveItem(endVia);
        //        endVia.Signal = null;
        //    }
        //}

        protected void HandleVia(ViaCanvasItem via, ILayerDesignerItem layer)
        {
            var canvasModel = trackRoutingMode.CanvasModel;

            if (canvasModel == null || currentSegment.Layer == null)
                return;

            if (layer != null && currentSegment.Layer != null && layer != currentSegment.Layer)
            {
                //check if we already have a via placed
                //we should check if vias collide
                var otherVia = canvasModel.Items.OfType<ViaCanvasItem>()
                                                .FirstOrDefault(v => v.IsPlaced
                                                                  && v.X == via.X
                                                                  && v.Y == via.Y);


                if (otherVia == null)
                {
                    if (canvasModel.Items.Contains(via))
                        return;
                    via.Signal = currentSegment.Signal;
                    via.AssignOnDrillPair(layer, currentSegment.Layer);

                    canvasModel.AddItem(via);
                }
            }
            else
            {
                //remove start via
                if (via != null)
                {
                    canvasModel.RemoveItem(via);
                    via.DrillPair = null;
                    via.Signal = null;
                }

            }
        }


        public void SetEndLayer(ILayerDesignerItem layer, XPoint mp)
        {
            endLayer = layer;

            //init end via
            endVia = CreateVia(mp);

            HandleVia(endVia, endLayer);
        }

        ViaCanvasItem CreateVia(XPoint position)
        {
            return new ViaCanvasItem()
            {
                LayerDocument = currentSegment.LayerDocument,
                X = position.X,
                Y = position.Y,
                IsPlaced = false,
                Drill = 0.3,
                Diameter = 0.7,

                Signal = currentSegment.Signal
            };
        }

        public virtual void CommitRouting()
        {
            var canvasModel = trackRoutingMode.CanvasModel;

            if (canvasModel.Items.Contains(startVia))
                startVia.IsPlaced = true;
            if (canvasModel.Items.Contains(endVia))
                endVia.IsPlaced = true;
        }

        public virtual void RollbackRouting()
        {

        }

        public virtual void CyclePlacementMode()
        {
            var pm = 1 + (int)placementMode;
            placementMode = (TracePlacementMode)((pm) % (int)TracePlacementMode.Count);
        }

        public virtual void PlacementMouseMove(XPoint mousePosition)
        {
            //updateStartItem(true)

        }

        public virtual void PlacementMouseUp(XPoint mousePosition)
        {
            //updateStartItem(ignorePads: false)
            //performRouting()
        }

        protected XPoint GetDiagonalDirectPoint(XPoint mp, XPoint startPoint, XPoint endPoint)
        {
            //candidate points
            var cp1 = new XPoint();
            var cp2 = new XPoint();
            var x1 = startPoint.X;
            var x2 = endPoint.X;
            var y1 = startPoint.Y;
            var y2 = endPoint.Y;

            //quadrants 1 or 3 (slope = 1)
            if (mp.X > x1 && mp.Y > y1 || mp.X <= x1 && mp.Y <= y1)
            {
                cp1.X = mp.X;
                cp1.Y = mp.X - x1 + y1;

                cp2.Y = mp.Y;
                cp2.X = mp.Y + x1 - y1;
            }
            else //slope = -1
            {
                cp1.X = mp.X;
                cp1.Y = -mp.X + x1 + y1;

                cp2.Y = mp.Y;
                cp2.X = -mp.Y + x1 + y1;
            }

            var sp = new XPoint(x1, y1);
            var dp1 = (sp - cp1).LengthSquared;
            var dp2 = (sp - cp2).LengthSquared;
            if (dp1 <= dp2)
                return cp1;
            else
                return cp2;

        }

        protected XPoint GetDirectDiagonalPoint(XPoint mp, XPoint startPoint, XPoint endPoint)
        {
            //candidate points
            var cp1 = new XPoint();
            var cp2 = new XPoint();
            var x1 = startPoint.X;
            var x2 = endPoint.X;
            var y1 = startPoint.Y;
            var y2 = endPoint.Y;

            //quadrants 1 or 3 (slope = 1)
            if (mp.X > x1 && mp.Y > y1 || mp.X <= x1 && mp.Y <= y1)
            {
                cp1.X = x1;
                cp1.Y = x1 - mp.X + mp.Y;

                cp2.Y = y1;
                cp2.X = y1 + mp.X - mp.Y;
            }
            else //slope = -1
            {
                cp1.X = x1;
                cp1.Y = -x1 + mp.X + mp.Y;

                cp2.Y = y1;
                cp2.X = -y1 + mp.X + mp.Y;
            }

            var sp = new XPoint(x1, y1);
            var dp1 = (sp - cp1).LengthSquared + (mp - cp1).LengthSquared;
            var dp2 = (sp - cp2).LengthSquared + (mp - cp2).LengthSquared;
            if (dp1 <= dp2)
                return cp1;
            else
                return cp2;

        }
    }
}
