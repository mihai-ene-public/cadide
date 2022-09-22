using System.Windows;

namespace IDE.Controls.WPF.Docking.Controls;

internal interface IOverlayWindowArea
{
    Rect ScreenDetectionArea
    {
        get;
    }
}
