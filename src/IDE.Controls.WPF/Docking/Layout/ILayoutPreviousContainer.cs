namespace IDE.Controls.WPF.Docking.Layout;

interface ILayoutPreviousContainer
{
    ILayoutContainer PreviousContainer
    {
        get; set;
    }

    string PreviousContainerId
    {
        get; set;
    }
}
