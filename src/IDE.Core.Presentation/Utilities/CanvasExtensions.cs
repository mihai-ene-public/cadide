using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.Presentation.Utilities;

public static class CanvasExtensions
{
    public static IEnumerable<FootprintBoardCanvasItem> GetFootprints(this IDrawingViewModel canvasModel)
    {
        return canvasModel.GetItems().OfType<FootprintBoardCanvasItem>();
    }

    public static INetManager GetNetManager(this IDrawingViewModel canvasModel)
    {
        return (canvasModel.FileDocument as ISchematicDesigner)?.NetManager;
    }

    public static IBusManager GetBusManager(this IDrawingViewModel canvasModel)
    {
        return (canvasModel.FileDocument as ISchematicDesigner)?.BusManager;
    }
}
