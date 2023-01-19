using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using IDE.Core.Designers;
using IDE.Core.Storage;
using IDE.Core.Commands;
using IDE.Core.ViewModels;
using IDE.Core.Toolbars;
using IDE.Core.Interfaces;
using IDE.Core.Common;
using IDE.Core;
using System.Threading.Tasks;
using IDE.Core.Types.Media;
using System.Collections;

namespace IDE.Documents.Views
{
    public class SymbolDesignerViewModel : CanvasDesignerFileViewModel, ISymbolDesignerViewModel
    {
        #region ctors

        public SymbolDesignerViewModel()
            : base()
        {
            DocumentKey = "SymbolEditor";
            Description = "Symbol files";
            FileFilterName = "Symbol file";
            DefaultFilter = "symbol";
            documentTypeKey = DocumentKey;
            defaultFileType = "symbol";
            defaultFileName = "Symbol";

            symbolDocument = new Symbol();

            Toolbar = new SymbolToolbar(this);

            var docSize = 25.4 * 10;
            var halfSize = docSize * 0.5;
            canvasModel.DocumentWidth = docSize;
            canvasModel.DocumentHeight = docSize;
            canvasModel.Origin = new XPoint(halfSize, halfSize);

            canvasGrid.GridSizeModel.SelectedItem = new Core.Units.MilUnit(50);
        }

        CanvasGrid canvasGrid => canvasModel.CanvasGrid as CanvasGrid;

        public override IList<IDocumentToolWindow> GetToolWindowsWhenActive()
        {
            var tools = new List<IDocumentToolWindow>();

            tools.Add(ServiceProvider.GetToolWindow<DocumentOverviewViewModel>());
            tools.Add(ServiceProvider.GetToolWindow<SelectionFilterToolViewModel>());

            return tools;
        }

        IList<IOverviewSelectNode> BuildCategories()
        {
            var list = new List<IOverviewSelectNode>();
            var primitivesCat = new OverviewFolderNode { Name = "Primitives" };
            list.Add(primitivesCat);
            var pinsCat = new OverviewFolderNode { Name = "Pins" };
            list.Add(pinsCat);
            var pins = canvasModel.Items.OfType<PinCanvasItem>().Select(p => new OverviewSelectNode
            {
                //FormatText = "Pin {0}",
                //DisplayPropertyName = nameof(PinCanvasItem.Name),
                DataItem = p,
            });
            pinsCat.Children.AddRange(pins);

            primitivesCat.Children.AddRange(canvasModel.Items.Except(canvasModel.Items.OfType<PinCanvasItem>()).Select(p => new OverviewSelectNode
            {
                DataItem = p
            }));

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
                        TooltipText = "Pins",
                        Type = typeof(PinCanvasItem)
                    }
                };

                return canSelectList;
            }
        }

        #endregion ctors

        #region Fields



        Symbol symbolDocument;

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

                          canvasModel.CancelPlacement();


                          var projectInfo = GetCurrentProjectInfo();

                          var itemSelectDlg = new ItemSelectDialogViewModel(TemplateType.Symbol, projectInfo);
                          if (itemSelectDlg.ShowDialog() == true)
                          {
                              var symbol = itemSelectDlg.SelectedItem.Document as Symbol;

                              if (symbol.Items != null)
                              {
                                  var canvasItems = symbol.Items.Select(c => c.CreateDesignerItem()).ToList();


                                  var rect = XRect.Empty;
                                  foreach (var item in canvasItems)
                                      rect.Union(item.GetBoundingRectangle());
                                  var offset = canvasModel.SnapToGrid(new XPoint(rect.X, rect.Y));
                                  canvasItems.ForEach(c => c.Translate(-offset.X, -offset.Y));

                                  var group = new VolatileGroupCanvasItem
                                  {
                                      Items = canvasItems.Cast<ISelectableItem>().ToList()
                                  };
                                  //canvasModel.AddItem(group);
                                  canvasModel.StartPlacement(group);
                              }
                          }
                      }

                      );

                return addSymbolCommand;
            }
        }


        #region Save File

        protected override void SaveDocumentInternal(string filePath)
        {
            //remove the currently adding item so that it won't be saved
            ISelectableItem placeObjects = null;
            if (canvasModel.IsPlacingItem())
            {
                placeObjects = canvasModel.PlacementTool.CanvasItem;
                canvasModel.RemoveItem(placeObjects);
            }

            //assign a new id if needed
            if (string.IsNullOrEmpty(symbolDocument.Id))
            {
                symbolDocument.Id = LibraryItem.GetNextId();
            }
            symbolDocument.Name = Path.GetFileNameWithoutExtension(filePath);
            symbolDocument.Items = canvasModel.Items.Cast<BaseCanvasItem>().Select(d => (SchematicPrimitive)d.SaveToPrimitive()).ToList();

            XmlHelper.Save(symbolDocument, filePath);

            if (placeObjects != null)
                canvasModel.AddItem(placeObjects);
        }

        #endregion Save File

        #region LoadFile

        protected override Task LoadDocumentInternal(string filePath)
        {
            symbolDocument = XmlHelper.Load<Symbol>(filePath);

            if (symbolDocument.Items != null)
            {
                foreach (var primitive in symbolDocument.Items)
                {
                    var canvasItem = primitive.CreateDesignerItem();
                    canvasModel.AddItem(canvasItem);
                }
            }

            return Task.CompletedTask;
        }

        protected override async Task AfterLoadDocumentInternal()
        {
            await base.AfterLoadDocumentInternal();
            await RefreshOverview();
        }

        #endregion LoadFile

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
                TooltipText = "Pin",
                Type = typeof(PinCanvasItem)
            });
        }

    }
}
