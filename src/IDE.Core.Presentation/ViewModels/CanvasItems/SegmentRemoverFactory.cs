using IDE.Core.Interfaces;

namespace IDE.Core.Designers;

public class SegmentRemoverFactory
{
    public ISegmentRemoverHelper Create(ISegmentedPolylineSelectableCanvasItem wire, IDispatcherHelper dispatcher)
    {
        switch (wire)
        {
            case NetWireCanvasItem:
                {
                    return new NetWireSegmentRemover(dispatcher);
                }

            case BusWireCanvasItem:
                {
                    return new BusWireSegmentRemover(dispatcher);
                }

            case TrackBoardCanvasItem:
                {
                    return new TrackBoardWireSegmentRemover(dispatcher);
                }
        }

        throw new NotImplementedException();
    }
}
