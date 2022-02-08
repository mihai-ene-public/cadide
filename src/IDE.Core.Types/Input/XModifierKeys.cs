using System;

namespace IDE.Core.Types.Input
{
    [Flags]
    public enum XModifierKeys
    {
        /// <summary>
        ///    No modifiers are pressed.
        /// </summary>
        None = 0,

        /// <summary>
        ///    An alt key.
        /// </summary>
        Alt = 1,

        /// <summary>
        ///    A control key.
        /// </summary>
        Control = 2,

        /// <summary>
        ///    A shift key.
        /// </summary>
        Shift = 4,

        /// <summary>
        ///    A windows key.
        /// </summary>
        Windows = 8
    }
}
