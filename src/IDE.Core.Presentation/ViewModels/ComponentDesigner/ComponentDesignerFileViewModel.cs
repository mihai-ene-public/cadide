using System;
using System.IO;
using System.Windows.Input;
using IDE.Core.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IDE.Core.Commands;
using IDE.Core.Designers;
using IDE.Core.Storage;
using IDE.Core.Utilities;
using IDE.Core;
using IDE.Core.BOM;
using IDE.Core.Interfaces;
using IDE.Core.Common;
using System.Threading.Tasks;
using IDE.Core.Types.Media;

namespace IDE.Documents.Views
{
    // in the future we add BOM data: Suplier, Manuf, Descr, Price, ...
    // datasheet linking
    public class ComponentDesignerFileViewModel : FileBaseViewModel, IComponentDesigner
    {

        public ComponentDesignerFileViewModel()
            : base(null)
        {


            DocumentKey = "Component Editor";
            Description = "component files";
            FileFilterName = "component file";
            DefaultFilter = "component";
            documentTypeKey = DocumentKey;

            //make these abstract properties
            defaultFileType = "component";
            defaultFileName = "Component";


            IsDirty = false;

            componentDocument = new ComponentDocument();

            Gates = new ObservableCollection<GateDisplay>();
            Properties = new ObservableCollection<PropertyDisplay>();
            BomItems = new ObservableCollection<BomItemDisplay>();

            dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
        }

        #region Fields


        IDispatcherHelper dispatcher;

        ComponentDocument componentDocument;

        #endregion Fields

        public override object Document
        {
            get
            {
                return componentDocument;
            }
        }


        protected override void RefreshFromCache()
        {
            if (State != DocumentState.IsEditing)
                return;

            RefreshGates();
            LoadFootprint();

            OnPropertyChanged(nameof(Gates));
            OnPropertyChanged(nameof(Footprint));
        }


        #region Commands


        ICommand addGateCommand;
        public ICommand AddGateCommand
        {
            get
            {
                if (addGateCommand == null)
                    addGateCommand = CreateCommand((p) =>
                    {
                        var project = (Item as SolutionExplorerNodeModel).ProjectNode;
                        if (project == null)
                            throw new Exception("This component does not belong to any project");

                        var itemSelectDlg = new ItemSelectDialogViewModel();
                        itemSelectDlg.TemplateType = TemplateType.Symbol;
                        itemSelectDlg.ProjectModel = project;
                        if (itemSelectDlg.ShowDialog() == true)
                        {
                            var symbol = itemSelectDlg.SelectedItem.Document as Symbol;
                            var g = new GateDisplay
                            {
                                Gate = new Gate
                                {
                                    name = "newGateName",
                                },
                                Symbol = symbol
                            };
                            long newGateId = 1;
                            if (Gates.Count > 0)
                                newGateId = Gates.Max(gg => gg.Gate.Id) + 1;
                            g.Gate.Id = newGateId;

                            g.Gate.symbolId = g.Symbol.Id;
                            g.Gate.symbolName = g.Symbol.Name;

                            if (g.Symbol.Items != null)
                            {
                                // g.Preview.SetPrimitives(g.Symbol.GetDesignerPrimitiveItems());
                                g.Preview.PreviewDocument(g.Symbol, null);
                            }

                            Gates.Add(g);

                            RefreshGateNames();
                            RefreshConnects();

                            IsDirty = true;
                        }
                    }
                    ,
                    (p) =>
                    {
                        return true;
                    });

                return addGateCommand;
            }
        }

