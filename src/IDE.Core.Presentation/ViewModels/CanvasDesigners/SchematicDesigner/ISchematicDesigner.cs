using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Designers;
using IDE.Core.ViewModels;

namespace IDE.Core.Interfaces
{
    public interface ISchematicDesigner : IDocumentOverview
                                        , ICanvasWithHighlightedItems
                                        , ICanvasDesignerFileViewModel
    {
        IList<ISheetDesignerItem> Sheets { get; }
        INetManager NetManager { get; }
        IBusManager BusManager { get; }

        IList<ISchematicRuleModel> Rules { get; }

        IList<ISchematicNet> GetNets();
    }


}
