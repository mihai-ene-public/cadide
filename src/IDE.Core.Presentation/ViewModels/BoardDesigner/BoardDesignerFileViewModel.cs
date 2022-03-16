using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using IDE.Core.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using IDE.Core.Storage;
using IDE.Core.Designers;
using IDE.Core.Utilities;
using IDE.Core;
using IDE.Core.Commands;
using IDE.Core.Toolbars;
using IDE.Core.Excelon;
using IDE.Core.Interfaces;
using IDE.Core.Gerber;
using IDE.Core.Common;
using System.Threading.Tasks;
using IDE.Core.Presentation.Placement;
using IDE.Core.Presentation.PlacementRouting;
using System.Diagnostics;
using IDE.Core.Settings;
using System.IO.Compression;
using IDE.Core.BOM;
using IDE.Core.PDF;
using IDE.Core.Build;
using IDE.Core.Types.Media;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Presentation.Importers.DXF;
using IDE.Core.Presentation.Compilers;
using IDE.Core.Presentation.ObjectFinding;

namespace IDE.Documents.Views
{
    public class BoardDesignerFileViewModel : CanvasDesignerFileViewModel
                                            , IBoardDesigner
    {
        public BoardDesignerFileViewModel(IObjectFinder objectFinder)
            : base()
        {
            DocumentKey = "Board Editor";
            Description = "Board files";
            FileFilterName = "Board file";
            DefaultFilter = "board";
            documentTypeKey = DocumentKey;
            defaultFileType = "board";
            defaultFileName = "Board";

            BoardViewMode = BoardViewMode.Board;

            boardDocument = new BoardDocument();

            canvasGrid.GridSizeModel.CurrentValue = 0.25;

            Toolbar = new BoardToolbar(this);

            PropertyChanged += BoardDesignerFileViewModel_PropertyChanged;
            CanvasModel.DrawingChanged += CanvasModel_DrawingChanged;
            CanvasModel.SelectionChanged += CanvasModel_SelectionChanged;
            CanvasModel.HighlightChanged += CanvasModel_HighlightChanged;

            if (applicationModel == null)
                return;
            applicationModel.SelectionChanged += ApplicationModel_SelectionChanged;
            applicationModel.HighlightChanged += ApplicationModel_HighlightChanged;

            dirtyPropertiesProvider = ServiceProvider.Resolve<IDirtyMarkerTypePropertiesMapper>();
            _settingsManager = ServiceProvider.Resolve<ISettingsManager>();
            _activeCompiler = ServiceProvider.Resolve<IActiveCompiler>();
            _objectFinder = objectFinder;
        }

        private readonly IDirtyMarkerTypePropertiesMapper dirtyPropertiesProvider;
        private readonly ISettingsManager _settingsManager;
        private readonly IActiveCompiler _activeCompiler;
        private readonly IObjectFinder _objectFinder;

        protected override async Task AfterLoadDocumentInternal()
        {
            if (LoadedForCompiler)
            {
                await UpdateConnections();
                await RepourPolygons();
            }
            else
            {

                await base.AfterLoadDocumentInternal();

                await UpdateConnections();

                await RefreshOverview();

                await CheckServices();
            }
        }

        protected override bool CanZoomToFitCommand()
        {
            return Is2DMode();
        }

        protected override bool CanZoomToSelectedItemsCommand()
        {
            return Is2DMode();
        }

        async Task RepourPolygons()
        {
            var polygons = from l in LayerItems
                           from p in l.Items.OfType<IRepourGeometry>()
                           select p;

            foreach (var p in polygons.ToList())
                await p.RepourPolygonAsync();
        }

        public override object Document
        {
            get
            {
                return boardDocument;
            }
        }

        CanvasGrid canvasGrid => canvasModel.CanvasGrid as CanvasGrid;

        IList canSelectList;
        public override IList CanSelectList
        {
            get
            {
                if (canSelectList == null)
                    canSelectList = new List<SelectionFilterItemViewModel>
                {
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Lines",
                        Type = typeof(LineBoardCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Texts",
                        Type = typeof(TextBoardCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Small Text Monoline",
                        Type = typeof(TextSingleLineBoardCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Rectangles",
                        Type = typeof(RectangleBoardCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Polygons",
                        Type = typeof(PolygonBoardCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Circles",
                        Type = typeof(CircleBoardCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Arcs",
                        Type = typeof(ArcBoardCanvasItem)
                    },

                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Holes",
                        Type = typeof(HoleCanvasItem)
                    },
                     new SelectionFilterItemViewModel
                    {
                        TooltipText = "Vias",
                        Type = typeof(ViaCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Tracks",
                        Type = typeof(TrackBoardCanvasItem),
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Pads",
                        Type = typeof(PadThtCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Smds",
                        Type = typeof(PadSmdCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Parts",
                        Type = typeof(FootprintBoardCanvasItem)
                    }
            };

                return canSelectList;
            }
        }


        protected override void RefreshFromCache()
        {
            if (State != DocumentState.IsEditing)
                return;

            try
            {
                checkServicesBusy = true;

                foreach (var fp in canvasModel.GetFootprints())
                {
                    //this will refresh the primitive (will save primitive so far: pos, rot, etc)
                    var p = fp.SaveToPrimitive();

                    fp.LoadFromPrimitive(p);
                }
            }
            finally
            {
                checkServicesBusy = false;
            }


            if (DesignerViewMode == DesignerViewMode.ViewMode3D)
            {
                ShowLayersGeometries().ConfigureAwait(false);
            }
        }

        bool highlightChangeBusy;
        void ApplicationModel_HighlightChanged(object sender, EventArgs e)
        {
            if (sender == canvasModel || highlightChangeBusy) return;

            highlightChangeBusy = true;
            //highlighted nets from schematic
            var schCanvasModel = sender as DrawingViewModel;

            //todo: we need to check if this is our schematic
            var highlightedNets = schCanvasModel.Items.OfType<NetSegmentCanvasItem>().Where(p => p.Net != null && p.Net.IsHighlighted)
                                                .Select(n => n.Net).Distinct().ToList();

            foreach (var boardNet in NetList)
            {
                var schNet = highlightedNets.FirstOrDefault(n => n.Name == boardNet.Name);
                if (schNet != null)
                    boardNet.HighlightNet(schNet.IsHighlighted);
                else
                    boardNet.HighlightNet(false);
            }

            highlightChangeBusy = false;

        }

        public void ClearHighlightedItems()
        {
            foreach (var net in NetList)
                net.HighlightNet(false);
        }

        bool selectionChangeBusy;

        List<string> selectedPartNames = new List<string>();
        void ApplicationModel_SelectionChanged(object sender, EventArgs e)
        {
            if (sender == canvasModel || selectionChangeBusy) return;

            selectionChangeBusy = true;
            //selected parts from schematic
            var schCanvasModel = sender as DrawingViewModel;

            //todo: we need to check if this is our schematic
            var selectedParts = schCanvasModel.Items.OfType<SchematicSymbolCanvasItem>().Where(p => p.IsSelected).ToList();

            //canvasModel.ClearSelectedItems();

            var boardParts = canvasModel.GetFootprints().ToList();
            foreach (var bp in boardParts)
            {
                var partName = bp.PartName;
                var schPart = selectedParts.FirstOrDefault(s => s.PartName == partName);
                if (schPart != null)
                    bp.IsSelected = schPart.IsSelected;
                else
                    bp.IsSelected = false;

                if (bp.IsSelected)
                {
                    if (!selectedPartNames.Contains(partName))
                        selectedPartNames.Add(partName);
                }
                else
                {
                    selectedPartNames.Remove(partName);
                }
            }

            selectionChangeBusy = false;
        }

        void UpdateSelectedPartNames()
        {
            var selecteditems = canvasModel.SelectedItems.OfType<FootprintBoardCanvasItem>().ToList();
            if (selecteditems.Count == 0)
                selectedPartNames.Clear();

            var toRemove = new List<string>();

            foreach (var partName in selectedPartNames)
            {
                var part = selecteditems.FirstOrDefault(s => s.PartName == partName);
                if (part == null)
                {
                    toRemove.Add(partName);
                }
            }

            foreach (var pn in toRemove)
                selectedPartNames.Remove(pn);

            foreach (var si in selecteditems)
            {
                var pn = selectedPartNames.FirstOrDefault(p => p == si.PartName);
                if (pn == null)
                    selectedPartNames.Add(si.PartName);
            }
        }

        void CanvasModel_HighlightChanged(object sender, EventArgs e)
        {
            applicationModel.OnHighlightChanged(sender, e);
        }

        void CanvasModel_SelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectedPartNames();
            applicationModel.OnSelectionChanged(sender, e);
        }

        async void BoardDesignerFileViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsActive):
                    {
                        if (IsActive)
                        {
                            await CheckServices();
                        }
                        break;
                    }

                case nameof(SelectedLayer):
                    if (canvasModel.IsPlacingItem())
                    {
                        var placingitem = canvasModel.PlacementTool.CanvasItem as SingleLayerBoardCanvasItem;
                        if (placingitem != null)
                            placingitem.Layer = SelectedLayer;
                    }
                    HandleLayers();
                    break;
                case nameof(SelectedLayerGroup):
                case nameof(MaskUnselectedLayer):
                case nameof(HideUnselectedLayer):
                    {
                        HandleLayers();
                        break;
                    }
            }

        }

        public override IList<IDocumentToolWindow> GetToolWindowsWhenActive()
        {
            var tools = new List<IDocumentToolWindow>();

            tools.Add(ServiceProvider.GetToolWindow<LayersToolWindowViewModel>());
            tools.Add(ServiceProvider.GetToolWindow<DocumentOverviewViewModel>());
            tools.Add(ServiceProvider.GetToolWindow<SelectionFilterToolViewModel>());
            tools.Add(ServiceProvider.GetToolWindow<Preview3DWindowViewModel>());

            documentTools = tools;

            return tools;
        }

        List<IDocumentToolWindow> documentTools;

        void RefreshToolWindowsVisibility()
        {
            if (documentTools == null)
                return;

            ShowVisibleToolsFiltered(documentTools);
        }

        protected override bool IsToolWindowVisible(IDocumentToolWindow toolWindow)
        {
            var isView3D = designerViewMode == DesignerViewMode.ViewMode3D;

            if (toolWindow is Preview3DWindowViewModel)
                return isView3D;

            return !isView3D;
        }

        async void CanvasModel_DrawingChanged(DrawingChangedReason reason)
        {
            //update services: connections, online DRC, etc
            await CheckServices();
        }

        bool GetShowUnconnectedLinesSetting()
        {
            var sm = _settingsManager;
            if (sm != null)
            {
                var brdSetting = sm.GetSetting<BoardEditorGeneralSetting>();
                if (brdSetting != null)
                {
                    return brdSetting.ShowUnconnectedSignals;
                }
            }

            return true;
        }

        bool checkServicesBusy;
        async Task CheckServices()
        {
            if (State == DocumentState.IsLoading || checkServicesBusy)
                return;

            try
            {
                checkServicesBusy = true;

                var repourPolys = true;
                var refreshConnections = true;
                var runAutoCompiler = true;

                var sm = _settingsManager;
                if (sm != null)
                {
                    var brdSetting = sm.GetSetting<BoardEditorGeneralSetting>();
                    if (brdSetting != null)
                    {
                        repourPolys = brdSetting.RepourPolysOnDocumentChange;
                        refreshConnections = brdSetting.ShowUnconnectedSignals;
                        runAutoCompiler = brdSetting.OnlineDRC;
                    }
                }

                if (repourPolys)
                    await RepourPolygons();

                if (refreshConnections)
                    await RefreshConnections();

                if (runAutoCompiler)
                    await RunActiveCompiler();
            }
            finally
            {
                checkServicesBusy = false;
            }
        }


        async Task RunActiveCompiler()
        {
            await _activeCompiler.Compile(this);
        }

        void HandleLayers()
        {
            if (LayerItems != null)
                foreach (var layer in LayerItems)
                    layer.HandleLayer();
        }

        IList<IOverviewSelectNode> BuildCategories()
        {
            var list = new List<IOverviewSelectNode>();
            var showPrimitives = true;
            var primitivesCat = new OverviewFolderNode { IsExpanded = false, Name = "Primitives" };
            var partsCat = new OverviewFolderNode { IsExpanded = false, Name = "Parts" };
            var netsCat = new OverviewFolderNode { IsExpanded = false, Name = "Nets" };
            //todo: unrouted nets

            if (showPrimitives)
                list.Add(primitivesCat);

            list.Add(partsCat);
            list.Add(netsCat);
            var partItems = canvasModel.GetFootprints().ToList();
            var parts = partItems.OrderBy(p => p.PartName)
                                 .Select(p => new OverviewSelectNode
                                 {
                                     DataItem = p,
                                 });
            partsCat.Children.AddRange(parts);
            var netSegmentItems = canvasModel.GetItems().OfType<ISignalPrimitiveCanvasItem>().Cast<ISelectableItem>().ToList();


            if (showPrimitives)
            {
                var primitiveItems = canvasModel.GetItems().Except(partItems).Except(netSegmentItems);
                primitivesCat.Children.AddRange(primitiveItems.Where(p => !(p is BoardNetGraph) && !(p is RegionBoardCanvasItem)).Select(p => new OverviewSelectNode
                {
                    DataItem = p
                }));
            }

            //nets
            foreach (var net in NetList.OrderBy(n => n.Name))
            {
                var netNode = new OverviewFolderNode { IsExpanded = false, DataItem = net };
                netNode.Children.AddRange(net.Pads.Select(n => new OverviewSelectNode { DataItem = n }));
                netNode.Children.AddRange(net.Items.Select(n => new OverviewSelectNode { DataItem = n }));
                netsCat.Children.Add(netNode);
            }

            return list;
        }

        IList<IOverviewSelectNode> categories;
        public IList<IOverviewSelectNode> Categories
        {
            get
            {
                return categories;
            }
            set
            {
                categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        public Task RefreshOverview()
        {
            return Task.Run(() =>
            {
                var nodes = BuildCategories();

                _dispatcher.RunOnDispatcher(() => Categories = nodes);
            });
        }

        #region Layer selection

        ILayerDesignerItem selectedLayer;

        public ILayerDesignerItem SelectedLayer
        {
            get { return selectedLayer; }
            set
            {
                selectedLayer = value;
                OnPropertyChanged(nameof(SelectedLayer));
            }
        }

        ILayerGroupDesignerItem selectedLayerGroup;
        public ILayerGroupDesignerItem SelectedLayerGroup
        {
            get { return selectedLayerGroup; }
            set
            {
                if (selectedLayerGroup == value) return;
                selectedLayerGroup = value;

                if (selectedLayerGroup != null)
                    SelectedLayer = selectedLayerGroup.Layers.FirstOrDefault();

                OnPropertyChanged(nameof(SelectedLayerGroup));
            }
        }

        bool dimmLayerNotSelected;
        public bool MaskUnselectedLayer
        {
            get { return dimmLayerNotSelected; }
            set
            {
                if (dimmLayerNotSelected == value) return;
                dimmLayerNotSelected = value;
                OnPropertyChanged(nameof(MaskUnselectedLayer));
            }
        }


        bool hideLayerNotSelected;
        public bool HideUnselectedLayer
        {
            get { return hideLayerNotSelected; }
            set
            {
                if (hideLayerNotSelected == value) return;
                hideLayerNotSelected = value;
                OnPropertyChanged(nameof(HideUnselectedLayer));
            }
        }

        public void ChangeToCopperLayer(int layerNumber)
        {
            var layerTypes = new[] { LayerType.Plane, LayerType.Signal };
            var copperLayers = LayerItems.Where(l => layerTypes.Contains(l.LayerType)).ToList();
            var layerIndex = layerNumber - 1;
            if (layerIndex < 0)
                layerIndex = 0;
            if (layerIndex > copperLayers.Count - 1)
                layerIndex = copperLayers.Count - 1;

            SelectedLayer = copperLayers[layerIndex];
        }

        #endregion

        #region Fields

        BoardDocument boardDocument;

        public IList<INetClassBaseItem> NetClasses { get; set; } = new List<INetClassBaseItem>();
        public IList<IBoardNetDesignerItem> NetList { get; set; } = new List<IBoardNetDesignerItem>();

        public IList<ILayerPairModel> DrillPairs { get { return BoardProperties.DrillPairs; } }

        public IList<ILayerPairModel> LayerPairs { get { return BoardProperties.LayerPairs; } }

        public IList<IBoardRuleModel> Rules { get { return BoardProperties.Rules; } }

        bool isLoading = true;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public bool HasHighlightedNets
        {
            get
            {
                return NetList.Any(n => n.IsHighlighted);
            }
        }
        public IList<IBoardNetGraph> Connections { get; set; } = new List<IBoardNetGraph>();

        DesignerViewMode designerViewMode;
        public DesignerViewMode DesignerViewMode
        {
            get { return designerViewMode; }
            set
            {
                designerViewMode = value;
                OnPropertyChanged(nameof(DesignerViewMode));
            }
        }

        BoardPreview3DViewModel boardPreview3DViewModel;
        public BoardPreview3DViewModel BoardPreview3DViewModel
        {
            get { return boardPreview3DViewModel; }
            set
            {
                boardPreview3DViewModel = value;
                OnPropertyChanged(nameof(BoardPreview3DViewModel));
            }
        }

        #endregion Fields

        #region Toolbox

        protected override void InitToolbox()
        {
            base.InitToolbox();

            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Line",
                Type = typeof(LineBoardCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Text",
                Type = typeof(TextBoardCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Small Text Monoline",
                Type = typeof(TextSingleLineBoardCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Rectangle",
                Type = typeof(RectangleBoardCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Polygon",
                Type = typeof(PolygonBoardCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Circle",
                Type = typeof(CircleBoardCanvasItem)
            });
            //Toolbox.Primitives.Add(new PrimitiveItem
            //{
            //    TooltipText = "Ellipse",
            //    Type = typeof(EllipseBoardCanvasItem)
            //});
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Arc",
                Type = typeof(ArcBoardCanvasItem)
            });

            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Hole",
                Type = typeof(HoleCanvasItem)
            });

            //Via
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Via",
                Type = typeof(ViaCanvasItem)
            });
            //Track
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Track",
                Type = typeof(TrackBoardCanvasItem),
                PlacementToolType = typeof(TrackPlacementTool<SingleTrackRoutingMode>)
            });

            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Pad",
                Type = typeof(PadThtCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Smd",
                Type = typeof(PadSmdCanvasItem)
            });
        }