        ICommand importComponentCommand;
        public ICommand ImportComponentCommand
        {
            get
            {
                if (importComponentCommand == null)
                {
                    importComponentCommand = CreateCommand(p =>
                      {
                          var project = (Item as SolutionExplorerNodeModel).ProjectNode;
                          if (project == null)
                              throw new Exception("This component does not belong to any project");

                          var itemSelectDlg = new ItemSelectDialogViewModel();
                          itemSelectDlg.TemplateType = TemplateType.Component;
                          itemSelectDlg.ProjectModel = project;

                          if (itemSelectDlg.ShowDialog() == true)
                          {
                              var oldId = componentDocument.Id;
                              componentDocument = itemSelectDlg.SelectedItem.Document as ComponentDocument;

                              //redo old stuff that are important
                              componentDocument.Id = oldId;
                              componentDocument.Library = null;

                              LoadComponentDocument();

                              IsDirty = true;
                          }
                      });
                }

                return importComponentCommand;
            }
        }


        ICommand moveUpSymbolCommand;
        public ICommand MoveUpSymbolCommand
        {
            get
            {
                if (moveUpSymbolCommand == null)
                    moveUpSymbolCommand = CreateCommand((p) =>
                    {
                        var thisSymbol = p as GateDisplay;
                        if (thisSymbol == null)
                            throw new Exception("There is no gate associated with this item");

                        var index = Gates.IndexOf(thisSymbol);
                        index--;
                        if (index <= 0)
                            index = 0;
                        Gates.Remove(thisSymbol);
                        Gates.Insert(index, thisSymbol);

                        RefreshGateNames();
                        RefreshConnects();

                        IsDirty = true;
                    }
                    ,
                    (p) =>
                    {
                        return true;
                    });

                return moveUpSymbolCommand;
            }
        }

        ICommand moveDownSymbolCommand;
        public ICommand MoveDownSymbolCommand
        {
            get
            {
                if (moveDownSymbolCommand == null)
                    moveDownSymbolCommand = CreateCommand((p) =>
                    {
                        var thisSymbol = p as GateDisplay;
                        if (thisSymbol == null)
                            throw new Exception("There is no gate associated with this item");

                        var index = Gates.IndexOf(thisSymbol);
                        index++;
                        if (index > Gates.Count - 1)
                            index = Gates.Count - 1;
                        Gates.Remove(thisSymbol);
                        Gates.Insert(index, thisSymbol);

                        RefreshGateNames();
                        RefreshConnects();

                        IsDirty = true;
                    }
                    ,
                    (p) =>
                    {
                        return true;
                    });

                return moveDownSymbolCommand;
            }
        }

        ICommand deleteSymbolCommand;
        public ICommand DeleteSymbolCommand
        {
            get
            {
                if (deleteSymbolCommand == null)
                    deleteSymbolCommand = CreateCommand((p) =>
                    {
                        var thisSymbol = p as GateDisplay;
                        if (thisSymbol == null)
                            throw new Exception("There is no gate associated with this item");

                        Gates.Remove(thisSymbol);

                        RefreshGateNames();
                        RefreshConnects();

                        IsDirty = true;
                    }
                    ,
                    (p) =>
                    {
                        return true;
                    });

                return deleteSymbolCommand;
            }
        }

        //ChangeSymbolCommand

        ICommand changeSymbolCommand;
        public ICommand ChangeSymbolCommand
        {
            get
            {
                if (changeSymbolCommand == null)
                    changeSymbolCommand = CreateCommand((p) =>
                    {
                        var project = (Item as SolutionExplorerNodeModel).ProjectNode;
                        if (project == null)
                            throw new Exception("This component does not belong to any project");

                        var thisSymbol = p as GateDisplay;
                        if (thisSymbol == null)
                            throw new Exception("There is no gate associated with this item");

                        var itemSelectDlg = new ItemSelectDialogViewModel();
                        itemSelectDlg.TemplateType = TemplateType.Symbol;
                        itemSelectDlg.ProjectModel = project;
                        if (itemSelectDlg.ShowDialog() == true)
                        {
                            thisSymbol.Symbol = itemSelectDlg.SelectedItem.Document as Symbol;
                            thisSymbol.Gate.symbolId = thisSymbol.Symbol.Id;

                            //thisSymbol.Canvas.Items.Clear();
                            if (thisSymbol.Symbol.Items != null)
                            {
                                //thisSymbol.Preview.SetPrimitives(thisSymbol.Symbol.GetDesignerPrimitiveItems());
                                thisSymbol.Preview.PreviewDocument(thisSymbol.Symbol, null);
                            }

                            thisSymbol.Preview.ZoomToFit();

                            RefreshGateNames();
                            RefreshConnects();

                            IsDirty = true;
                        }

                    }
                    ,
                    (p) =>
                    {
                        return true;
                    });

                return changeSymbolCommand;
            }
        }



