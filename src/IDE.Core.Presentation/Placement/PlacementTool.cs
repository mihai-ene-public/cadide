using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.ViewModels;
using System;
using System.Collections.Generic;

namespace IDE.Core.Presentation.Placement;

public abstract class PlacementTool : IPlacementTool
{

    public PlacementTool()
    {

    }

    /// <summary>
    /// canvasItem that we are currently placing
    /// </summary>
    protected ISelectableItem canvasItem;

    public ISelectableItem CanvasItem
    {
        get { return canvasItem; }
        set { canvasItem = value; }
    }

    /// <summary>
    /// in the case we show a dialog (addding a part in schematic) the selectedItem from the dialog is stored here
    /// <para>May be NULL</para>
    /// </summary>
    protected object dialogSelectedItem;

    public ICanvasDesignerFileViewModel CanvasModel { get; set; }

    public PlacementStatus PlacementStatus { get; set; }

    public virtual void PlacementMouseMove(XPoint mousePosition)
    {

    }

    public virtual void PlacementMouseUp(XPoint mousePosition)
    {

    }

    public virtual bool Show()
    {
        return true;
    }

    public virtual void ChangeMode()//TAB
    {

    }

    public virtual void CyclePlacement()//SPACE
    {
        if (canvasItem != null)
            canvasItem.Rotate();
    }

    public virtual void StartPlacement(Type canvasItemType)
    {
        CanvasModel.ClearSelectedItems();
        CanvasModel.CancelPlacement();

        var c = (ISelectableItem)Activator.CreateInstance(canvasItemType);

        StartPlacementInternal(c);
    }

    public virtual void StartPlacement(ISelectableItem placementItem)
    {
        CanvasModel.ClearSelectedItems();
        CanvasModel.CancelPlacement();

        StartPlacementInternal(placementItem);
    }

    public virtual void StartPlacement()
    {
        CanvasModel.CancelPlacement();

        StartPlacementInternal(null);
    }

    public virtual void CancelPlacement()
    {
        if (canvasItem != null && !canvasItem.IsPlaced)
            CanvasModel.RemoveItem(canvasItem);
    }

    protected virtual void RegisterUndoActionExecuted()
    {
        var item = canvasItem;

        Func<object, object> undo = (i) =>
        {
            CanvasModel.RemoveItem(item);
            return item;
        };
        Func<object, object> redo = (i) =>
        {
            CanvasModel.AddItem(item);
            return item;
        };

        CanvasModel.RegisterUndoActionExecuted(undo, redo, item);
    }

    protected void CommitPlacement()
    {
        RegisterUndoActionExecuted();
        CanvasModel.OnDrawingChanged(DrawingChangedReason.ItemPlacementFinished);
    }
    private void StartPlacementInternal(ISelectableItem placementItem)
    {
        canvasItem = placementItem;
        CanvasModel.PlacementTool = this;
        PlacementStatus = PlacementStatus.Ready;

        SetupCanvasItem();

        CanvasModel.AddItem(canvasItem);

        ShowPlacingItemProperties();
    }

    //todo: this could be an event that is handled in canvas designer
    private void ShowPlacingItemProperties()
    {
        try
        {
            //show properties for the current placing item UpdateSelection()?
            var pw = ServiceProvider.GetToolWindow<PropertiesToolWindowViewModel>(false);
            if (pw != null)
            {
                pw.SelectedObject = canvasItem;
                pw.IsVisible = true;
                pw.IsActive = true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    private ILayeredViewModel GetLayeredDocument()
    {
        return CanvasModel as ILayeredViewModel;
    }

    public virtual void SetupCanvasItem()
    {
        if (canvasItem is SingleLayerBoardCanvasItem boardItem)
        {
            var layeredDoc = GetLayeredDocument();
            if (layeredDoc != null)
            {
                boardItem.LayerDocument = layeredDoc;
                if (layeredDoc.SelectedLayer != null)
                    boardItem.LayerId = layeredDoc.SelectedLayer.LayerId;
                boardItem.LoadLayers();
                boardItem.AssignLayerForced(boardItem.Layer);
            }
            //boardItem.Layer = layersWindow.SelectedLayer;
        }
        else if (canvasItem is BoardCanvasItemViewModel brdItem)
        {
            var layeredDoc = GetLayeredDocument();
            if (layeredDoc != null)
            {
                brdItem.LayerDocument = layeredDoc;
                brdItem.LoadLayers();
            }
        }

    }
}
