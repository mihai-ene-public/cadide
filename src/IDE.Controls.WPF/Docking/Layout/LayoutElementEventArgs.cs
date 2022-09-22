using System;

namespace IDE.Controls.WPF.Docking.Layout;

public class LayoutElementEventArgs : EventArgs
{
    #region Constructors

    public LayoutElementEventArgs(LayoutElement element)
    {
        Element = element;
    }

    #endregion

    #region Properties

    public LayoutElement Element
    {
        get;
        private set;
    }

    #endregion
}