        ICommand deleteFootprintCommand;
        public ICommand DeleteFootprintCommand
        {
            get
            {
                if (deleteFootprintCommand == null)
                    deleteFootprintCommand = CreateCommand((p) =>
                    {
                        //var thisFootprint = p as FootprintDisplay;
                        //if (thisFootprint == null)
                        //    throw new Exception("There is no footprint associated with this item");

                        //Footprints.Remove(thisFootprint);
                        Footprint = null;

                        IsDirty = true;
                    }
                    ,
                    (p) =>
                    {
                        return true;
                    });

                return deleteFootprintCommand;
            }
        }

        ICommand changeFootprintCommand;
        public ICommand ChangeFootprintCommand
        {
            get
            {
                if (changeFootprintCommand == null)
                    changeFootprintCommand = CreateCommand(p =>
                    {
                        var project = (Item as SolutionExplorerNodeModel).ProjectNode;
                        if (project == null)
                            throw new Exception("This component does not belong to any project");

                        //var thisFootprint = p as FootprintDisplay;
                        //if (thisFootprint == null)
                        //    throw new Exception("There is no footprint associated with this item");

                        var itemSelectDlg = new ItemSelectDialogViewModel();
                        itemSelectDlg.TemplateType = TemplateType.Footprint;
                        itemSelectDlg.ProjectModel = project;
                        if (itemSelectDlg.ShowDialog() == true)
                        {
                            var fpDoc = itemSelectDlg.SelectedItem.Document as Footprint;
                            var footprint = new FootprintDisplay
                            {
                                //Canvas = new DrawingViewModel(),

                                Device = new FootprintRef
                                {
                                    footprintId = fpDoc.Id,
                                    footprintName = fpDoc.Name,
                                    LibraryName = fpDoc.Library,
                                },

                                Footprint = fpDoc,
                                Name = itemSelectDlg.SelectedItem.Name,
                            };

                            //if (footprint.Footprint.Items != null)
                            //{
                            //    var layeredDoc = LoadFootprintLayers(fpDoc);

                            //    var primitives = new List<ISelectableItem>();
                            //    foreach (var primitive in fpDoc.Items)
                            //    {
                            //        var canvasItem = (BoardCanvasItemViewModel)primitive.CreateDesignerItem();
                            //        canvasItem.LayerDocument = layeredDoc;
                            //        canvasItem.LoadLayers();
                            //        primitives.Add(canvasItem);
                            //    }
                            //    // footprint.Preview.SetPrimitives(primitives);
                            //    footprint.Preview.PreviewDocument(g.Symbol, null);
                            //}

                            footprint.Preview.PreviewDocument(fpDoc, null);

                            footprint.Preview.ZoomToFit();

                            Footprint = footprint;

                            RefreshConnects();

                            IsDirty = true;
                        }
                    }
                    ,
                    (p) =>
                    {
                        return true;
                    });

                return changeFootprintCommand;
            }
        }

        ILayeredViewModel LoadFootprintLayers(Footprint fp)
        {
            //load layers
            var layeredDoc = new GenericLayeredViewModel();
            IList<Layer> layers = null;
            if (fp.Layers != null && fp.Layers.Count > 0)
            {
                layers = fp.Layers;
            }
            else
            {
                layers = IDE.Core.Storage.Footprint.CreateDefaultLayers();
            }
            fp.Layers = layers.ToList();
            var groups = LayerGroup.GetLayerGroupDefaults(layers);

            //var layerOrder = 0;
            var layerItems = layers.Select(l => new LayerDesignerItem(layeredDoc)
            {
                LayerName = l.Name,
                LayerId = l.Id,
                LayerType = l.Type,
                LayerColor = XColor.FromHexString(l.Color),
                //LayerOrder = layerOrder++
            }).ToList();
            layeredDoc.LayerItems.Clear();
            layeredDoc.LayerItems.AddRange(layerItems);

            return layeredDoc;
        }


