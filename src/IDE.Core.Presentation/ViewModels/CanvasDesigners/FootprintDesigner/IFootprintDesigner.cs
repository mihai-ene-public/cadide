using System.Collections.Generic;
using IDE.Core.Interfaces;
using IDE.Core.Storage;

namespace IDE.Documents.Views
{
    public interface IFootprintDesigner : IFileBaseViewModel
                                       , ILayeredViewModel
                                       , IDocumentOverview
    {
        IList<ModelDocument> GetModels();
    }
}
