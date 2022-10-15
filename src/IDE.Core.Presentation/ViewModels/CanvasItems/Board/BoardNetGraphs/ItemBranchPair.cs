using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;

namespace IDE.Core.Designers
{
    public class ItemBranchPair
    {
        public ISignalPrimitiveCanvasItem Item { get; set; }

        IGeometryOutline itemGeometry;
        /// <summary>
        /// geometry of the item; must have the transform as is placed on board
        /// </summary>
        public IGeometryOutline ItemGeometry
        {
            get
            {
                if (itemGeometry == null)
                {
                    var GeometryHelper = ServiceProvider.Resolve<IGeometryOutlineHelper>();
                    itemGeometry = GeometryHelper.GetGeometry(Item, applyTransform: true);
                }


                return itemGeometry;
            }
        }


        /// <summary>
        /// the Branch this item belongs to
        /// </summary>
        public BoardNetBranch Branch { get; set; }

        /// <summary>
        /// the Joint link this item belongs to
        /// </summary>
        public JointLink Link { get; set; }

        /// <summary>
        /// the Joint this item belongs to
        /// </summary>
        public Joint Joint { get; set; }

        public bool IsJointCandidate()
        {
            //Poly, Via, Pad, Point, Plane

            //todo: !!!it is not conclusive if we have a PointJoint (intersection of tracks) or a LooseJoint - a track that ends nowhere
            //or a PlaneJoint
            return Item != null
                && (Item is PolygonBoardCanvasItem
                || Item is ViaCanvasItem
                || Item is IPadCanvasItem
                || Item is IPlaneBoardCanvasItem);
        }

        public void CreateJoint()
        {
            if (Joint == null)
            {
                if (Item is PolygonBoardCanvasItem)
                    Joint = new PolyJoint
                    {
                        Item = Item
                    };
                else if (Item is ViaCanvasItem)
                    Joint = new ViaJoint
                    {
                        Item = Item
                    };
                else if (Item is IPadCanvasItem)
                    Joint = new PadJoint
                    {
                        Item = Item
                    };
                else if (Item is IPlaneBoardCanvasItem)
                    Joint = new PlaneJoint
                    {
                        Item = Item
                    };
            }

            if (Branch != null && Joint != null && !Branch.Joints.Contains(Joint))
            {
                Branch.Joints.Add(Joint);
            }
        }

        public bool IsLinkCandidate()
        {
            //Poly | Via | Track | Plane
            return Item is PolygonBoardCanvasItem
                || Item is ViaCanvasItem
                || Item is TrackBoardCanvasItem
                || Item is IPlaneBoardCanvasItem;
        }

        public void CreateLink()
        {
            if (Link == null)
                Link = new JointLink();
            if (Item is TrackBoardCanvasItem)
            {
                var track = Link.Item as Track;
                if (track == null)
                {
                    track = new Track();
                    Link.Item = track;
                }

                track.AddSegment(Item as TrackBoardCanvasItem);
            }
            else
            {
                Link.Item = Item;
            }

            if (Branch != null && Link != null && !Branch.Links.Contains(Link))
            {
                Branch.Links.Add(Link);
            }
        }

        public void AddLinkToJoint(ItemBranchPair linkPair)
        {
            Joint.Neighbors.Add(new JointLinkPair
            {
                Link = linkPair.Link,
                Joint = linkPair.Joint
            });
        }

        public void AddLinkToLink(ItemBranchPair linkPair)
        {
            if (Link == null || linkPair.Link == null
             || Item == null || linkPair.Item == null)
                return;

            //for now, we're interested only for trace to trace
            if (Item is TrackBoardCanvasItem && linkPair.Item is TrackBoardCanvasItem)
            {
                var thisTrack = Link.Item as Track;
                var otherTrack = linkPair.Link.Item as Track;

                //we need to check where these tracks intersect
                //if one continues the other, we merge them
                //else, if one splits the other, we must split them, register separate links and create a PointJoint

                //test if they continue to one another
                if (thisTrack.TrackContinuesWith(otherTrack))
                {
                    thisTrack.MergeWith(otherTrack);
                }
                else
                {

                    if (thisTrack.TrackCreatesSplitWith(otherTrack, out TrackBoardCanvasItem outSeg))
                    {
                        var splitTracks = thisTrack.Split(outSeg);
                    }


                }
            }
        }

        public override bool Equals(object obj)
        {
            var otherPair = obj as ItemBranchPair;
            if (otherPair != null)
            {
                return otherPair.Item == Item;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Item.ToString();
        }
    }
}