        ICommand moveBomItemDownCommand;
        public ICommand MoveBomItemDownCommand
        {
            get
            {
                if (moveBomItemDownCommand == null)
                    moveBomItemDownCommand = CreateCommand(p =>
                    {
                        if (bomSelectedItem != null)
                        {
                            var oldIndex = BomItems.IndexOf(bomSelectedItem);
                            var newIndex = oldIndex + 1;
                            if (newIndex < BomItems.Count)
                            {
                                try { MoveLayers(oldIndex, newIndex); } catch { }
                                //stackLayersView.Refresh();
                            }
                        }
                    });

                return moveBomItemDownCommand;
            }

        }

        void MoveLayers(int oldIndex, int newIndex)
        {
            BomItems.Move(oldIndex, newIndex);
        }

        ICommand moveBomItemUpCommand;
        public ICommand MoveBomItemUpCommand
        {
            get
            {
                if (moveBomItemUpCommand == null)
                    moveBomItemUpCommand = CreateCommand(p =>
                    {
                        if (bomSelectedItem != null)
                        {
                            var oldIndex = BomItems.IndexOf(bomSelectedItem);
                            var newIndex = oldIndex - 1;
                            if (newIndex >= 0)
                            {
                                try { MoveLayers(oldIndex, newIndex); } catch { }
                                //stackLayersView.Refresh();
                            }

                        }
                    });

                return moveBomItemUpCommand;
            }

        }

        //ICommand searchBomCommand;

        //public ICommand SearchBomCommand
        //{
        //    get
        //    {
        //        if (searchBomCommand == null)
        //            searchBomCommand = CreateCommand(p =>
        //            {
        //                try
        //                {
        //                    var bomSearchWindow = new BomSearchViewModel();
        //                    if (bomSearchWindow.ShowDialog() == true)
        //                    {
        //                        // var model = bomSearchWindow.Model;
        //                        if (bomSearchWindow.SelectedItem != null)
        //                        {
        //                            BomItems.Add(bomSearchWindow.SelectedItem);
        //                            //add properties
        //                            if (bomSearchWindow.AddParameters)
        //                            {
        //                                var itemProps = bomSearchWindow.SelectedItem.Properties;
        //                                foreach (var prop in itemProps)
        //                                {
        //                                    var existingProp = Properties.FirstOrDefault(pp => pp.Name == prop.Name);
        //                                    if (existingProp != null)
        //                                    {
        //                                        existingProp.Value = prop.Value;
        //                                    }
        //                                    else
        //                                    {
        //                                        Properties.Add(new PropertyDisplay
        //                                        {
        //                                            Name = prop.Name,
        //                                            Value = prop.Value
        //                                        });
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageDialog.Show(ex.Message);
        //                }
        //            });

        //        return searchBomCommand;
        //    }
        //}

        private ICommand addBomCommand;

        public ICommand AddBomCommand
        {
            get
            {
                if (addBomCommand == null)
                    addBomCommand = CreateCommand(p =>
                      {
                          var newBomItem = new BomItemDisplay();
                          BomItems.Add(newBomItem);
                          BomEditingItem = newBomItem;

                      });

                return addBomCommand;
            }
        }

        private ICommand editBomCommand;

        public ICommand EditBomCommand
        {
            get
            {
                if (editBomCommand == null)
                    editBomCommand = CreateCommand(p =>
                    {
                        BomEditingItem = BomSelectedItem;
                    });

                return editBomCommand;
            }
        }

        private ICommand backToBomCommand;

