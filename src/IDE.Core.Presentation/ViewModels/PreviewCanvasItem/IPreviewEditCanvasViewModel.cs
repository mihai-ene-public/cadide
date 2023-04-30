using IDE.Core.Interfaces;

namespace IDE.Documents.Views;

public interface IPreviewEditCanvasViewModel : IPreviewCanvasViewModel
{
    IPlacementTool PlacementTool { get; set; }
    void OnDrawingChanged(DrawingChangedReason reason);

    void AddItem(ISelectableItem item);
    void AddItems(IEnumerable<ISelectableItem> addItems);

    void RemoveItems(IEnumerable<ISelectableItem> removeItems);
    void RemoveItem(ISelectableItem item);

    bool IsPlacingItem();
    void CancelPlacement();
}
