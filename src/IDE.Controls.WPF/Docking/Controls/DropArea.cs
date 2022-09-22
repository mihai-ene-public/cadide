using System.Windows;

namespace IDE.Controls.WPF.Docking.Controls;

public class DropArea<T> : IDropArea where T : FrameworkElement
{
    #region Members

    private Rect _detectionRect;
    private DropAreaType _type;
    private T _element;

    #endregion

    #region Constructors

    internal DropArea(T areaElement, DropAreaType type)
    {
        _element = areaElement;
        _detectionRect = areaElement.GetScreenArea();
        _type = type;
    }

    #endregion

    #region Properties

    public Rect DetectionRect
    {
        get
        {
            return _detectionRect;
        }
    }

    public DropAreaType Type
    {
        get
        {
            return _type;
        }
    }

    public T AreaElement
    {
        get
        {
            return _element;
        }
    }

    #endregion
}
