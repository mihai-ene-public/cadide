using System;
using System.Xml.Serialization;

namespace IDE.Controls.WPF.Docking.Layout;

[Serializable]
public abstract class LayoutGroupBase : LayoutElement
{
    #region Internal Methods

    protected virtual void OnChildrenCollectionChanged()
    {
        if (ChildrenCollectionChanged != null)
            ChildrenCollectionChanged(this, EventArgs.Empty);
    }

    protected void NotifyChildrenTreeChanged(ChildrenTreeChange change)
    {
        OnChildrenTreeChanged(change);
        var parentGroup = Parent as LayoutGroupBase;
        if (parentGroup != null)
            parentGroup.NotifyChildrenTreeChanged(ChildrenTreeChange.TreeChanged);
    }

    protected virtual void OnChildrenTreeChanged(ChildrenTreeChange change)
    {
        if (ChildrenTreeChanged != null)
            ChildrenTreeChanged(this, new ChildrenTreeChangedEventArgs(change));
    }

    #endregion

    #region Events

    [field: NonSerialized]
    [field: XmlIgnore]
    public event EventHandler ChildrenCollectionChanged;

    [field: NonSerialized]
    [field: XmlIgnore]
    public event EventHandler<ChildrenTreeChangedEventArgs> ChildrenTreeChanged;

    #endregion
}
