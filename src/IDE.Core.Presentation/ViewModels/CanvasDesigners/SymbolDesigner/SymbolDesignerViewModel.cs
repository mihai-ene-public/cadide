using System.Collections;
using System.IO;
using System.Windows.Input;
using IDE.Core;
using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Placement;
using IDE.Core.Storage;
using IDE.Core.Toolbars;
using IDE.Core.Types.Media;
using IDE.Core.ViewModels;

namespace IDE.Documents.Views;

public class SymbolDesignerViewModel : CanvasDesignerFileViewModel, ISymbolDesignerViewModel
{
    public SymbolDesignerViewModel(
            IDispatcherHelper dispatcher,
            IDebounceDispatcher drawingChangedDebouncer,
            IDebounceDispatcher selectionDebouncer,
            IDirtyMarkerTypePropertiesMapper dirtyMarkerTypePropertiesMapper,
            IPlacementToolFactory placementToolFactory)
        : base(dispatcher, drawingChangedDebouncer, selectionDebouncer, dirtyMarkerTypePropertiesMapper, placementToolFactory)
    {

        symbolDocument = new Symbol();

        Toolbar = new SymbolToolbar(this);

        var docSize = 25.4 * 10;
        var halfSize = docSize * 0.5;
        DocumentWidth = docSize;
        DocumentHeight = docSize;
        Origin = new XPoint(halfSize, halfSize);

        canvasGrid.SetUnit(new Core.Units.MilUnit(50));
    }

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
        var pins = Items.OfType<PinCanvasItem>().Select(p => new OverviewSelectNode
        {
            DataItem = p,
        });
        pinsCat.Children.AddRange(pins);

        primitivesCat.Children.AddRange(Items.Except(Items.OfType<PinCanvasItem>()).Select(p => new OverviewSelectNode
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

    Symbol symbolDocument;

    ICommand addSymbolCommand;

    public ICommand AddSymbolCommand
    {
        get
        {
            if (addSymbolCommand == null)
                addSymbolCommand = CreateCommand(p =>
                  {
                      ClearSelectedItems();
                      CancelPlacement();


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
                              var offset = SnapToGrid(new XPoint(rect.X, rect.Y));
                              canvasItems.ForEach(c => c.Translate(-offset.X, -offset.Y));

                              var group = new VolatileGroupCanvasItem
                              {
                                  Items = canvasItems.Cast<ISelectableItem>().ToList()
                              };
                              StartPlacement(group);
                          }
                      }
                  }

                  );

            return addSymbolCommand;
        }
    }

    protected override void SaveDocumentInternal(string filePath)
    {
        //remove the currently adding item so that it won't be saved
        ISelectableItem placeObjects = null;
        if (IsPlacingItem())
        {
            placeObjects = PlacementTool.CanvasItem;
            RemoveItem(placeObjects);
        }

        //assign a new id if needed
        if (string.IsNullOrEmpty(symbolDocument.Id))
        {
            symbolDocument.Id = LibraryItem.GetNextId();
        }
        symbolDocument.Name = Path.GetFileNameWithoutExtension(filePath);
        symbolDocument.Items = Items.Cast<BaseCanvasItem>().Select(d => (SchematicPrimitive)d.SaveToPrimitive()).ToList();

        XmlHelper.Save(symbolDocument, filePath);

        if (placeObjects != null)
            AddItem(placeObjects);
    }

    protected override Task LoadDocumentInternal(string filePath)
    {
        symbolDocument = XmlHelper.Load<Symbol>(filePath);

        if (symbolDocument.Items != null)
        {
            foreach (var primitive in symbolDocument.Items)
            {
                var canvasItem = primitive.CreateDesignerItem();
                AddItem(canvasItem);
            }
        }

        return Task.CompletedTask;
    }

    protected override async Task AfterLoadDocumentInternal()
    {
        await base.AfterLoadDocumentInternal();
        await RefreshOverview();
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
            TooltipText = "Pin",
            Type = typeof(PinCanvasItem)
        });
    }

}
