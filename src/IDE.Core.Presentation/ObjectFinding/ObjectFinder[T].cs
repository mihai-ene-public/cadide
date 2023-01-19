using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IDE.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace IDE.Core.Presentation.ObjectFinding;

public class ObjectFinder<T> : IObjectFinder<T> where T : ILibraryItem
{
    private readonly IObjectRepository<T> _objectRepository;
    private readonly IMemoryCache _memoryCache;


    IList<T> cachedItems = new List<T>();
    public ObjectFinder(IObjectRepository<T> objectRepository, IMemoryCache memoryCache)
    {
        _objectRepository = objectRepository;
        _memoryCache = memoryCache;
    }

    public void LoadCache(ProjectInfo project)
    {
        cachedItems = _objectRepository.LoadObjects(project);
    }
    public void ClearCache()
    {
        if (cachedItems != null)
            cachedItems.Clear();
    }

    public T FindCachedObject(string id)
    {

        if (cachedItems != null && cachedItems.Count > 0)
        {
            return cachedItems.FirstOrDefault(o => o.Id == id);
        }

        return default(T);
    }

    public T FindObject(ProjectInfo project, string id)
    {
        if (project == null)
            throw new Exception("Project document was not initialized");

        IList<T> items = null;

        var predicate = new Func<T, bool>(li =>
        {
            var idMatched = li.Id == id;

            return idMatched;
        });

        items = _objectRepository.LoadObjects(project,
              predicate,
              stopIfFound: true,
              lastModified: null);

        var item = items.FirstOrDefault();

        return item;
    }

    public T FindObject(ProjectInfo project, string libraryName, string id, DateTime? lastModified = null)
    {
        if (project == null)
            throw new Exception("Project document was not initialized");

        //lib name local or null?
        var isLocal = libraryName == null || libraryName == "local";

        var predicate = new Func<T, bool>(li =>
        {
            var idMatched = li.Id == id;
            if (idMatched)
            {
                return isLocal && li.IsLocal || li.Library == libraryName;
            }

            return false;
        });

        if (isLocal)
        {
            libraryName = Path.GetFileNameWithoutExtension(project.ProjectPath);
        }

        var cacheKey = $"{libraryName}.{id}";

        if (lastModified == null)
        {
            var cachedItem = _memoryCache.Get<T>(cacheKey);

            if (cachedItem != null)
            {
                return cachedItem;
            }
        }

        var items = _objectRepository.LoadObjects(project,
                  predicate,
                  stopIfFound: true,
                  lastModified: lastModified);

        var item = items.FirstOrDefault();

        if (item != null)
        {
            _memoryCache.Set(cacheKey, item);
        }

        return item;
    }
}
