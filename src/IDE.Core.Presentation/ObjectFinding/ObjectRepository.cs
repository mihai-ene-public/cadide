using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Solution;
using IDE.Core.Storage;

namespace IDE.Core.Presentation.ObjectFinding
{
    public class ObjectRepository<T> : IObjectRepository<T> where T : ILibraryItem
    {

        public ObjectRepository(ISolutionRepository solutionRepository)
        {
            _solutionRepository = solutionRepository;
        }

        private IList<IProjectDocument> _solutionProjects;
        private readonly ISolutionRepository _solutionRepository;

        public IList<T> LoadObjects(IProjectDocument project, Func<T, bool> predicate = null, bool stopIfFound = false, DateTime? lastModified = null)
        {
            if (project == null)
                throw new Exception("Project document is null");

            _solutionProjects = _solutionRepository.GetSolutionProjects(SolutionManager.SolutionFilePath);

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
                               IList<T> items, IProjectDocument project,
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
            IProjectDocument project,
            Func<LibraryDocument, IEnumerable<ILibraryItem>> source,
            string libraryName = "local",
            Func<T, bool> predicate = null,
            bool stopIfFound = false,
            DateTime? lastModifed = null
            )
        {
            LoadItemsRecursive(fileExtension, items,
                               Path.GetDirectoryName(project.FilePath),
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
            IList<T> items, IProjectDocument project,
            Func<LibraryDocument, IEnumerable<ILibraryItem>> source,
            Func<T, bool> predicate,
            bool stopIfFound,
            DateTime? lastModifed)
        {
            if (source == null)
                return;

            if (project.References == null)
                return;

            var otherSolutionProjects = _solutionProjects.Where(p => p.FilePath != project.FilePath).ToList();
            var projectFolder = Path.GetDirectoryName(project.FilePath);
            var referencesFolder = Path.Combine(projectFolder, "References");

            foreach (var reference in project.References)
            {
                //project reference
                if (reference is ProjectProjectReference projRef)
                {
                    var projRefName = Path.GetFileNameWithoutExtension(projRef.ProjectPath);
                    var referedProj = otherSolutionProjects.FirstOrDefault(p => Path.GetFileNameWithoutExtension(p.FilePath) == projRefName);
                    if (referedProj != null)
                    {
                        LoadItemsFromProject(fileExtension, items, referedProj, source, projRefName, predicate, stopIfFound, lastModifed);
                    }
                    //todo: load recursive in references of the reference
                }
                else if (reference is LibraryProjectReference libRef)
                {
                    var libFile = Path.Combine(referencesFolder, libRef.LibraryName + ".libref");
                    if (File.Exists(libFile))
                    {
                        try
                        {
                            var libInfo = new FileInfo(libFile);
                            if (lastModifed != null && libInfo.LastWriteTime <= lastModifed.Value)
                            {
                                continue;
                            }

                            var libDoc = XmlHelper.Load<LibraryDocument>(libFile);
                            var itemsInDoc = source(libDoc).ToList();
                            if (itemsInDoc != null)
                            {
                                foreach (T li in itemsInDoc)
                                {
                                    li.Library = libRef.LibraryName;
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
                }
            }
        }

        private class ItemType
        {
            public string FileExtension { get; set; }
            public Func<LibraryDocument, IEnumerable<ILibraryItem>> ReferencesSource { get; set; }
        }
    }
}
