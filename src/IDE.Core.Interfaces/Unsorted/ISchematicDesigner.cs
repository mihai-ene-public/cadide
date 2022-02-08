using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface ISchematicDesigner : IDocumentOverview
                                        , ICanvasWithHighlightedItems
                                        , IFileBaseViewModel
    {
        IDrawingViewModel CanvasModel { get; }

        INetManager NetManager { get; }
        IBusManager BusManager { get; }

        IList<string> OutputFiles { get; }

        IList<ISchematicRuleModel> Rules { get; }

        Task Build();
    }
}
