using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IDE.Controls.WPF.Docking.Controls;
public class AnchorablePaneControlOverlayArea : OverlayArea
{
    #region Members

    private LayoutAnchorablePaneControl _anchorablePaneControl;

    #endregion

    #region constructors

    internal AnchorablePaneControlOverlayArea(
        IOverlayWindow overlayWindow,
        LayoutAnchorablePaneControl anchorablePaneControl)
        : base(overlayWindow)
    {

        _anchorablePaneControl = anchorablePaneControl;
        base.SetScreenDetectionArea(new Rect(
            _anchorablePaneControl.PointToScreenDPI(new Point()),
            _anchorablePaneControl.TransformActualSizeToAncestor()));

    }

    #endregion
}
