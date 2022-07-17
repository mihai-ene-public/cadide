namespace IDE.Core.Presentation.Placement
{
    public interface IPlacementToolFactory
    {
        PlacementTool Create(Type canvasItemType);
    }
}
