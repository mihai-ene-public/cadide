using IDE.Core.Common;
using IDE.Core.Compilation;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IDE.Core.ViewModels
{
    public class SolutionProjectNodeModel : FilesContainerNodeModel, ISolutionProjectNodeModel
    {
        public IProjectDocument Project { get { return Document as ProjectDocument; } }

        ///// <summary>
        ///// the reference to the file
        ///// </summary>
        public IProjectFileRef FileItem { get; set; }

        protected override string GetNameInternal()
        {
            return Path.GetFileNameWithoutExtension(Project.FilePath);
        }

        protected override void DeleteItemInternal()
        {
            //remove the project from the solution, but don't delete any files
            var solution = SolutionManager.Solution;
            solution.Children.Remove((ProjectBaseFileRef)FileItem);

            //todo remove any project references

            SolutionManager.SaveSolution();
        }

        public override void Load(string filePath)
        {
            //load references and files in this project

            //references
            var referencesNode = new ProjectReferencesNodeModel();
            AddChild(referencesNode);
            referencesNode.Load(null);

            //files
            //order of the files in the project will be saved alphabetically

            //folders
            var projectDirectoryPath = Path.GetDirectoryName(filePath);
            LoadFolder(projectDirectoryPath);

        }

        public async override Task Compile()
        {
            var compiler = new Compiler();
            await compiler.CompileProject(this);
        }

        public async override Task Build()
        {
            var compiler = new Compiler();
            await compiler.BuildProject(this);
        }

        /// <summary>
        /// searches for an object defined in local project and in references
        /// <para>used to solve for refenced objects in xml documents</para>
        /// </summary>
        /// <param name="name">name of the object. Not null</param>
        /// <param name="libraryName">name of the library. May be null which is local</param>
        /// <param name="type">type of object to find: symbol, footrpint, component, etc</param>
        /// <returns></returns>
        public ILibraryItem FindObject(TemplateType type, string libraryName, long id, DateTime? lastModified = null)
        {
            if (Project == null)
                throw new Exception("Project document was not initialized");

            //lib name local or null?
            var isLocal = libraryName == null || libraryName == "local";

            List<ILibraryItem> items = null;

            var predicate = new Func<ILibraryItem, bool>(li =>
              {
                  var idMatched = li.Id == id;
                  if (idMatched)
                  {
                      return isLocal && li.IsLocal || li.Library == libraryName;
                  }

                  return false;
              });

            if (cachedItems != null && cachedItems.ContainsKey(type) && cachedItems[type].Count > 0)
            {
                items = cachedItems[type].Where(predicate).ToList();
            }
            else
            {
                items = LoadObjects(null, type,
                      predicate,
                      stopIfFound: true,
                      lastModified: lastModified);
            }

            var item = items.FirstOrDefault();

            return item;
        }

        public ILibraryItem FindObject(TemplateType type, long id)
        {
            if (Project == null)
                throw new Exception("Project document was not initialized");

            //var items = LoadObjects(type);

            //////lib name local or null?
            ////var isLocal = libraryName == null || libraryName == "local";
            ////items = items.Where(l => l.Id == id).ToList();
            ////LibraryItem item = null;
            ////if (isLocal)
            ////    item = items.FirstOrDefault(l => l.IsLocal);
            ////else
            ////    item = items.FirstOrDefault(l => l.Library == libraryName);
            //var item = items.FirstOrDefault(l => l.Id == id);

            List<ILibraryItem> items = null;

            var predicate = new Func<ILibraryItem, bool>(li =>
            {
                var idMatched = li.Id == id;
                //if (idMatched)
                //{
                //    return isLocal && li.IsLocal || li.Library == libraryName;
                //}

                return idMatched;

                // return false;
            });

            if (cachedItems != null && cachedItems.ContainsKey(type) && cachedItems[type].Count > 0)
            {
                items = cachedItems[type].Where(predicate).ToList();
            }
            else
            {
                items = LoadObjects(null, type,
                      predicate,
                      stopIfFound: true,
                      lastModified: null);
            }
            var item = items.FirstOrDefault();

            return item;

        }

        public List<ILibraryItem> LoadObjects(string searchText, TemplateType type, Func<ILibraryItem, bool> predicate = null, bool stopIfFound = false, DateTime? lastModified = null)
        {
            if (Project == null)
                throw new Exception("Project document was not initialized");

            var items = new List<ILibraryItem>();

            switch (type)
            {
                case TemplateType.Symbol:
                    LoadSymbols(items, predicate, stopIfFound, lastModified);
                    break;

                case TemplateType.Footprint:
                    LoadFootprints(items, predicate, stopIfFound, lastModified);
                    break;

                case TemplateType.Component:
                    LoadComponents(items, predicate, stopIfFound, lastModified);
                    break;

                case TemplateType.Schematic:
                    LoadSchematics(items, predicate);//there are not many schematics in a solution, so we don't optimize for now
                    break;

                case TemplateType.Model:
                    LoadModels(items, predicate, stopIfFound, lastModified);
                    break;
            }

            return items;
        }

        Dictionary<TemplateType, List<ILibraryItem>> cachedItems = new Dictionary<TemplateType, List<ILibraryItem>>();

        public void CreateCacheItems(TemplateType type)
        {
            if (cachedItems.ContainsKey(type) == false)
                cachedItems.Add(type, new List<ILibraryItem>());

            cachedItems[type] = LoadObjects(null, type);
        }

        public void ClearCachedItems()
        {
            if (cachedItems != null)
                cachedItems.Clear();
        }

        void LoadSymbols(List<ILibraryItem> symbols, Func<ILibraryItem, bool> predicate = null, bool stopIfFound = false, DateTime? lastModifed = null)
        {
            LoadItems<Symbol>("symbol", symbols, lib => lib.Symbols.Cast<LibraryItem>(), predicate, stopIfFound, lastModifed);
        }

        void LoadFootprints(List<ILibraryItem> footprints, Func<ILibraryItem, bool> predicate = null, bool stopIfFound = false, DateTime? lastModifed = null)
        {
            LoadItems<Footprint>("footprint", footprints, lib => lib.Footprints.Cast<LibraryItem>(), predicate, stopIfFound, lastModifed);
        }

        void LoadComponents(List<ILibraryItem> components, Func<ILibraryItem, bool> predicate = null, bool stopIfFound = false, DateTime? lastModifed = null)
        {
            LoadItems<ComponentDocument>("component", components, lib => lib.Components.Cast<LibraryItem>(), predicate, stopIfFound, lastModifed);
        }

        void LoadSchematics(List<ILibraryItem> schematics, Func<ILibraryItem, bool> predicate = null)
        {
            LoadSchematicsRecursive(schematics, Path.GetDirectoryName(Project.FilePath), predicate);
        }

        void LoadSchematicsRecursive(List<ILibraryItem> schematics
                                    , string containerFolderPath
                                    , Func<ILibraryItem, bool> predicate = null
                                    , bool stopIfFound = false)
        {

            var projectFolder = Path.GetDirectoryName(Project.FilePath);

            const string schExt = "schematic";
            foreach (var fileSchematicPath in Directory.GetFiles(containerFolderPath, $"*.{schExt}", SearchOption.AllDirectories))
            {
                var schDocument = XmlHelper.Load<SchematicDocument>(fileSchematicPath);
                schDocument.Library = "local";
                schDocument.Name = Path.GetFileNameWithoutExtension(fileSchematicPath);
                schDocument.FoundPath = DirectoryName.GetRelativePath(projectFolder, fileSchematicPath);

                if (predicate != null)
                {
                    if (predicate(schDocument))
                        schematics.Add(schDocument);
                }
                else
                {
                    schematics.Add(schDocument);
                }

                if (stopIfFound && schematics.Count > 0)
                    return;
            }
        }

        void LoadModels(List<ILibraryItem> models, Func<ILibraryItem, bool> predicate = null, bool stopIfFound = false, DateTime? lastModifed = null)
        {
            LoadItems<ModelDocument>("model", models, lib => lib.Models.Cast<LibraryItem>(), predicate, stopIfFound, lastModifed);
        }

        void LoadItems<LibraryItemType>(string fileExtension,
                                        List<ILibraryItem> items,
                                        Func<LibraryDocument, IEnumerable<LibraryItem>> source,
                                        Func<ILibraryItem, bool> predicate = null,
                                        bool stopIfFound = false,
                                        DateTime? lastModifed = null)
                                                                      where LibraryItemType : LibraryItem
        {
            LoadItemsFromProject<LibraryItemType>(fileExtension, items, this, source,
                                                  predicate: predicate,
                                                  stopIfFound: stopIfFound,
                                                  lastModifed: lastModifed);
        }

        void LoadItemsFromProject<LibraryItemType>(string fileExtension,
                                                   List<ILibraryItem> items, SolutionProjectNodeModel projNode,
                                                   Func<LibraryDocument, IEnumerable<LibraryItem>> source,
                                                   string libraryName = "local",
                                                   Func<ILibraryItem, bool> predicate = null,
                                                   bool stopIfFound = false,
                                                   DateTime? lastModifed = null) where LibraryItemType : LibraryItem
        {
            LoadItemsRecursive<LibraryItemType>(fileExtension, items,
                                                                 Path.GetDirectoryName(projNode.Project.FilePath),
                                                                 libraryName,
                                                                 predicate: predicate,
                                                                 stopIfFound: stopIfFound,
                                                                 lastModifed: lastModifed);

            LoadItemsFromReferences<LibraryItemType>(fileExtension, items, projNode, source,
                                                                      predicate: predicate,
                                                                      stopIfFound: stopIfFound,
                                                                      lastModifed: lastModifed);
        }

        void LoadItemsFromReferences<LibraryItemType>(string fileExtension, List<ILibraryItem> items,
                                                                        SolutionProjectNodeModel projNode,
                                                                        Func<LibraryDocument, IEnumerable<LibraryItem>> source,
                                                                        Func<ILibraryItem, bool> predicate = null,
                                                                        bool stopIfFound = false,
                                                                        DateTime? lastModifed = null) where LibraryItemType : LibraryItem
        {
            var projDocument = projNode.Project;

            if (projDocument.References != null)
            {
                var otherSolutionProjects = SolutionNode.Children.OfType<SolutionProjectNodeModel>()
                                                        .Where(p => p.Project != projDocument).ToList();
                var projectFolder = projNode.GetItemFolderFullPath();
                var referencesFolder = Path.Combine(projectFolder, "References");

                foreach (var reference in projDocument.References)
                {
                    //project reference
                    if (reference is ProjectProjectReference)
                    {
                        var projRef = reference as ProjectProjectReference;
                        var projRefName = Path.GetFileNameWithoutExtension(projRef.ProjectPath);
                        var referedProj = otherSolutionProjects.FirstOrDefault(pp => pp.Name == projRefName);
                        if (referedProj != null)
                        {
                            LoadItemsFromProject<LibraryItemType>(fileExtension, items, referedProj, source, projRefName, predicate, stopIfFound, lastModifed);
                        }
                        //todo: load recursive in references of the reference
                    }
                    else if (reference is LibraryProjectReference)
                    {
                        var libRef = reference as LibraryProjectReference;
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
                                    foreach (var li in itemsInDoc)
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
        }

        void LoadItemsRecursive<LibraryItemType>(string fileExtension,
                                                 List<ILibraryItem> items,
                                                 string containerFolderPath,
                                                 string libraryName = "local",
                                                 Func<ILibraryItem, bool> predicate = null,
                                                 bool stopIfFound = false,
                                                 DateTime? lastModifed = null) where LibraryItemType : LibraryItem
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

                    var itemDocument = XmlHelper.Load<LibraryItemType>(file);
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

        /// <summary>
        /// saves the ProjectDocument
        /// </summary>
        public void Save()
        {
            Project.Save();
        }


    }
}
