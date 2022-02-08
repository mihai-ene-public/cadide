using IDE.Core.Types.Media;


namespace IDE.Core.Interfaces
{
    public interface ILineSchematicCanvasItem : ILineCanvasItem
    {
        XColor LineColor { get; set; }

        LineStyle LineStyle { get; set; }
    }
}