        public ICommand BackToBomCommand
        {
            get
            {
                if (backToBomCommand == null)
                    backToBomCommand = CreateCommand(p =>
                    {
                        BomEditingItem = null;
                    });

                return backToBomCommand;
            }
        }


        #endregion Commands

        public ObservableCollection<GateDisplay> Gates { get; set; }

        //public ObservableCollection<FootprintDisplay> Footprints { get; set; }

        FootprintDisplay footprint;
        public FootprintDisplay Footprint
        {
            get
            {
                return footprint;
            }
            set
            {
                footprint = value;
                OnPropertyChanged(nameof(Footprint));
            }
        }

        string prefix;
        public string Prefix
        {
            get { return prefix; }
            set
            {
                prefix = value;
                IsDirty = true;
                OnPropertyChanged(nameof(Prefix));
            }
        }

        string comment;
        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                IsDirty = true;
                OnPropertyChanged(nameof(Comment));
            }
        }

        string compDescription;
        public string ComponentDescription
        {
            get { return compDescription; }
            set
            {
                compDescription = value;
                IsDirty = true;
                OnPropertyChanged(nameof(ComponentDescription));
            }
        }

        string _namespace;
        public string Namespace
        {
            get { return _namespace; }
            set
            {
                _namespace = value;
                IsDirty = true;
                OnPropertyChanged(nameof(Namespace));
            }
        }

        ComponentType componentType;
        public ComponentType ComponentType
        {
            get { return componentType; }
            set
            {
                componentType = value;
                IsDirty = true;
                OnPropertyChanged(nameof(ComponentType));
            }
        }

        public ObservableCollection<PropertyDisplay> Properties { get; set; }

        public ObservableCollection<BomItemDisplay> BomItems { get; set; }

        BomItemDisplay bomSelectedItem;
        public BomItemDisplay BomSelectedItem
        {
            get { return bomSelectedItem; }
            set
            {
                bomSelectedItem = value;
                OnPropertyChanged(nameof(BomSelectedItem));
            }
        }

        BomItemDisplay bomEditingItem;
        public BomItemDisplay BomEditingItem
        {
            get { return bomEditingItem; }
            set
            {
                bomEditingItem = value;
                OnPropertyChanged(nameof(BomEditingItem));
                OnPropertyChanged(nameof(IsBomEditing));

                OnPropertyChanged(nameof(IsBomSearchEnabled));

            }
        }

        public bool IsBomEditing => BomEditingItem != null;

        public bool IsBomSearchEnabled => !IsBomEditing;

        protected override void SaveDocumentInternal(string filePath)
        {
            BuildDocument();

            XmlHelper.Save(componentDocument, filePath);
        }

        void BuildDocument()
        {
            componentDocument.Name = Path.GetFileNameWithoutExtension(FilePath);

            foreach (var g in Gates)
                g.Gate.LibraryName = g.Symbol.Library;
            componentDocument.Gates = Gates.Select(g => g.Gate).ToList();

            //update connects
            //foreach (var fp in Footprints)
            var fp = Footprint;
            if (fp != null)
            {
                if (fp.Connects == null)
                    fp.Device.Connects = null;
                else
                {
                    fp.Device.Connects = fp.Connects.Select(c => new Connect
                    {
                        gateId = c.GateId,
                        pad = c.Pad,
                        pin = c.Pin.ToString()
                    }).ToList();
                }

                componentDocument.Footprint = Footprint.Device;//Footprints.Select(f => f.Device).ToList();
                componentDocument.Footprint.LibraryName = footprint.Footprint.Library;
                componentDocument.Footprint.footprintName = footprint.Footprint.Name;
            }
            else
            {
                componentDocument.Footprint = null;
            }

            //Properties
            componentDocument.Prefix = Prefix;
            componentDocument.Comment = Comment;
            componentDocument.Description = ComponentDescription;
            componentDocument.Type = ComponentType;
            componentDocument.Namespace = Namespace;
            //Properties list
            componentDocument.Properties = Properties.Select(p => new Property
            {
                Name = p.Name,
                Value = p.Value,
                Type = p.PropertyType
            }).ToList();

            //bom
            componentDocument.BomItems = BomItems.Select(b => new BomItem
            {
                Currency = b.Currency,
                Documents = b.Documents,
                Description = b.Description,
                ImageURLMedium = b.ImageURLMedium,
                ImageURLSmall = b.ImageURLSmall,
                Manufacturer = b.Manufacturer,
                MPN = b.MPN,
                Package = b.Package,
                Packaging = b.Packaging,
                Prices = b.Prices,
                Properties = b.Properties,
                RoHS = b.RoHS,
                Sku = b.Sku,
                Stock = b.Stock,
                Supplier = b.Supplier
            }).ToList();
        }

        protected override Task LoadDocumentInternal(string filePath)
        {
            return Task.Run(() =>
            {
                componentDocument = XmlHelper.Load<ComponentDocument>(filePath);

                //assign a new id if needed
                if (componentDocument.Id == 0)
                {
                    componentDocument.Id = LibraryItem.GetNextId();
                    IsDirty = true;
                }

                LoadComponentDocument();

                IsDirty = false;
            });
        }

        void LoadComponentDocument()
        {
            FixLocals();

            //Properties
            Prefix = componentDocument.Prefix;
            Comment = componentDocument.Comment;
            ComponentDescription = componentDocument.Description;
            ComponentType = componentDocument.Type;
            Namespace = componentDocument.Namespace;
            //Properties list
            Properties.Clear();
            if (componentDocument.Properties != null)
                Properties.AddRange(componentDocument.Properties.Select(p => new PropertyDisplay
                {
                    Name = p.Name,
                    Value = p.Value,
                    PropertyType = p.Type
                }));

            //BOM
            BomItems.Clear();
            if (componentDocument.BomItems != null)
            {
                BomItems.AddRange(componentDocument.BomItems.Select(b => new BomItemDisplay
                {
                    Currency = b.Currency,
                    Documents = b.Documents,
                    Description = b.Description,
                    ImageURLMedium = b.ImageURLMedium,
                    ImageURLSmall = b.ImageURLSmall,
                    Manufacturer = b.Manufacturer,
                    MPN = b.MPN,
                    Package = b.Package,
                    Packaging = b.Packaging,
                    Prices = b.Prices,
                    Properties = b.Properties,
                    RoHS = b.RoHS,
                    Sku = b.Sku,
                    Stock = b.Stock,
                    Supplier = b.Supplier
                }));
            }

            //   var project = (Item as SolutionExplorerNodeModel).ProjectNode;
            LoadGates();
            LoadFootprint();
        }

        void FixLocals()
        {
            if (componentDocument != null && componentDocument.IsLocal == false)
            {
                if (componentDocument.Gates != null)
                {
                    foreach (var g in componentDocument.Gates)
                    {
                        if (g.LibraryName == "local")
                        {
                            g.LibraryName = componentDocument.Library;
                        }
                    }
                }

                if (componentDocument.Footprint != null)
                {
                    if (componentDocument.Footprint.LibraryName == "local")
                    {
                        componentDocument.Footprint.LibraryName = componentDocument.Library;
                    }
                }
            }
        }

        void LoadFootprint()
        {
            var project = ProjectNode;

            //load footprints
            // Footprints.Clear();
            if (componentDocument.Footprint != null)
            {
                //foreach (var device in componentDocument.Footprints)
                {
                    var device = componentDocument.Footprint;
                    var footprint = new FootprintDisplay
                    {
                        // Name = device.footprint,
                        //Canvas = new DrawingViewModel(),
                        Device = device,
                        Connects = new ObservableCollection<ConnectDisplay>()
                    };

                    var fp = project.FindObject(TemplateType.Footprint, /*device.LibraryName,*/ device.footprintId) as Footprint;
                    //var fp = componentDocument.Footprint.CachedFootprint;

                    if (fp == null)
                    {
                        //footprint not solved
                    }
                    else
                    {
                        footprint.Footprint = fp;
                        device.LibraryName = fp.Library;
                        device.footprintName = fp.Name;

                        //if (fp.Items != null)
                        //{

                        //    var layeredDoc = LoadFootprintLayers(fp);


                        //    var primitives = new List<ISelectableItem>();
                        //    foreach (var primitive in fp.Items)
                        //    {
                        //        var canvasItem = (BoardCanvasItemViewModel)primitive.CreateDesignerItem();
                        //        canvasItem.LayerDocument = layeredDoc;
                        //        canvasItem.LoadLayers();
                        //        primitives.Add(canvasItem);
                        //    }
                        //    footprint.Preview.SetPrimitives(primitives);
                        //}

                        footprint.Preview.PreviewDocument(fp, null);
                    }

                    //footprints connects
                    if (device.Connects != null)
                    {
                        foreach (var c in device.Connects)
                        {
                            footprint.Connects.Add(new ConnectDisplay(Gates)
                            {
                                GateId = c.gateId,
                                Pad = c.pad,
                                Pin = c.pin,
                            });
                        }
                    }
                    // Footprints.Add(footprint);
                    Footprint = footprint;
                }

                RefreshConnects();
            }
        }

        private void LoadGates()
        {
            var project = ProjectNode;

            //load symbol gates
            //Gates.Clear();
            var gates = new List<GateDisplay>();
            if (componentDocument.Gates != null)
            {
                foreach (var gate in componentDocument.Gates)
                {
                    var g = new GateDisplay
                    {
                        //Canvas = new DrawingViewModel(),
                        Gate = gate,
                    };

                    //solve symbol
                    //var lastRead=
                    var symbol = project.FindObject(TemplateType.Symbol, /*gate.LibraryName,*/ gate.symbolId) as Symbol;
                    //var symbol = gate.CachedSymbol;

                    //todo if the symbol is not solved we should show something and log to output
                    if (symbol == null)
                    {

                    }
                    else
                    {
                        g.Symbol = symbol;
                        g.Gate.symbolName = symbol.Name;
                        g.Gate.LibraryName = symbol.Library;

                        //if (symbol.Items != null)
                        //{
                        //    g.Preview.SetPrimitives(symbol.GetDesignerPrimitiveItems());
                        //}
                        g.Preview.PreviewDocument(symbol, null);
                    }

                    gates.Add(g);
                }
            }

            dispatcher.RunOnDispatcher(() =>
            {
                Gates.Clear();
                Gates.AddRange(gates);
            });

        }

        private void RefreshGates()
        {
            foreach (var gateDisplay in Gates)
            {
                var gate = gateDisplay.Gate;
                var symbol = gateDisplay.Symbol;
                DateTime? lastModified = null;

                if (symbol != null)
                {
                    if (gate.symbolId == symbol.Id && gate.LibraryName == symbol.Library)
                    {
                        lastModified = symbol.LastAccessed;
                    }
                }

                var foundSymbol = ProjectNode.FindObject(TemplateType.Symbol, gate.LibraryName, gate.symbolId, lastModified) as Symbol;

                if (foundSymbol != null)
                {
                    gateDisplay.Symbol = foundSymbol;
                    gateDisplay.Gate.symbolName = foundSymbol.Name;
                    gateDisplay.Gate.LibraryName = foundSymbol.Library;
                    gateDisplay.Preview.PreviewDocument(foundSymbol, null);
                }
            }
        }

        void RefreshConnects()
        {
            //there are a few choices here: if we have more pads defined than the number of pins in all gates we show all pads and whatever is asociated with it
            //if we have less pads than pins we could either show the pads and pin asociated; the other unassign pin will be not shown (considered NC)
            //or, show pad-pin pair and remaining pins which are not associated; 
            //or just show a message that there are pins not connected to pads
            //foreach (var footprint in Footprints)
            if (footprint != null && footprint.Footprint != null)
            {

                if (footprint.Connects == null)
                    footprint.Connects = new ObservableCollection<ConnectDisplay>();
                var oldConnects = footprint.Connects.ToList();
                footprint.Connects.Clear();

                var pads = ((from p in footprint.Footprint.Items.OfType<Pad>()
                             select new { p.number })
                         .Union(from p in footprint.Footprint.Items.OfType<Smd>()
                                select new { p.number })).OrderBy(p => p.number).ToList();


                foreach (var pad in pads)
                {
                    var newConnect = new ConnectDisplay(Gates)
                    {
                        Pad = pad.number,
                    };

                    //if the connect was defined last time, we keep all the info
                    var oldConnect = oldConnects.FirstOrDefault(c => c.Pad == pad.number);
                    //var oldFootprintConnect = footprint.Device.Connects.FirstOrDefault(c => c.pad == pad.number);
                    if (oldConnect != null)
                    {
                        newConnect.Pin = oldConnect.Pin;
                        newConnect.GateId = oldConnect.GateId;
                        //we must handle the case when the gate name changes
                        //newConnect.GateName = oldConnect.GateName;


                    }
                    else
                    {
                        //we must suggest something
                        newConnect.Pin = pad.number;
                        newConnect.GateId = Gates.Select(g => g.Gate.Id).FirstOrDefault();
                    }

                    //newConnect.PinName = GetPinName(newConnect.GateId, newConnect.Pin);

                    footprint.Connects.Add(newConnect);
                }

                footprint.OnPropertyChanged(nameof(Footprint.Connects));
            }
        }

        void RefreshGateNames()
        {
            //we update the names of the gates to be in a particular order: G1, G2, etc, but we must update the references
            var gateIndex = 1;
            foreach (var gate in Gates)
            {
                gate.Name = "G" + gateIndex;
                gateIndex++;
            }
        }

        public override Task<bool> Compile()
        {
            CompileErrors.Clear();

            var slnNodeName = FileName;
            var projectName = ProjectNode.Name;
            var hasErrors = false;

            return Task.Run(() =>
            {
                //prefix not specified
                if (string.IsNullOrEmpty(Prefix))
                {
                    var msg = $"Component prefix not specified for {slnNodeName})";
                    // output.AppendLine(msg);
                    AddCompileError(msg, slnNodeName, projectName);
                    hasErrors = true;
                }

                //at least one gate
                if (Gates == null || Gates.Count == 0)
                {
                    var msg = $"Component has no gates specified for {slnNodeName})";
                    //output.AppendLine(msg);
                    AddCompileError(msg, slnNodeName, projectName);
                    hasErrors = true;
                }

                //gate references
                foreach (var gate in Gates)
                {
                    try
                    {
                        if (gate.Name != null)
                        {
                            var symbolSearch = ProjectNode.FindObject(TemplateType.Symbol, gate.Gate.LibraryName, gate.Gate.symbolId);
                            if (symbolSearch == null)
                                throw new Exception($"Symbol {gate.Symbol.Name} was not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        hasErrors = true;
                        //output.AppendLine($"Error: {ex.Message}");
                        AddCompileError(ex.Message, slnNodeName, projectName);
                    }
                }

                //footprint references
                if (Footprint != null)
                {
                    try
                    {
                        if (Footprint.Name != null)
                        {
                            var fptSearch = ProjectNode.FindObject(TemplateType.Footprint, Footprint.Footprint.Library, Footprint.Footprint.Id);
                            if (fptSearch == null)
                                throw new Exception($"Footprint {Footprint.Name} was not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        hasErrors = true;
                        //output.AppendLine($"Error: {ex.Message}");
                        AddCompileError(ex.Message, slnNodeName, projectName);
                    }
                }

                return !hasErrors;
            });
        }

        public override IList<IDocumentToolWindow> GetToolWindowsWhenActive()
        {
            return new List<IDocumentToolWindow>();
        }
    }
}
