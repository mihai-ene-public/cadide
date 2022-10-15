using System;
using System.IO;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using IDE.Core.Documents;
using IDE.Core.Storage;
using IDE.Core.Utilities;
using IDE.Core.Commands;
using IDE.Core.Settings;
using IDE.Core.Presentation.Resources;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using IDE.Core.Interfaces;
using IDE.Core.Common;

namespace IDE.Core.ViewModels
{
    //this class should have the most of implementation of FileBaseViewModelClass
    public class SolutionExplorerViewModel : ToolViewModel, ISolutionExplorerToolWindow
    {
        public SolutionExplorerViewModel()
            : base("Solution")
        {
            Solutions = new ObservableCollection<SolutionRootNodeModel>();
            ContentId = ToolContentId;

            dispatcherHelper = ServiceProvider.Resolve<IDispatcherHelper>();
        }

        private IDispatcherHelper dispatcherHelper;

        public const string DocumentKey = "Solution";
        public const string Description = "Solution files";
        public const string FileFilterName = "Solution Files";

        public const string ToolContentId = "SolutionExplorer";

        object lockObject = new object();
        DocumentState mState = DocumentState.IsLoading;
        string filePath;
        bool isDirty = false;
        string defaultFileType = SolutionDocument.SolutionExtension;

        public override PaneLocation PreferredLocation => PaneLocation.Right;

        /// <summary>
        /// just one solution, but used as a list for binding in tree;
        /// updated when Document property changes
        /// </summary>
        public ObservableCollection<SolutionRootNodeModel> Solutions
        {
            get; set;
        }

        public ObservableCollection<SolutionExplorerNodeModel> SelectedNodes { get; set; } = new ObservableCollection<SolutionExplorerNodeModel>();

        public DocumentState State
        {
            get
            {
                lock (lockObject)
                {
                    return mState;
                }
            }

            protected set
            {
                lock (lockObject)
                {
                    if (mState != value)
                    {
                        mState = value;

                        OnPropertyChanged(nameof(State));
                    }
                }
            }
        }

        public string FilePath
        {
            get
            {
                if (string.IsNullOrEmpty(filePath))
                    return GetDefaultFileNewName();

                return filePath;
            }

            protected set
            {
                if (filePath != value)
                {
                    filePath = value;

                    OnPropertyChanged(nameof(FilePath));
                    OnPropertyChanged(nameof(FileName));
                    OnPropertyChanged(nameof(Title));

                }
            }
        }

        string GetDefaultFileNewName(string defaultFileName = null,
                                     string defaultFileExtension = null)
        {

            if (defaultFileExtension != null)
                defaultFileType = defaultFileExtension;

            return $"Solution{defaultFileType}";

        }

