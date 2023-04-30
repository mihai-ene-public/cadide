using IDE.Core;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.Converters;
using System.ComponentModel;
using IDE.Core.Presentation.Placement;
using IDE.Core.ViewModels;
using System.Collections;


namespace IDE.Documents.Views;

public abstract class PreviewCanvasViewModel : BaseViewModel, IPreviewCanvasViewModel
{
    public PreviewCanvasViewModel()
    {
        Scale = 1;
        CanvasBackground = XColor.FromHexString("#FF616161");
        GridColor = XColors.Gray;

        _selectionDebouncer = new DebounceDispatcher();
    }

    private readonly IDebounceDispatcher _selectionDebouncer;


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

    private double x;
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

    protected ICanvasGrid canvasGrid = new CanvasGrid();

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

    public double GridSize => canvasGrid.GridSize;

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

    protected IList<ISelectableItem> items = new SpatialItemsSource();
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

    public void ZoomToFit()
    {
        ZoomToItems(GetItems().ToList());
    }

    private void ZoomToItems(IList<ISelectableItem> items)
    {
        var rect = XRect.Empty;

        foreach (BaseCanvasItem item in items.OfType<BaseCanvasItem>())
        {
            var itemRect = item.GetBoundingRectangle();

            rect.Union(itemRect);
        }

        ZoomToRectangle(rect);
    }
    private void ZoomToRectangle(XRect rect)
    {
        if (rect.IsEmpty)
            return;

        rect.Inflate(5, 5);
        const double zoomFactor = 0.9;//0.8f;
        var size = new XSize(DocumentWidth, DocumentHeight);

        //var origin = canvasModel.Origin;
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

    public void ClearSelectedItems()
    {
        foreach (ISelectableItem item in GetItems())
        {
            item.IsSelected = false;
        }

        UpdateSelection();
    }
    public void UpdateSelection()
    {
        _selectionDebouncer.Debounce(50, p => UpdateSelectionInternal());
    }
    private void UpdateSelectionInternal()
    {
        var pw = ServiceProvider.GetToolWindow<PropertiesToolWindowViewModel>();
        if (pw != null)
        {
            var oldSelected = pw.SelectedObject;
            pw.SelectedObject = null;

            //if (PlacementTool != null)
            //    AssignSelected(pw, PlacementTool.CanvasItem);
            //else
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

    protected virtual void OnSelectionChanged(object sender, EventArgs args)
    {

    }



}
