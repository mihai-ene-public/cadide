using System.Windows;

namespace IDE.Controls.WPF.Docking.Controls;

public interface IDropArea
{
    Rect DetectionRect
    {
        get;
    }
    DropAreaType Type
    {
        get;
    }
}
