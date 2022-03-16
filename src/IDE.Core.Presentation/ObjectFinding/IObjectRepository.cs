using System;
using System.Collections.Generic;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.ObjectFinding
{
    public interface IObjectRepository<T> where T : ILibraryItem
    {
        IList<T> LoadObjects(IProjectDocument project, string searchText, Func<T, bool> predicate = null, bool stopIfFound = false, DateTime? lastModified = null);
    }
}
