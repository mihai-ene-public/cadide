using IDE.Core.Interfaces;
using IDE.Core.Presentation.Utilities;

namespace IDE.Core.Designers;

public class BusWireSegmentRemover : SegmentRemoverHelper<BusWireCanvasItem>
{
    public BusWireSegmentRemover(IDispatcherHelper dispatcher)
       : base(dispatcher)
    {

    }

    protected override BusWireCanvasItem CreateAnotherItem(BusWireCanvasItem canvasItem)
    {
        return new BusWireCanvasItem()
        {
            ParentObject = canvasItem.ParentObject,
            Width = canvasItem.Width,
            LineColor = canvasItem.LineColor,
            Nets = canvasItem.Nets,
            ZIndex = canvasItem.ZIndex,
            IsPlaced = true
        };
    }

    protected override void RunRemoveBehavior(BusWireCanvasItem canvasItem, IList<BusWireCanvasItem> newTracks, ICanvasDesignerFileViewModel canvasModel)
    {
        var bus = canvasItem.Bus;

        var busManager = canvasModel.GetBusManager();

        if (bus == null)
            return;

        canvasItem.Bus = null;

        foreach (var track in newTracks)
        {
            if (track.Points.Count > 1)
            {
                var newBus = (SchematicBus)busManager.AddNew();
                track.Bus = newBus;
            }
        }

        if (bus.BusItems?.Count == 0)
        {
            busManager.Remove(bus);
        }
    }

    protected override object GetSignal(BusWireCanvasItem canvasItem)
    {
        return canvasItem.Bus;
    }
    protected override void SetSignal(BusWireCanvasItem canvasItem, object signal)
    {
        canvasItem.Bus = (SchematicBus)signal;
    }
}
