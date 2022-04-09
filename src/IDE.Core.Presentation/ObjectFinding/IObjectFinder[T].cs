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
        void LoadCache(IProjectDocument project);

        void ClearCache();

        T FindCachedObject(long id);

        T FindObject(IProjectDocument project, long id);

        T FindObject(IProjectDocument project, string libraryName, long id, DateTime? lastModified = null);
    }
}
