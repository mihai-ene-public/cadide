using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.Presentation.Placement
{
    public class BusLabelPlacementTool : PlacementTool, IBusLabelPlacementTool
    {
        public BusLabelPlacementTool()
        {
            GeometryHelper = ServiceProvider.Resolve<IGeometryHelper>();
        }

        private readonly IGeometryHelper GeometryHelper;

        BusLabelCanvasItem GetItem() => canvasItem as BusLabelCanvasItem;

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

            switch (PlacementStatus)
            {
                case PlacementStatus.Ready:
                    item.X = mp.X;
                    item.Y = mp.Y - 3;

                    //nets that intersect at this point
                    var busWires = CanvasModel.Items.OfType<BusWireCanvasItem>().ToList();
                    var intersectedBuses = new List<BusWireCanvasItem>();
                    foreach (var busWire in busWires)
                    {

                        var intersects = GeometryHelper.ItemIntersectsPoint(busWire, mp, 0.5);
                        if (!intersects)
                            continue;

                        //add a net that wasn't added before
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
                        var netRef = intersectedBuses.FirstOrDefault().Bus;

                        item.Bus = netRef;
                        item.OnPropertyChanged(nameof(item.Bus));
                        item.OnPropertyChanged(nameof(item.BusName));
                        item.IsPlaced = true;
                        CanvasModel.OnDrawingChanged(DrawingChangedReason.ItemPlacementFinished);

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
}
