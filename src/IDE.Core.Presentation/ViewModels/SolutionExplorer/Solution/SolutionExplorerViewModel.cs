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
using CommunityToolkit.Mvvm.Messaging;
using IDE.Core.Presentation.Messages;

namespace IDE.Core.ViewModels
{
    //this class should have the most of implementation of FileBaseViewModelClass
    public class SolutionExplorerViewModel : ToolViewModel, ISolutionExplorerToolWindow
    {
        public SolutionExplorerViewModel()
            : base("Solution")
        {
            ContentId = ToolContentId;

            Messenger.Register<ISolutionExplorerToolWindow, SolutionClosedMessage>(this,
               (vm, message) =>
               {
                   IsVisible = false;
                   CloseSolution();
               });
        }

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

        IList<SolutionRootNodeModel> _solutions;
        /// <summary>
        /// just one solution, but used as a list for binding in tree;
        /// updated when Document property changes
        /// </summary>
        public IList<SolutionRootNodeModel> Solutions
        {
            get
            {
                return _solutions;
            }
            set
            {
                _solutions = new ObservableCollection<SolutionRootNodeModel>(value);
                OnPropertyChanged(nameof(Solutions));
            }
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
                                    var file = workspace.Files.FirstOrDefault(f => f.FilePath == node.GetItemFullPath());
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
                        if (Solutions == null)
                            return;

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

                var fileExists = File.Exists(filePath);
                if (!fileExists)
                    throw new FileNotFoundException(filePath);

                FilePath = filePath;
                ContentId = this.filePath;
                IsDirty = false;

                var solutionModel = new SolutionRootNodeModel();
                var solutions = new List<SolutionRootNodeModel>() { solutionModel };

                //dispatcherHelper.RunOnDispatcher(() =>
                //{
                //    Solutions.Clear();
                //    Solutions.Add(solutionModel);
                //});

                Solutions = solutions;
                solutionModel.Load(filePath);


                State = DocumentState.IsEditing;
            }
            finally
            {
                IsLoading = false;
            }

            return true;
        }

        public bool CanClose()
        {
            if (State == DocumentState.IsLoading)
                return false;

            return true;
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

        private void CloseSolution()
        {
            try
            {
                //cleanup
                Solutions.Clear();
                SelectedNodes.Clear();
            }
            catch
            {
                throw;
            }
        }

    }
}
