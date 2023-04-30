using Eagle;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Types.Media;

namespace IDE.Core.Designers;

public class NetWireSegmentRemover : SegmentRemoverHelper<NetWireCanvasItem>
{
    public NetWireSegmentRemover(IDispatcherHelper dispatcher)
        : base(dispatcher)
    {

    }

    protected override NetWireCanvasItem CreateAnotherItem(NetWireCanvasItem canvasItem)
    {
        return new NetWireCanvasItem
        {
            ParentObject = canvasItem.ParentObject,
            Width = canvasItem.Width,
            LineColor = canvasItem.LineColor,
            ZIndex = canvasItem.ZIndex,
            IsPlaced = true
        };
    }

    private IList<ISelectableItem> pins = new List<ISelectableItem>();
    protected override IList<ISelectableItem> GetOtherAffectedItems()
    {
        return pins;
    }

    protected override void RunRemoveBehavior(NetWireCanvasItem canvasItem, IList<NetWireCanvasItem> newTracks, ICanvasDesignerFileViewModel canvasModel)
    {
        //assumptions:
        //one wire net endpoint (start, end) connect to a pin
        //a pin connects to a single net wire endpoint

        var net = canvasItem.Net;

        if (net == null)
            return;

        var netManager = canvasModel.GetNetManager();

        canvasItem.Net = null;

        foreach (var pin in net.NetItems.OfType<PinCanvasItem>().ToList())
        {
            if (PinConnectsWithWire(pin, canvasItem))
            {
                pin.Net = null;
                pins.Add(pin);

                foreach (var track in newTracks)
                {
                    if (PinConnectsWithWire(pin, track))
                    {
                        var newNet = (SchematicNet)netManager.AddNew();
                        pin.Net = newNet;
                        track.Net = newNet;

                        break;
                    }
                }
            }
        }

        foreach (var track in newTracks)
        {
            if (track.Net == null)
            {
                var newNet = (SchematicNet)netManager.AddNew();
                track.Net = newNet;
            }
        }

        if (net.NetItems?.Count == 0)
        {
            netManager.Remove(net);
        }
    }

    private bool PinConnectsWithWire(PinCanvasItem pin, NetWireCanvasItem netWire)
    {
        var t = pin.GetTransform();
        var pinPos = t.Transform(new XPoint()).Round();

        //maybe improve this with GeometryHelper.CirclesIntersect

        var sp = netWire.StartPoint.Round();
        var ep = netWire.EndPoint.Round();

        return pinPos == sp || pinPos == ep;
    }

    protected override object GetSignal(NetWireCanvasItem canvasItem)
    {
        return canvasItem.Net;
    }
    protected override object GetSignalOther(object canvasItem)
    {
        if (canvasItem is NetSegmentCanvasItem segmentItem)
        {
            return segmentItem.Net;
        }
        return null;
    }
    protected override void SetSignal(NetWireCanvasItem canvasItem, object signal)
    {
        canvasItem.Net = (SchematicNet)signal;
    }

    protected override void SetSignalOther(object canvasItem, object signal)
    {
        if(canvasItem is NetSegmentCanvasItem segmentItem)
        {
            segmentItem.Net = (SchematicNet)signal;
        }
    }

}
