using System.Collections.Generic;

namespace IDE.Core.Designers
{
    /// <summary>
    /// A branch is a "node" in the graph that is disconnected from the other branches. Multiple branches will form the graph of the net
    /// </summary>
    public class BoardNetBranch
    {
        // public Joint StartJoint { get; set; }

        public List<Joint> Joints { get; set; } = new List<Joint>();

        public List<JointLink> Links { get; set; } = new List<JointLink>();

        public List<ItemBranchPair> Items { get; set; } = new List<ItemBranchPair>();

        public void AddItem(ItemBranchPair item)
        {
            if (Items.Contains(item))
                return;

            Items.Add(item);
        }

        public void RemoveItem(ItemBranchPair item)
        {
            Items.Remove(item);
        }


    }
}
