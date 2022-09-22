namespace IDE.Controls.WPF.Docking.Layout;

public enum ChildrenTreeChange
{
    /// <summary>
    /// Direct insert/remove operation has been perfomed to the group
    /// </summary>
    DirectChildrenChanged,

    /// <summary>
    /// An element below in the hierarchy as been added/removed
    /// </summary>
    TreeChanged
}
