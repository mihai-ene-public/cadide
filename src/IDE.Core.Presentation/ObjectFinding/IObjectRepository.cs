using System;
using System.Collections.Generic;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Solution;

namespace IDE.Core.Presentation.ObjectFinding
{
    public interface IObjectRepository<T> where T : ILibraryItem
    {
        IList<T> LoadObjects(ProjectInfo project, Func<T, bool> predicate = null, bool stopIfFound = false, DateTime? lastModified = null);
    }

    public interface IObjectRepository
    {
        IList<T> LoadObjects<T>(ProjectInfo project,
                                Func<T, bool> predicate = null,
                                bool stopIfFound = false,
                                DateTime? lastModified = null) where T : ILibraryItem;
    }
}
