namespace IDE.Core.Presentation.PlacementRouting
{
    public enum TracePlacementMode
    {
        /// <summary>
        /// a single, direct wire
        /// </summary>
        Single = 0,

        /// <summary>
        /// 2 wires: Diagonal (45 deg) then Direct
        /// </summary>
        DiagonalDirect,

        /// <summary>
        /// 2 wires: Direct then Diagonal (45 deg)
        /// </summary>
        DirectDiagonal,

        Count
    }
}
