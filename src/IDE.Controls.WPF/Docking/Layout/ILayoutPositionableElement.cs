using System.Windows;

namespace IDE.Controls.WPF.Docking.Layout;

internal interface ILayoutPositionableElement : ILayoutElement, ILayoutElementForFloatingWindow
{
    GridLength DockWidth
    {
        get;
        set;
    }

    double FixedDockWidth { get; }

    double ResizableAbsoluteDockWidth
    {
        get;
        set;
    }

    GridLength DockHeight
    {
        get;
        set;
    }

    double FixedDockHeight { get; }

    double ResizableAbsoluteDockHeight
    {
        get;
        set;
    }

    double CalculatedDockMinWidth();

    double DockMinWidth
    {
        get; set;
    }

    double CalculatedDockMinHeight();

    double DockMinHeight
    {
        get; set;
    }

    bool AllowDuplicateContent
    {
        get; set;
    }

    bool IsVisible
    {
        get;
    }
}
