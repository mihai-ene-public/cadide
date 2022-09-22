using System;

namespace IDE.Controls.WPF.Docking.Controls;

internal class WindowActivateEventArgs : EventArgs
{
    #region Constructors

    public WindowActivateEventArgs(IntPtr hwndActivating)
    {
        HwndActivating = hwndActivating;
    }

    #endregion

    #region Properties

    public IntPtr HwndActivating
    {
        get;
        private set;
    }

    #endregion
}
