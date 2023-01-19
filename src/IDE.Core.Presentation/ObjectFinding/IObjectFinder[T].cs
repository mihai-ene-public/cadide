using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.ObjectFinding
{
    public interface IObjectFinder<T> where T : ILibraryItem
    {
        void LoadCache(ProjectInfo project);

        void ClearCache();

        T FindCachedObject(string id);

        T FindObject(ProjectInfo project, string id);

        T FindObject(ProjectInfo project, string libraryName, string id, DateTime? lastModified = null);
    }
}
