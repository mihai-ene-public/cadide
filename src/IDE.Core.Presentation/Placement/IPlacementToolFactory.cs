using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Placement;

public interface IPlacementToolFactory
{
    IPlacementTool Create(Type canvasItemType);
    IPlacementTool Create(IToolboxItem toolboxItem);
}
