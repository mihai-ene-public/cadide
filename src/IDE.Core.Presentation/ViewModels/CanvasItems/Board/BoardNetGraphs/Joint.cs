using System.Collections.Generic;

namespace IDE.Core.Designers
{
    public abstract class Joint
    {
        /// <summary>
        /// Poly, Via, Pad, Point, Plane
        /// </summary>
        public object Item { get; set; }

        public List<JointLinkPair> Neighbors { get; set; } = new List<JointLinkPair>();//joint link

        /// <summary>
        /// the branch this joint belongs to
        /// </summary>
        public BoardNetBranch Branch { get; set; }

        public override string ToString()
        {
            return GetType().ToString();
        }

    }
}
