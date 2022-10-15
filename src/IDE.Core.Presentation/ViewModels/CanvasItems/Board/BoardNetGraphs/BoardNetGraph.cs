using IDE.Core.Common.Geometries;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Spatial2D;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace IDE.Core.Designers
{
    /// <summary>
    /// The net graph for a specific net. Shows what is connected with what in that net.
    /// <para>A net graph is formed by branches</para>
    /// <para>The purpose is to show unconnected lines, on the fly</para>
    /// </summary>
    public class BoardNetGraph : BaseCanvasItem, IBoardNetGraph
    {
        public BoardNetGraph(BoardNetDesignerItem pNet)
        {
            net = pNet;
            dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
            ZIndex = 5000;//topmost
            CanEdit = false;
        }
        IDispatcherHelper dispatcher;

        BoardNetDesignerItem net;
        public IBoardNetDesignerItem Net => net;

        List<BoardNetBranch> Branches { get; set; } = new List<BoardNetBranch>();
        public ObservableCollection<BoardConnectionLine> Lines { get; } = new ObservableCollection<BoardConnectionLine>();

        [Browsable(false)]
        public override bool CanClone => false;

        //todo: we should have a single branch
        public bool IsCompletelyRouted =>// Branches.Count <= 1;
                                        Lines.Count == 0;


        RTree<SignalTreeNodeItem> tree;
        List<SignalTreeNodeItem> treeItems = new List<SignalTreeNodeItem>();
        List<BranchClass> branchConnections = new List<BranchClass>();
        public void Build(bool showUnConnectedLines)
        {
            RegisterTree();

            branchConnections.Clear();

            //start from a pad and walk the connection
            foreach (var pad in net.Pads)
            {
                var items = new List<SignalTreeNodeItem>();
                var padTreeItem = treeItems.FirstOrDefault(p => p.CanvasItem == pad);

                if (padTreeItem == null)
                    continue;

                WalkPad(padTreeItem, items);

                var branch = new BranchClass();
                branch.Pads.Add(padTreeItem);
                branch.Pads.AddRange(items.Where(p => p.CanvasItem is IPadCanvasItem));

                branch.Items.AddRange(items.Where(p => !(p.CanvasItem is IPadCanvasItem)));
                branchConnections.Add(branch);
            }

            BuildLines(showUnConnectedLines);
        }

        void RegisterTree()
        {
            tree = new RTree<SignalTreeNodeItem>();
            treeItems = new List<SignalTreeNodeItem>();

            foreach (var item in net.GetAllNetItems())
            {

                var geom = GeometryHelper.GetGeometry(item, applyTransform: true);
                var rect = geom.GetBounds();

                var treeItem = new SignalTreeNodeItem
                {
                    CanvasItem = item,
                    ItemGeometry = geom,
                    Envelope = Envelope.FromRect(rect)
                };

                tree.Insert(treeItem);
                treeItems.Add(treeItem);
            }
        }

        void WalkPad(SignalTreeNodeItem pad, List<SignalTreeNodeItem> items)
        {
            var obstacles = tree.Search(pad.Envelope);

            foreach (var obs in obstacles)
            {
                WalkObstacle(pad, obs, items);
            }
        }

        void WalkObstacle(SignalTreeNodeItem sourceItem, SignalTreeNodeItem obstacle, List<SignalTreeNodeItem> items)
        {

            if (//!obstacle.Walked &&
                !items.Contains(obstacle) &&
                 obstacle != sourceItem
                && GeometryHelper.Intersects(sourceItem.CanvasItem, obstacle.CanvasItem))
            {
                obstacle.Walked = true;
                items.Add(obstacle);

                //we stop if we have a pad
                if (obstacle.CanvasItem is IPadCanvasItem)
                    return;

                //var nextEnd = GetNextPosition(sourceItem, obstacle);

                //if (nextEnd != null)
                {
                    //var obstacles = tree.Nearest(nextEnd.Point, nextEnd.Radius);
                    var obstacles = tree.Search(obstacle.Envelope);

                    foreach (var obs in obstacles)
                    {
                        WalkObstacle(obstacle, obs, items);
                    }
                }
            }
        }

        //CircleClass GetNextPosition(SignalTreeNodeItem sourceNode, SignalTreeNodeItem signalNode)
        //{
        //    //via, track, poly, pad
        //    switch (signalNode.CanvasItem)
        //    {
        //        case ViaCanvasItem via:
        //            return new CircleClass
        //            {
        //                Point = new PointSpatial(via.X, via.Y),
        //                Radius = via.Radius
        //            };

        //        case TrackBoardCanvasItem track:
        //            var sourceRect = sourceNode.Rectangle;

        //            var startPoint = PointSpatial.FromPoint(track.StartPoint);
        //            var endPoint = PointSpatial.FromPoint(track.EndPoint);
        //            var radius = track.Width * 0.5;

        //            //need the point that is further away
        //            var pos = startPoint;
        //            if (sourceRect.distance(endPoint) > sourceRect.distance(startPoint))
        //                pos = endPoint;

        //            return new CircleClass
        //            {
        //                Point = pos,
        //                Radius = radius
        //            };

        //            //case PadBaseCanvasItem pad:
        //            //    return new CircleClass
        //            //    {
        //            //        pad.x
        //            //    }
        //    }

        //    return null;
        //}

        //class CircleClass
        //{
        //    public PointSpatial Point { get; set; }

        //    public double Radius { get; set; }
        //}

        class BranchClass
        {
            /// <summary>
            /// pads that completely routes to one another. Contains at least one pad
            /// </summary>
            public List<SignalTreeNodeItem> Pads { get; set; } = new List<SignalTreeNodeItem>();

            /// <summary>
            /// items that creates the links in the pads
            /// </summary>
            public List<SignalTreeNodeItem> Items { get; set; } = new List<SignalTreeNodeItem>();
        }

        private void BuildBranches(List<ItemBranchPair> branchPairs)
        {
            for (int i = 0; i < branchPairs.Count - 1; i++)
            {
                var thisBP = branchPairs[i];
                if (thisBP.Branch == null)
                    thisBP.Branch = new BoardNetBranch();
                if (!Branches.Contains(thisBP.Branch))
                    Branches.Add(thisBP.Branch);

                thisBP.Branch.AddItem(thisBP);

                var thisBPIsJointCandidate = thisBP.IsJointCandidate();
                var thisBPIsLinkCandidate = thisBP.IsLinkCandidate();

                if (thisBPIsJointCandidate)
                    thisBP.CreateJoint();
                if (thisBPIsLinkCandidate)
                    thisBP.CreateLink();

                //an item can be both joint and link at the same time
                //thisBP is joint

                //thisBP is link

                for (int j = i + 1; j < branchPairs.Count; j++)
                {
                    var otherBP = branchPairs[j];
                    var otherBPIsJointCandidate = otherBP.IsJointCandidate();
                    var otherBPIsLinkCandidate = otherBP.IsLinkCandidate();

                    if (otherBPIsJointCandidate)
                        otherBP.CreateJoint();
                    if (otherBPIsLinkCandidate)
                        otherBP.CreateLink();

                    if (BelongsToSameLayer(thisBP, otherBP)
                        && GeometryOutline.Intersects(thisBP.ItemGeometry, otherBP.ItemGeometry))
                    {
                        //items are on the same branch

                        //todo: we need to move all items from the same branch under the same branch

                        //remove the old branch?
                        otherBP.Branch?.RemoveItem(otherBP);
                        otherBP.Branch = thisBP.Branch;
                        otherBP.Branch.AddItem(thisBP);

                        if (thisBPIsJointCandidate && otherBPIsLinkCandidate)
                        {
                            //create other as link to this
                            thisBP.AddLinkToJoint(otherBP);
                        }
                        if (thisBPIsLinkCandidate && otherBPIsJointCandidate)
                        {
                            //create this as link to other
                            otherBP.AddLinkToJoint(thisBP);
                        }
                        if (thisBPIsLinkCandidate && otherBPIsLinkCandidate)
                        {
                            //both links intersect
                            thisBP.AddLinkToLink(otherBP);
                        }
                        if (thisBPIsJointCandidate && thisBPIsLinkCandidate)
                        {
                            //link with itself (applies for vias)
                            thisBP.AddLinkToJoint(thisBP);
                        }
                        if (otherBPIsJointCandidate && otherBPIsLinkCandidate)
                        {
                            //link with itself (applies for vias)
                            otherBP.AddLinkToJoint(otherBP);
                        }
                    }
                    else
                    {
                        //items don't intersect, they are on different branches
                        if (otherBP.Branch == null)
                            otherBP.Branch = new BoardNetBranch();
                        if (!Branches.Contains(otherBP.Branch))
                            Branches.Add(otherBP.Branch);

                        otherBP.Branch.AddItem(otherBP);
                    }
                }
            }
        }

        void BuildLooseJoints()
        {

        }
        void BuildLines(bool showUnconnected)
        {
            dispatcher.RunOnDispatcher(() =>
            {
                Lines.Clear();
            });

            if (showUnconnected)
            {
                //if we have a single branch, then all is connected
                if (branchConnections.Count <= 1 || net.Pads.Count <= 1)
                    return;

                var tempLines = new List<BoardConnectionLine>();
                var points = new List<XPoint>();

                for (int i = 0; i < net.Pads.Count - 1; i++)
                {
                    var thisPad = net.Pads[i];

                    for (int j = i + 1; j < net.Pads.Count; j++)
                    {
                        var otherPad = net.Pads[j];

                        if (!PadsConnect(thisPad, otherPad))
                        {
                            var connectionLine = CreateConnectionLine(thisPad, otherPad);

                            if (connectionLine != null)
                                //tempLines.Add(connectionLine);
                                points.AddRange(connectionLine);
                        }
                    }
                }

                points = points.Distinct().ToList();

                for (int i = 0; i < points.Count - 1; i++)
                {
                    var startPoint = points[i];
                    var endPoint = new XPoint();
                    var distSq = double.MaxValue;
                    for (int j = i + 1; j < points.Count; j++)
                    {
                        var ep = points[j];
                        var dSq = (startPoint - ep).LengthSquared;
                        if (dSq < distSq)
                        {
                            distSq = dSq;
                            endPoint = ep;
                        }
                    }

                    var airWire = CreateConnectionLine(startPoint, endPoint);
                    tempLines.Add(airWire);
                }

                dispatcher.RunOnDispatcher(() =>
                {
                    Lines.AddRange(tempLines);
                });
            }
        }

        bool PadsConnect(IPadCanvasItem pad1, IPadCanvasItem pad2)
        {
            //contains both pads
            return branchConnections.Any(b => b.Pads.Any(p => p.CanvasItem == pad1)
                                           && b.Pads.Any(p => p.CanvasItem == pad2));
        }

        XPoint[] CreateConnectionLine(IPadCanvasItem pad1, IPadCanvasItem pad2)
        {
            var pad1Branches = branchConnections.Where(b => b.Pads.Any(p => p.CanvasItem == pad1));

            var pad2Branches = branchConnections.Where(b => b.Pads.Any(p => p.CanvasItem == pad2));

            var startPoints = new List<XPoint>();
            var endPoints = new List<XPoint>();

            //points that will start from pad1
            foreach (var pad1Branch in pad1Branches)
            {
                startPoints.AddRange(GetPoints(pad1Branch));
            }

            //points that will end to pad2
            foreach (var pad2Branch in pad2Branches)
            {
                endPoints.AddRange(GetPoints(pad2Branch));
            }

            if (startPoints.Count > 0 && endPoints.Count > 0)
            {
                var startPoint = new XPoint();
                var endPoint = new XPoint();
                var distSq = double.MaxValue;
                foreach (var sp in startPoints)
                {
                    foreach (var ep in endPoints)
                    {
                        var dSq = (sp - ep).LengthSquared;
                        if (dSq < distSq)
                        {
                            distSq = dSq;
                            startPoint = sp;
                            endPoint = ep;
                        }
                    }
                }

                return new[] { startPoint, endPoint };
            }

            return null;
        }

        BoardConnectionLine CreateConnectionLine(XPoint startPoint, XPoint endPoint)
        {
            return new BoardConnectionLine
            {
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endPoint.X,
                Y2 = endPoint.Y,
                Width = 0.05,
                LineColor = XColors.LightYellow,
                Net = net
            };
        }

        bool BelongsToSameLayer(ItemBranchPair thisBP, ItemBranchPair otherBP)
        {
            if (thisBP.Item is SingleLayerBoardCanvasItem thisLayerItem && otherBP.Item is SingleLayerBoardCanvasItem otherLayerItem)
            {
                return thisLayerItem.LayerId == otherLayerItem.LayerId;
                //todo: for via
            }
            else
            {
                if (thisBP.Item is SingleLayerBoardCanvasItem thisLayerItem1 && otherBP.Item is IPadCanvasItem otherPad)
                {
                    var padPlacement = FootprintPlacement.Top;
                    if (otherPad.ParentObject is IFootprintBoardCanvasItem fp)
                        padPlacement = fp.Placement;
                    switch (padPlacement)
                    {
                        case FootprintPlacement.Top:
                            return thisLayerItem1.LayerId == LayerConstants.SignalTopLayerId;
                        case FootprintPlacement.Bottom:
                            return thisLayerItem1.LayerId == LayerConstants.SignalBottomLayerId;
                    }
                }
                else if (thisBP.Item is IPadCanvasItem thisPad && otherBP.Item is SingleLayerBoardCanvasItem otherLayerItem2)
                {
                    var padPlacement = FootprintPlacement.Top;
                    if (thisPad.ParentObject is IFootprintBoardCanvasItem fp)
                        padPlacement = fp.Placement;
                    switch (padPlacement)
                    {
                        case FootprintPlacement.Top:
                            return otherLayerItem2.LayerId == LayerConstants.SignalTopLayerId;
                        case FootprintPlacement.Bottom:
                            return otherLayerItem2.LayerId == LayerConstants.SignalBottomLayerId;
                    }
                }
            }

            return true;
        }

        List<XPoint> GetPoints(BoardNetBranch partialBranch)
        {
            var listPoints = new List<XPoint>();
            if (partialBranch != null)
            {
                foreach (var joint in partialBranch.Joints)
                {
                    if (joint.Item != null)
                    {
                        switch (joint)
                        {
                            case PadJoint pad:
                                listPoints.Add(pad.Pad.GetAbsolutePoint());
                                break;
                            case LooseJoint looseJoint:
                                listPoints.Add(looseJoint.Location);
                                break;
                            case PlaneJoint plane:
                                break;
                            case PointJoint point:
                                listPoints.Add(point.Location);
                                break;
                            case ViaJoint viaJoint:
                                {
                                    listPoints.Add(viaJoint.Via.Center);
                                    break;
                                }
                            case PolyJoint poly:
                                {
                                    listPoints.Add(poly.Poly.MiddlePoint);
                                    break;
                                }
                        }
                    }
                }

                foreach (var linkTrack in partialBranch.Links.Where(l => l.Item is Track))
                {
                    var track = linkTrack.Item as Track;
                    listPoints.Add(track.StartPoint);
                    listPoints.Add(track.EndPoint);
                }
            }

            return listPoints;
        }

        List<XPoint> GetPoints(BranchClass partialBranch)
        {
            var listPoints = new List<XPoint>();
            if (partialBranch != null)
            {
                //pads
                foreach (var pad in partialBranch.Pads)
                {
                    var p = pad.CanvasItem as IPadCanvasItem;
                    listPoints.Add(p.GetAbsolutePoint());
                }

                //items
                foreach (var item in partialBranch.Items)
                {
                    switch (item.CanvasItem)
                    {
                        case ViaCanvasItem via:
                            listPoints.Add(via.Center);
                            break;

                        case PolygonBoardCanvasItem poly:
                            listPoints.Add(poly.MiddlePoint);
                            break;

                        case TrackBoardCanvasItem track:
                            listPoints.Add(track.StartPoint);
                            listPoints.Add(track.EndPoint);
                            break;
                    }
                }
            }

            return listPoints;
        }

        public override void Translate(double dx, double dy)
        {
        }

        public override XRect GetBoundingRectangle()
        {
            return XRect.Empty;
        }

        public override void MirrorX()
        {
        }

        public override void MirrorY()
        {
        }

        public override void Rotate()
        {
        }
    }
}
