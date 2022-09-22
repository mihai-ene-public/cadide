using System.Windows;
using System.Windows.Media;
using IDE.Controls.WPF.Docking.Layout;

namespace IDE.Controls.WPF.Docking.Controls;

internal interface IDropTarget
{
    #region Properties

    DropTargetType Type
    {
        get;
    }

    #endregion

    #region Methods

    Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindow);

    bool HitTest(Point dragPoint);

    void Drop(LayoutFloatingWindow floatingWindow);

    void DragEnter();

    void DragLeave();

    #endregion
}
