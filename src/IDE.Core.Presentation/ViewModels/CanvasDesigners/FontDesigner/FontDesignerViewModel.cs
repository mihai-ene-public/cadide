using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IDE.Core.Storage;
using IDE.Core.Designers;
using IDE.Core.Utilities;
using IDE.Core.Commands;
using IDE.Documents.Views;
using IDE.Core.Interfaces;
using IDE.Core.Common;
using System.Collections;
using System.Threading.Tasks;
using IDE.Core.Types.Media;

namespace IDE.Core.ViewModels
{
    //subject to be removed (keep it until we implement the new font source implementation)
    public class FontDesignerViewModel : CanvasDesignerFileViewModel
                                       , ILayeredViewModel
    {
        public FontDesignerViewModel()
            : base()
        {
            DocumentKey = "FontEditor";
            Description = "Font files";
            FileFilterName = "Font file";
            DefaultFilter = "font";
            documentTypeKey = DocumentKey;
            defaultFileType = "font";
            defaultFileName = "Default";

            this.canvasModel.Scale = 150;
            var cg = canvasModel.CanvasGrid as CanvasGrid;
            cg.GridSizeModel.CurrentValue = 0.1;//mm

            canvasModel.DocumentWidth = 1;
            canvasModel.DocumentHeight = 1;

            fontDocument = new FontDocument();

            PropertyChanged += FontDesignerViewModel_PropertyChanged;
        }

        private void FontDesignerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {

                case nameof(SelectedLayer):
                    if (canvasModel.IsPlacingItem())
                    {
                        //var placingItems = canvasModel.PlacingObject.PlacingObjects.OfType<SingleLayerBoardCanvasItem>().ToList();
                        //if (placingItems != null)
                        //    placingItems.ForEach(c => c.Layer = SelectedLayer);
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

        void HandleLayers()
        {
            if (LayerItems != null)
                foreach (var layer in LayerItems)
                    layer.HandleLayer();
        }

        FontDocument fontDocument;

        public override object Document
        {
            get
            {
                return fontDocument;
            }
        }

        public ObservableCollection<FontCharViewModel> Chars { get; set; } = new ObservableCollection<FontCharViewModel>();

        FontCharViewModel currentChar;

        public FontCharViewModel CurrentChar
        {
            get { return currentChar; }
            set
            {
                //currentChar = value;
                if (currentChar != null)
                {
                    //canvasModel.Items = currentChar.Items;
                    //save current char
                    var items = canvasModel.GetItems().OfType<SingleLayerBoardCanvasItem>();

                    currentChar.Items.Clear();
                    currentChar.Items.AddRange(items);

                    foreach (var item in items)
                        item.Layer = null;
                }

                currentChar = value;
                if (currentChar != null)
                {
                    foreach (var item in currentChar.Items.OfType<SingleLayerBoardCanvasItem>())
                        item.Layer = SelectedLayer;
                }

                OnPropertyChanged(nameof(CurrentChar));
            }
        }

        ICommand addCharCommand;
        public ICommand AddCharCommand
        {
            get
            {
                if (addCharCommand == null)
                    addCharCommand = CreateCommand(p =>
                    {
                        var newSChar = new FontCharViewModel { Name = "New" };
                        Chars.Add(newSChar);
                        CurrentChar = newSChar;
                    });

                return addCharCommand;
            }
        }

        ICommand deleteCharCommand;

        public ICommand DeleteCharCommand
        {
            get
            {
                if (deleteCharCommand == null)
                    deleteCharCommand = CreateCommand(p =>
                    {
                        if (MessageDialog.Show("Are you sure you want to delete this character?",
                            "Confirm delete",
                             XMessageBoxButton.YesNo) == XMessageBoxResult.Yes)
                        {
                            if (currentChar != null)
                            {
                                Chars.Remove(currentChar);
                                CurrentChar = Chars.FirstOrDefault();
                            }
                        }


                    });

                return deleteCharCommand;
            }
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

        bool maskUnselectedLayer;
        public bool MaskUnselectedLayer
        {
            get { return maskUnselectedLayer; }
            set
            {
                if (maskUnselectedLayer == value) return;
                maskUnselectedLayer = value;
                OnPropertyChanged(nameof(MaskUnselectedLayer));
            }
        }


        bool hideUnselectedLayer;
        public bool HideUnselectedLayer
        {
            get { return hideUnselectedLayer; }
            set
            {
                if (hideUnselectedLayer == value) return;
                hideUnselectedLayer = value;
                OnPropertyChanged(nameof(HideUnselectedLayer));
            }
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


        #endregion

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
                TooltipText = "Arc",
                Type = typeof(ArcBoardCanvasItem)
            });
        }

        protected override Task LoadDocumentInternal(string filePath)
        {
            return Task.Run(() =>
            {
                fontDocument = XmlHelper.Load<FontDocument>(filePath);

                LoadLayers();

                //load chars
                foreach (var charFont in fontDocument.Symbols)
                {
                    var newChar = new FontCharViewModel();
                    newChar.Name = charFont.Name;
                    if (charFont.Items != null)
                    {
                        foreach (var primitive in charFont.Items)
                        {
                            var canvasItem = (SingleLayerBoardCanvasItem)primitive.CreateDesignerItem();
                            canvasItem.LayerDocument = this;
                            newChar.Items.Add(canvasItem);
                        }
                    }

                    Chars.Add(newChar);
                }

                if (Chars.Count == 0)
                {
                    Chars.Add(new FontCharViewModel { Name = "new" });
                }

                CurrentChar = Chars[0];
            });
        }


        //GenericLayeredViewModel layerModel = new GenericLayeredViewModel();
        private void LoadLayers()
        {
            var silkLayer = Layer.GetTopOverlayLayer();
            LayerItems.Add(new LayerDesignerItem(this)
            {
                LayerName = silkLayer.Name,
                LayerId = silkLayer.Id,
                LayerType = silkLayer.Type,
                LayerColor = XColor.FromHexString(silkLayer.Color),
            });

            canvasModel.AddItems(LayerItems);

            SelectedLayer = LayerItems[0];
        }

        protected override void SaveDocumentInternal(string filePath)
        {
            //remove the currently adding item si that it won't be saved
            ISelectableItem placeObjects = null;
            if (canvasModel.IsPlacingItem())
            {
                placeObjects = canvasModel.PlacementTool.CanvasItem;
                canvasModel.RemoveItem(placeObjects);
            }

            //save
            //save current char
            var items = canvasModel.GetItems().OfType<SingleLayerBoardCanvasItem>();

            currentChar.Items.Clear();
            currentChar.Items.AddRange(items);

            fontDocument.Name = Path.GetFileNameWithoutExtension(filePath);
            fontDocument.Symbols = new List<FontSymbol>();
            foreach (var fc in Chars)
            {
                var dc = new FontSymbol
                {
                    Name = fc.Name,
                    Items = fc.Items.Cast<BaseCanvasItem>()
                                   .Select(d => (LayerPrimitive)d.SaveToPrimitive()).ToList()
                };
                fontDocument.Symbols.Add(dc);
            }

            XmlHelper.Save(fontDocument, filePath);

            //add the item back
            if (placeObjects != null)
                canvasModel.AddItem(placeObjects);
        }

    }

    public class FontCharViewModel : EditBoxModel
    {
        SpatialItemsSource items = new SpatialItemsSource();

        //it will need some items that are not selectable
        public SpatialItemsSource Items
        {
            get { return items; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
