using System;

namespace IDE.Controls.WPF.Docking.Controls;

internal class FocusChangeEventArgs : EventArgs
{
    #region Constructors

    public FocusChangeEventArgs(IntPtr gotFocusWinHandle, IntPtr lostFocusWinHandle)
    {
        GotFocusWinHandle = gotFocusWinHandle;
        LostFocusWinHandle = lostFocusWinHandle;
    }

    #endregion

    #region Properties

    public IntPtr GotFocusWinHandle
    {
        get;
        private set;
    }
    public IntPtr LostFocusWinHandle
    {
        get;
        private set;
    }

    #endregion
}
