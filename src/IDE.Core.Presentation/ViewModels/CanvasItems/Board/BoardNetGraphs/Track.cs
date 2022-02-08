using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.Designers
{
    /// <summary>
    /// represents a continous link of trace items that are on the same layer, and between 2 joints
    /// </summary>
    public class Track : LinkedList<TrackBoardCanvasItem>
    {


        int LayerId { get; set; }

        public XPoint StartPoint
        {
            get
            {
                if (Count == 0)
                    return new XPoint();
                else if (Count == 1)
                    return First.Value.StartPoint;
                else
                {
                    //count > 1
                    var firstSeg = First.Value;
                    var nextSeg = First.Next.Value;

                    //if first seg start point continues with the next, our track starts at the 1st.EndPoint
                    if (firstSeg.StartPoint == nextSeg.StartPoint
                     || firstSeg.StartPoint == nextSeg.EndPoint)
                        return firstSeg.EndPoint;

                    return firstSeg.StartPoint;
                }
            }
        }

        public XPoint EndPoint
        {
            get
            {
                if (Count == 0)
                    return new XPoint();
                else if (Count == 1)
                    return First.Value.EndPoint;
                else
                {//count > 1
                    var lastSeg = Last.Value;
                    var prevSeg = Last.Previous.Value;

                    if (lastSeg.EndPoint == prevSeg.StartPoint
                     || lastSeg.EndPoint == prevSeg.EndPoint)
                        return lastSeg.StartPoint;

                    return lastSeg.EndPoint;
                }
            }
        }



        public void AddSegment(TrackBoardCanvasItem segment)
        {
            if (Count == 0)
            {
                AddFirst(segment);
                LayerId = segment.LayerId;
            }
            else
            {
                if (LayerId == segment.LayerId)
                {
                    //check if we are at the start or the end of the track
                    //check if there are inner connections
                    var startSeg = First.Value;
                    var endSeg = Last.Value;
                    var collidesWithFirst = SegmentContinuesWith(startSeg, segment);

                    if (Count > 1)
                    {
                        var collidesWithLast = SegmentContinuesWith(endSeg, segment);
                        var isJoint = SegmentCreatesJoint(segment, out TrackBoardCanvasItem hitSeg);

                        if (collidesWithLast && !isJoint)
                            AddLast(segment);
                    }
                }
            }
        }

        /// <summary>
        /// splits current track in 2 other tracks, second track is starting at the specified segment
        /// </summary>
        /// <param name="segment"></param>
        /// <returns>returns the new 2 tracks</returns>
        public Track[] Split(TrackBoardCanvasItem segment)
        {
            //we could also keep the current track until the segment, and return the rest of the track starting from segment

            var tracks = new Track[2];
            tracks[0] = new Track();
            tracks[1] = new Track();

            var track2Started = false;

            foreach (var seg in this)
            {
                if (segment == seg)
                    track2Started = true;

                if (track2Started)
                {
                    tracks[1].AddLast(segment);
                }
                else
                {
                    tracks[0].AddLast(segment);
                }
            }

            return tracks;
        }

        //returns the new track we could merge it into this
        public void MergeWith(Track otherTrack)
        {
            //var t = new Track();

            //foreach (var s in this)
            //    t.AddLast(s);

            ////fixme: we need to check where the 2 tracks continue to one another
            //foreach (var s in otherTrack)
            //    AddLast(s);

            //return t;

            var thisSp = StartPoint;
            var thisEp = EndPoint;

            var otherSp = otherTrack.StartPoint;
            var otherEp = otherTrack.EndPoint;

            if (thisEp == otherSp)
            {
                foreach (var s in otherTrack)
                    AddLast(s);
            }
            else if (thisEp == otherEp)
            {
                foreach (var s in otherTrack.Reverse())
                    AddLast(s);
            }
            else if (thisSp == otherSp)
            {
                foreach (var s in otherTrack)
                    AddFirst(s);
            }
            else if (thisSp == otherEp)
            {
                foreach (var s in otherTrack.Reverse())
                    AddFirst(s);
            }

            otherTrack.Clear();
            //we should also remove the other track from the branch
        }

        bool SegmentCreatesJoint(TrackBoardCanvasItem segment, out TrackBoardCanvasItem outSeg)
        {
            outSeg = null;
            if (Count > 1)
            {
                var hitCount = 0;
                foreach (var seg in this)
                {
                    if (SegmentContinuesWith(seg, segment))
                    {
                        outSeg = seg;
                        hitCount++;
                        if (hitCount >= 2)
                            break;
                    }
                }

                return hitCount >= 2;
            }

            return false;
        }

        bool SegmentContinuesWith(TrackBoardCanvasItem segment1, TrackBoardCanvasItem segment2)
        {
            if (segment1.Points.Count == 0 || segment2.Points.Count == 0)
                return false;

            return segment1.StartPoint == segment2.StartPoint
                || segment1.StartPoint == segment2.EndPoint
                || segment1.EndPoint == segment2.StartPoint
                || segment1.EndPoint == segment2.EndPoint;
        }

        public bool TrackContinuesWith(Track otherTrack)
        {
            var startPoint = StartPoint;
            var endPoint = EndPoint;
            var otherSP = otherTrack.StartPoint;
            var otherEP = otherTrack.EndPoint;

            return startPoint == otherSP
                || startPoint == otherEP
                || endPoint == otherSP
                || endPoint == otherEP;
        }

        public bool TrackCreatesSplitWith(Track otherTrack, out TrackBoardCanvasItem outSeg)
        {
            outSeg = null;
            if (Count > 1)
            {
                var otherSp = otherTrack.StartPoint;
                var otherEp = otherTrack.EndPoint;

                var hitCount = 0;
                foreach (var seg in this)
                {
                    if (PointContinuesAt(seg.StartPoint, otherSp, otherEp)
                     || PointContinuesAt(seg.EndPoint, otherSp, otherEp))
                    {
                        outSeg = seg;
                        hitCount++;
                        if (hitCount >= 2)
                            break;
                    }
                }

                return hitCount >= 2;
            }

            return false;
        }

        bool PointContinuesAt(XPoint thisPoint, XPoint startPoint, XPoint endPoint)
        {
            return thisPoint == startPoint
               || thisPoint == endPoint;

        }
    }
}
