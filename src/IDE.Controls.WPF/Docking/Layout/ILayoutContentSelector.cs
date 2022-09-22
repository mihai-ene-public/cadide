namespace IDE.Controls.WPF.Docking.Layout;

public interface ILayoutContentSelector
{
    #region Properties

    int SelectedContentIndex
    {
        get; set;
    }

    LayoutContent SelectedContent
    {
        get;
    }

    #endregion

    #region Methods

    int IndexOf(LayoutContent content);

    #endregion
}
