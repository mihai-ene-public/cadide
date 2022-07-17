using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using IDE.Core.ViewModels;
using IDE.Core.Commands;
using System.Collections;
using IDE.Core.Converters;
using IDE.Core.Interfaces;
using System.ComponentModel;
using IDE.Core.Types.Media;
using IDE.Core.Storage;

namespace IDE.Core.Designers
{
    public class DrawingViewModel : BaseViewModel
                                  , IDrawingViewModel
    {
        public event Action<DrawingChangedReason> DrawingChanged;
        public event EventHandler SelectionChanged;
        public event EventHandler HighlightChanged;

        public DrawingViewModel(IFileBaseViewModel file, IDispatcherHelper dispatcher)
        {
            Scale = 1;
            FileDocument = file;

            CanvasBackground = XColor.FromHexString("#FF616161");
            GridColor = XColors.Gray;

            getViewPort = () => new XSize(DocumentWidth, DocumentHeight);

            _dispatcher = dispatcher;
        }

        private readonly IDispatcherHelper _dispatcher;

        IDebounceDispatcher _selectionDebouncer;
        IDebounceDispatcher selectionDebouncer
        {
            get
            {
                if (_selectionDebouncer == null)
                {
                    _selectionDebouncer = ServiceProvider.Resolve<IDebounceDispatcher>();
                }

                return _selectionDebouncer;
            }
        }

        IDebounceDispatcher _debounceDrawingChanged;
        IDebounceDispatcher debounceDrawingChanged
        {
            get
            {
                if (_debounceDrawingChanged == null)
                {
                    _debounceDrawingChanged = ServiceProvider.Resolve<IDebounceDispatcher>();
                }

                return _debounceDrawingChanged;
            }
        }
        public IPlacementTool PlacementTool { get; set; }

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

        //public FrameworkElement Canvas { get; set; }

        public IFileBaseViewModel FileDocument { get; set; }

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

        public double GridSize => ((CanvasGrid)canvasGrid).CurrentUnit.CurrentValue;

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

        IList<ISelectableItem> items = new SpatialItemsSource();
        public IList<ISelectableItem> Items
        {
            get { return items; }
            set
            {
                if (value is SpatialItemsSource)
                    items = value;
                else
                    items = new SpatialItemsSource(value);

                OnPropertyChanged(nameof(Items));
            }
        }

        public void UpdateSelection()
        {
            selectionDebouncer.Debounce(50, p => UpdateSelectionInternal());
        }

        void UpdateSelectionInternal()
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

        ICanvasGrid canvasGrid = new CanvasGrid();



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

        List<ISelectableItem> GetActionObjects()
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
                    return new List<ISelectableItem> { GetGroupFromItems(selected) };

                return selected;
            }
        }





        ICommand mirrorXCommand;
        public ICommand MirrorXCommand
        {
            get
            {
                if (mirrorXCommand == null)
                    mirrorXCommand = CreateCommand(p =>
                      {
                          MirrorXSelectedItems();
                      },
                      p => GetActionObjects() != null);

                return mirrorXCommand;
            }
        }

        public void MirrorXSelectedItems()
        {
            var item = GetActionObjects();

            if (item == null)
                return;

            item.ForEach(c => c.MirrorX());

            if (item.Count == 1 && item.First() is VolatileGroupCanvasItem groupItem && !IsPlacingItem())
                UngroupItemsFromGroup(groupItem);
        }

        public void MirrorYSelectedItems()
        {
            var item = GetActionObjects();

            if (item == null)
                return;

            item.ForEach(c => c.MirrorY());

            if (item.Count == 1 && item.First() is VolatileGroupCanvasItem groupItem && !IsPlacingItem())
                UngroupItemsFromGroup(groupItem);
        }

        public void CyclePlacementOrRotate()
        {
            if (IsPlacingItem())
            {
                PlacementTool.CyclePlacement();
                return;
            }

            var item = GetActionObjects();

            if (item == null)
                return;

            item.ForEach(c => c.Rotate());

            if (item.Count == 1 && item.First() is VolatileGroupCanvasItem groupItem)
                UngroupItemsFromGroup(groupItem);
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

        private VolatileGroupCanvasItem GetGroupFromItems(List<ISelectableItem> items)
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

        ICommand mirrorYCommand;
        public ICommand MirrorYCommand
        {
            get
            {
                if (mirrorYCommand == null)
                    mirrorYCommand = CreateCommand(p =>
                    {
                        MirrorYSelectedItems();
                    });

                return mirrorYCommand;
            }
        }

        public void ChangeFootprintPlacement()
        {

            //all selected items are footprints only
            var selectedItems = SelectedItems;
            var fps = selectedItems.OfType<FootprintBoardCanvasItem>().ToList();
            if (selectedItems.Count == fps.Count)
            {
                fps.ForEach(fp => fp.TogglePlacement());
            }
        }

        public void CopySelectedItems()
        {
            if (SelectedItems != null && SelectedItems.Count > 0)
                ApplicationClipboard.Items = SelectedItems.Cast<object>().ToList();
        }

        private class CopyNetMapping
        {
            public SchematicNet OldNet { get; set; }
            public SchematicNet NewNet { get; set; }
            public List<NetSegmentCanvasItem> NetItems { get; set; }
        }

        public void PasteSelectedItems()
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

        public void DeleteSelectedItems()
        {
            var selectedItems = SelectedItems.ToList();

            if (selectedItems.Count == 1 && selectedItems[0] is ISegmentedPolylineSelectableCanvasItem wire)
            {
                var segmentRemover = new SegmentRemoverHelper(this, wire, _dispatcher);
                segmentRemover.RemoveSelectedSegments();
            }
            else
            {
                foreach (var s in selectedItems)
                    RemoveItem(s);
            }

            OnDrawingChanged(DrawingChangedReason.ItemRemoved);
        }

        public void AddItem(ISelectableItem item)
        {
            item.PropertyChanged += DesignerItemBaseViewModel_PropertyChanged;

            if (item is SingleLayerBoardCanvasItem)
                return;
            items.Add(item);

        }

        void DesignerItemBaseViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var thisItem = sender as ISelectableItem;
            if (thisItem == null)
                return;
            var ignorePropNames = new[]
                                {
                                    nameof(ISelectableItem.IsSelected),
                                   nameof(BoardCanvasItemViewModel.IsFaulty),
                                   nameof(BaseCanvasItem.ZIndex),
                                   nameof(PolygonBoardCanvasItem.PolygonGeometry),
                                   nameof(PlaneBoardCanvasItem.RegionGeometry),
                                   nameof(ISegmentedPolylineSelectableCanvasItem.SelectedPoints)
                                };
            if (ignorePropNames.Contains(e.PropertyName))
                return;

            if (thisItem.IsPlaced)
                OnDrawingChanged(DrawingChangedReason.ItemModified);
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

                //RemoveFromTree(item);
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

        public void ClearHighlightedItems()
        {
            if (FileDocument is ICanvasWithHighlightedItems canvas)
                canvas.ClearHighlightedItems();

            OnHighlightChanged(this, EventArgs.Empty);
        }

        public void CancelPlacement()
        {
            //remove the object if we had one placing
            if (PlacementTool != null && PlacementTool.CanvasItem != null && !PlacementTool.CanvasItem.IsPlaced)
                RemoveItem(PlacementTool.CanvasItem);

            PlacementTool = null;
        }

        public bool IsPlacingItem()
        {
            return PlacementTool != null && PlacementTool.CanvasItem != null;
        }

        public void StartPlacement(Type canvasItemType, Type placementToolType = null)
        {
            //create  placement tool
            var placementTool = IDE.Core.Presentation.Placement.PlacementTool.CreateTool(canvasItemType, placementToolType);
            placementTool.CanvasModel = this;

            //show any dialogs
            if (placementTool.Show())
            {
                placementTool.StartPlacement(canvasItemType);
            }
        }

        public void StartPlacement(ISelectableItem canvasItem)
        {
            //create  placement tool
            var placementTool = IDE.Core.Presentation.Placement.PlacementTool.CreateTool(canvasItem.GetType());
            placementTool.CanvasModel = this;

            //show any dialogs
            if (placementTool.Show())
            {
                placementTool.StartPlacement(canvasItem);
            }
        }

        public void ZoomToFit()
        {
            ZoomToItems(GetItems().ToList());
        }

        Func<XSize> getViewPort;

        Func<XPoint> requestMousePosition;

        public void SetViewportSize(Func<XSize> f)
        {
            getViewPort = f;
        }

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

        public void ZoomToRectangle(XRect rect)
        {
            if (rect.IsEmpty)
                return;

            rect.Inflate(5, 5);
            const double zoomFactor = 0.9;//0.8f;
            var size = getViewPort();

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

        void ZoomToItems(IList<ISelectableItem> items)
        {
            var rect = XRect.Empty;

            foreach (BaseCanvasItem item in items.OfType<BaseCanvasItem>())
            {
                var itemRect = item.GetBoundingRectangle();

                rect.Union(itemRect);
            }

            ZoomToRectangle(rect);
        }

        public void ZoomToItem(ISelectableItem item)
        {
            ZoomToItems(new[] { item }.ToList());
        }

        public void ZoomToSelectedItems()
        {
            if (SelectedItems == null || SelectedItems.Count == 0)
                return;

            ZoomToItems(SelectedItems);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="position">position is in mm</param>
        ///// <returns></returns>
        //public double SnapToGrid(double position)
        //{
        //    var grid = CanvasGrid as CanvasGrid;

        //    if (grid.CanSnapToGrid == false)
        //        return position;

        //    //var gridSize = grid.GridSizeModel.CurrentValue;
        //    var gridSize = grid.CurrentUnit.CurrentValue;
        //    var counts = Math.Truncate(position / gridSize);
        //    var delta = Math.Abs(position - counts * gridSize);

        //    if (delta <= (gridSize / 2))
        //        return counts * gridSize;
        //    else
        //        return (counts + 1) * gridSize;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">position is in mm</param>
        /// <returns></returns>
        public XPoint SnapToGrid(XPoint position)
        {

            var grid = CanvasGrid as CanvasGrid;

            if (grid.CanSnapToGrid == false)
                return position;

            //var gridSize = grid.GridSizeModel.CurrentValue;
            var gridSize = grid.CurrentUnit.CurrentValue;

            var absX = Math.Abs(position.X);
            var absY = Math.Abs(position.Y);
            var signX = Math.Sign(position.X);
            var signY = Math.Sign(position.Y);

            var countsX = Math.Truncate(absX / gridSize);
            var countsY = Math.Truncate(absY / gridSize);
            var deltaX = absX - countsX * gridSize;
            var deltaY = absY - countsY * gridSize;

            double snapX = 0;
            double snapY = 0;

            if (deltaX <= (gridSize * 0.5))
                snapX = countsX * gridSize;
            else
                snapX = (countsX + 1) * gridSize;

            if (deltaY <= (gridSize * 0.5))
                snapY = countsY * gridSize;
            else
                snapY = (countsY + 1) * gridSize;

            return new XPoint(signX * snapX, signY * snapY);
        }

        public XPoint SnapToGridFromDpi(XPoint positionDpi)
        {
            var position = MilimetersToDpiHelper.ConvertToMM(positionDpi);

            return SnapToGrid(position);
        }


        public void OnDrawingChanged(DrawingChangedReason reason)
        {
            if (DrawingChanged != null)
            {
                debounceDrawingChanged.Debounce(300, p => DrawingChanged.Invoke(reason));

            }
        }

        public void OnSelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }

        public void OnHighlightChanged(object sender, EventArgs e)
        {
            HighlightChanged?.Invoke(sender, e);
        }
    }
}
