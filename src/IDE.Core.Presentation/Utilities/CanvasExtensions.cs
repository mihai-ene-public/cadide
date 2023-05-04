using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.Presentation.Utilities;

public static class CanvasExtensions
{
    public static IEnumerable<FootprintBoardCanvasItem> GetFootprints(this ICanvasDesignerFileViewModel canvasModel)
    {
        return canvasModel.GetItems().OfType<FootprintBoardCanvasItem>();
    }

    public static INetManager GetNetManager(this ICanvasDesignerFileViewModel canvasModel)
    {
        return (canvasModel as ISchematicDesigner)?.NetManager;
    }

    public static IBusManager GetBusManager(this ICanvasDesignerFileViewModel canvasModel)
    {
        return (canvasModel as ISchematicDesigner)?.BusManager;
    }
}
