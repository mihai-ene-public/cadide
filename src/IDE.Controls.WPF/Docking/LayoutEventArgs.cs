using System;
using IDE.Controls.WPF.Docking.Layout;

namespace IDE.Controls.WPF.Docking;

class LayoutEventArgs : EventArgs
{
    public LayoutEventArgs(LayoutRoot layoutRoot)
    {
        LayoutRoot = layoutRoot;
    }

    public LayoutRoot LayoutRoot
    {
        get;
        private set;
    }
}