        #endregion Toolbox

        /// <summary>
        /// a reference to our board outline
        /// </summary>
        RegionBoardCanvasItem boardOutlineItem = null;

        public IRegionCanvasItem BoardOutline { get { return boardOutlineItem; } set { boardOutlineItem = (RegionBoardCanvasItem)value; } }

        BoardPropertiesViewModel boardProperties;
        public BoardPropertiesViewModel BoardProperties
        {
            get { return boardProperties; }
            set
            {
                if (boardProperties != null)
                    boardProperties.PropertyChanged -= BoardProperties_PropertyChanged;
                boardProperties = value;
                boardProperties.PropertyChanged += BoardProperties_PropertyChanged;

                OnPropertyChanged(nameof(BoardProperties));
            }
        }

        async void BoardProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(boardProperties.SchematicReference):
                    await RefreshSchematicReferenceStatus();
                    break;
            }

            if (State == DocumentState.IsLoading)
                return;

            _dispatcher.RunOnDispatcher(() =>
            {
                IsDirty = true;//maybe not all properties
            });

        }


        ObservableCollection<LayerGroupDesignerItem> layerGroups = new ObservableCollection<LayerGroupDesignerItem>();
        public IList LayerGroups
        {
            get
            {
                return layerGroups;
            }
        }

        public IList<ILayerDesignerItem> LayerItems { get; set; } = new ObservableCollection<ILayerDesignerItem>();

        BoardViewMode boardViewMode;
        public BoardViewMode BoardViewMode
        {
            get { return boardViewMode; }
            set
            {
                boardViewMode = value;
                OnPropertyChanged(nameof(BoardViewMode));
            }
        }

        bool isSchematicReferenceRequired;
        public bool IsSchematicReferenceRequired
        {
            get { return isSchematicReferenceRequired; }
            set
            {
                isSchematicReferenceRequired = value;
                OnPropertyChanged(nameof(IsSchematicReferenceRequired));
            }
        }

        bool isUpdateBoardFromSchematicRequired;
        public bool IsUpdateBoardFromSchematicRequired
        {
            get { return isUpdateBoardFromSchematicRequired; }
            set
            {
                isUpdateBoardFromSchematicRequired = value;
                OnPropertyChanged(nameof(IsUpdateBoardFromSchematicRequired));
            }
        }

        #region Commands

        ICommand addFootprintCommand;

        /// <summary>
        /// imports primitives from an exiting footprint
        /// </summary>
        public ICommand AddFootprintCommand
        {
            get
            {
                if (addFootprintCommand == null)
                    addFootprintCommand = CreateCommand(p =>
                    {
                        canvasModel.ClearSelectedItems();

                        canvasModel.CancelPlacement();


                        var itemSelectDlg = new ItemSelectDialogViewModel();
                        itemSelectDlg.TemplateType = TemplateType.Footprint;
                        itemSelectDlg.ProjectModel = ParentProject;
                        if (itemSelectDlg.ShowDialog() == true)
                        {
                            var symbol = itemSelectDlg.SelectedItem.Document as Footprint;

                            if (symbol.Items != null)
                            {
                                var canvasItems = symbol.Items.Where(sItem => !(sItem is Smd || sItem is Pad))
                                                              .Select(c => c.CreateDesignerItem()).ToList();

                                var rect = XRect.Empty;
                                foreach (var item in canvasItems)
                                    rect.Union(item.GetBoundingRectangle());

                                var group = new VolatileGroupCanvasItem();
                                canvasItems.ForEach(c =>
                                {
                                    var canvasItem = c as BoardCanvasItemViewModel;
                                    canvasItem.LayerDocument = this;
                                    canvasItem.ParentObject = group;
                                    canvasItem.LoadLayers();
                                    canvasItem.Translate(-rect.X, -rect.Y);
                                });

                                group.Items = canvasItems.ToList();

                                canvasModel.StartPlacement(group);
                            }
                        }
                    },
                    p => Is2DMode()
                    );

                return addFootprintCommand;
            }
        }

        ICommand importDxfCommand;

        public ICommand ImportDxfCommand
        {
            get
            {
                if (importDxfCommand == null)
                    importDxfCommand = CreateCommand((p) =>
                    {
                        try
                        {
                            var dlg = new DxfImporterViewModel(this);
                            if (dlg.ShowDialog() == true)
                            {
                                var primitives = dlg.RunImport();

                                var canvasItems = primitives.Select(c => c.CreateDesignerItem()).ToList();

                                var rect = XRect.Empty;
                                foreach (var item in canvasItems)
                                    rect.Union(item.GetBoundingRectangle());

                                var group = new VolatileGroupCanvasItem();
                                canvasItems.ForEach(c =>
                                {
                                    var canvasItem = c as BoardCanvasItemViewModel;
                                    canvasItem.LayerDocument = this;
                                    canvasItem.ParentObject = group;
                                    canvasItem.LoadLayers();
                                    canvasItem.Translate(-rect.X, -rect.Y);
                                });


                                group.Items = canvasItems.ToList();

                                canvasModel.StartPlacement(group);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageDialog.Show(ex.Message);
                        }
                    },
                    p => Is2DMode());

                return importDxfCommand;
            }
        }

        ICommand showBoardPropertiesCommand;
        public ICommand ShowBoardPropertiesCommand
        {
            get
            {
                if (showBoardPropertiesCommand == null)
                    showBoardPropertiesCommand = CreateCommand((p) =>
                      {
                          boardProperties.Show();
                          BoardViewMode = BoardViewMode.Properties;
                      },
                    p => Is2DMode() && BoardViewMode != BoardViewMode.Properties);

                return showBoardPropertiesCommand;
            }
        }

        ICommand showBoardCommand;

        public ICommand ShowBoardCommand
        {
            get
            {
                if (showBoardCommand == null)
                    showBoardCommand = CreateCommand((p) =>
                    {
                        BoardViewMode = BoardViewMode.Board;
                    },
                    p => BoardViewMode != BoardViewMode.Board);

                return showBoardCommand;
            }
        }

        ICommand repourPolygonsCommand;

        public ICommand RepourPolygonsCommand
        {
            get
            {
                if (repourPolygonsCommand == null)
                {
                    repourPolygonsCommand = CreateCommand(async p => await RepourPolygons(), p => Is2DMode());
                }

                return repourPolygonsCommand;
            }
        }

        ICommand checkConnectionsCommand;

        public ICommand CheckConnectionsCommand
        {
            get
            {
                if (checkConnectionsCommand == null)
                    checkConnectionsCommand = CreateCommand(async p => await CheckConnections(), p => Is2DMode());

                return checkConnectionsCommand;
            }
        }

        ICommand compileCommand;

        public ICommand CompileCommand
        {
            get
            {
                if (compileCommand == null)
                    compileCommand = CreateCommand(async p => await RunActiveCompiler(), p => Is2DMode());

                return compileCommand;
            }
        }

        ICommand updateBoardFromSchematicCommand;
        public ICommand UpdateBoardFromSchematicCommand
        {
            get
            {
                if (updateBoardFromSchematicCommand == null)
                    updateBoardFromSchematicCommand = CreateCommand(async p => await UpdateBoardFromSchematic(), p => Is2DMode());
                return updateBoardFromSchematicCommand;
            }
        }

        async Task UpdateBoardFromSchematic()
        {
            if (boardProperties == null || boardProperties.SchematicReference == null)
                return;

            //open the schematic; should be saved
            //var schematic = await Task.Run(() => ParentProject.FindObject(TemplateType.Schematic, null, boardProperties.SchematicReference.schematicId) as SchematicDocument);
            var schematic =  _objectFinder.FindObject<SchematicDocument>(ParentProject.Project, null, boardProperties.SchematicReference.schematicId);
            if (schematic == null)
                return;//we could show a message

            State = DocumentState.IsLoading;
            IsLoading = true;

            var brdUpdater = new BoardUpdateFromSchematicOperation(_dispatcher, _objectFinder);
            await brdUpdater.Update(this, schematic, ParentProject);

            await AfterLoadDocumentInternal();

            State = DocumentState.IsEditing;
            IsLoading = false;
        }

        Task UpdateConnections()
        {
            return Task.Run(() =>
            {
                var oldConnections = Connections.ToList();

                Connections.Clear();

                var newConnections = new List<ISelectableItem>();
                foreach (var net in NetList)
                {
                    if (net.Id == 0) continue;

                    var conn = new BoardNetGraph((BoardNetDesignerItem)net, this);

                    newConnections.Add(conn);
                    Connections.Add(conn);
                }

                _dispatcher.RunOnDispatcher(() =>
                {
                    canvasModel.RemoveItems(oldConnections);
                    canvasModel.AddItems(newConnections);
                });
            });
        }


        async Task RefreshConnections()
        {
            await Task.Run(() =>
            {
                //todo we must execute this on a separate thread with cancelation
                foreach (var c in Connections)
                    c.Build(GetShowUnconnectedLinesSetting());
            }
            );
        }

        public async Task<IList<IBoardNetGraph>> GetUnroutedConnections()
        {
            await RefreshConnections();

            return Connections.Where(c => !c.IsCompletelyRouted).ToList();
        }

        async Task CheckConnections()
        {
            await Task.Run(() =>
            {
                //todo we must execute this on a separate thread with cancelation
                foreach (var c in Connections)
                    c.Build(true);
            }
           );
        }

        ICommand updateBoardOutlineCommand;
        public ICommand UpdateBoardOutlineCommand
        {
            get
            {
                if (updateBoardOutlineCommand == null)
                    updateBoardOutlineCommand = CreateCommand(p => UpdateBoardOutline(), p => Is2DMode());
                return updateBoardOutlineCommand;
            }
        }

        //move this in its own class: BoardOutlineUpdater
        void UpdateBoardOutline()
        {
            var layerId = (int)LayerType.BoardOutline + 1;
            var outlineLayer = canvasModel.Items.OfType<LayerDesignerItem>().FirstOrDefault(l => l.LayerId == layerId);
            if (outlineLayer == null) return;

            var outlineItems = outlineLayer.Items.Cast<BaseCanvasItem>().OfType<SingleLayerBoardCanvasItem>().ToList();

            //we don't want to clear outline if we don't have anything
            if (outlineItems.Count == 0)
                return;

            boardOutlineItem.LayerId = layerId;
            boardOutlineItem.Items.Clear();

            List<BaseRegionItem> regionItems = new List<BaseRegionItem>();
            XPoint startPoint;
            XPoint endPoint = new XPoint();
            var first = true;

            while (outlineItems.Count > 0)
            {
                SingleLayerBoardCanvasItem item = null;

                if (first)
                {
                    item = outlineItems[0];
                    outlineItems.RemoveAt(0);
                    first = false;
                }
                else
                {
                    foreach (var sItem in outlineItems)
                    {
                        if (sItem is ArcBoardCanvasItem)
                        {
                            var sArc = sItem as ArcBoardCanvasItem;
                            if (sArc.StartPoint == endPoint || sArc.EndPoint == endPoint)
                            {
                                item = sItem;
                                break;
                            }

                        }
                        else if (sItem is LineBoardCanvasItem)
                        {
                            var sLine = sItem as LineBoardCanvasItem;
                            if (new XPoint(sLine.X1, sLine.Y1) == endPoint || new XPoint(sLine.X2, sLine.Y2) == endPoint)
                            {
                                item = sItem;
                                break;
                            }
                        }
                    }

                    //we stop if we didnt find an item
                    if (item == null)
                        return;

                    outlineItems.Remove(item);
                }


                //if (regionCanvasItems.Count == outlineItems.Count) break;

                BaseRegionItem regionItem = null;
                var cSp = new XPoint();
                var cEp = new XPoint();
                if (item is ArcBoardCanvasItem)
                {
                    var arc = item as ArcBoardCanvasItem;
                    cSp = arc.StartPoint;
                    cEp = arc.EndPoint;
                    regionItem = new ArcRegionItem
                    {
                        EndPointX = cEp.X,
                        EndPointY = cEp.Y,
                        IsLargeArc = arc.IsLargeArc,
                        SizeDiameter = arc.Radius,
                        SweepDirection = arc.SweepDirection,
                    };
                }
                else if (item is LineBoardCanvasItem)
                {
                    var line = item as LineBoardCanvasItem;
                    cSp = new XPoint(line.X1, line.Y1);
                    cEp = new XPoint(line.X2, line.Y2);
                    regionItem = new LineRegionItem { EndPointX = cEp.X, EndPointY = cEp.Y };
                }

                if (regionItems.Count == 0)
                {
                    startPoint = cSp;
                    endPoint = cEp;
                    boardOutlineItem.StartPointX = cSp.X;
                    boardOutlineItem.StartPointY = cSp.Y;
                    regionItems.Add(regionItem);
                    //regionCanvasItems.Add(item);
                }
                else
                {
                    if (cSp == endPoint)//current Startpoint is last end point
                        endPoint = cEp;
                    else
                    {
                        endPoint = cSp;
                        if (regionItem is ArcRegionItem)
                        {
                            //revert
                            var arcRegion = regionItem as ArcRegionItem;
                            arcRegion.SweepDirection = (XSweepDirection)(1 - (int)arcRegion.SweepDirection);
                        }
                    }

                    regionItem.EndPointX = endPoint.X;
                    regionItem.EndPointY = endPoint.Y;
                    regionItems.Add(regionItem);
                    //regionCanvasItems.Add(item);
                }

            }

            boardOutlineItem.Items.AddRange(regionItems);
            boardOutlineItem.OnPropertyChanged(nameof(boardOutlineItem.Items));
        }

        ICommand repositionSelectedComponentsCommand;
        public ICommand RepositionSelectedComponentsCommand
        {
            get
            {
                if (repositionSelectedComponentsCommand == null)
                {
                    repositionSelectedComponentsCommand = CreateCommand(
                        p => RepositionSelectedComponents(),
                        p => Is2DMode()
                    );
                }
                return repositionSelectedComponentsCommand;
            }
        }

        private void RepositionSelectedComponents()
        {
            var selecteditems = CanvasModel.SelectedItems.OfType<FootprintBoardCanvasItem>().ToList();
            if (selecteditems.Count == 0)
                return;

            var partsBySelectionOrder = new List<FootprintBoardCanvasItem>();
            foreach (var partName in selectedPartNames)
            {
                var part = selecteditems.FirstOrDefault(s => s.PartName == partName);
                if (part != null)
                {
                    partsBySelectionOrder.Add(part);
                }
            }
            if (partsBySelectionOrder.Count == 0)
                return;

            var placementTool = new BoardRepositionSelectedComponentsPlacementTool(partsBySelectionOrder);
            placementTool.CanvasModel = CanvasModel;

            placementTool.StartPlacement();
        }

        ICommand show2DCommand;
        public ICommand Show2DCommand
        {
            get
            {
                if (show2DCommand == null)
                {
                    show2DCommand = CreateCommand(p =>
                    {
                        Show2D();
                    },
                    p => DesignerViewMode == DesignerViewMode.ViewMode3D);
                }

                return show2DCommand;
            }
        }

        void Show2D()
        {
            if (DesignerViewMode == DesignerViewMode.ViewMode2D)
                return;

            DesignerViewMode = DesignerViewMode.ViewMode2D;
            RefreshToolWindowsVisibility();
        }

        ICommand show3DCommand;
        public ICommand Show3DCommand
        {
            get
            {
                if (show3DCommand == null)
                {
                    show3DCommand = CreateCommand(async p =>
                    {
                        await Show3D();
                    },
                    p => Is2DMode());
                }

                return show3DCommand;
            }
        }



        async Task Show3D()
        {
            if (DesignerViewMode == DesignerViewMode.ViewMode3D)
                return;

            IsLoading = true;

            try
            {
                //show layers geometries (silk, top copper)
                await ShowLayersGeometries();

                DesignerViewMode = DesignerViewMode.ViewMode3D;
                RefreshToolWindowsVisibility();
            }
            finally
            {
                IsLoading = false;
            }
        }

        bool Is2DMode()
        {
            return designerViewMode == DesignerViewMode.ViewMode2D;
        }

        async Task ShowLayersGeometries()
        {
            var boardPreviewHelper = new Board3DPreviewHelper(_dispatcher);

            var model = await boardPreviewHelper.ShowBoardGeometries(this, ParentProject, boardProperties.StackupTotalThickness);

            BoardPreview3DViewModel = model;


        }



        #endregion Commands

        public IBoardBuildOptions BuildOptions { get { return boardProperties.BuildOptions; } }

        protected override void SaveDocumentInternal(string filePath)
        {
            var boardSaver = new BoardSaver();
            boardSaver.Save(this, boardDocument);

            boardProperties.SaveTo(boardDocument);

            XmlHelper.Save(boardDocument, filePath);
        }

        #region LoadFile


        protected override Task LoadDocumentInternal(string filePath)
        {
            return Task.Run(() =>
            {
                var sw = Stopwatch.StartNew();

                _dispatcher.RunOnDispatcher(() => IsLoading = true);

                boardDocument = XmlHelper.Load<BoardDocument>(filePath);

                BoardProperties = new BoardPropertiesViewModel(this);

                var brdLoader = new BoardLoader(_dispatcher, ParentProject);
                brdLoader.Load(boardDocument, this);

                boardProperties.ParentProject = ParentProject;
                boardProperties.LoadFrom(boardDocument);

                IsDirty = false;
                IsLoading = false;

                sw.Stop();
                Debug.WriteLine($"Board loaded: {sw.ElapsedMilliseconds} ms");


                //32-42 sec!!!
                //25 sec
                //10 sec
            });
        }



        async Task RefreshSchematicReferenceStatus()
        {
            IsSchematicReferenceRequired = boardProperties.SchematicReference.schematicId == 0;
            if (IsSchematicReferenceRequired)//these should not be true at the same time
                IsUpdateBoardFromSchematicRequired = false;
            //evaluate when we check what we have in schematic
            //IsUpdateBoardFromSchematicRequired = true;
            if (boardProperties.IsUpdateBoardFromSchematicRequired)
            {
                _dispatcher.RunOnDispatcher(() => { IsDirty = true; });

                //update board from schematic
                await UpdateBoardFromSchematic();
            }
        }


        #endregion LoadFile

        public IList<string> OutputFiles { get; private set; } = new List<string>();


        public List<BoardComponentInstance> GetBoardParts()
        {

            var footprints = CanvasModel.GetFootprints().ToList();

            var partsList = footprints.Select(f => f.FootprintPrimitive).ToList();

            partsList.Sort(new BoardPartNameComparer());

            return partsList;
        }

        public List<FootprintBoardCanvasItem> GetBoardFootprints()
        {
            return CanvasModel.GetFootprints()
                                    .OrderBy(f => f.PartName, new IndexedNameComparer())
                                    .ToList();
        }


        public override void ApplySettings()
        {
            var settingsManager = _settingsManager;
            if (settingsManager != null)
            {
                var brdColorsSettings = settingsManager.GetSetting<BoardEditorColorsSetting>();
                if (brdColorsSettings == null)
                    return;

                canvasModel.CanvasBackground = XColor.FromHexString(brdColorsSettings.CanvasBackground);
                canvasModel.GridColor = XColor.FromHexString(brdColorsSettings.GridColor);
            }

        }
    }
}
