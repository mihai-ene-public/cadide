using IDE.Core.Designers;
using IDE.Core.Storage;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IDE.Core.Toolbars;
using IDE.Core.Interfaces;
using IDE.Core.ViewModels;
using IDE.Core;
using IDE.Core.Common;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Core.Presentation.Placement;

namespace IDE.Documents.Views
{
    public class MeshDesignerViewModel : CanvasDesignerFileViewModel, IMeshDesigner
    {
        public MeshDesignerViewModel(
            IDispatcherHelper dispatcher,
            IDebounceDispatcher drawingChangedDebouncer,
            IDebounceDispatcher selectionDebouncer,
            IDirtyMarkerTypePropertiesMapper dirtyMarkerTypePropertiesMapper,
            IPlacementToolFactory placementToolFactory)
        : base(dispatcher, drawingChangedDebouncer, selectionDebouncer, dirtyMarkerTypePropertiesMapper, placementToolFactory)
        {
            modelDocument = new ModelDocument();

            Toolbar = new ModelToolbar(this);

        }

        public override IList<IDocumentToolWindow> GetToolWindowsWhenActive()
        {
            var tools = new List<IDocumentToolWindow>();

            tools.Add(ServiceProvider.GetToolWindow<DocumentOverviewViewModel>());

            return tools;
        }

        ModelDocument modelDocument;

