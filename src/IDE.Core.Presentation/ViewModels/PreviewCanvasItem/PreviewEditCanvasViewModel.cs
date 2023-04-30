using System.ComponentModel;
using IDE.Core.Designers;
using IDE.Core.Interfaces;

namespace IDE.Documents.Views;

public abstract class PreviewEditCanvasViewModel:PreviewCanvasViewModel, IPreviewEditCanvasViewModel
{
    public PreviewEditCanvasViewModel(IDirtyMarkerTypePropertiesMapper dirtyMarkerTypePropertiesMapper)
    {
        _dirtyMarkerTypePropertiesMapper = dirtyMarkerTypePropertiesMapper;
    }

    private readonly IDirtyMarkerTypePropertiesMapper _dirtyMarkerTypePropertiesMapper;

    public IPlacementTool PlacementTool { get; set; }

    public void RemoveItem(ISelectableItem item)
    {
        item.RemoveFromCanvas();

        items.Remove(item);

        item.PropertyChanged -= DesignerItemBaseViewModel_PropertyChanged;

    }

    public void CancelPlacement()
    {
        PlacementTool?.CancelPlacement();

        PlacementTool = null;
    }
    public bool IsPlacingItem()
    {
        return PlacementTool?.CanvasItem != null;
    }

    public void AddItem(ISelectableItem item)
    {
        item.PropertyChanged += DesignerItemBaseViewModel_PropertyChanged;

        if (item is SingleLayerBoardCanvasItem)
            return;
        items.Add(item);
    }

    public void AddItems(IEnumerable<ISelectableItem> addItems)
    {
        foreach (var item in addItems)
        {
            AddItem(item);
        }
    }
    public void RemoveItems(IEnumerable<ISelectableItem> removeItems)
    {
        foreach (var item in removeItems)
        {
            RemoveItem(item);
        }
    }

    void DesignerItemBaseViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var thisItem = sender as ISelectableItem;
        if (thisItem == null)
            return;

        var propertyNames = _dirtyMarkerTypePropertiesMapper.GetPropertyNames(sender);
        if (!propertyNames.Contains(e.PropertyName))
        {
            return;
        }

        if (thisItem.IsPlaced)
            OnDrawingChanged(sender, DrawingChangedReason.ItemModified);
    }

    public void OnDrawingChanged(DrawingChangedReason reason)
    {
        OnDrawingChanged(null, reason);
    }
    protected void OnDrawingChanged(object sender, DrawingChangedReason reason)
    {
       // _drawingChangedDebouncer.Debounce(300, p => OnDrawingChangedVirtual(sender, reason));
    }
}