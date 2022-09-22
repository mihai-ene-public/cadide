using System.ComponentModel;
using IDE.Controls.WPF.Docking.Layout;

namespace IDE.Controls.WPF.Docking;

public class DocumentClosingEventArgs : CancelEventArgs
{
    public DocumentClosingEventArgs(LayoutDocument document)
    {
        Document = document;
    }

    public LayoutDocument Document
    {
        get;
        private set;
    }
}

