using System;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.ObjectFinding
{
    public class ObjectFinder<T> : IObjectFinder<T> where T : ILibraryItem
    {
        private readonly IObjectRepository<T> _objectRepository;


        IList<T> cachedItems = new List<T>();
        public ObjectFinder(IObjectRepository<T> objectRepository)
        {
            _objectRepository = objectRepository;
        }

        public T FindObject(IProjectDocument project, long id)
        {
            if (project == null)
                throw new Exception("Project document was not initialized");

            IList<T> items = null;

            var predicate = new Func<T, bool>(li =>
            {
                var idMatched = li.Id == id;

                return idMatched;
            });

            if (cachedItems != null && cachedItems.Count > 0)
            {
                items = cachedItems.Where(predicate).ToList();
            }
            else
            {
                items = _objectRepository.LoadObjects(project, null,
                      predicate,
                      stopIfFound: true,
                      lastModified: null);
            }
            var item = items.FirstOrDefault();

            return item;
        }

        public T FindObject(IProjectDocument project, string libraryName, long id, DateTime? lastModified = null)
        {
            if (project == null)
                throw new Exception("Project document was not initialized");

            //lib name local or null?
            var isLocal = libraryName == null || libraryName == "local";

            IList<T> items = null;

            var predicate = new Func<T, bool>(li =>
            {
                var idMatched = li.Id == id;
                if (idMatched)
                {
                    return isLocal && li.IsLocal || li.Library == libraryName;
                }

                return false;
            });

            if (cachedItems != null && cachedItems.Count > 0)
            {
                items = cachedItems.Where(predicate).ToList();
            }
            else
            {
                items = _objectRepository.LoadObjects(project, null,
                      predicate,
                      stopIfFound: true,
                      lastModified: lastModified);
            }

            var item = items.FirstOrDefault();

            return item;
        }
    }
}
