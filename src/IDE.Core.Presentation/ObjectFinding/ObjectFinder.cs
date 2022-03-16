using System;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.ObjectFinding
{
    public class ObjectFinder : IObjectFinder
    {
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
