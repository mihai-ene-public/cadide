using System;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.ObjectFinding
{
    public interface IObjectFinder
    {
        void LoadCache<T>(IProjectDocument project) where T : ILibraryItem;

        void ClearCache<T>() where T : ILibraryItem;

        T FindCachedObject<T>(long id) where T : ILibraryItem;
        T FindObject<T>(IProjectDocument project, long id) where T : ILibraryItem;
        T FindObject<T>(IProjectDocument project, string libraryName, long id, DateTime? lastModified = null) where T : ILibraryItem;
    }
}
