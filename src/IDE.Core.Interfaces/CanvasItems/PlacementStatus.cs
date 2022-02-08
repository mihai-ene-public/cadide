namespace IDE.Core.Interfaces
{
    public enum PlacementStatus
    {
        /// <summary>
        /// Status value when we don't have a placement object
        /// </summary>
        None,

        /// <summary>
        /// We have a placement object ready to be placed, but wasn't placed yet
        /// </summary>
        Ready,

        /// <summary>
        /// Placement of object started
        /// </summary>
        Started,

        /// <summary>
        /// Placement of object finished
        /// </summary>
        Finished
    }
}
