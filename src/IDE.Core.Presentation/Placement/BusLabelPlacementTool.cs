using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Placement;

public class BusLabelPlacementTool : PlacementTool<BusLabelCanvasItem>, IBusLabelPlacementTool
{
    public BusLabelPlacementTool()
    {
        GeometryHelper = ServiceProvider.Resolve<IGeometryOutlineHelper>();
    }

    private readonly IGeometryOutlineHelper GeometryHelper;

    public override void PlacementMouseMove(XPoint mousePosition)
    {
        var mp = CanvasModel.SnapToGrid(mousePosition);

        var item = GetItem();

        switch (PlacementStatus)
        {
            case PlacementStatus.Ready:
                item.X = mp.X;
                item.Y = mp.Y - 3;//3mm upper
                break;
        }
    }

    public override void PlacementMouseUp(XPoint mousePosition)
    {
        var mp = CanvasModel.SnapToGrid(mousePosition);

        var item = GetItem();
        var circle = new CircleCanvasItem { Diameter = 0.5, X = mp.X, Y = mp.Y, BorderWidth = 0.0 };

        switch (PlacementStatus)
        {
            case PlacementStatus.Ready:
                item.X = mp.X;
                item.Y = mp.Y - 3;

                //buses that intersect at this point
                var busWires = CanvasModel.Items.OfType<BusWireCanvasItem>().ToList();
                var intersectedBuses = new List<BusWireCanvasItem>();
                foreach (var busWire in busWires)
                {

                    var intersects = GeometryHelper.Intersects(busWire, circle);
                    if (!intersects)
                        continue;

                    //add a bus that wasn't added before
                    if (busWire.Bus != null)
                    {
                        var net = intersectedBuses.FirstOrDefault(n => n.Bus != null && n.Bus.Name == busWire.Bus.Name);
                        if (net == null)
                            intersectedBuses.Add(busWire);
                    }
                }

                if (intersectedBuses.Count > 0)
                {
                    //we can select an item, but for now we take the 1st
                    var busRef = intersectedBuses.FirstOrDefault().Bus;

                    item.Bus = busRef;
                    item.OnPropertyChanged(nameof(item.Bus));
                    item.OnPropertyChanged(nameof(item.BusName));
                    item.IsPlaced = true;
                    CommitPlacement();

                    //create another text
                    var newItem = (BusLabelCanvasItem)canvasItem.Clone();

                    PlacementStatus = PlacementStatus.Ready;
                    canvasItem = newItem;

                    CanvasModel.AddItem(canvasItem);
                }
                else
                {
                    return;
                }
                break;
        }
    }
}
