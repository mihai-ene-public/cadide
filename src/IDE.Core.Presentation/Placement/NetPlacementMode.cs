namespace IDE.Core.Presentation.Placement
{
    public enum NetPlacementMode
    {
        /// <summary>
        /// a single, direct wire
        /// </summary>
        Single = 0,

        /// <summary>
        /// 2 wires: Horizontal then Vertical
        /// </summary>
        HorizontalVertical,

        /// <summary>
        /// 2 wires: Vertical then Horizontal
        /// </summary>
        VerticalHorizontal,

        Count
    }
}
