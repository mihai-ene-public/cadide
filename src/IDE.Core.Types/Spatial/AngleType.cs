namespace IDE.Core.Types.Media
{
    public enum AngleType
    {
        /// <summary>
        /// Between 90 and 180 deg
        /// </summary>
        OBTUSE = 0x01,

        /// <summary>
        /// 90 deg
        /// </summary>
        RIGHT = 0x02,

        /// <summary>
        /// less than 90 deg
        /// </summary>
        ACUTE = 0x04,

        /// <summary>
        /// 0 deg
        /// </summary>
        STRAIGHT = 0x08,

        /// <summary>
        /// 180 deg
        /// </summary>
        HALF_FULL = 0x10,

        UNDEFINED = 0x20
    }
}
