using System;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.ObjectFinding
{
    public class ObjectFinder : IObjectFinder
    {
        public void LoadCache<T>(IProjectDocument project) where T : ILibraryItem
        {
            var finder = ServiceProvider.Resolve<IObjectFinder<T>>();
            finder.LoadCache(project);
        }
        public void ClearCache<T>() where T : ILibraryItem
        {
            var finder = ServiceProvider.Resolve<IObjectFinder<T>>();
            finder.ClearCache();
        }

        public T FindCachedObject<T>(long id) where T : ILibraryItem
        {
            var finder = ServiceProvider.Resolve<IObjectFinder<T>>();
            return finder.FindCachedObject(id);
        }

        public T FindObject<T>(IProjectDocument project, string libraryName, long id, DateTime? lastModified = null) where T : ILibraryItem
        {
            var finder = ServiceProvider.Resolve<IObjectFinder<T>>();

            return finder.FindObject(project, libraryName, id, lastModified);
        }

        public T FindObject<T>(IProjectDocument project, long id) where T : ILibraryItem
        {
            var finder = ServiceProvider.Resolve<IObjectFinder<T>>();

            return finder.FindObject(project, id);
        }


    }
}
