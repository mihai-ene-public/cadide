using System;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.ObjectFinding
{
    public interface IObjectFinder
    {
        void LoadCache<T>(ProjectInfo project) where T : ILibraryItem;

        void ClearCache<T>() where T : ILibraryItem;

        T FindCachedObject<T>(string id) where T : ILibraryItem;
        T FindObject<T>(ProjectInfo project, string id) where T : ILibraryItem;
        T FindObject<T>(ProjectInfo project, string libraryName, string id, DateTime? lastModified = null) where T : ILibraryItem;
    }
}
