using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using System.Windows;
using System.ComponentModel;
using IDE.Core.Types.Media;
using IDE.Core.Storage;

namespace IDE.Core.Interfaces
{
    /// <summary>
    /// An interface that can have other selectable items inside
    /// </summary>
    public interface IDrawingViewModel : INotifyPropertyChanged
    {
        event Action<DrawingChangedReason> DrawingChanged;

        /// <summary>
        /// raised when selection in canvas was changed
        /// </summary>
        event EventHandler SelectionChanged;

        /// <summary>
        /// raised when highlight in canvas was changed
        /// </summary>
        event EventHandler HighlightChanged;

        void OnSelectionChanged(object sender, EventArgs e);

        void OnHighlightChanged(object sender, EventArgs e);

        //PlacementData PlacingObject { get; set; }
        IPlacementTool PlacementTool { get; set; }

        IFileBaseViewModel FileDocument { get; set; }

        double X { get; set; }
        double Y { get; set; }

        double Scale { get; set; }
        XPoint Offset { get; set; }

        /// <summary>
        /// Origin in top-left coordinates
        /// </summary>
        XPoint Origin { get; set; }

        double DocumentWidth { get; set; }
        double DocumentHeight { get; set; }

        DocumentSize DocumentSize { get; set; }

        XColor GridColor { get; set; }

        double GridSize { get; }

        XColor CanvasBackground { get; set; }

        //FrameworkElement Canvas { get; set; }
        //FrameworkElement Canvas { get; set; }

        List<ISelectableItem> SelectedItems { get; }

        ICanvasGrid CanvasGrid { get; set; }

        IList<ISelectableItem> Items { get; set; }

        IEnumerable<ISelectableItem> GetItems();

        void AddItem(ISelectableItem item);

        void AddItems(IEnumerable<ISelectableItem> addItems);

        void RemoveItem(ISelectableItem item);

        void RemoveItems(IEnumerable<ISelectableItem> removeItems);

        void ClearSelectedItems();

        void ClearHighlightedItems();

        void CancelPlacement();


        bool IsPlacingItem();

        void StartPlacement(Type canvasItemType, Type placementToolType = null);
        void StartPlacement(ISelectableItem canvasItem);


        void SetViewportSize(Func<XSize> f);

        void SetRequestMousePosition(Func<XPoint> f);

        XPoint RequestMousePosition();

       // double SnapToGrid(double positionMM);

        XPoint SnapToGrid(XPoint positionMM);

        /// <summary>
        /// returns a snap position in mm from a dpi mouse position
        /// </summary>
        /// <param name="positionDpi"></param>
        /// <returns></returns>
        XPoint SnapToGridFromDpi(XPoint positionDpi);

        void OnDrawingChanged(DrawingChangedReason reason);

        void UpdateSelection();

        void ZoomToFit();

        void ZoomToSelectedItems();

        void ZoomToRectangle(XRect rect);

        void MirrorXSelectedItems();

        void MirrorYSelectedItems();

        void CyclePlacementOrRotate();

        void ChangeFootprintPlacement();

        void CopySelectedItems();

        void PasteSelectedItems();

        void DeleteSelectedItems();
    }
}
