using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface ISolutionProjectNodeModel : ISolutionExplorerNodeModel
    {
        IProjectDocument Project { get; }

        IProjectFileRef FileItem { get; set; }

        void Save();

        List<ILibraryItem> LoadObjects(string searchText, TemplateType type, Func<ILibraryItem, bool> predicate = null, bool stopIfFound = false, DateTime? lastModified = null);
    }
}
