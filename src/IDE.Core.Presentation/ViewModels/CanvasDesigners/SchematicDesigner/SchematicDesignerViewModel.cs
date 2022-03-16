using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IDE.Core.Storage;
using IDE.Core.Designers;
using IDE.Core.Utilities;
using IDE.Core.Commands;
using IDE.Core.Toolbars;
using IDE.Documents.Views;
using IDE.Core.Interfaces;
using IDE.Core.Common;
using System.Threading.Tasks;
using IDE.Core.PDF;
using IDE.Core.Settings.Options;
using IDE.Core.Settings;
using IDE.Core.Types.Media;
using Newtonsoft.Json.Serialization;
using IDE.Core.Presentation.Utilities;
using System.Collections;
using IDE.Core.Presentation.Compilers;

namespace IDE.Core.ViewModels
{
    public class SchematicDesignerViewModel : CanvasDesignerFileViewModel
                                            , ISchematicDesigner
    {
        #region ctor

        public SchematicDesignerViewModel()
            : base()
        {
            DocumentKey = "SchemaEditor";
            Description = "Schematic files";
            FileFilterName = "Schematic file";
            DefaultFilter = "schematic";
            documentTypeKey = DocumentKey;
            defaultFileType = "schematic";
            defaultFileName = "Schematic";

            schematicDocument = new SchematicDocument();
            Sheets = new ObservableCollection<ISheetDesignerItem>();

            SchematicViewMode = SchematicViewMode.Canvas;

            Toolbar = new SchematicToolbar(this);
            //PropertyChanged += SchematicDesignerViewModel_PropertyChanged;

            SchematicProperties = new SchematicDesignerPropertiesViewModel(this);

            CanvasModel.SelectionChanged += CanvasModel_SelectionChanged;
            CanvasModel.HighlightChanged += CanvasModel_HighlightChanged;

            if (applicationModel == null)
                return;
            applicationModel.SelectionChanged += ApplicationModel_SelectionChanged;
            applicationModel.HighlightChanged += ApplicationModel_HighlightChanged;

            canvasGrid.GridSizeModel.SelectedItem = new Units.MilUnit(50);

            _settingsManager = ServiceProvider.Resolve<ISettingsManager>();
            _activeCompiler = ServiceProvider.Resolve<IActiveCompiler>();
        }

        private readonly ISettingsManager _settingsManager;
        private readonly IActiveCompiler _activeCompiler;

        CanvasGrid canvasGrid => canvasModel.CanvasGrid as CanvasGrid;

        INetManager netManager = new SchematicNetManager();
        IBusManager busManager = new SchematicBusManager();

        public INetManager NetManager => netManager;
        public IBusManager BusManager => busManager;

        public override object Document
        {
            get
            {
                return schematicDocument;
            }
        }

        bool highlightChangeBusy;
        void ApplicationModel_HighlightChanged(object sender, EventArgs e)
        {
            if (sender == canvasModel || highlightChangeBusy) return;

            highlightChangeBusy = true;
            try
            {


                //highlighted nets from board
                var brdCanvasModel = sender as DrawingViewModel;
                if (brdCanvasModel == null) return;
                var brdFile = brdCanvasModel.FileDocument as BoardDesignerFileViewModel;
                if (brdFile == null) return;

                //todo: we need to check if the board uses this schematic
                var highlightedNets = brdFile.NetList.Where(p => p.IsHighlighted).ToList();

                var schNets = canvasModel.Items.OfType<NetSegmentCanvasItem>()
                                                .Select(n => n.Net).Distinct().ToList();

                foreach (var schNet in schNets)
                {
                    var brdNet = highlightedNets.FirstOrDefault(n => n.Name == schNet.Name);
                    if (brdNet != null)
                        schNet.HighlightNet(brdNet.IsHighlighted);
                    else
                        schNet.HighlightNet(false);
                }
            }
            finally
            {
                highlightChangeBusy = false;
            }
        }

        bool selectionChangeBusy;
        void ApplicationModel_SelectionChanged(object sender, EventArgs e)
        {
            if (sender == canvasModel || selectionChangeBusy) return;

            selectionChangeBusy = true;
            //selected parts from board
            var brdCanvasModel = sender as DrawingViewModel;

            //todo: we need to check if the board uses this schematic
            var selectedParts = brdCanvasModel.GetFootprints().Where(p => p.IsSelected).ToList();

            //canvasModel.ClearSelectedItems();

            var schParts = canvasModel.Items.OfType<SchematicSymbolCanvasItem>().ToList();
            foreach (var sp in schParts)
            {
                var brdPart = selectedParts.FirstOrDefault(s => s.PartName == sp.PartName);
                if (brdPart != null)
                    sp.IsSelected = brdPart.IsSelected;
                else
                    sp.IsSelected = false;
            }

            selectionChangeBusy = false;
        }

        void CanvasModel_HighlightChanged(object sender, EventArgs e)
        {
            applicationModel.OnHighlightChanged(sender, e);
        }

        void CanvasModel_SelectionChanged(object sender, EventArgs e)
        {
            applicationModel.OnSelectionChanged(sender, e);
        }

        //void SchematicDesignerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    switch (e.PropertyName)
        //    {
        //        case nameof(IsActive):
        //            {
        //                if (IsActive)
        //                {
        //                    var sheetsToolWindow = ServiceProvider.GetToolWindow<SchematicSheetsViewModel>();
        //                    if (sheetsToolWindow != null)
        //                    {
        //                        sheetsToolWindow.Schematic = this;
        //                        sheetsToolWindow.IsVisible = true;
        //                    }
        //                    var toolOverview = ServiceProvider.GetToolWindow<DocumentOverviewViewModel>();
        //                    if (toolOverview != null)
        //                    {
        //                        toolOverview.Document = this;
        //                        toolOverview.IsVisible = true;
        //                    }

        //                }
        //                break;
        //            }
        //    }
        //}

        public override IList<IDocumentToolWindow> GetToolWindowsWhenActive()
        {
            var tools = new List<IDocumentToolWindow>();

            tools.Add(ServiceProvider.GetToolWindow<SchematicSheetsViewModel>());
            tools.Add(ServiceProvider.GetToolWindow<DocumentOverviewViewModel>());
            tools.Add(ServiceProvider.GetToolWindow<SelectionFilterToolViewModel>());

            return tools;
        }

        public IList<ISchematicRuleModel> Rules { get; } = new List<ISchematicRuleModel>();

        IList<IOverviewSelectNode> BuildCategories()
        {
            var list = new List<IOverviewSelectNode>();
            var primitivesCat = new OverviewFolderNode { IsExpanded = false, Name = "Primitives" };
            var partsCat = new OverviewFolderNode { IsExpanded = false, Name = "Parts" };
            var netsCat = new OverviewFolderNode { IsExpanded = false, Name = "Nets" };
            list.Add(primitivesCat);
            list.Add(partsCat);
            list.Add(netsCat);
            var partItems = canvasModel.Items.OfType<SchematicSymbolCanvasItem>().ToList();
            var parts = partItems.OrderBy(p => p.PartName)
                                 .Select(p => new OverviewSelectNode
                                 {
                                     DataItem = p,
                                 });
            partsCat.Children.AddRange(parts);
            var netSegmentItems = canvasModel.Items.OfType<NetSegmentCanvasItem>().ToList();
            var primitiveItems = canvasModel.Items.Except(partItems).Except(netSegmentItems);
            primitivesCat.Children.AddRange(primitiveItems.Select(p => new OverviewSelectNode
            {
                DataItem = p
            }));

            //nets
            foreach (var net in GetNets().OrderBy(n => n.Name))
            {
                var netNode = new OverviewSelectNode { IsExpanded = false, DataItem = net };
                netNode.Children.AddRange(net.NetItems.Select(n => new OverviewSelectNode { DataItem = n }));
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

        #endregion

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
                        Type = typeof(LineSchematicCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Texts",
                        Type = typeof(TextCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Rectangles",
                        Type = typeof(RectangleCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Polygons",
                        Type = typeof(PolygonCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Circles",
                        Type = typeof(CircleCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Ellipses",
                        Type = typeof(EllipseCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Arcs",
                        Type = typeof(ArcCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Images",
                        Type = typeof(ImageCanvasItem)
                    },

                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Net wires",
                        Type = typeof(NetWireCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Junctions",
                        Type = typeof(JunctionCanvasItem)
                    },
                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Net Labels",
                        Type = typeof(NetLabelCanvasItem)
                    },


                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Bus wires",
                        Type = typeof(BusWireCanvasItem)
                    },

                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Bus labels",
                        Type = typeof(BusLabelCanvasItem)
                    },

                    new SelectionFilterItemViewModel
                    {
                        TooltipText = "Parts",
                        Type = typeof(SchematicSymbolCanvasItem)
                    }
                };

                return canSelectList;
            }
        }

        #region Fields



        SchematicDocument schematicDocument;
        // List<NetDesignerItem> netsCache = new List<NetDesignerItem>();

        public IList<ISchematicNet> GetNets()
        {
            //we need a faster way for this
            IList<ISchematicNet> nets = new List<ISchematicNet>();

            foreach (var sheet in Sheets)
            {
                var netGroups = sheet.Items.OfType<NetSegmentCanvasItem>().GroupBy(p => p.Net);

                nets.AddRange(netGroups.Select(ng => ng.Key));
            }

            return nets;
        }

        public void ClearHighlightedItems()
        {
            foreach (var net in GetNets())
                net.HighlightNet(false);
        }

        #endregion Fields

        ICommand addSymbolCommand;

        public ICommand AddSymbolCommand
        {
            get
            {
                if (addSymbolCommand == null)
                    addSymbolCommand = CreateCommand(p =>
                    {
                        canvasModel.ClearSelectedItems();

                        //remove the object if we had one placing
                        canvasModel.CancelPlacement();


                        var itemSelectDlg = new ItemSelectDialogViewModel();
                        itemSelectDlg.TemplateType = TemplateType.Symbol;
                        itemSelectDlg.ProjectModel = ParentProject;
                        if (itemSelectDlg.ShowDialog() == true)
                        {
                            var symbol = itemSelectDlg.SelectedItem.Document as Symbol;

                            if (symbol.Items != null)
                            {
                                var canvasItems = symbol.Items.Where(sItem => !(sItem is Pin)).Select(c => c.CreateDesignerItem()).ToList();

                                var group = new VolatileGroupCanvasItem
                                {
                                    Items = canvasItems.Cast<ISelectableItem>().ToList()
                                };
                                canvasModel.AddItem(group);
                                canvasModel.StartPlacement(group);
                            }
                        }
                        //#endif
                    }
                    );

                return addSymbolCommand;
            }
        }

        private ICommand replaceSelectedPartsCommand;
        public ICommand ReplaceSelectedPartsCommand
        {
            get
            {
                if (replaceSelectedPartsCommand == null)
                    replaceSelectedPartsCommand = CreateCommand((p) =>
                    {
                        if (ReplaceSelectedPartsCommandCanExecute())
                        {
                            ReplaceSelectedParts();
                        }
                    }
                    );

                return replaceSelectedPartsCommand;
            }
        }

        private void ReplaceSelectedParts()
        {
            var itemSelectDlg = new ItemSelectDialogViewModel();
            itemSelectDlg.TemplateType = TemplateType.Component;
            itemSelectDlg.ProjectModel = ProjectNode;
            if (itemSelectDlg.ShowDialog() == true)
            {
                var componentItemDisplay = itemSelectDlg.SelectedItem;
                if (componentItemDisplay != null)
                {

                }
            }
        }

        private bool ReplaceSelectedPartsCommandCanExecute()
        {
            var selectedParts = canvasModel.SelectedItems.OfType<SchematicSymbolCanvasItem>().ToList();
            //all are selected parts
            if (selectedParts.Count == canvasModel.SelectedItems.Count)
            {
                var partsHaveOneGate = selectedParts.All(p => p.ComponentDocument?.Gates?.Count == 1);
                if (partsHaveOneGate)
                {
                    var components = selectedParts.Where(p => !string.IsNullOrEmpty(p.ComponentName)).Select(p => p.ComponentName).Distinct();

                    //all parts have the same component
                    return selectedParts.Count == components.Count();
                }
            }
            return false;
        }

        ICommand showSchematicPropertiesCommand;

        public ICommand ShowSchematicPropertiesCommand
        {
            get
            {
                if (showSchematicPropertiesCommand == null)
                    showSchematicPropertiesCommand = CreateCommand((p) =>
                    {
                        schematicProperties.LoadFromCurrentSchematic(this);
                        SchematicViewMode = SchematicViewMode.Properties;
                    },
                    p => SchematicViewMode != SchematicViewMode.Properties);

                return showSchematicPropertiesCommand;
            }
        }

        ICommand showSchematicCommand;

        public ICommand ShowSchematicCommand
        {
            get
            {
                if (showSchematicCommand == null)
                    showSchematicCommand = CreateCommand((p) =>
                    {
                        SchematicViewMode = SchematicViewMode.Canvas;
                    },
                    p => SchematicViewMode != SchematicViewMode.Canvas);

                return showSchematicCommand;
            }
        }


        ICommand compileCommand;

        public ICommand CompileCommand
        {
            get
            {
                if (compileCommand == null)
                    compileCommand = CreateCommand(async p => await RunActiveCompiler());

                return compileCommand;
            }
        }
        async Task RunActiveCompiler()
        {
            await _activeCompiler.Compile(this);
        }

        SchematicViewMode schematicViewMode;
        public SchematicViewMode SchematicViewMode
        {
            get { return schematicViewMode; }
            set
            {
                schematicViewMode = value;
                OnPropertyChanged(nameof(SchematicViewMode));
            }
        }

        SchematicDesignerPropertiesViewModel schematicProperties;
        public SchematicDesignerPropertiesViewModel SchematicProperties
        {
            get { return schematicProperties; }
            set
            {
                if (schematicProperties != null)
                    schematicProperties.PropertyChanged -= SchematicProperties_PropertyChanged;
                schematicProperties = value;
                schematicProperties.PropertyChanged += SchematicProperties_PropertyChanged;

                OnPropertyChanged(nameof(SchematicProperties));
            }
        }

        void SchematicProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        #region Caching

        //protected override LibraryItem GetLibraryItem()
        //{
        //    BuildDocument();
        //    return schematicDocument;
        //}


        protected override void RefreshFromCache()
        {

            if (State != DocumentState.IsEditing)
                return;


            foreach (var sheet in Sheets)
            {
                //we could improve by updating only what we updated
                foreach (var p in sheet.Items.OfType<SchematicSymbolCanvasItem>())
                {
                    p.Part = p.Part;
                    p.LoadFromPrimitive(p.SaveToPrimitive());//this should reload the items
                }
            }
        }

        #endregion Caching

        #region Toolbox



        public IList<ISheetDesignerItem> Sheets { get; set; }

        ISheetDesignerItem currentSheet;

        public ISheetDesignerItem CurrentSheet
        {
            get { return currentSheet; }
            set
            {
                currentSheet = value;
                if (currentSheet != null)
                {
                    canvasModel.Items = currentSheet.Items;
                    // CurrentSheetSetConnectionStatus();
                }

                OnPropertyChanged(nameof(CurrentSheet));
            }
        }

        ICommand addSheetCommand;
        public ICommand AddSheetCommand
        {
            get
            {
                if (addSheetCommand == null)
                    addSheetCommand = CreateCommand(p =>
                      {
                          var newSheet = new SheetDesignerItem { Name = "New Sheet" };
                          Sheets.Add(newSheet);
                          CurrentSheet = newSheet;
                      });

                return addSheetCommand;
            }
        }

        ICommand deleteSheetCommand;

        public ICommand DeleteSheetCommand
        {
            get
            {
                if (deleteSheetCommand == null)
                    deleteSheetCommand = CreateCommand(p =>
                    {
                        if (MessageDialog.Show("Are you sure you want to delete this sheet?",
                            "Confirm delete",
                             XMessageBoxButton.YesNo) == XMessageBoxResult.Yes)
                        {
                            if (currentSheet != null)
                            {
                                Sheets.Remove(currentSheet);
                                CurrentSheet = Sheets.FirstOrDefault();
                            }
                        }


                    });

                return deleteSheetCommand;
            }
        }

        ICommand moveUpSheetCommand;
        public ICommand MoveUpSheetCommand
        {
            get
            {
                if (moveUpSheetCommand == null)
                    moveUpSheetCommand = CreateCommand(p =>
                    {
                        try
                        {
                            Sheets.MoveUp(currentSheet);
                        }
                        catch { }

                    });

                return moveUpSheetCommand;
            }
        }

        ICommand moveDownSheetCommand;
        public ICommand MoveDownSheetCommand
        {
            get
            {
                if (moveDownSheetCommand == null)
                    moveDownSheetCommand = CreateCommand(p =>
                    {
                        try
                        {
                            Sheets.MoveDown(currentSheet);
                        }
                        catch { }

                    });

                return moveDownSheetCommand;
            }
        }

        protected override void InitToolbox()
        {
            base.InitToolbox();


            //add the specific primitives for the symbols
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Line",
                Type = typeof(LineSchematicCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Text",
                Type = typeof(TextCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Rectangle",
                Type = typeof(RectangleCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Polygon",
                Type = typeof(PolygonCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Circle",
                Type = typeof(CircleCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Ellipse",
                Type = typeof(EllipseCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Arc",
                Type = typeof(ArcCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Image",
                Type = typeof(ImageCanvasItem)
            });

            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Net wire",
                Type = typeof(NetWireCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Junction",
                Type = typeof(JunctionCanvasItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Net Label",
                Type = typeof(NetLabelCanvasItem)
            });


            //#if DEBUG
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Bus wire",
                Type = typeof(BusWireCanvasItem)
            });

            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Bus label",
                Type = typeof(BusLabelCanvasItem)
            });
            //#endif

            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Add new part",
                Type = typeof(SchematicSymbolCanvasItem)
            });

        }

        #endregion Toolbox

        #region Save Command

        protected override void SaveDocumentInternal(string filePath)
        {
            //remove the currently adding item si that it won't be saved
            ISelectableItem placeObjects = null;
            if (canvasModel.IsPlacingItem())
            {
                placeObjects = canvasModel.PlacementTool.CanvasItem;
                canvasModel.RemoveItem(placeObjects);
            }

            BuildDocument();
            schematicProperties.SaveToSchematic(schematicDocument);

            XmlHelper.Save(schematicDocument, filePath);

            //add the item back
            if (placeObjects != null)
                canvasModel.AddItem(placeObjects);
        }

        void BuildDocument()
        {
            var partsList = new List<Part>();

            schematicDocument.DocumentWidth = canvasModel.DocumentWidth;
            schematicDocument.DocumentHeight = canvasModel.DocumentHeight;
            schematicDocument.DocumentSize = canvasModel.DocumentSize;

            schematicDocument.Sheets = new List<Sheet>();
            foreach (var sheet in Sheets)
            {
                var newSheet = new Sheet { Name = sheet.Name };
                schematicDocument.Sheets.Add(newSheet);

                #region Plain


                var sheetItems = sheet.Items.OfType<IPlainDesignerItem>().Cast<BaseCanvasItem>().Where(d => d.IsPlaced)
                                            .Select(d => (SchematicPrimitive)d.SaveToPrimitive()).ToList();
                newSheet.PlainItems = sheetItems;

                #endregion Plain

                #region Nets

                newSheet.Nets = new List<Net>();
                //var netItems = sheet.Items.OfType<NetSegmentCanvasItem>().ToList();

                var netGroups = sheet.Items.OfType<NetSegmentCanvasItem>().GroupBy(p => p.Net);

                foreach (var sheetNetGroup in netGroups)
                {
                    var sheetNet = sheetNetGroup.Key;

                    var net = newSheet.Nets.FirstOrDefault(n => n.Id == sheetNet.Id);
                    if (net == null)
                    {
                        net = new Net
                        {
                            Id = sheetNet.Id,
                            Name = sheetNet.Name,
                            ClassId = sheetNet.ClassId
                        };
                        newSheet.Nets.Add(net);
                    }

                    if (net.Items == null)
                        net.Items = new List<NetSegmentItem>();

                    foreach (var netItem in sheetNet.NetItems)
                    {
                        if (netItem is PinCanvasItem pin)
                        {
                            net.Items.Add(new PinRef
                            {
                                PartInstanceId = pin.PartInstanceId,
                                Pin = pin.Number
                            });
                        }
                        else
                        {

                            net.Items.Add((NetSegmentItem)netItem.SaveToPrimitive());
                        }
                    }
                }

                #endregion Nets

                #region Buses

                var busGroups = sheet.Items.OfType<BusSegmentCanvasItem>().GroupBy(p => p.Bus);

                foreach (var sheetBusGroup in busGroups)
                {
                    var sheetBus = sheetBusGroup.Key;

                    if (sheetBus == null)
                        sheetBus = new SchematicBus { Name = "unknown" };

                    var bus = newSheet.Busses.FirstOrDefault(b => b.Name == sheetBus.Name);

                    if (bus == null)
                    {
                        bus = new Bus();

                        newSheet.Busses.Add(bus);
                    }

                    bus.Name = sheetBus.Name;
                    bus.Nets = sheetBus.Nets.Select(n => new BusNet { Name = n }).ToList();
                    bus.Items = sheetBus.BusItems.Select(bi => (BusSegmentItem)bi.SaveToPrimitive()).ToList();


                }

                #endregion

                #region Parts

                var schematicSymbols = sheet.Items.OfType<SchematicSymbolCanvasItem>().ToList();

                foreach (var schematicSymbol in schematicSymbols)
                {
                    //add the part as needed; identify by part id
                    var currentPart = schematicSymbol.Part;
                    var part = partsList.FirstOrDefault(p => p.Id == currentPart.Id);
                    if (part == null)
                        partsList.Add(currentPart);

                }

                newSheet.Instances = schematicSymbols.Select(s => (Instance)s.SaveToPrimitive()).ToList();

                #endregion Parts
            }

            schematicDocument.Parts = partsList;

            //rules
            schematicDocument.Rules = Rules.Cast<AbstractSchematicRule>().Select(r => r.SaveToSchematicRule()).ToList();
        }

        public List<Part> GetSchematicParts()
        {
            var partsList = new List<Part>();

            foreach (var sheet in Sheets)
            {
                var schematicSymbols = sheet.Items.OfType<SchematicSymbolCanvasItem>()
                                            .Where(s => s.ComponentDocument?.Type != ComponentType.StandardNoBom)
                                            .ToList();

                foreach (var schematicSymbol in schematicSymbols)
                {
                    //add the part as needed; identify by part id
                    var currentPart = schematicSymbol.Part;
                    var part = partsList.FirstOrDefault(p => p.Id == currentPart.Id);
                    if (part == null)
                        partsList.Add(currentPart);

                }
            }

            partsList.Sort(new PartNameComparer());

            return partsList;
        }

        #endregion Save Command

        #region LoadFile


        protected override Task LoadDocumentInternal(string filePath)
        {
            return Task.Run(() =>
            {
                schematicDocument = XmlHelper.Load<SchematicDocument>(filePath);

                //assign a new id if needed
                if (schematicDocument.Id == 0)
                {
                    schematicDocument.Id = LibraryItem.GetNextId();
                    IsDirty = true;
                }


                canvasModel.DocumentWidth = schematicDocument.DocumentWidth;
                canvasModel.DocumentHeight = schematicDocument.DocumentHeight;
                canvasModel.DocumentSize = schematicDocument.DocumentSize;

                //load rules
                Rules.Clear();
                schematicDocument.EnsureDefaultRules();
                Rules.AddRange(schematicDocument.Rules.Select(r => r.CreateRuleItem()));

                //var netsCache = new List<SchematicNet>();
                //var buses = new List<SchematicBus>();

                //var projectNode = (SolutionProjectNodeModel)ProjectNode;
                //if (projectNode != null)
                //{
                //    projectNode.CreateCacheItems(TemplateType.Component);
                //    projectNode.CreateCacheItems(TemplateType.Symbol);
                //}

                if (schematicDocument.Sheets != null)
                {
                    //register all nets and buses from all sheets in the net manager
                    foreach (var sheet in schematicDocument.Sheets)
                    {
                        //nets
                        if (sheet.Nets != null)
                        {
                            foreach (var netDoc in sheet.Nets)
                            {
                                var net = new SchematicNet
                                {
                                    Id = netDoc.Id,
                                    ClassId = netDoc.ClassId,
                                    Name = netDoc.Name,
                                };
                                netManager.Add(net);
                            }
                        }

                        //buses
                        if (sheet.Busses != null)
                        {
                            foreach (var busDoc in sheet.Busses)
                            {
                                var bus = new SchematicBus
                                {
                                    Name = busDoc.Name,
                                };
                                busManager.Add(bus);
                            }
                        }
                    }

                    foreach (var sheet in schematicDocument.Sheets)
                    {
                        var sheetItem = new SheetDesignerItem { Name = sheet.Name };


                        #region Plain

                        //plain (basic primitives)
                        if (sheet.PlainItems != null)
                        {
                            foreach (var primitive in sheet.PlainItems)
                            {
                                var canvasItem = primitive.CreateDesignerItem();
                                sheetItem.Items.Add(canvasItem);
                            }
                        }

                        #endregion Plain

                        #region Components

                        if (sheet.Instances != null && schematicDocument.Parts != null)
                        {
                            foreach (var instance in sheet.Instances)
                            {
                                var part = schematicDocument.Parts.FirstOrDefault(p => p.Id == instance.PartId);
                                if (part != null)
                                {
                                    var symbolItem = new SchematicSymbolCanvasItem();
                                    symbolItem.ProjectModel = (SolutionProjectNodeModel)ParentProject;
                                    symbolItem.Part = part;
                                    symbolItem.LoadFromPrimitive(instance);
                                    sheetItem.Items.Add(symbolItem);
                                }

                            }
                        }

                        #endregion Components

                        #region Nets

                        //nets
                        if (sheet.Nets != null)
                        {


                            foreach (var netDoc in sheet.Nets)
                            {
                                var net = (SchematicNet)netManager.Get(netDoc.Name);

                                /* old lookup
                                ////lookup the reference
                                //var net = netsCache.FirstOrDefault(n => n.Id == netDoc.Id);

                                //if (net == null)
                                //{
                                //    net = new SchematicNet
                                //    {
                                //        Id = netDoc.Id,
                                //        ClassId = netDoc.ClassId,
                                //        Name = netDoc.Name,
                                //        CanvasModel = CanvasModel
                                //    };

                                //    netsCache.Add(net);
                                //}
                                */

                                if (netDoc.Items != null)
                                {
                                    foreach (var primitive in netDoc.Items)
                                    {
                                        if (primitive is PinRef pinRef)
                                        {
                                            var part = sheetItem.Items.OfType<SchematicSymbolCanvasItem>()
                                                                      .FirstOrDefault(p => p.SymbolPrimitive.Id == pinRef.PartInstanceId);
                                            if (part != null)
                                            {
                                                var pin = part.Pins.FirstOrDefault(p => p.Number == pinRef.Pin);
                                                if (pin != null)
                                                    pin.Net = net;
                                            }
                                        }
                                        else
                                        {
                                            var canvasItem = primitive.CreateDesignerItem();
                                            if (canvasItem is NetSegmentCanvasItem netSegment)
                                            {
                                                netSegment.Net = net;
                                            }
                                            sheetItem.Items.Add(canvasItem);

                                        }

                                    }
                                }

                            }
                        }

                        #endregion Nets

                        #region Busses

                        if (sheet.Busses != null)
                        {
                            foreach (var busDoc in sheet.Busses)
                            {
                                ////lookup the reference
                                //var bus = buses.FirstOrDefault(n => n.Name == busDoc.Name);
                                //if (bus == null)
                                //{
                                //    bus = new SchematicBus
                                //    {
                                //        Name = busDoc.Name,
                                //        CanvasModel = CanvasModel
                                //    };

                                //    buses.Add(bus);
                                //}
                                var bus = (SchematicBus)busManager.Get(busDoc.Name);

                                foreach (var net in busDoc.Nets)
                                    bus.AddNet(net.Name);

                                foreach (var primitive in busDoc.Items)
                                {
                                    var canvasItem = primitive.CreateDesignerItem();
                                    if (canvasItem is BusSegmentCanvasItem busSegment)
                                    {
                                        busSegment.Bus = bus;
                                    }
                                    sheetItem.Items.Add(canvasItem);
                                }
                            }
                        }

                        #endregion Busses


                        _dispatcher.RunOnDispatcher(() =>
                        {
                            Sheets.Add(sheetItem);
                        });
                    }
                }

                //if (projectNode != null)
                //    projectNode.ClearCachedItems();

                //create a new sheet if neeeded
                if (Sheets.Count == 0)
                {
                    Sheets.Add(new SheetDesignerItem { Name = "Main sheet" });
                }

                CurrentSheet = Sheets[0];

                _dispatcher.RunOnDispatcher(() =>
                {
                    schematicProperties.LoadFromSchematic(schematicDocument);
                });
            });
        }

        protected override async Task AfterLoadDocumentInternal()
        {
            await base.AfterLoadDocumentInternal();
            await RefreshOverview();
        }

        #endregion LoadFile

        public override void ApplySettings()
        {
            var settingsManager = _settingsManager;
            if (settingsManager != null)
            {
                var schColorsSettings = settingsManager.GetSetting<SchematicEditorColorsSetting>();
                if (schColorsSettings == null)
                    return;

                canvasModel.CanvasBackground = XColor.FromHexString(schColorsSettings.CanvasBackground);
                canvasModel.GridColor = XColor.FromHexString(schColorsSettings.GridColor);
            }

        }
    }
}
