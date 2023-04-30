using System;
using System.Collections;
using System.Collections.Generic;
using IDE.Core.Types.Media;

namespace IDE.Core.Interfaces;

public interface ICanvasDesignerFileViewModel : ICanvasViewModel, IFileBaseViewModel, ISnapshotManager
{

    /// <summary>
    /// Origin in top-left coordinates
    /// </summary>
    XPoint Origin { get; set; }

    double DocumentWidth { get; set; }
    double DocumentHeight { get; set; }

    void ZoomToFit();

    bool IsPlacingItem();

    void SetViewportSize(Func<XSize> f);

    void SetRequestMousePosition(Func<XPoint> f);

    IList<ISelectableItem> Items { get; set; }

    void AddItem(ISelectableItem item);

    void AddItems(IEnumerable<ISelectableItem> addItems);

    void RemoveItem(ISelectableItem item);

    void RemoveItems(IEnumerable<ISelectableItem> removeItems);

    /// <summary>
    /// If item can be selected based on selection filter
    /// </summary>
    bool CanSelectItem(ISelectableItem item);

    IList CanSelectList { get; }


    void MirrorXSelectedItems();

    void MirrorYSelectedItems();

    void ChangeFootprintPlacement();

    IPlacementTool PlacementTool { get; set; }

    void StartPlacement(IToolboxItem toolBoxItem);
    void StartPlacement(ISelectableItem canvasItem);

    void CancelPlacement();

    void OnDrawingChanged(DrawingChangedReason reason);
    void OnHighlightChanged(object sender, EventArgs e);

    XPoint SnapToGrid(XPoint positionMM);

    /// <summary>
    /// returns a snap position in mm from a dpi mouse position
    /// </summary>
    /// <param name="positionDpi"></param>
    /// <returns></returns>
    XPoint SnapToGridFromDpi(XPoint positionDpi);


    void RegisterUndoActionExecuted(
            Func<object, object> undo,
            Func<object, object> redo,
            object data);
}
