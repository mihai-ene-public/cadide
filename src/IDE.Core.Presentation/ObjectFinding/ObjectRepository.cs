using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eagle;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Solution;
using IDE.Core.Storage;

namespace IDE.Core.Presentation.ObjectFinding;

public class ObjectRepository<T> : IObjectRepository<T> where T : ILibraryItem
{

    public ObjectRepository(ISolutionRepository solutionRepository, ILibraryRepository libraryRepository)
    {
        _solutionRepository = solutionRepository;
        _libraryRepository = libraryRepository;
    }

    private IList<ProjectInfo> _solutionProjects;
    private readonly ISolutionRepository _solutionRepository;
    private readonly ILibraryRepository _libraryRepository;

    public IList<T> LoadObjects(ProjectInfo project, Func<T, bool> predicate = null, bool stopIfFound = false, DateTime? lastModified = null)
    {
        if (project == null)
            throw new Exception("Project document is null");

        var solutionFilePath = _solutionRepository.GetSolutionFilePath(project.ProjectPath);
        _solutionProjects = _solutionRepository.GetSolutionProjects(solutionFilePath);

        var items = new List<T>();

        var itemTypes = new Dictionary<Type, ItemType>
        {
            {typeof(Symbol), new ItemType{FileExtension = "symbol", ReferencesSource = lib => lib.Symbols } },
            {typeof(Footprint), new ItemType{FileExtension = "footprint", ReferencesSource = lib => lib.Footprints } },
            {typeof(ComponentDocument), new ItemType{FileExtension = "component", ReferencesSource = lib => lib.Components } },
            {typeof(ModelDocument), new ItemType{FileExtension = "model", ReferencesSource = lib => lib.Models } },
            {typeof(SchematicDocument), new ItemType{FileExtension = "schematic", ReferencesSource = null } },
        };

        var itemType = itemTypes[typeof(T)];

        LoadItems(itemType.FileExtension, items, project, itemType.ReferencesSource, predicate, stopIfFound, lastModified);

        return items;
    }

    private void LoadItems(string fileExtension,
                           IList<T> items, ProjectInfo project,
                           Func<LibraryDocument, IEnumerable<ILibraryItem>> source,
                           Func<T, bool> predicate = null,
                           bool stopIfFound = false,
                           DateTime? lastModifed = null)
    {
        LoadItemsFromProject(fileExtension, items, project, source,
                                               predicate: predicate,
                                               stopIfFound: stopIfFound,
                                               lastModifed: lastModifed);
    }

    private void LoadItemsFromProject(string fileExtension,
        IList<T> items,
        ProjectInfo project,
        Func<LibraryDocument, IEnumerable<ILibraryItem>> source,
        string libraryName = "local",
        Func<T, bool> predicate = null,
        bool stopIfFound = false,
        DateTime? lastModifed = null
        )
    {
        LoadItemsRecursive(fileExtension, items,
                           Path.GetDirectoryName(project.ProjectPath),
                           libraryName,
                           predicate: predicate,
                           stopIfFound: stopIfFound,
                           lastModifed: lastModifed);

        LoadItemsFromReferences(fileExtension, items, project, source,
                                predicate: predicate,
                                stopIfFound: stopIfFound,
                                lastModifed: lastModifed);
    }

    private void LoadItemsRecursive(string fileExtension,
        IList<T> items,
        string containerFolderPath,
        string libraryName = "local",
        Func<T, bool> predicate = null,
        bool stopIfFound = false,
        DateTime? lastModifed = null)
    {
        foreach (var file in Directory.GetFiles(containerFolderPath, $"*.{fileExtension}", SearchOption.AllDirectories))
        {
            try
            {
                var fileInfo = new FileInfo(file);
                if (lastModifed != null && fileInfo.LastWriteTime <= lastModifed.Value)
                {
                    continue;
                }

                var itemDocument = XmlHelper.Load<T>(file);
                itemDocument.Library = libraryName;
                itemDocument.Name = Path.GetFileNameWithoutExtension(file);
                itemDocument.LastAccessed = DateTime.Now;

                if (predicate != null)
                {
                    if (predicate(itemDocument))
                        items.Add(itemDocument);
                }
                else
                {
                    items.Add(itemDocument);
                }

                if (stopIfFound && items.Count > 0)
                    return;
            }
            catch { }
        }

    }

