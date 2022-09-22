using System.Windows;
using System.Windows.Controls;
using IDE.Controls.WPF.Docking.Layout;

namespace IDE.Controls.WPF.Docking.Controls;

public class LayoutAnchorablePaneGroupControl : LayoutGridControl<ILayoutAnchorablePane>, ILayoutControl
{
    #region Members

    private LayoutAnchorablePaneGroup _model;

    #endregion

    #region Constructors

    internal LayoutAnchorablePaneGroupControl(LayoutAnchorablePaneGroup model)
        : base(model, model.Orientation)
    {
        _model = model;
    }

    #endregion

    #region Overrides

    protected override void OnFixChildrenDockLengths()
    {
        #region Setup DockWidth/Height for children
        if (_model.Orientation == Orientation.Horizontal)
        {
            for (int i = 0; i < _model.Children.Count; i++)
            {
                var childModel = _model.Children[i] as ILayoutPositionableElement;
                if (!childModel.DockWidth.IsStar)
                {
                    childModel.DockWidth = new GridLength(1.0, GridUnitType.Star);
                }
            }
        }
        else
        {
            for (int i = 0; i < _model.Children.Count; i++)
            {
                var childModel = _model.Children[i] as ILayoutPositionableElement;
                if (!childModel.DockHeight.IsStar)
                {
                    childModel.DockHeight = new GridLength(1.0, GridUnitType.Star);
                }
            }
        }
        #endregion
    }

    #endregion
}
