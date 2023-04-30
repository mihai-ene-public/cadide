using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using IDE.Core;
using IDE.Core.Converters;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Placement;
using IDE.Core.Storage;
using IDE.Core.Toolbars;
using IDE.Core.Types.Media;
using IDE.Core.UndoRedoFramework;
using IDE.Core.ViewModels;

namespace IDE.Documents.Views;

/// <summary>
/// Represents a canvas designer with toolbox and top toolbar. (Symbol, Footprint, Schematic, Board)
/// <para>It is a common class that will be inherited from.</para>
/// </summary>
public abstract class CanvasDesignerFileViewModel : FileBaseViewModel, ICanvasDesignerFileViewModel
{

    protected CanvasDesignerFileViewModel(IDispatcherHelper dispatcher,
        IDebounceDispatcher drawingChangedDebouncer,
        IDebounceDispatcher selectionDebouncer,
        IDirtyMarkerTypePropertiesMapper dirtyMarkerTypePropertiesMapper,
        IPlacementToolFactory placementToolFactory
        )
    {
        _dispatcher = dispatcher;
        _drawingChangedDebouncer = drawingChangedDebouncer;
        _selectionDebouncer = selectionDebouncer;
        _dirtyMarkerTypePropertiesMapper = dirtyMarkerTypePropertiesMapper;
        _placementToolFactory = placementToolFactory;

        Scale = 1;

        CanvasBackground = XColor.FromHexString("#FF616161");
        GridColor = XColors.Gray;

        getViewPort = () => new XSize(DocumentWidth, DocumentHeight);

        InitToolbox();

        IsDirty = false;

        _undoRedoContext = new UndoRedoContext();

    }

    protected readonly IUndoRedoContext _undoRedoContext;
    protected readonly IDispatcherHelper _dispatcher;
    private readonly IDebounceDispatcher _drawingChangedDebouncer;
    private readonly IDebounceDispatcher _selectionDebouncer;
    private readonly IDirtyMarkerTypePropertiesMapper _dirtyMarkerTypePropertiesMapper;
    private readonly IPlacementToolFactory _placementToolFactory;

    public ToolbarModel Toolbar { get; set; }

    #region CanvasModel

    public double GridSize => canvasGrid.GridSize;

    protected ICanvasGrid canvasGrid = new CanvasGrid();

    public ICanvasGrid CanvasGrid
    {
        get
        {
            return canvasGrid;
        }
        set
        {
            canvasGrid = value;
            OnPropertyChanged(nameof(CanvasGrid));
        }
    }

    public IPlacementTool PlacementTool { get; set; }

    public List<ISelectableItem> SelectedItems
    {
        get
        {
            return GetItems().Where(x => x.IsSelected).ToList();
        }
    }

    public IEnumerable<ISelectableItem> GetItems()
    {
        var lst = new List<ISelectableItem>();
        foreach (var item in Items)
        {
            if (item is LayerDesignerItem layer)
                lst.AddRange(layer.Items);
            else
                lst.Add(item);
        }

        return lst;
    }