    private void LoadItemsFromReferences(string fileExtension,
        IList<T> items, ProjectInfo projectInfo,
        Func<LibraryDocument, IEnumerable<ILibraryItem>> source,
        Func<T, bool> predicate,
        bool stopIfFound,
        DateTime? lastModifed)
    {
        if (source == null)
            return;

        var project = projectInfo.Project;
        if (project.References == null)
            return;

        var otherSolutionProjects = _solutionProjects.Where(p => p.ProjectPath != projectInfo.ProjectPath).ToList();
        var projectFolder = Path.GetDirectoryName(projectInfo.ProjectPath);


        foreach (var reference in project.References)
        {
            switch (reference)
            {
                //project reference
                case ProjectProjectReference projRef:
                    {
                        var projRefName = Path.GetFileNameWithoutExtension(projRef.ProjectPath);
                        var referedProj = otherSolutionProjects.FirstOrDefault(p => Path.GetFileNameWithoutExtension(p.ProjectPath) == projRefName);
                        if (referedProj != null)
                        {
                            LoadItemsFromProject(fileExtension, items, referedProj, source, projRefName, predicate, stopIfFound, lastModifed);
                        }
                    }
                    break;

                //library reference
                case LibraryProjectReference libRef:
                    {
                        LoadItemsFromLibRef(projectFolder, libRef, source, lastModifed, predicate, stopIfFound, items);

                    }
                    break;

                case PackageProjectReference packRef:
                    {
                        LoadItemsFromPackage(packRef, source, lastModifed, predicate, stopIfFound, items);
                        break;
                    }
            }


        }
    }

    private void LoadItemsFromPackage(PackageProjectReference packRef,
        Func<LibraryDocument, IEnumerable<ILibraryItem>> source,
        DateTime? lastModifed,
        Func<T, bool> predicate,
        bool stopIfFound,
        IList<T> items)
    {
        var libFiles = _libraryRepository.GetAllLibrariesFilePaths(packRef.PackageId, packRef.PackageVersion);

        foreach (var libFile in libFiles)
        {
            LoadItemsFromLibraryFile(Path.GetFileNameWithoutExtension(libFile), source, lastModifed, predicate, stopIfFound, items, libFile);
        }
    }

    private void LoadItemsFromLibRef(string projectFolder, LibraryProjectReference libRef,
        Func<LibraryDocument, IEnumerable<ILibraryItem>> source,
        DateTime? lastModifed,
        Func<T, bool> predicate,
        bool stopIfFound,
        IList<T> items)
    {
        var libFile = _libraryRepository.FindLibraryFilePath(libRef.LibraryName);
        if (!File.Exists(libFile))
            return;

        LoadItemsFromLibraryFile(libRef.LibraryName, source, lastModifed, predicate, stopIfFound, items, libFile);
    }

    private static void LoadItemsFromLibraryFile(string libraryName, Func<LibraryDocument, IEnumerable<ILibraryItem>> source, DateTime? lastModifed, Func<T, bool> predicate, bool stopIfFound, IList<T> items, string libFile)
    {
        try
        {
            var libInfo = new FileInfo(libFile);
            if (lastModifed != null && libInfo.LastWriteTime <= lastModifed.Value)
            {
                return;
            }

            var libDoc = XmlHelper.Load<LibraryDocument>(libFile);
            var itemsInDoc = source(libDoc).ToList();
            if (itemsInDoc != null)
            {
                foreach (T li in itemsInDoc)
                {
                    li.Library = libraryName;
                    li.LastAccessed = DateTime.Now;

                    if (predicate != null)
                    {
                        if (predicate(li))
                            items.Add(li);
                    }
                    else
                    {
                        items.Add(li);
                    }

                    if (stopIfFound && items.Count > 0)
                        return;
                }
            }
        }
        catch { }
    }

    private class ItemType
    {
        public string FileExtension { get; set; }
        public Func<LibraryDocument, IEnumerable<ILibraryItem>> ReferencesSource { get; set; }
    }
}

public class ObjectRepository : IObjectRepository
{
    public IList<T> LoadObjects<T>(ProjectInfo project, Func<T, bool> predicate = null, bool stopIfFound = false, DateTime? lastModified = null) where T : ILibraryItem
    {
        var repo = ServiceProvider.Resolve<IObjectRepository<T>>();
        var objects = repo.LoadObjects(project, predicate, stopIfFound, lastModified);

        return objects;
    }
}
