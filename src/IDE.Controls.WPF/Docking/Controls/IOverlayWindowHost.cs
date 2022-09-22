using System.Collections.Generic;
using System.Windows;

namespace IDE.Controls.WPF.Docking.Controls;

internal interface IOverlayWindowHost
{
    #region Properties

    DockingManager Manager
    {
        get;
    }

    #endregion

    #region Methods

    bool HitTest(Point dragPoint);

    IOverlayWindow ShowOverlayWindow(LayoutFloatingWindowControl draggingWindow);

    void HideOverlayWindow();

    IEnumerable<IDropArea> GetDropAreas(LayoutFloatingWindowControl draggingWindow);

    #endregion
}
