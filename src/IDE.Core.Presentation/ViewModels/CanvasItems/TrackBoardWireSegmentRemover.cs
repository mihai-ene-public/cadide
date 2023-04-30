using IDE.Core.Interfaces;

namespace IDE.Core.Designers;

public class TrackBoardWireSegmentRemover : SegmentRemoverHelper<TrackBoardCanvasItem>
{
    public TrackBoardWireSegmentRemover(IDispatcherHelper dispatcher)
       : base(dispatcher)
    {
        
    }

    protected override TrackBoardCanvasItem CreateAnotherItem(TrackBoardCanvasItem canvasItem)
    {
        return new TrackBoardCanvasItem()
        {
            LayerDocument = canvasItem.LayerDocument,
            ParentObject = canvasItem.ParentObject,
            Layer = canvasItem.Layer,
            Signal = canvasItem.Signal,//same signal (this is intended)
            Width = canvasItem.Width,
            IsPlaced = true
        };
    }

    protected override void RunRemoveBehavior(TrackBoardCanvasItem canvasItem, IList<TrackBoardCanvasItem> newTracks, ICanvasDesignerFileViewModel canvasModel)
    {
        //noop
    }

    protected override object GetSignal(TrackBoardCanvasItem canvasItem)
    {
       return canvasItem.Signal;
    }
    protected override void SetSignal(TrackBoardCanvasItem canvasItem, object signal)
    {
        canvasItem.Signal = (IBoardNetDesignerItem)signal;
    }
}
