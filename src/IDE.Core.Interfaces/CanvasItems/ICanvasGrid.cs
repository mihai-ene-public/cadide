namespace IDE.Core.Interfaces;

public interface ICanvasGrid
{
    double MinorDistance { get; }
    double MajorDistance { get; }
    double GridSize { get; }
    bool CanSnapToGrid { get; }

    ICanvasGridUnit CurrentUnit { get; }

    void SetUnit(ICanvasGridUnit unit);
    void SetValue(double value);
}
