using IDE.Core.Types.Media;


namespace IDE.Core.Interfaces
{
    public interface ICircleSchematicCanvasItem : ICircleCanvasItem
    {
        XColor BorderColor { get; set; }

        XColor FillColor { get; set; }
    }
}
