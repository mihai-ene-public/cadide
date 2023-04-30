using System.Collections.Generic;
using IDE.Core.Interfaces;
using IDE.Core.Storage;

namespace IDE.Documents.Views
{
    public interface IFootprintDesigner : ICanvasDesignerFileViewModel
                                       , ILayeredViewModel
                                       , IDocumentOverview
    {
        IList<ModelDocument> GetModels();
    }
}
