using System;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.ObjectFinding
{
    public interface IObjectFinder
    {
        T FindObject<T>(IProjectDocument project, long id) where T : ILibraryItem;
        T FindObject<T>(IProjectDocument project, string libraryName, long id, DateTime? lastModified = null) where T : ILibraryItem;
    }
}
