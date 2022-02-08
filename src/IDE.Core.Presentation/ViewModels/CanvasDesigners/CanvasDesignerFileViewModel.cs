using System;
using System.Windows.Input;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using IDE.Core.Designers;
using IDE.Core.ViewModels;
using IDE.Core.Commands;
using IDE.Core.Toolbars;
using IDE.Core;
using IDE.Core.UndoRedoFramework;
using IDE.Core.Converters;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.Collections;

namespace IDE.Documents.Views
{
    /// <summary>
    /// Represents a canvas designer with toolbox and top toolbar. (Symbol, Footprint, Schematic, Board)
    /// <para>It is a common class that will be inherited from.</para>
    /// </summary>
    public class CanvasDesignerFileViewModel : FileBaseViewModel, ICanvasDesignerFileViewModel
    {

        protected CanvasDesignerFileViewModel() : base(null)
        {
            applicationModel = ServiceProvider.Resolve<IApplicationViewModel>();//ServiceProvider.GetService<IApplicationViewModel>();

            _dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();

            CanvasModel = new DrawingViewModel(this, _dispatcher);
            CanvasModel.DrawingChanged += CanvasModel_DrawingChanged;
            CanvasModel.PropertyChanged += CanvasModel_PropertyChanged;
            InitToolbox();

            IsDirty = false;

            savedStateManager = new SavedStateManager(this);

        }

        protected UndoRedoContext undoRedoContext = new UndoRedoContext();

        protected IApplicationViewModel applicationModel;

        protected ISavedStateManager savedStateManager;

        protected IDispatcherHelper _dispatcher;

        public ToolbarModel Toolbar { get; set; }

        #region CanvasModel