        public bool IsDirty
        {
            get
            {
                return isDirty;
            }

            set
            {
                if (isDirty != value)
                {
                    isDirty = value;

                    OnPropertyChanged(nameof(IsDirty));
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string FileName
        {
            get
            {
                // This option should never happen - its an emergency break for those cases that never occur
                if (string.IsNullOrEmpty(FilePath))
                    return GetDefaultFileNewName();

                return Path.GetFileName(FilePath);
            }
        }

        bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        IList<SolutionExplorerNodeModel> clipboardItems;

        ICommand copySelectedItemsCommand;
        public ICommand CopySelectedItemsCommand
        {
            get
            {
                if (copySelectedItemsCommand == null)
                    copySelectedItemsCommand = CreateCommand(p =>
                      {
                          if (SelectedNodes != null)
                          {
                              clipboardItems = SelectedNodes.ToList();
                          }
                      },
                    p => SelectedNodes != null && SelectedNodes.Count > 0
                    );
                return copySelectedItemsCommand;
            }
        }

        ICommand cutSelectedItemsCommand;
        public ICommand CutSelectedItemsCommand
        {
            get
            {
                if (cutSelectedItemsCommand == null)
                    cutSelectedItemsCommand = CreateCommand(p =>
                    {
                        if (SelectedNodes != null)
                        {
                            clipboardItems = SelectedNodes.ToList();
                            foreach (var c in clipboardItems)
                                c.IsCut = true;
                        }
                    },
                    p => SelectedNodes != null && SelectedNodes.Count > 0
                    );
                return cutSelectedItemsCommand;
            }
        }

        ICommand pasteItemsCommand;
        public ICommand PasteItemsCommand
        {
            get
            {
                if (pasteItemsCommand == null)
                    pasteItemsCommand = CreateCommand(p =>
                    {
                        try
                        {
                            if (clipboardItems == null || clipboardItems.Count == 0)
                                return;
                            var container = p as FilesContainerNodeModel;
                            if (container == null)
                                return;
                            var project = container.ProjectNode;
                            if (project == null)
                                return;
                            var itemFolder = container.GetItemFolderFullPath();

                            foreach (var pasteItem in clipboardItems)
                            {
                                //dest
                                var destFile = Path.Combine(itemFolder, pasteItem.Name);
                                var itemName = Path.GetFileName(destFile);

                                //copy file on disk
                                //todo copy folder recursive (folder with folders)
                                var srcFilePath = pasteItem.GetItemFullPath();
                                var attr = File.GetAttributes(srcFilePath);
                                if (attr.HasFlag(FileAttributes.Directory))
                                {
                                    Extensions.CopyDirectory(srcFilePath, destFile);
                                }
                                else
                                {
                                    File.Copy(srcFilePath, destFile);
                                }

                                var fileExtension = Path.GetExtension(destFile);
                                var solutionNode = container.CreateSolutionExplorerNodeModel(fileExtension);

                                container.AddChild(solutionNode);
                                solutionNode.Load(destFile);

                                if (pasteItem.IsCut)
                                    pasteItem.DeleteItem();
                            }

                            project.Save();
                        }
                        catch (Exception ex)
                        {
                            MessageDialog.Show(ex.Message);
                        }
                    },
                    p =>
                    {
                        return p is FilesContainerNodeModel
                           && clipboardItems != null
                           && clipboardItems.Count > 0;
                    }
                    );
                return pasteItemsCommand;
            }
        }

        ICommand deleteSelectedItemsCommand;
        public ICommand DeleteSelectedItemsCommand
        {
            get
            {
                if (deleteSelectedItemsCommand == null)
                    deleteSelectedItemsCommand = CreateCommand(p =>
                    {
                        try
                        {
                            var selectedNodes = SelectedNodes;
                            if (selectedNodes != null && selectedNodes.Count > 0)
                            {
                                if (MessageDialog.Show("Are you sure you want to delete the selected items?",
                                    "Confirm delete",
                                     XMessageBoxButton.YesNo) != XMessageBoxResult.Yes)
                                    return;

                                var nodesList = selectedNodes.ToList();
                                foreach (var node in nodesList)
                                {
                                    node.DeleteItem();
                                }

                                var workspace = ServiceProvider.Resolve<IApplicationViewModel>();//ApplicationServices.ApplicationViewModel;
                                //close open items
                                foreach (var node in nodesList)
                                {
                                    var file = workspace.Files.FirstOrDefault(f => f.Item == node);
                                    if (file != null)
                                        workspace.Close(file);
                                }

                                SelectedNodes.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageDialog.Show(ex.Message);
                        }
                    },
                    p => SelectedNodes != null
                        && SelectedNodes.Count > 0
                    //&& SelectedNodes.All(n => n.FileItem is ProjectBaseFileRef
                    //                       || n.Document is ProjectDocument
                    //                       || n.Document is ProjectDocumentReference
                    //                    )
                    );
                return deleteSelectedItemsCommand;
            }
        }

        ICommand updateSelectedReferencesCommand;
        public ICommand UpdateSelectedReferencesCommand
        {
            get
            {
                if (updateSelectedReferencesCommand == null)
                    updateSelectedReferencesCommand = CreateCommand(p =>
                    {
                        try
                        {
                            if (SelectedNodes != null && SelectedNodes.Count > 0)
                            {
                                //if (MessageDialog.Show("Are you sure you want to update references?",
                                //    "Confirm delete",
                                //     MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                                //    return;

                                IEnumerable<ISolutionExplorerNodeModel> nodes = SelectedNodes;
                                if (SelectedNodes.Count == 1 && SelectedNodes[0] is ProjectReferencesNodeModel)
                                {
                                    nodes = SelectedNodes[0].Children;
                                }

                                var h = new ProjectReferencesHelper();
                                h.UpdateReferences(nodes);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageDialog.Show(ex.Message);
                        }
                    },
                    p => SelectedNodes != null
                        && SelectedNodes.Count > 0
                        && ((SelectedNodes.Count == 1 && SelectedNodes[0] is ProjectReferencesNodeModel)
                            || SelectedNodes.All(n => n is ProjectReferenceNodeModel)
                                            )
                    );
                return updateSelectedReferencesCommand;
            }
        }

        ICommand refreshNodesCommand;

        public ICommand RefreshNodesCommand
        {
            get
            {
                //todo: this command coulld potentially mess the references to the nodes in the already opened documents
                //if this happens, remove the command
                if (refreshNodesCommand == null)
                {
                    refreshNodesCommand = CreateCommand(async p =>
                      {
                          await LoadFileAsync2(FilePath);
                      });
                }

                return refreshNodesCommand;
            }
        }

        ICommand collapseAllNodesCommand;


        public ICommand CollapseAllNodesCommand
        {
            get
            {
                if (collapseAllNodesCommand == null)
                {
                    collapseAllNodesCommand = CreateCommand(p =>
                    {
                        if (Solutions != null)
                        {
                            foreach (var s in Solutions)
                            {
                                if (s.Children != null)
                                {
                                    foreach (var proj in s.Children.OfType<FilesContainerNodeModel>())
                                    {
                                        CollapseFilesContainer(proj);
                                    }
                                }
                            }

                        }
                    });
                }

                return collapseAllNodesCommand;
            }
        }

        void CollapseFilesContainer(FilesContainerNodeModel folder)
        {
            folder.IsExpanded = false;
            if (folder.Children != null)
            {
                foreach (var subFolder in folder.Children.OfType<FilesContainerNodeModel>())
                    CollapseFilesContainer(subFolder);
            }
        }

        public bool CanSaveData
        {
            get
            {
                return true;
            }
        }

        public async Task ReOpen()
        {
            await LoadFileAsync2(FilePath);
        }

        async Task LoadFileAsync2(string path)
        {
            var output = ServiceProvider.Resolve<IToolWindowRegistry>().GetTool<IOutputToolWindow>();//ApplicationServices.ToolRegistry.Output;
            output.AppendLine($"Loading {Path.GetFileName(path)}");

            try
            {
                var sw = new Stopwatch();
                sw.Start();
                State = DocumentState.IsLoading;

                await Task.Run(() => OpenFileInternal(path));

                sw.Stop();
                output.AppendLine($"Solution loaded in {sw.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                output.AppendLine(ex.Message);
            }
            finally
            {
                // Set this to invalid if viewmodel still things its loading...
                if (State == DocumentState.IsLoading)
                    State = DocumentState.IsInvalid;
            }
        }

        /// <summary>
        /// Attempt to open a file and load it into the viewmodel if it exists.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>True if file exists and was succesfully loaded. Otherwise false.</returns>
        protected bool OpenFileInternal(string filePath)
        {
            try
            {
                IsLoading = true;

                var isReal = File.Exists(filePath);
                if (!isReal)
                    throw new FileNotFoundException(filePath);

                FilePath = filePath;
                ContentId = this.filePath;
                IsDirty = false; // Mark document loaded from persistence as unedited copy (display without dirty mark '*' in name)

                try
                {


                    var solutionModel = new SolutionRootNodeModel();

                    dispatcherHelper.RunOnDispatcher(() =>
                    {
                        Solutions.Clear();
                        Solutions.Add(solutionModel);
                    });


                    solutionModel.Load(filePath);

                    //DocumentModel = solutionModel;//7.634s


                    State = DocumentState.IsEditing;
                }
                catch                 // File may be blocked by another process
                {                    // Try read-only shared method and set file access to read-only
                    throw;
                }
                //}
                //else
                //    throw new FileNotFoundException(filePath);   // File does not exist
            }
            catch (Exception exp)
            {
                throw new Exception(Strings.STR_FILE_OPEN_ERROR_MSG_CAPTION, exp);
            }
            finally
            {
                IsLoading = false;
            }

            return true;
        }

        ///// <summary>
        ///// Method is executed when the background process finishes and returns here
        ///// because it was cancelled or is done processing.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void FileLoaderLoadResultEvent(object sender, ResultEvent e)
        //{
        //    mAsyncProcessor.ProcessingResultEvent -= FileLoaderLoadResultEvent;
        //    mAsyncProcessor = null;

        //    CommandManager.InvalidateRequerySuggested();

        //    // close documents automatically without message when re-loading on startup
        //    if (State == DocumentState.IsInvalid)
        //    {
        //        OnClose();
        //        return;
        //    }

        //    // Continue processing in parent of this viewmodel if there is any such requested
        //    FireFileProcessingResultEvent(e, TypeOfResult.FileLoad);
        //}

        //bool FireFileProcessingResultEvent(ResultEvent e, TypeOfResult typeOfResult)
        //{
        //    // Continue processing in parent of this viewmodel if there is any such requested
        //    if (ProcessingResultEvent != null)
        //    {
        //        ProcessingResultEvent(this, new ProcessResultEvent(e.Message, e.Error, e.Cancel, typeOfResult,
        //                                                                      e.ResultObjects, e.InnerException));

        //        return true;
        //    }

        //    return false;
        //}

        public bool CanClose()
        {
            if (State == DocumentState.IsLoading)
                return false;

            //return DocumentEvent != null;
            return true;
        }

        //public bool CanSave()
        //{
        //    return false;
        // //   return DocumentModel != null;
        //}

        //public bool CanSaveAs()
        //{
        //    return CanSave();
        //}

        //public bool SaveFile(string filePath)
        //{
        //    try
        //    {
        //        throw new NotSupportedException("Save operation is not suported for Solution Explorer.");

        //        //this saves the solution file only; don't forget project files and so on
        //        // Document.Save(filePath);


        //        // Set new file name in viewmodel and model
        //        FilePath = filePath;
        //        ContentId = filePath;
        //        documentModel.SetFileNamePath(filePath, true);

        //        IsDirty = false;

        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public SolutionExplorerNodeModel FindNodeByFilePath(string filePath)
        {
            foreach (var s in Solutions)
            {
                var searchNode = FindNodeByPath(s, filePath);
                if (searchNode != null)
                    return searchNode;
            }

            return null;
        }

        SolutionExplorerNodeModel FindNodeByPath(SolutionExplorerNodeModel currentNode, string filePath)
        {
            if (currentNode.GetItemFullPath() == filePath)
            {
                return currentNode;
            }
            if (currentNode.Children != null)
            {
                foreach (SolutionExplorerNodeModel childNode in currentNode.Children)
                {
                    var searchNode = FindNodeByPath(childNode, filePath);
                    if (searchNode != null)
                        return searchNode;
                }
            }

            return null;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public async Task OpenFile(string filePath)
        {
            FilePath = filePath;
            await LoadFileAsync2(filePath);
        }

        public void CloseSolution()
        {
            try
            {
                SolutionManager.CloseSolution();

                //cleanup
                Solutions.Clear();
                SelectedNodes.Clear();

            }
            catch
            {
                throw;
            }
        }

        public bool CanCloseSolution()
        {
            return SolutionManager.Solution != null;
        }
    }
}