    IList<ISelectableItem> items = new SpatialItemsSource();
    public IList<ISelectableItem> Items
    {
        get { return items; }
        set
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    item.PropertyChanged -= DesignerItemBaseViewModel_PropertyChanged;
                }
            }

            if (value is SpatialItemsSource)
                items = value;
            else
                items = new SpatialItemsSource(value);

            if (items != null)
            {
                foreach (var item in items)
                {
                    item.PropertyChanged += DesignerItemBaseViewModel_PropertyChanged;
                }
            }

            OnPropertyChanged(nameof(Items));
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

    double documentWidth = 297;
    public double DocumentWidth
    {
        get { return documentWidth; }
        set
        {
            if (documentWidth != value)
            {
                documentWidth = value;
                OnPropertyChanged(nameof(DocumentWidth));
            }
        }
    }

    //sizes in mm
    double documentHeight = 210;
    public double DocumentHeight
    {
        get { return documentHeight; }
        set
        {
            if (documentHeight != value)
            {
                documentHeight = value;
                OnPropertyChanged(nameof(DocumentHeight));
            }
        }
    }

    DocumentSize documentSize = DocumentSize.A4;
    public DocumentSize DocumentSize
    {
        get { return documentSize; }
        set
        {
            if (documentSize != value)
            {
                documentSize = value;
                OnPropertyChanged(nameof(DocumentSize));
            }
        }
    }

    double scale;
    public const double minScale = 0.75;
    public const double maxScale = 150;
    public double Scale
    {
        get { return scale; }
        set
        {
            scale = value;
            if (scale < minScale)
                scale = minScale;
            if (scale > maxScale)
                scale = maxScale;

            OnPropertyChanged(nameof(Scale));
        }
    }


    XPoint offset;
    public XPoint Offset
    {
        get { return offset; }
        set
        {
            offset = value;
            OnPropertyChanged(nameof(Offset));
        }
    }

    XColor canvasBackground;
    public XColor CanvasBackground
    {
        get { return canvasBackground; }
        set
        {
            canvasBackground = value;
            OnPropertyChanged(nameof(CanvasBackground));
        }
    }

    XColor gridColor;
    public XColor GridColor
    {
        get { return gridColor; }
        set
        {
            gridColor = value;
            OnPropertyChanged(nameof(GridColor));
        }
    }

    #endregion CanvasModel
    public virtual IList CanSelectList { get; }
    public bool CanSelectItem(ISelectableItem item)
    {
        var list = CanSelectList as List<SelectionFilterItemViewModel>;
        if (list != null)
        {
            var selectionItem = list.FirstOrDefault(f => f.Type == item.GetType());
            if (selectionItem != null)
                return selectionItem.CanSelect;
        }
        return true;
    }

    DocumentSizeTemplate selectedSizeTemplate;
    public DocumentSizeTemplate SelectedSizeTemplate
    {
        get
        {
            selectedSizeTemplate = SizeTemplates.FirstOrDefault(t => t.DocumentSize == DocumentSize);

            return selectedSizeTemplate;
        }
        set
        {
            if (selectedSizeTemplate == value)
                return;

            selectedSizeTemplate = value;

            DocumentSize = selectedSizeTemplate.DocumentSize;
            DocumentWidth = selectedSizeTemplate.DocumentWidth;
            DocumentHeight = selectedSizeTemplate.DocumentHeight;

            IsDirty = true;
            OnPropertyChanged(nameof(SelectedSizeTemplate));
        }
    }

    public IList<DocumentSizeTemplate> SizeTemplates { get; set; } = DocumentSizeTemplates.GetTemplates();

    #region Toolbox

    protected ToolBoxViewModel toolbox;
    public ToolBoxViewModel Toolbox
    {
        get
        {
            return toolbox;
        }
        set
        {
            toolbox = value;
            OnPropertyChanged(nameof(Toolbox));
        }
    }

    #endregion Toolbox

    #region X

    double x;
    public double X
    {
        get { return x; }
        set
        {
            if (x != value)
            {
                x = value;
                OnPropertyChanged(nameof(X));
            }
        }
    }

    #endregion X

    #region Y

    double y;
    public double Y
    {
        get { return y; }
        set
        {
            if (y != value)
            {
                y = value;
                OnPropertyChanged(nameof(Y));
            }
        }
    }

    #endregion Y

    XPoint origin;
    public XPoint Origin
    {
        get { return origin; }
        set
        {
            origin = value;
            OnPropertyChanged(nameof(Origin));
        }
    }


    ICommand zoomToFitCommand;

    public ICommand ZoomToFitCommand
    {
        get
        {
            if (zoomToFitCommand == null)
                zoomToFitCommand = CreateCommand(p =>
                  {
                      ZoomToFit();
                  },
                  p => CanZoomToFitCommand());

            return zoomToFitCommand;
        }
    }

    protected virtual bool CanZoomToFitCommand()
    {
        return true;
    }

    ICommand zoomToSelectedItemsCommand;

    public ICommand ZoomToSelectedItemsCommand
    {
        get
        {
            if (zoomToSelectedItemsCommand == null)
                zoomToSelectedItemsCommand = CreateCommand(p =>
                {
                    ZoomToSelectedItems();
                },
                p => CanZoomToSelectedItemsCommand());

            return zoomToSelectedItemsCommand;
        }
    }
    public void ZoomToFit()
    {
        ZoomToItems(GetItems().ToList());
    }

    private void ZoomToSelectedItems()
    {
        var selectedItems = SelectedItems;
        if (selectedItems == null || selectedItems.Count == 0)
            return;

        ZoomToItems(selectedItems);
    }

    private void ZoomToItems(IList<ISelectableItem> items)
    {
        var rect = XRect.Empty;

        foreach (BaseCanvasItem item in items.OfType<BaseCanvasItem>())
        {
            var itemRect = item.GetBoundingRectangle();

            rect.Union(itemRect);
        }

        ZoomToRectangle(rect);
    }

    private void ZoomToRectangle(XRect rect)
    {
        if (rect.IsEmpty)
            return;

        rect.Inflate(5, 5);
        const double zoomFactor = 0.9;//0.8f;
        var size = getViewPort();

        //var origin = canvasModel.Origin;
        var vbWidth = MilimetersToDpiHelper.ConvertToMM(size.Width);
        var vbHeight = MilimetersToDpiHelper.ConvertToMM(size.Height);
        if (vbWidth == 0.0d)
            vbWidth = DocumentWidth;
        if (vbHeight == 0.0d)
            vbHeight = DocumentHeight;

        var sWidth = vbWidth / rect.Width;
        var sHeight = vbHeight / rect.Height;
        var newScale = zoomFactor * Math.Min(sWidth, sHeight);
        if (newScale < minScale)
            return;

        //this is not the perfect solution, but seems to fit the best
        Offset = new XPoint((origin.X + rect.X) * newScale, (origin.Y + rect.Y) * newScale);
        Scale = newScale;
    }

    protected virtual bool CanZoomToSelectedItemsCommand()
    {
        return true;
    }

    public void AddItem(ISelectableItem item)
    {
        item.PropertyChanged += DesignerItemBaseViewModel_PropertyChanged;

        if (item is SingleLayerBoardCanvasItem singleLayerItem)
        {
            singleLayerItem.LoadLayers();//?
            return;
        }
        items.Add(item);
    }


    public void AddItems(IEnumerable<ISelectableItem> addItems)
    {
        foreach (var item in addItems)
        {
            AddItem(item);
        }
    }

    public void RemoveItem(ISelectableItem item)
    {
        item.RemoveFromCanvas();

        items.Remove(item);

        item.PropertyChanged -= DesignerItemBaseViewModel_PropertyChanged;

    }

    public void RemoveItems(IEnumerable<ISelectableItem> removeItems)
    {
        foreach (var item in removeItems)
        {
            RemoveItem(item);
        }
    }

    public void ClearSelectedItems()
    {
        foreach (ISelectableItem item in GetItems())
        {
            item.IsSelected = false;
        }

        UpdateSelection();
    }

    public void CancelPlacement()
    {
        PlacementTool?.CancelPlacement();

        PlacementTool = null;
    }

    private Func<XSize> getViewPort;
    public void SetViewportSize(Func<XSize> f)
    {
        getViewPort = f;
    }

    private Func<XPoint> requestMousePosition;
    public void SetRequestMousePosition(Func<XPoint> f)
    {
        requestMousePosition = f;
    }

    public XPoint RequestMousePosition()
    {
        if (requestMousePosition != null)
        {
            var mp = requestMousePosition();
            mp.Offset(-origin.X, -origin.Y);

            return mp;
        }

        return new XPoint();
    }

    public void Undo()
    {
        _undoRedoContext.Undo();
    }
    public void Redo()
    {
        _undoRedoContext.Redo();
    }

    #region DeleteSelectedItems command

    #region ISnapshotManager
    public virtual ISavedState CreateSnapshot()
    {
        return new GenericCanvasSavedState
        {
            CanvasItems = GetItems().Select(s => s.Clone())
                                     .Cast<ICanvasItem>()
                                     .ToList()
        };
    }

    public virtual void RestoreFromSnapshot(ISavedState state)
    {

    }

    #endregion ISnapshotManager

    #endregion DeleteSelectedItems command


    protected virtual void InitToolbox()
    {
        Toolbox = new ToolBoxViewModel(this);
    }

    bool busyCats = false;

    public void OnDrawingChanged(DrawingChangedReason reason)
    {
        OnDrawingChanged(null, reason);
    }
    protected void OnDrawingChanged(object sender, DrawingChangedReason reason)
    {
        _drawingChangedDebouncer.Debounce(300, p => OnDrawingChangedVirtual(sender, reason));
    }
    protected virtual async void OnDrawingChangedVirtual(object sender, DrawingChangedReason reason)
    {
        IsDirty = true;

        try
        {
            if (reason == DrawingChangedReason.ItemAdded || reason == DrawingChangedReason.ItemRemoved)
            {
                if (!busyCats)
                {
                    busyCats = true;
                    if (this is IDocumentOverview overview)
                        await overview.RefreshOverview();

                    busyCats = false;
                }
            }

        }
        catch { }
    }

    public void ChangeMode()
    {
        if (IsPlacingItem())
        {
            PlacementTool.ChangeMode();
        }
    }
    public bool IsPlacingItem()
    {
        return PlacementTool?.CanvasItem != null;
    }
    private List<ISelectableItem> GetActionObjects()
    {
        if (IsPlacingItem())
        {
            if (PlacementTool.CanvasItem.IsEditing)
                return null;
            return new List<ISelectableItem>() { PlacementTool.CanvasItem };
        }
        else
        {
            var selected = SelectedItems;

            if (selected.Any(s => s.IsEditing))
                return null;

            if (selected.Count > 1)
                return new List<ISelectableItem> { CreateGroupFromItems(selected) };

            return selected;
        }
    }

    public void CyclePlacementOrRotate()
    {
        if (IsPlacingItem())
        {
            PlacementTool.CyclePlacement();
            return;
        }

        var items = GetActionObjects();

        if (items == null)
            return;

        RotateItemsInternal(items);

        var selectedItems = SelectedItems.ToList();
        items = selectedItems;

        RegisterUndoActionExecuted(undo: (o) =>
        {
            if (selectedItems.Count > 1)
                items = new List<ISelectableItem> { CreateGroupFromItems(selectedItems) };
            RotateItemsInternal(items, -90.0);
            return null;
        },
        redo: (o) =>
        {
            if (selectedItems.Count > 1)
                items = new List<ISelectableItem> { CreateGroupFromItems(selectedItems) };
            RotateItemsInternal(items);
            return null;
        }, null);
    }

    private void RotateItemsInternal(List<ISelectableItem> items, double angle = 90)
    {
        items.ForEach(c => c.Rotate(angle));

        if (items.Count == 1 && items[0] is VolatileGroupCanvasItem groupItem && !IsPlacingItem())
            UngroupItemsFromGroup(groupItem);
    }

    private VolatileGroupCanvasItem CreateGroupFromItems(List<ISelectableItem> items)
    {
        //group rectangle
        var rect = XRect.Empty;
        foreach (var item in items)
            rect.Union(item.GetBoundingRectangle());

        var rCenter = rect.GetCenter();

        //translate item by group rectangle
        items.ForEach(c => c.Translate(-rCenter.X, -rCenter.Y));

        var group = new VolatileGroupCanvasItem
        {
            Items = items.ToList(),
            DisplayWidth = rect.Width,
            DisplayHeight = rect.Height
        };
        group.Translate(rCenter.X, rCenter.Y);

        return group;
    }

    private void UngroupItemsFromGroup(VolatileGroupCanvasItem groupItem)
    {
        foreach (var c in groupItem.Items)
        {
            var tg = new XTransformGroup();

            var scaleTransform = new XScaleTransform(groupItem.ScaleX, groupItem.ScaleY);
            tg.Children.Add(scaleTransform);

            var rotateTransform = new XRotateTransform(groupItem.Rot);
            tg.Children.Add(rotateTransform);
            tg.Children.Add(new XTranslateTransform(groupItem.X, groupItem.Y));

            ((BaseCanvasItem)c).TransformBy(tg.Value);
        }
    }

    public void MirrorXSelectedItems()
    {
        var items = GetActionObjects();

        if (items == null)
            return;

        MirrorXInternal(items);

        if (!IsPlacingItem())
        {
            var selectedItems = SelectedItems.ToList();
            items = selectedItems;

            RegisterUndoActionExecuted(undo: (o) =>
            {
                if (selectedItems.Count > 1)
                    items = new List<ISelectableItem> { CreateGroupFromItems(selectedItems) };
                MirrorXInternal(items);
                return null;
            },
            redo: (o) =>
            {
                if (selectedItems.Count > 1)
                    items = new List<ISelectableItem> { CreateGroupFromItems(selectedItems) };
                MirrorXInternal(items);
                return null;
            }, null);
        }
    }

    private void MirrorXInternal(List<ISelectableItem> items)
    {
        items.ForEach(c => c.MirrorX());

        if (items.Count == 1 && items[0] is VolatileGroupCanvasItem groupItem && !IsPlacingItem())
            UngroupItemsFromGroup(groupItem);
    }

    public void MirrorYSelectedItems()
    {
        var items = GetActionObjects();

        if (items == null)
            return;

        MirrorYInternal(items);

        if (!IsPlacingItem())
        {
            var selectedItems = SelectedItems.ToList();
            items = selectedItems;

            RegisterUndoActionExecuted(undo: (o) =>
            {
                if (selectedItems.Count > 1)
                    items = new List<ISelectableItem> { CreateGroupFromItems(selectedItems) };
                MirrorYInternal(items);
                return null;
            },
            redo: (o) =>
            {
                if (selectedItems.Count > 1)
                    items = new List<ISelectableItem> { CreateGroupFromItems(selectedItems) };
                MirrorYInternal(items);
                return null;
            }, null);
        }
    }

    private void MirrorYInternal(List<ISelectableItem> items)
    {
        items.ForEach(c => c.MirrorY());

        if (items.Count == 1 && items.First() is VolatileGroupCanvasItem groupItem && !IsPlacingItem())
            UngroupItemsFromGroup(groupItem);
    }

    public virtual void DeleteSelectedItems()
    {
        var selectedItems = SelectedItems.ToList();

        if (selectedItems.Count == 0)
            return;

        if (selectedItems.Count == 1 && selectedItems[0] is ISegmentedPolylineSelectableCanvasItem wire)
        {
            //todo: implement undo
            var segmentRemoverFactory = new SegmentRemoverFactory();
            var segmentRemover = segmentRemoverFactory.Create(wire, _dispatcher);
            segmentRemover.RemoveSelectedSegments(this, wire);
        }
        else
        {
            RemoveItems(selectedItems);

            RegisterUndoActionExecuted(undo: o =>
            {
                AddItems(selectedItems);
                return null;
            },
            redo: o =>
            {
                RemoveItems(selectedItems);
                return null;
            }, null);
        }

        OnDrawingChanged(DrawingChangedReason.ItemRemoved);
    }

    public void CopySelectedItems()
    {
        if (SelectedItems != null && SelectedItems.Count > 0)
            ApplicationClipboard.Items = SelectedItems.Cast<object>().ToList();
    }

    public virtual void PasteSelectedItems()
    {
        //if we want to paste multiple times, we need to clone
        var canvasItemPairs = ApplicationClipboard.Items.OfType<BaseCanvasItem>()
                            .Where(s => s.CanClone)
                             .Select(s => new
                             {
                                 OriginalObject = s,
                                 CloneObject = (BaseCanvasItem)s.Clone()
                             }).ToList();


        //in schematic, for parts: create copy of parts and add them to schematic
        var canvasItems = canvasItemPairs.Select(s => s.CloneObject).ToList();

        //for net segments (on schematic) we need to handle the Nets
        //we need to copy the nets, but have the same (similar) linking
        //there is a special case for power nets (we might want to keep the GND and VCC - later)
        //pinrefs are not copied because they are not visible
        var netSegments = canvasItemPairs.Where(s => s.OriginalObject is NetSegmentCanvasItem)
                                         .Select(s => (NetSegmentCanvasItem)s.CloneObject).ToList();
        var netMapping = netSegments.Where(n => n.Net != null).GroupBy(n => n.Net)
                                  .Select(n => new CopyNetMapping
                                  {
                                      OldNet = n.Key,
                                      NewNet = new SchematicNet
                                      {
                                          ClassId = n.Key.ClassId,
                                          Id = LibraryItem.GetNextId(),
                                      },
                                      NetItems = n.ToList()
                                  }).ToList();
        //net name
        netMapping.ForEach(n => n.NewNet.Name = $"Net{n.NewNet.Id}");
        //todo: keep power nets
        foreach (var net in netMapping)
        {
            var isPowerNet = net.OldNet.NetItems.OfType<PinCanvasItem>().Any(predicate => predicate.PinType == PinType.Power);
            if (isPowerNet)
            {
                net.NewNet = net.OldNet;
            }

            foreach (var item in net.NetItems.ToList())
            {
                item.Net = net.NewNet;
            }
        }

        var partMapping = canvasItemPairs.Where(s => s.OriginalObject is SchematicSymbolCanvasItem).ToList();
        //part names
        var existingPartNames = Items.OfType<SchematicSymbolCanvasItem>()
                 .Where(s => s.PartName != null && s.PartName.StartsWith(s.PartPrefix))
                 .Select(s => s.PartName).ToList();

        foreach (var partPair in partMapping)
        {
            var schPart = partPair.CloneObject as SchematicSymbolCanvasItem;
            var schPartOriginal = partPair.OriginalObject as SchematicSymbolCanvasItem;
            var newPartName = schPart.GetNextPartName(existingPartNames, schPart.PartPrefix);
            schPart.PartName = newPartName;
            existingPartNames.Add(newPartName);

            //assign new pins to corresponding nets
            if (schPart.Pins != null && schPartOriginal.Pins != null)
            {
                foreach (var newPin in schPart.Pins)
                {
                    var oldPin = schPartOriginal.Pins.FirstOrDefault(p => p.Number == newPin.Number);
                    if (oldPin != null)
                    {
                        var netPair = netMapping.FirstOrDefault(n => n.OldNet == oldPin.Net);
                        if (netPair != null)
                        {
                            newPin.Net = netPair.NewNet;
                        }
                    }
                }
            }
        }

        //group rectangle
        var rect = XRect.Empty;
        foreach (var item in canvasItems)
            rect.Union(item.GetBoundingRectangle());

        //mouse pos
        var mp = RequestMousePosition();
        mp = MilimetersToDpiHelper.ConvertToMM(mp);

        //translate item by group rectangle
        var center = rect.GetCenter();
        center = SnapToGrid(center);
        canvasItems.ForEach(c => c.Translate(-center.X, -center.Y));

        var group = new VolatileGroupCanvasItem
        {
            Items = canvasItems.Cast<ISelectableItem>().ToList()
        };

        mp = SnapToGrid(mp);
        group.Translate(mp.X, mp.Y);

        StartPlacement(group);
    }

    public void ChangeFootprintPlacement()
    {
        //all selected items are footprints only
        var selectedItems = SelectedItems;
        var fps = selectedItems.OfType<FootprintBoardCanvasItem>().ToList();
        if (selectedItems.Count == fps.Count)
        {
            fps.ForEach(fp => fp.TogglePlacement());

            RegisterUndoActionExecuted(
                undo: (o) =>
            {
                fps.ForEach(fp => fp.TogglePlacement());
                return null;
            },
            redo: (o) =>
            {
                fps.ForEach(fp => fp.TogglePlacement());
                return null;
            },
            null);
        }
    }

    public void StartPlacement(IToolboxItem toolBoxItem)
    {
        //create  placement tool
        var placementTool = _placementToolFactory.Create(toolBoxItem);
        placementTool.CanvasModel = this;

        //show any dialogs
        if (placementTool.Show())
        {
            placementTool.StartPlacement(toolBoxItem.Type);
        }
    }

    public void StartPlacement(ISelectableItem canvasItem)
    {
        //create  placement tool
        var placementTool = _placementToolFactory.Create(canvasItem.GetType());
        placementTool.CanvasModel = this;

        //show any dialogs
        if (placementTool.Show())
        {
            placementTool.StartPlacement(canvasItem);
        }
    }

    public void UpdateSelection()
    {
        _selectionDebouncer.Debounce(50, p => UpdateSelectionInternal());
    }

    private void UpdateSelectionInternal()
    {
        var pw = ServiceProvider.GetToolWindow<PropertiesToolWindowViewModel>();
        if (pw != null)
        {
            var oldSelected = pw.SelectedObject;
            pw.SelectedObject = null;

            if (PlacementTool != null)// && PlacementTool.CanvasItem != null)
                AssignSelected(pw, PlacementTool.CanvasItem);
            else
            {
                AssignSelected(pw, SelectedItems);
            }

            //show properties to view (this could be an option)
            if (pw.SelectedObject != null)
            {
                pw.IsVisible = true;

                if (oldSelected != pw.SelectedObject)
                    pw.IsActive = true;
            }
        }

        OnSelectionChanged(this, EventArgs.Empty);
    }

    protected virtual void OnSelectionChanged(object sender, EventArgs args)
    {

    }

    public void OnHighlightChanged(object sender, EventArgs e)
    {

    }

    void AssignSelected(PropertiesToolWindowViewModel pw, IList<ISelectableItem> items)
    {
        if (items.Count == 1)
        {
            pw.SelectedObject = items.FirstOrDefault();
        }
        else if (items.Count > 1)
        {
            pw.SelectedObject = new MultipleSelectionObject()
            {
                SelectedObjects = (IList)items,
            };
        }
    }

    void AssignSelected(PropertiesToolWindowViewModel pw, ISelectableItem item)
    {
        pw.SelectedObject = item;
    }

    /// <summary>
    /// </summary>
    /// <param name="position">position is in mm</param>
    /// <returns></returns>
    public XPoint SnapToGrid(XPoint position)
    {
        if (canvasGrid.CanSnapToGrid == false)
            return position;

        return SnapHelper.SnapToGrid(position, GridSize);
    }

    public XPoint SnapToGridFromDpi(XPoint positionDpi)
    {
        var position = MilimetersToDpiHelper.ConvertToMM(positionDpi);

        return SnapToGrid(position);
    }

    public void RegisterUndoActionExecuted(
        Func<object, object> undo,
        Func<object, object> redo,
        object data)
    {
        var undoableAction = new UndoableAction(undo, redo, null, _undoRedoContext);
        _undoRedoContext.ActionExecuted(undoableAction, data);
    }

    public override IList<IDocumentToolWindow> GetToolWindowsWhenActive()
    {
        return new List<IDocumentToolWindow>();
    }

    private class CopyNetMapping
    {
        public SchematicNet OldNet { get; set; }
        public SchematicNet NewNet { get; set; }
        public List<NetSegmentCanvasItem> NetItems { get; set; }
    }
}
