using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    /// <summary>
    /// this interface should be implemented by the document model that want to push its items on Document Overview tool window
    /// </summary>
    public interface IDocumentOverview
    {
        IList<IOverviewSelectNode> Categories { get; }

        Task RefreshOverview();
    }

    public interface ICanvasWithHighlightedItems
    {
        void ClearHighlightedItems();
    }
}
