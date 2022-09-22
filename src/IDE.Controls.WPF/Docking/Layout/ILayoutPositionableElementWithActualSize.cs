namespace IDE.Controls.WPF.Docking.Layout;

internal interface ILayoutPositionableElementWithActualSize : ILayoutPositionableElement
{
    double ActualWidth
    {
        get; set;
    }
    double ActualHeight
    {
        get; set;
    }
}
