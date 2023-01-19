using System;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.ObjectFinding;

public class ObjectFinder : IObjectFinder
{
    public void LoadCache<T>(ProjectInfo project) where T : ILibraryItem
    {
        var finder = ServiceProvider.Resolve<IObjectFinder<T>>();
        finder.LoadCache(project);
    }
    public void ClearCache<T>() where T : ILibraryItem
    {
        var finder = ServiceProvider.Resolve<IObjectFinder<T>>();
        finder.ClearCache();
    }

    public T FindCachedObject<T>(string id) where T : ILibraryItem
    {
        var finder = ServiceProvider.Resolve<IObjectFinder<T>>();
        return finder.FindCachedObject(id);
    }

    public T FindObject<T>(ProjectInfo project, string libraryName, string id, DateTime? lastModified = null) where T : ILibraryItem
    {
        var finder = ServiceProvider.Resolve<IObjectFinder<T>>();

        return finder.FindObject(project, libraryName, id, lastModified);
    }

    public T FindObject<T>(ProjectInfo project, string id) where T : ILibraryItem
    {
        var finder = ServiceProvider.Resolve<IObjectFinder<T>>();

        return finder.FindObject(project, id);
    }


}