        IList<IOverviewSelectNode> BuildCategories()
        {
            var list = new List<IOverviewSelectNode>();
            var primitivesCat = new OverviewFolderNode { Name = "Primitives" };
            list.Add(primitivesCat);
            var padsCat = new OverviewFolderNode { Name = "Pads" };
            list.Add(padsCat);
            var padItems = Items.OfType<BaseMeshItem>().Where(b => b.PadNumber > 0).ToList();
            var pads = padItems.Select(p => new OverviewSelectNode
            {
                DataItem = p,
            });
            padsCat.Children.AddRange(pads);

            var primitives = Items.Except(padItems).Select(p => new OverviewSelectNode
            {
                DataItem = p
            });

            foreach (var p in primitives)
            {
                primitivesCat.Children.Add(p);

                if (p.DataItem is GroupMeshItem g)
                {
                    foreach (var child in g.Items)
                    {
                        p.Children.Add(new OverviewSelectNode { DataItem = child });
                    }
                }
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


        #region Toolbox

        protected override void InitToolbox()
        {
            base.InitToolbox();

            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Box",
                Type = typeof(BoxMeshItem)
            });

            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Cone",
                Type = typeof(ConeMeshItem)
            });

            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Cylinder",
                Type = typeof(CylinderMeshItem)
            });

            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Sphere",
                Type = typeof(SphereMeshItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Ellipsoid",
                Type = typeof(EllipsoidMeshItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Extruded polygon",
                Type = typeof(ExtrudedPolyMeshItem)
            });
            Toolbox.Primitives.Add(new PrimitiveItem
            {
                TooltipText = "Text",
                Type = typeof(TextMeshItem)
            });

        }

        #endregion Toolbox

        #region Commands

        ICommand addModelCommand;

        public ICommand AddModelCommand
        {
            get
            {
                if (addModelCommand == null)
                    addModelCommand = CreateCommand(p =>
                    {
                        ClearSelectedItems();

                        CancelPlacement();

                        var itemSelectDlg = new ItemSelectDialogViewModel(TemplateType.Model, GetCurrentProjectInfo());

                        if (itemSelectDlg.ShowDialog() == true)
                        {
                            var symbol = itemSelectDlg.SelectedItem.Document as ModelDocument;

                            if (symbol.Items != null)
                            {
                                var canvasItems = symbol.Items.Select(c => c.CreateDesignerItem()).ToList();

                                var group = new VolatileGroup3DCanvasItem
                                {
                                    Items = canvasItems.Cast<ISelectableItem>().ToList()
                                };
                                StartPlacement(group);
                            }
                        }
                    }

                    );

                return addModelCommand;
            }
        }

        ICommand importModelCommand;
        string GetOpenFileFilter(IModelImporter modelImporter) //= "3D model files (*.3ds;*.obj;*.lwo;*.stl;*.step)|*.3ds;*.obj;*.objz;*.lwo;*.stl;*.step;*.stp"
        {

            string extensions = string.Join(";", modelImporter.GetSupportedFileFormats());
            return $"3D model files ({extensions})|{extensions}";
        }

        public ICommand ImportModelCommand
        {
            get
            {
                if (importModelCommand == null)
                {
                    importModelCommand = CreateCommand((p) =>
                    {
                        var modelImporter = ServiceProvider.Resolve<IModelImporter>();
                        var dlg = ServiceProvider.Resolve<IOpenFileDialog>();//new OpenFileDialog();
                        dlg.Filter = GetOpenFileFilter(modelImporter);
                        dlg.DefaultExt = ".stl";

                        if (dlg.ShowDialog() == true)
                        {
                            var canvasItem = modelImporter.Import(dlg.FileName);
                            AddItem(canvasItem);

                            RegisterUndoActionExecuted(
                            undo: o =>
                            {
                                RemoveItem(canvasItem);
                                return canvasItem;
                            },
                            redo: o =>
                            {
                                AddItem(canvasItem);
                                return canvasItem;
                            },
                            canvasItem);
                        }
                    });
                }

                return importModelCommand;
            }
        }

        ICommand groupItemsCommand;

        public ICommand GroupItemsCommand
        {
            get
            {
                if (groupItemsCommand == null)
                {
                    groupItemsCommand = CreateCommand((p) =>
                    {
                        var selectedItems = SelectedItems.ToList();
                        if (selectedItems.Count > 1)//at least 2 selected items
                        {
                            //we should calculate the XYZ for group
                            var groupItem = CreateGroupFromItems(selectedItems);
                            AddItem(groupItem);

                            RegisterUndoActionExecuted(undo: o =>
                            {
                                UngroupToItems(groupItem);

                                OnDrawingChanged(DrawingChangedReason.ItemAdded);
                                return groupItem;
                            },
                            redo: o =>
                            {
                                groupItem = CreateGroupFromItems(selectedItems);
                                AddItem(groupItem);

                                OnDrawingChanged(DrawingChangedReason.ItemAdded);
                                return groupItem;
                            },
                            null);

                            OnDrawingChanged(DrawingChangedReason.ItemAdded);
                        }
                    });
                }

                return groupItemsCommand;
            }
        }

        private GroupMeshItem CreateGroupFromItems(List<ISelectableItem> items)
        {
            var groupItem = new GroupMeshItem();
            groupItem.IsPlaced = true;
            foreach (var item in items)
            {
                item.IsSelected = false;
                RemoveItem(item);
            }

            groupItem.Items.AddRange(items.Cast<BaseMeshItem>());
            foreach (var item in groupItem.Items)
                item.ParentObject = groupItem;

            return groupItem;
        }

        ICommand ungroupItemsCommand;
        public ICommand UngroupItemsCommand
        {
            get
            {
                if (ungroupItemsCommand == null)
                {
                    ungroupItemsCommand = CreateCommand((p) =>
                    {
                        var selectedItems = SelectedItems.OfType<GroupMeshItem>().ToList();
                        if (selectedItems.Count == 0)
                            return;
                        foreach (var g in selectedItems)
                        {
                            UngroupToItems(g);
                        }

                        RegisterUndoActionExecuted(undo: o =>
                        {
                            for (int i = 0; i < selectedItems.Count; i++)
                            {
                                var groupItem = selectedItems[i];
                                groupItem = CreateGroupFromItems(groupItem.Items.Cast<ISelectableItem>().ToList());
                                AddItem(groupItem);
                                selectedItems[i] = groupItem;
                            }
                            OnDrawingChanged(DrawingChangedReason.ItemAdded);

                            return null;
                        },
                           redo: o =>
                           {
                               foreach (var g in selectedItems)
                               {
                                   UngroupToItems(g);
                               }

                               OnDrawingChanged(DrawingChangedReason.ItemAdded);
                               return null;
                           },
                           null);

                        OnDrawingChanged(DrawingChangedReason.ItemAdded);
                    },
                    p =>
                    {
                        return SelectedItems.OfType<GroupMeshItem>().Count() > 0;
                    });
                }

                return ungroupItemsCommand;
            }
        }

        private void UngroupToItems(GroupMeshItem g)
        {
            RemoveItem(g);

            foreach (var item in g.Items)
            {
                item.ParentObject = null;
                item.IsSelected = false;
                item.IsPlaced = true;
                AddItem(item);
            }
        }

        public override void PasteSelectedItems()
        {
            //if we want to paste multiple times, we need to clone, so we do that
            var canvasItemPairs = ApplicationClipboard.Items.OfType<ISelectableItem>()
                                 .Select(s => new
                                 {
                                     OriginalObject = s,
                                     CloneObject = (BaseMeshItem)s.Clone()
                                 }).ToList();

            var canvasItems = canvasItemPairs.Select(s => s.CloneObject).ToList();

            var group = new VolatileGroup3DCanvasItem
            {
                Items = canvasItems.Cast<ISelectableItem>().ToList()
            };

            StartPlacement(group);
        }

        public PackageGeneratorViewModel PackageGenerator { get; set; }

        ICommand showPackageGeneratorCommand;
        public ICommand ShowPackageGeneratorCommand
        {
            get
            {

                if (showPackageGeneratorCommand == null)
                {
                    showPackageGeneratorCommand = CreateCommand(p =>
                      {
                          ClearSelectedItems();
                          CancelPlacement();


                          PackageGenerator = new PackageGeneratorViewModel(this);
                          PackageGenerator.Close += () =>
                            {
                                PackageGenerator = null;
                                ShowPackageGenerator = false;
                            };
                          OnPropertyChanged(nameof(PackageGenerator));
                          ShowPackageGenerator = true;
                      }
                      );
                }

                return showPackageGeneratorCommand;
            }
        }

        bool showPackageGenerator;
        public bool ShowPackageGenerator
        {
            get { return showPackageGenerator; }
            set
            {
                showPackageGenerator = value;
                OnPropertyChanged(nameof(ShowPackageGenerator));
            }
        }

        #region Parametric Package

        bool showParametricPackageGenerator;
        public bool ShowParametricPackageGenerator
        {
            get { return showParametricPackageGenerator; }
            set
            {
                showParametricPackageGenerator = value;
                OnPropertyChanged(nameof(ShowParametricPackageGenerator));
            }
        }

        ICommand showParametricPackageCommand;
        public ICommand ShowParametricPackageCommand
        {
            get
            {

                if (showParametricPackageCommand == null)
                {
                    showParametricPackageCommand = CreateCommand(p =>
                    {
                        ClearSelectedItems();
                        CancelPlacement();

                        ParametricPackageVM = new ParametricPackageViewModel(this);
                        ParametricPackageVM.Close += () =>
                        {
                            ParametricPackageVM = null;
                            ShowParametricPackageGenerator = false;
                        };
                        OnPropertyChanged(nameof(ParametricPackageVM));
                        ShowParametricPackageGenerator = true;
                    }
                      );
                }

                return showParametricPackageCommand;
            }
        }


        public ParametricPackageViewModel ParametricPackageVM { get; set; }

        #endregion Parametric Package

        #endregion Commands

        protected override void SaveDocumentInternal(string filePath)
        {
            //remove the currently adding item si that it won't be saved
            ISelectableItem placeObjects = null;
            if (IsPlacingItem())
            {
                placeObjects = PlacementTool.CanvasItem;
                RemoveItem(placeObjects);
            }

            //InternalSaveDocument()
            modelDocument.Name = Path.GetFileNameWithoutExtension(filePath);
            modelDocument.Items = Items.Cast<BaseMeshItem>().Select(d => (MeshPrimitive)d.SaveToPrimitive()).ToList();

            XmlHelper.Save(modelDocument, filePath);

            //PostSave
            if (placeObjects != null)
                AddItem(placeObjects);
        }

        protected override Task LoadDocumentInternal(string filePath)
        {
            return Task.Run(() =>
            {
                modelDocument = XmlHelper.Load<ModelDocument>(filePath);

                //assign a new id if needed
                if (string.IsNullOrEmpty(modelDocument.Id))
                {
                    modelDocument.Id = LibraryItem.GetNextId();
                    IsDirty = true;
                }


                if (modelDocument.Items != null)
                {
                    foreach (var primitive in modelDocument.Items)
                    {
                        var canvasItem = primitive.CreateDesignerItem();
                        AddItem(canvasItem);
                    }
                }
            });
        }

        protected override async Task AfterLoadDocumentInternal()
        {
            await base.AfterLoadDocumentInternal();
            await RefreshOverview();
        }

    }
}