        protected IDrawingViewModel canvasModel;
        public IDrawingViewModel CanvasModel
        {
            get
            {
                return canvasModel;
            }
            set
            {
                if (canvasModel != value)
                {
                    canvasModel = value;
                    OnPropertyChanged(nameof(CanvasModel));
                }
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
                selectedSizeTemplate = SizeTemplates.FirstOrDefault(t => t.DocumentSize == canvasModel.DocumentSize);

                return selectedSizeTemplate;
            }
            set
            {
                if (selectedSizeTemplate == value)
                    return;

                selectedSizeTemplate = value;

                canvasModel.DocumentSize = selectedSizeTemplate.DocumentSize;
                canvasModel.DocumentWidth = selectedSizeTemplate.DocumentWidth;
                canvasModel.DocumentHeight = selectedSizeTemplate.DocumentHeight;
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

        double originX;
        public double OriginX
        {
            get { return originX; }
            set
            {
                originX = value;
                OnPropertyChanged(nameof(OriginX));
            }
        }

        double originY;
        public double OriginY
        {
            get { return originY; }
            set
            {
                originY = value;
                OnPropertyChanged(nameof(OriginY));
            }
        }

        #region ParentProject

        public ISolutionProjectNodeModel ParentProject
        {
            get
            {
                var itemNode = Item as SolutionExplorerNodeModel;
                if (itemNode == null)
                    return null;
                var project = itemNode.ProjectNode;
                if (project == null)
                    throw new Exception("This component does not belong to any project");

                return project;
            }
        }

        #endregion ParentProject

        ICommand zoomToFitCommand;

        public ICommand ZoomToFitCommand
        {
            get
            {
                if (zoomToFitCommand == null)
                    zoomToFitCommand = CreateCommand(p =>
                      {
                          canvasModel?.ZoomToFit();
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
                        canvasModel?.ZoomToSelectedItems();
                    },
                    p => CanZoomToSelectedItemsCommand());

                return zoomToSelectedItemsCommand;
            }
        }

        protected virtual bool CanZoomToSelectedItemsCommand()
        {
            return true;
        }

        ICommand undoCommand;
        public ICommand UndoCommand
        {
            get
            {
                return undoCommand ?? (undoCommand = undoRedoContext.GetUndoCommand());
            }
        }



        ICommand redoCommand;
        public ICommand RedoCommand { get { return redoCommand ?? (redoCommand = undoRedoContext.GetRedoCommand()); } }

        #region DeleteSelectedItems command

        //ICommand deleteSelectedItemsCommand;
        //public ICommand DeleteSelectedItemsCommand
        //{
        //    get
        //    {
        //        if (deleteSelectedItemsCommand == null)
        //            deleteSelectedItemsCommand = new RelayCommand((p) =>
        //            {

        //                var selectedItems = canvasModel.SelectedItems.ToList();
        //                foreach (var s in selectedItems)
        //                    canvasModel.RemoveItem(s);

        //                canvasModel.OnDrawingChanged();
        //            },
        //            (p) =>
        //            {
        //                return canvasModel.SelectedItems.Count > 0;
        //            });

        //        return deleteSelectedItemsCommand;
        //    }
        //}

        #region ISnapshotManager
        public virtual ISavedState CreateSnapshot()
        {
            return new GenericCanvasSavedState
            {
                CanvasItems = canvasModel.GetItems().Select(s => s.Clone())
                                         .Cast<ICanvasItem>()
                                         .ToList()
            };
        }

        public virtual void RestoreFromSnapshot(ISavedState state)
        {

        }

        #endregion ISnapshotManager

        //public ICommand DeleteSelectedItemsCommand
        //{
        //    get
        //    {
        //        if (deleteSelectedItemsCommand == null)
        //            deleteSelectedItemsCommand = new UndoableDelegateCommand<IList<ISelectableItem>>(
        //            () =>//execute
        //            {
        //                //savedStateManager.Backup();

        //                var selectedItems = canvasModel.SelectedItems.ToList();
        //                if (selectedItems.Count == 1 && selectedItems[0] is TrackBoardCanvasItem trackItem)
        //                {
        //                    trackItem.RemoveSelectedSegment();
        //                }
        //                else
        //                {
        //                    foreach (var s in selectedItems)
        //                        canvasModel.RemoveItem(s);
        //                }

        //                canvasModel.OnDrawingChanged(DrawingChangedReason.ItemRemoved);

        //                return selectedItems;
        //            },
        //            () =>//can execute
        //            {
        //                return canvasModel.SelectedItems.Count > 0;
        //            },
        //            (selectedItems) =>//undo
        //            {
        //                foreach (var s in selectedItems)
        //                {
        //                    canvasModel.AddItem(s);
        //                }

        //                //savedStateManager.Undo();

        //                canvasModel.OnDrawingChanged(DrawingChangedReason.ItemAdded);

        //                return selectedItems;
        //            },
        //            (selectedItems) =>//redo
        //            {
        //                foreach (var s in selectedItems)
        //                    canvasModel.RemoveItem(s);

        //                //savedStateManager.Redo();

        //                canvasModel.OnDrawingChanged(DrawingChangedReason.ItemRemoved);

        //                return selectedItems;
        //            },
        //            "Delete items",
        //            undoRedoContext
        //            );

        //        return deleteSelectedItemsCommand;
        //    }
        //}

        #endregion DeleteSelectedItems command

        #region CopySelectedItems command

        //ICommand copySelectedItemsCommand;

        //public ICommand CopySelectedItemsCommand
        //{
        //    get
        //    {
        //        if (copySelectedItemsCommand == null)
        //            copySelectedItemsCommand = CreateCommand((p) =>
        //            {
        //                CopySelectedItems();
        //            },
        //            (p) =>
        //            {
        //                return canvasModel.SelectedItems.Count > 0;
        //            });

        //        return copySelectedItemsCommand;
        //    }
        //}

        #endregion

        #region PasteSelectedItems command

        //ICommand pasteSelectedItemsCommand;

        //public ICommand PasteSelectedItemsCommand
        //{
        //    get
        //    {
        //        if (pasteSelectedItemsCommand == null)
        //            pasteSelectedItemsCommand = CreateCommand((p) =>
        //            {
        //                PasteSelectedItems();
        //            },
        //            (p) =>
        //            {
        //                return ApplicationClipboard.Items != null && ApplicationClipboard.Items.Count > 0;
        //            });

        //        return pasteSelectedItemsCommand;
        //    }
        //}

        #endregion


        protected virtual void InitToolbox()
        {
            Toolbox = new ToolBoxViewModel((DrawingViewModel)canvasModel);
        }

        bool busyCats = false;
        async void CanvasModel_DrawingChanged(DrawingChangedReason reason)
        {
            IsDirty = true;
            try
            {
                if (reason == DrawingChangedReason.ItemAdded || reason == DrawingChangedReason.ItemRemoved)
                {
                    if (!busyCats)
                    {
                        busyCats = true;
                        //OnPropertyChanged(nameof(IDocumentOverview.Categories));
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
            if (canvasModel.IsPlacingItem())
            {
                canvasModel.PlacementTool.ChangeMode();
            }
        }

        public void CyclePlacementOrRotate()
        {
            canvasModel.CyclePlacementOrRotate();
        }

        public void MirrorXSelectedItems()
        {
            canvasModel.MirrorXSelectedItems();
        }

        public void MirrorYSelectedItems()
        {
            canvasModel.MirrorYSelectedItems();
        }

        public void DeleteSelectedItems()
        {
            canvasModel.DeleteSelectedItems();
        }

        public void CopySelectedItems()
        {
            canvasModel.CopySelectedItems();
        }

        public virtual void PasteSelectedItems()
        {
            canvasModel.PasteSelectedItems();
        }

        public void ChangeFootprintPlacement()
        {
            canvasModel.ChangeFootprintPlacement();
        }

        void CanvasModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(canvasModel.X):
                    X = Math.Round(canvasModel.X, 4);
                    break;
                case nameof(canvasModel.Y):
                    Y = Math.Round(canvasModel.Y, 4);
                    break;
            }
        }

        public override void RegisterDocumentType(IDocumentTypeManager docTypeManager)
        {
            throw new NotImplementedException();
        }

        public override IList<IDocumentToolWindow> GetToolWindowsWhenActive()
        {
            return new List<IDocumentToolWindow>();
        }
    }
}
