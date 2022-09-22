using System.Windows;

namespace IDE.Controls.WPF.Docking.Controls;

interface IOverlayWindowDropTarget
{
    Rect ScreenDetectionArea
    {
        get;
    }

    OverlayWindowDropTargetType Type
    {
        get;
    }
}
