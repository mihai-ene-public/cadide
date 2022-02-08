namespace IDE.Core.Designers
{
    public class JointLink
    {
        public Joint StartJoint { get; set; }

        public Joint EndJoint { get; set; }

        public BoardNetBranch Branch { get; set; }

        /// <summary>
        /// the item that links StartJoint and EndJoint (Poly | Via | Track | Plane)
        /// </summary>
        public object Item { get; set; }

        public override string ToString()
        {
            if (Item == null)
                return "None";
            return Item.ToString();
        }
    }
}
