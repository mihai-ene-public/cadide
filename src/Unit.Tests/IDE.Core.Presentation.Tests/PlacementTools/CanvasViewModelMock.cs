using IDE.Core.Presentation.Placement;
using IDE.Core.Interfaces;
using IDE.Documents.Views;
using IDE.Core.Designers;

namespace IDE.Core.Presentation.Tests.PlacementTools;

public class CanvasViewModelMock: CanvasDesignerFileViewModel
{
    public CanvasViewModelMock(IDispatcherHelper dispatcher,
        IDebounceDispatcher drawingChangedDebouncer,
        IDebounceDispatcher selectionDebouncer,
        IDirtyMarkerTypePropertiesMapper dirtyMarkerTypePropertiesMapper,
        IPlacementToolFactory placementToolFactory):base(dispatcher, drawingChangedDebouncer, selectionDebouncer, dirtyMarkerTypePropertiesMapper, placementToolFactory)
    {
        
    }
}
