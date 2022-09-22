using System;

namespace IDE.Controls.WPF.Docking.Layout;

public class ChildrenTreeChangedEventArgs : EventArgs
{
    public ChildrenTreeChangedEventArgs(ChildrenTreeChange change)
    {
        Change = change;
    }

    public ChildrenTreeChange Change
    {
        get; private set;
    }
}
