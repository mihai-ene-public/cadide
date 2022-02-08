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

namespace IDE.Documents.Views
{
    public class MeshDesignerViewModel : CanvasDesignerFileViewModel, IDocumentOverview
    {
        public MeshDesignerViewModel() : base()
        {
            DocumentKey = "Model Editor";
            Description = "Model files";
            FileFilterName = "Model file";
            DefaultFilter = "model";
            documentTypeKey = DocumentKey;
            defaultFileType = "model";
            defaultFileName = "Model";
            dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();

            modelDocument = new ModelDocument();

            Toolbar = new ModelToolbar(this);

            //PropertyChanged += MeshDesignerViewModel_PropertyChanged;
        }
        IDispatcherHelper dispatcher;

        //void MeshDesignerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    switch (e.PropertyName)
        //    {
        //        case nameof(IsActive):
        //            {
        //                if (IsActive)
        //                {
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

            tools.Add(ServiceProvider.GetToolWindow<DocumentOverviewViewModel>());

            return tools;
        }

        ModelDocument modelDocument;

        public override object Document
        {
            get
            {
                return modelDocument;
            }
        }

        IList<IOverviewSelectNode> BuildCategories()
        {
            var list = new List<IOverviewSelectNode>();
            var primitivesCat = new OverviewFolderNode { Name = "Primitives" };
            list.Add(primitivesCat);
            var padsCat = new OverviewFolderNode { Name = "Pads" };
            list.Add(padsCat);
            var padItems = canvasModel.Items.OfType<BaseMeshItem>().Where(b => b.PadNumber > 0).ToList();
            var pads = padItems.Select(p => new OverviewSelectNode
            {
                DataItem = p,
            });
            padsCat.Children.AddRange(pads);

            var primitives = canvasModel.Items.Except(padItems).Select(p => new OverviewSelectNode
            {
                DataItem = p
            });

            foreach (var p in primitives)
            {
                primitivesCat.Children.Add(p);

                //if (p.DataItem is SolidBodyMeshItem solid)
                //{
                //    foreach (var m in solid.Models)
                //    {
                //        p.Children.Add(new OverviewSelectNode
                //        {
                //            DataItem = m
                //        });
                //    }
                //}
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

                dispatcher.RunOnDispatcher(() => Categories = nodes);
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
                        canvasModel.ClearSelectedItems();

                        canvasModel.CancelPlacement();


                        var itemSelectDlg = new ItemSelectDialogViewModel();
                        itemSelectDlg.TemplateType = TemplateType.Model;
                        itemSelectDlg.ProjectModel = ParentProject;
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
                                //canvasModel.AddItem(group);
                                canvasModel.StartPlacement(group);
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

            string extensions = string.Join(";", //"*.3ds",
                                                 //"*.obj",
                                                 //                                                     "*.stl",
                                                 //#if DEBUG
                                                 //                                                     "*.step",
                                                 //                                                     "*.stp"
                                                 //#endif

                                                modelImporter.GetSupportedFileFormats()
                                                );
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

                            modelImporter.Import(dlg.FileName, canvasModel);

                            //var model = LoadAsync(dlg.FileName, true);
                            //var modelItem = new SolidBodyMeshItem();
                            //modelItem.IsPlaced = true;
                            //modelItem.Model = model;
                            //canvasModel.AddItem(modelItem);
                        }
                    });
                }

                return importModelCommand;
            }
        }


        ////async Task<Model3DGroup>
        //Model3DGroup LoadAsync(string model3DPath, bool freeze)
        //{
        //    //return await Task.Factory.StartNew(() =>
        //    //{
        //    var mi = new GenericModelImporter();

        //    if (freeze)
        //    {
        //        // Alt 1. - freeze the model 
        //        return mi.Load(model3DPath, null, true);
        //    }

        //    //// Alt. 2 - create the model on the UI dispatcher
        //    //return mi.Load(model3DPath, Dispatcher.CurrentDispatcher);//?
        //    ////});
        //}

        ICommand groupItemsCommand;

        public ICommand GroupItemsCommand
        {
            get
            {
                if (groupItemsCommand == null)
                {
                    groupItemsCommand = CreateCommand((p) =>
                    {
                        var selectedItems = canvasModel.SelectedItems.ToList();
                        if (selectedItems.Count > 1)//at least 2 selected items
                        {
                            //we should calculate the XYZ for group
                            var groupItem = new GroupMeshItem();
                            groupItem.IsPlaced = true;
                            foreach (var item in selectedItems)
                            {
                                item.IsSelected = false;
                                canvasModel.RemoveItem(item);
                            }
                            //groupItem.Items= new selectedItems.Cast<BaseMeshItem>()

                            canvasModel.AddItem(groupItem);
                            canvasModel.OnDrawingChanged(DrawingChangedReason.ItemAdded);


                            groupItem.Items.AddRange(selectedItems.Cast<BaseMeshItem>());
                            foreach (var item in groupItem.Items)
                                item.ParentObject = groupItem;
                        }
                    });
                }

                return groupItemsCommand;
            }
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
                        var selectedItems = canvasModel.SelectedItems.OfType<GroupMeshItem>().ToList();
                        if (selectedItems.Count == 0)
                            return;
                        foreach (var g in selectedItems)
                        {
                            canvasModel.RemoveItem(g);

                            foreach (var item in g.Items)
                            {
                                item.ParentObject = null;
                                item.IsSelected = false;
                                item.IsPlaced = true;
                                canvasModel.AddItem(item);
                            }
                        }
                        canvasModel.OnDrawingChanged(DrawingChangedReason.ItemAdded);
                    },
                    p =>
                    {
                        return canvasModel.SelectedItems.OfType<GroupMeshItem>().Count() > 0;
                    });
                }

                return ungroupItemsCommand;
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

            //in schematic, for parts: create copy of parts and add them to schematic
            var canvasItems = canvasItemPairs.Select(s => s.CloneObject).ToList();

            var group = new VolatileGroup3DCanvasItem
            {
                Items = canvasItems.Cast<ISelectableItem>().ToList()
            };

            canvasModel.StartPlacement(group);
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
                          canvasModel.ClearSelectedItems();
                          canvasModel.CancelPlacement();


                          PackageGenerator = new PackageGeneratorViewModel(canvasModel);
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
                        canvasModel.ClearSelectedItems();
                        canvasModel.CancelPlacement();


                        ParametricPackageVM = new ParametricPackageViewModel(canvasModel);
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
            if (canvasModel.IsPlacingItem())
            {
                placeObjects = canvasModel.PlacementTool.CanvasItem;
                canvasModel.RemoveItem(placeObjects);
            }

            //InternalSaveDocument()
            modelDocument.Name = Path.GetFileNameWithoutExtension(filePath);
            modelDocument.Items = canvasModel.Items.Cast<BaseMeshItem>().Select(d => (MeshPrimitive)d.SaveToPrimitive()).ToList();

            XmlHelper.Save(modelDocument, filePath);

            //PostSave
            if (placeObjects != null)
                canvasModel.AddItem(placeObjects);
        }

        protected override Task LoadDocumentInternal(string filePath)
        {
            return Task.Run(() =>
            {
                modelDocument = XmlHelper.Load<ModelDocument>(filePath);

                //assign a new id if needed
                if (modelDocument.Id == 0)
                {
                    modelDocument.Id = LibraryItem.GetNextId();
                    IsDirty = true;
                }


                if (modelDocument.Items != null)
                {
                    foreach (var primitive in modelDocument.Items)
                    {
                        var canvasItem = primitive.CreateDesignerItem();
                        canvasModel.AddItem(canvasItem);
                    }
                }
            });
        }

        protected override async Task AfterLoadDocumentInternal()
        {
            await base.AfterLoadDocumentInternal();
            await RefreshOverview();
        }

        public override void RegisterDocumentType(IDocumentTypeManager docTypeManager)
        {
            var docType = docTypeManager.RegisterDocumentType(DocumentKey,
                                                              Description,
                                                              FileFilterName,
                                                              DefaultFilter,
                                                              GetType()
                                                              );

            if (docType != null) // Lets register some sub-types for editing with Edi's text editor
            {
                var t = docType.CreateItem("Model Files", new List<string>() { "model" });
                docType.RegisterFileTypeItem(t);
            }
        }
    }
}
