using IDE.Core.Types.Media;


namespace IDE.Core.Interfaces
{
    public interface IEllipseSchematicCanvasItem : IEllipseCanvasItem
    {
        XColor BorderColor { get; set; }

        XColor FillColor { get; set; }
    }
}
