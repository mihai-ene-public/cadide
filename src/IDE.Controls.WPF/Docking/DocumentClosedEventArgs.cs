using System;
using IDE.Controls.WPF.Docking.Layout;

namespace IDE.Controls.WPF.Docking;

public class DocumentClosedEventArgs : EventArgs
{
    public DocumentClosedEventArgs(LayoutDocument document)
    {
        Document = document;
    }

    public LayoutDocument Document
    {
        get;
        private set;
    }
}

