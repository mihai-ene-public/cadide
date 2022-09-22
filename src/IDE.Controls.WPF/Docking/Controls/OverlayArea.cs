using System.Windows;

namespace IDE.Controls.WPF.Docking.Controls;

public abstract class OverlayArea : IOverlayWindowArea
{
    #region Members

    private IOverlayWindow _overlayWindow;
    private Rect? _screenDetectionArea;

    #endregion

    #region Constructors

    internal OverlayArea(IOverlayWindow overlayWindow)
    {
        _overlayWindow = overlayWindow;
    }

    #endregion

    #region Internal Methods

    protected void SetScreenDetectionArea(Rect rect)
    {
        _screenDetectionArea = rect;
    }

    #endregion

    #region IOverlayWindowArea

    Rect IOverlayWindowArea.ScreenDetectionArea
    {
        get
        {
            return _screenDetectionArea.Value;
        }
    }

    #endregion
}
