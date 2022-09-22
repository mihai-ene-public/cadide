using System.Collections.Generic;

namespace IDE.Controls.WPF.Docking.Layout;

public interface ILayoutContainer : ILayoutElement
{
    #region Properties

    IEnumerable<ILayoutElement> Children
    {
        get;
    }

    int ChildrenCount
    {
        get;
    }

    #endregion

    #region Methods

    void RemoveChild(ILayoutElement element);

    void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement);

    #endregion
}
