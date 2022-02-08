namespace IDE.Core.Designers
{
    /// <summary>
    /// represents a neighbor for a Joint. It is a link that goes to another joint
    /// </summary>
    public class JointLinkPair
    {
        public Joint Joint { get; set; }
        public JointLink Link { get; set; }
    }
}
