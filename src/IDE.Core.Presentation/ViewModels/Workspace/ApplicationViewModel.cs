namespace IDE.Core.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;
    using IDE.Core.Events;

    using Core;
    using Core.Documents;
    using IDE.Core.Presentation.Resources;
    using Core.Settings;
    using Core.Utilities;
    using Core.MRU;
    using IDE.Documents.Views;
    using IDE.Dialogs.About;
    using IDE.Core.Interfaces;
    using IDE.Core.Storage;
    using IDE.Core.Common;
    using IDE.Core.Presentation;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// This class manages the complete application life cyle from start to end.
    /// It publishes the methodes, properties, and events necessary to integrate
    /// the application into a given shell (BootStrapper, App.xaml.cs etc).
    /// </summary>
    public partial class ApplicationViewModel : BaseViewModel,
                                                IViewModelResolver,
                                                IApplicationViewModel,
                                                IDocumentParent
    {
        public event EventHandler RequestClose;

        public event EventHandler RequestExit;

        public event EventHandler SelectionChanged;

        public event EventHandler HighlightChanged;

        public event EventHandler<string> LoadLayoutRequested;

        public ApplicationViewModel(
            IThemesManager themesManager,
            ISettingsManager settingsManager,
            IRecentFilesViewModel recentFilesViewModel,
            IToolWindowRegistry toolWindowRegistry,
            IDocumentTypeManager documentTypeManager,
            IAppCoreModel appCoreModel,
            IServiceProvider serviceProvider
            )
        {
            _settingsManager = settingsManager;
            _themesManager = themesManager;
            _recentFiles = recentFilesViewModel;
            _toolWindowRegistry = toolWindowRegistry;
            _documentTypeManager = documentTypeManager;
            _appCoreModel = appCoreModel;
            _serviceProvider = serviceProvider;
            dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
        }

        bool? dialogCloseResult = null;
        bool? isNotMaximized = null;

        bool shutDownInProgress = false;
        bool shuttingDownCancel = false;

        ObservableCollection<IFileBaseViewModel> files = new ObservableCollection<IFileBaseViewModel>();
        ReadOnlyObservableCollection<IFileBaseViewModel> readonyFiles = null;

        IFileBaseViewModel activeDocument = null;

        IDispatcherHelper dispatcher;

        private readonly ISettingsManager _settingsManager;

        private readonly IRecentFilesViewModel _recentFiles;
        private readonly IToolWindowRegistry _toolWindowRegistry;
        private readonly IDocumentTypeManager _documentTypeManager;
        private readonly IAppCoreModel _appCoreModel;
        private readonly IServiceProvider _serviceProvider;
        private readonly IThemesManager _themesManager;

        public void OnSelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }

        public void OnHighlightChanged(object sender, EventArgs e)
        {
            HighlightChanged?.Invoke(sender, e);
        }

        #region Properties


        #region ActiveDocument
        /// <summary>
        /// Gets/sets the dcoument that is currently active (has input focus) - if any.
        /// </summary>
        public IFileBaseViewModel ActiveDocument
        {
            get
            {
                return activeDocument;
            }

            set
            {
                if (activeDocument != value)
                {
                    SetPaneActive(activeDocument as PaneViewModel, false);

                    activeDocument = value;

                    SetPaneActive(activeDocument as PaneViewModel, true);

                    ShowToolPropertiesForActiveDocument();

                    OnPropertyChanged(nameof(ActiveDocument));

                }
            }
        }

        private void ShowToolPropertiesForActiveDocument()
        {
            if (activeDocument == null)
                return;

            var tools = _toolWindowRegistry.Tools.OfType<IDocumentToolWindow>().ToList();
            var visTools = activeDocument.GetToolWindowsWhenActive();

            //hide tools that are not needed (not all tools need to be hidden)
            foreach (var tool in tools.Except(visTools))
            {
                tool.IsVisible = false;
                tool.SetDocument(null);
            }

            activeDocument.ShowVisibleToolsFiltered(visTools);

            //show selected items in properties
            if (activeDocument is ICanvasDesignerFileViewModel canvas)
            {
                canvas.CanvasModel?.UpdateSelection();
            }
            else
            {
                var pw = ServiceProvider.GetToolWindow<PropertiesToolWindowViewModel>();
                pw.SelectedObject = null;
            }

        }

        void SetPaneActive(PaneViewModel pane, bool _isActive)
        {
            if (pane != null)
                pane.IsActive = _isActive;
        }

        #endregion

        /// <summary>
        /// data source for collection of documents managed in the the document manager
        /// </summary>
        public IList<IFileBaseViewModel> Files
        {
            get
            {
                if (readonyFiles == null)
                    readonyFiles = new ReadOnlyObservableCollection<IFileBaseViewModel>(files);

                return readonyFiles;
            }
        }

        public IList<IToolWindow> Tools
        {
            get
            {
                return _toolWindowRegistry.Tools;
            }
        }

        public bool ShuttingDownCancel
        {
            get
            {
                return shuttingDownCancel;
            }

            set
            {
                shuttingDownCancel = value;
            }
        }

        public string ApplicationTitle
        {
            get
            {
                var slnPath = SolutionManager.SolutionFilePath;

                var title = _appCoreModel.ApplicationTitle;

                if (!string.IsNullOrEmpty(slnPath))
                {
                    var slnName = Path.GetFileNameWithoutExtension(slnPath);
                    title = $"{title} - {slnName}";
                }

                return title;
            }
        }


        #endregion Properties

        #region methods


        void IntegrateDocument(IFileBaseViewModel docFile)
        {
            if (docFile == null)
                return;

            files.Add(docFile);
            SetActiveFileBaseDocument(docFile);
        }

        public object ContentViewModelFromID(string content_id)
        {
            //called on startup on layout loaded (we need this)

            // Query for a tool window and return it
            var toolWindow = Tools.FirstOrDefault(d => d.ContentId == content_id);

            return toolWindow;
        }

        void CheckSolutionIsOpen()
        {
            if (SolutionManager.IsSolutionOpen())
                throw new Exception("There is an already open solution. Please close current solution and try again");
        }

        async Task CreateNewSolution()
        {
            try
            {
                CheckSolutionIsOpen();

                //show template dialog
                var newSolutionDialog = new NewItemWindowViewModel();
                newSolutionDialog.IsSolution = true;
                newSolutionDialog.TemplateType = TemplateType.Solution;
                newSolutionDialog.ItemName = "New project";
                newSolutionDialog.SolutionName = "New project";
                newSolutionDialog.Location = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (newSolutionDialog.ShowDialog() == true)
                {
                    SolutionManager.CloseSolution();//because it was created
                    var fileName = newSolutionDialog.SelectedItemFilePath;
                    await OpenSolutionInternal(fileName);
                }
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }
        }

        private async Task OpenSolutionFromDialog()
        {
            try
            {
                CheckSolutionIsOpen();

                var typeOfDocument = nameof(TemplateType.Solution);
                var dlg = ServiceProvider.Resolve<IOpenFileDialog>();

                var fileEntries = _documentTypeManager.GetFileFilterEntries(typeOfDocument);
                dlg.Filter = fileEntries.GetFilterString();

                dlg.Multiselect = false;
                dlg.InitialDirectory = GetDefaultPath();

                if (dlg.ShowDialog() == true)
                {
                    var fileName = dlg.FileName;
                    await OpenSolutionInternal(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageDialog.Show(ex.Message);
            }
        }

        public async Task OpenSolution(string filePath)
        {
            try
            {
                await OpenSolutionInternal(filePath);
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }
        }

        //it opens both a solution and a file item by path
        async Task OpenSolutionInternal(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new Exception("Filepath is not specified");

            CheckSolutionIsOpen();

            var docFile = _serviceProvider.GetService<ISolutionExplorerToolWindow>();
            await docFile.OpenFile(filePath);

            docFile.CanHide = true;
            docFile.IsVisible = true;
            _recentFiles.AddNewEntryIntoMRU(filePath);

            OnPropertyChanged(nameof(ApplicationTitle));

            LoadXmlLayout();
        }

        public async Task<IFileBaseViewModel> Open(ISolutionExplorerNodeModel item, string filePath)
        {
            // if file is already open, make it active
            var fileViewModel = Files.FirstOrDefault(fm => fm.FilePath == filePath);

            if (fileViewModel != null) // File is already open so show it to the user
            {
                dispatcher.RunOnDispatcher(() => ActiveDocument = fileViewModel);
                return fileViewModel;
            }

            fileViewModel = await OpenDocumentAsync(item);

            if (fileViewModel != null)
            {
                IntegrateDocument(fileViewModel);
            }

            return fileViewModel;
        }

        public async Task<IFileBaseViewModel> OpenDocumentAsync(ISolutionExplorerNodeModel item, bool loadedForCompiler = false)
        {
            IFileBaseViewModel fileViewModel = null;
            var filePath = item.GetItemFullPath();

            var fileExtension = Path.GetExtension(filePath);
            // 1st try to find a document type handler based on the supplied extension
            var docType = _documentTypeManager.FindDocumentTypeByExtension(fileExtension);

            if (docType != null)
            {
                fileViewModel = _serviceProvider.GetService(docType.DocumentEditorClassType) as IFileBaseViewModel;
            }
            else
            {
                // open the file with default associated viewer
                ProcessStarter.Start(filePath);
                return null;
            }

            fileViewModel.LoadedForCompiler = loadedForCompiler;
            fileViewModel.Item = item;
            await fileViewModel.OpenFileAsync(filePath);

            return fileViewModel;
        }

        private void ExitApplication()
        {
            try
            {
                OnRequestExit();
            }
            catch (Exception ex)
            {
                MessageDialog.Show(ex.Message);
            }
        }

        private void ShowSettingsDialog()
        {
            try
            {
                var dlgVM = new SettingsDialogViewModel();
                dlgVM.LoadOptionsFromData(_settingsManager.SettingData);

                dlgVM.ShowDialog();

                if (dlgVM.WindowCloseResult == true)
                {
                    dlgVM.SaveOptionsToData(_settingsManager.SettingData);

                    _settingsManager.SaveOptions(_appCoreModel.DirFileAppSettingsData);

                    ApplySettings();
                }
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }
        }

        void ApplySettings()
        {
            foreach (var file in files)
                file.ApplySettings();
        }

        void CloseCurrentDocument()
        {
            Close(ActiveDocument);
        }

        bool CanCloseCurrentDocument()
        {
            return ActiveDocument != null && ActiveDocument.CanClose();
        }

        void CloseSolution()
        {
            try
            {
                var slnToolWindow = _toolWindowRegistry.GetTool<SolutionExplorerViewModel>();
                if (slnToolWindow != null)
                {
                    slnToolWindow.CloseSolution();

                    //close all files
                    if (CanCloseAndSaved())
                    {
                        //var fileList = Files.ToList();
                        //fileList.ForEach(f => Close(f));

                        CloseFiles(Files);

                        //close solution and properties
                        var st = ServiceProvider.GetToolWindow<SolutionExplorerViewModel>();
                        if (st != null)
                            st.IsVisible = false;

                        var pw = ServiceProvider.GetToolWindow<PropertiesToolWindowViewModel>();
                        if (pw != null)
                            pw.IsVisible = false;

                        OnPropertyChanged(nameof(ApplicationTitle));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageDialog.Show(ex.Message);
            }
        }

        bool CanCloseSolution()
        {
            var canClose = false;
            var slnToolWindow = Tools.OfType<SolutionExplorerViewModel>().FirstOrDefault();
            if (slnToolWindow != null)
            {
                canClose = slnToolWindow.CanCloseSolution();
            }

            return canClose;
        }

        void CloseFiles(IList<IFileBaseViewModel> files)
        {
            var fileList = files.Where(f => f.CanClose()).ToList();
            fileList.ForEach(f => f.State = DocumentState.IsClosing);

            fileList.ForEach(f => Close(f));
        }
        void CloseAllFiles()
        {
            CloseFiles(Files);
        }

        bool CanCloseAllFiles()
        {
            return Files.Where(f => f.CanClose()).Count() > 0;
        }

        void CloseAllFilesExceptCurrent()
        {
            var files = Files.Where(f => f != activeDocument).ToList();
            CloseFiles(files);
        }

        bool CanCloseAllFilesExceptCurrent()
        {
            return Files.Where(f => f != activeDocument && f.CanClose()).Count() > 0;
        }

        public void ShowStartPage()
        {
            var sp = GetStartPage();
            if (sp != null)
            {
                ActiveDocument = sp;
            }
        }
        public void CreateAppDataFolder()
        {
            try
            {
                System.IO.Directory.CreateDirectory(_appCoreModel.DirAppData);
            }
            catch
            {
            }
        }

        void SaveCurrentDocument()
        {
            try
            {
                if (ActiveDocument != null)
                    OnSaveCommandExecute(ActiveDocument);
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }
        }

        bool CanSaveCurrentDocument()
        {
            return ActiveDocument != null && ActiveDocument.CanSave();
        }

        void SaveAll()
        {
            try
            {
                if (files != null)
                {
                    var activeDoc = ActiveDocument;

                    try
                    {
                        foreach (var f in Files.Where(file => file.IsDirty && file.CanSaveData))
                        {
                            ActiveDocument = f;
                            OnSaveCommandExecute(f);
                        }
                    }
                    catch (Exception exp)
                    {
                        MessageDialog.Show(exp.Message);
                    }
                    finally
                    {
                        if (activeDoc != null)
                            ActiveDocument = activeDoc;
                    }
                }

                // Save program settings
                SaveConfig();
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }
        }

        private void ActivateToolWindow(IToolWindow toolWindow)
        {
            toolWindow.IsVisible = true;
            toolWindow.IsActive = true;
        }

        private async Task RunBuild(SolutionExplorerNodeModel node)
        {
            try
            {
                if (node == null)
                    return;

                var err = _toolWindowRegistry.GetTool<IErrorsToolWindow>();
                var output = _toolWindowRegistry.GetTool<IOutputToolWindow>();

                dispatcher.RunOnDispatcher(() =>
                {
                    err.Clear();

                    output.Clear();
                    ActivateToolWindow(output);
                });

                await node.Build();

                dispatcher.RunOnDispatcher(() =>
                {
                    if (err != null)
                    {
                        if (err.Errors.Count > 0)
                        {
                            ActivateToolWindow(err);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageDialog.Show(ex.Message);
            }
        }

        async Task RunCompile(SolutionExplorerNodeModel node)
        {
            try
            {
                if (node != null)
                {
                    var err = _toolWindowRegistry.GetTool<IErrorsToolWindow>();
                    var output = _toolWindowRegistry.GetTool<IOutputToolWindow>();

                    dispatcher.RunOnDispatcher(() =>
                    {
                        err.Clear();

                        output.Clear();
                        ActivateToolWindow(output);
                    });

                    await node.Compile();

                    dispatcher.RunOnDispatcher(() =>
                    {
                        if (err != null)
                        {
                            if (err.Errors.Count > 0)
                            {
                                ActivateToolWindow(err);
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageDialog.Show(ex.Message);
                throw;
            }
        }

        void ShowProjectReferences(SolutionExplorerNodeModel node)
        {
            if (node != null)
            {
                var project = node.ProjectNode;
                if (project != null)
                {
                    var projectReferencesNode = project.Children.OfType<ProjectReferencesNodeModel>().FirstOrDefault();

                    var addRefDlg = new AddReferencesDialogViewModel(project);

                    if (addRefDlg.ShowDialog() == true)
                    {
                        //remove references that are not in our list anymore
                        var refsToRemove = project.Project.References.OfType<LibraryProjectReference>().Where(pref => !addRefDlg.ReferencesList.Any(r => pref.ToString() == r.ToString())).ToList();

                        var projectFolder = project.GetItemFolderFullPath();
                        var referencesFolder = Directory.CreateDirectory(Path.Combine(projectFolder, "References"));
                        foreach (var existingRef in refsToRemove)
                        {
                            var refFile = Path.Combine(referencesFolder.FullName, existingRef.LibraryName + ".libref");
                            if (File.Exists(refFile))
                                File.Delete(refFile);
                        }

                        //the new references to the project
                        project.Project.References.Clear();
                        project.Project.References.AddRange(addRefDlg.ReferencesList.Cast<ProjectDocumentReference>());
                        project.Save();

                        //copy library references to the References folder
                        foreach (var libRef in project.Project.References.OfType<LibraryProjectReference>())
                        {
                            var refFile = Path.Combine(referencesFolder.FullName, libRef.LibraryName + ".libref");

                            var souceLibPath = ProjectReferencesHelper.FindLibraryFullPath(project, libRef.HintPath);
                            if (!string.IsNullOrEmpty(souceLibPath) && File.Exists(souceLibPath))
                            {
                                try
                                {
                                    File.Copy(souceLibPath, refFile, true);
                                }
                                catch { }
                            }
                        }

                        if (projectReferencesNode != null)
                            projectReferencesNode.Load(null);
                    }
                }
            }
        }

        string GetNewItemName(TemplateType templateType)
        {
            if (templateType == TemplateType.Misc)
                return "New item";

            return $"New {templateType.ToString().ToLower()}";
        }

        async Task AddNewItem(FilesContainerNodeModel container, TemplateType templateType)
        {
            try
            {
                if (container == null)
                    return;

                var itemFolder = container.GetItemFolderFullPath();

                //show template dialog
                var newSolutionDialog = new NewItemWindowViewModel();
                newSolutionDialog.TemplateType = templateType;
                newSolutionDialog.ItemName = GetNewItemName(templateType);
                newSolutionDialog.Location = itemFolder;

                if (newSolutionDialog.ShowDialog() == true)
                {
                    var fileName = newSolutionDialog.SelectedItemFilePath;

                    //we add the symbol to the project
                    var itemName = Path.GetFileName(fileName);
                    var fileExtension = Path.GetExtension(fileName);

                    var solutionNode = container.CreateSolutionExplorerNodeModel(fileExtension);

                    solutionNode.Load(fileName);
                    container.AddChild(solutionNode);

                    await Open(solutionNode, fileName);
                }
            }
            catch (Exception ex)
            {
                MessageDialog.Show(ex.Message);
            }
        }

        async Task AddNewSymbolItem(FilesContainerNodeModel container)
        {
            var templateType = TemplateType.Symbol;
            await AddNewItem(container, templateType);
        }

        async Task AddNewFootprintItem(FilesContainerNodeModel container)
        {
            var templateType = TemplateType.Footprint;
            await AddNewItem(container, templateType);
        }

        async Task AddNewModelItem(FilesContainerNodeModel container)
        {
            var templateType = TemplateType.Model;
            await AddNewItem(container, templateType);
        }

        async Task AddNewComponentItem(FilesContainerNodeModel container)
        {
            var templateType = TemplateType.Component;
            await AddNewItem(container, templateType);
        }

        async Task AddNewSchematicItem(FilesContainerNodeModel container)
        {
            var templateType = TemplateType.Schematic;
            await AddNewItem(container, templateType);
        }

        async Task AddNewBoardItem(FilesContainerNodeModel container)
        {
            var templateType = TemplateType.Board;
            await AddNewItem(container, templateType);
        }

        async Task AddNewGenericItem(FilesContainerNodeModel container)
        {
            var templateType = TemplateType.Misc;
            await AddNewItem(container, templateType);
        }

        Task AddNewFolder(FilesContainerNodeModel container)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (container == null)
                        return;

                    var folderPath = container.GetItemFolderFullPath();

                    //find a unique name
                    var currentIndex = 1;
                    var folderName = "New Folder";
                    while (Directory.Exists(Path.Combine(folderPath, folderName)))
                    {
                        folderName = $"New Folder {currentIndex}";
                        currentIndex++;
                    }

                    //create the folder on disk
                    var newFolderPath = Path.Combine(folderPath, folderName);
                    Directory.CreateDirectory(newFolderPath);

                    //add folder to project
                    // var folderItem = new ProjectFolderFile { RelativePath = folderName };
                    var folderModel = container.CreateSolutionExplorerFolderNodeModel(newFolderPath);

                    //container.AddFileItem(folderItem);
                    container.AddChild(folderModel);

                    //select and rename folder which is not working
                    //we need to redo/repair our multiple selection tree/behavior
                    var solutionExplorer = ServiceProvider.GetToolWindow<SolutionExplorerViewModel>();
                    if (solutionExplorer != null)
                    {
                        //select the folder
                        var folder = (SolutionExplorerNodeModel)folderModel;
                        folder.IsSelected = true;

                        ////begin rename
                        folder.IsEditing = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageDialog.Show(ex.Message);
                }
            });
        }

        Task AddExistingItems(FilesContainerNodeModel container)
        {
            return Task.Run(() =>
            {
                if (container == null)
                    return;

                var project = container.ProjectNode;
                if (project == null)
                    return;

                var itemFolder = container.GetItemFolderFullPath();

                var dlg = ServiceProvider.Resolve<IOpenFileDialog>();//new OpenFileDialog();

                //filter from all our supported files
                var fileEntries = _documentTypeManager.GetFileFilterEntries();
                dlg.Filter = fileEntries.GetFilterString();

                dlg.Multiselect = true;
                dlg.InitialDirectory = GetDefaultPath();

                if (dlg.ShowDialog() == true)
                {
                    foreach (var fileName in dlg.FileNames)
                    {
                        //we add the symbol to the project
                        var itemName = Path.GetFileName(fileName);

                        //var typeOfProjFile = documentTypeManager.FindDocumentTypeByExtension(Path.GetExtension(fileName), true);
                        //ProjectBaseFileRef fileItem = null;
                        //if (typeOfProjFile != null && typeOfProjFile.TypeOfProjectBaseFileRef != null)
                        //    fileItem = Activator.CreateInstance(typeOfProjFile.TypeOfProjectBaseFileRef) as ProjectBaseFileRef;
                        //if (fileItem == null)
                        //    fileItem = new ProjectGenericFile();
                        //fileItem.RelativePath = DirectoryName.GetRelativePath(itemFolder, itemName);

                        //copy the file to our folder
                        var destFilePath = Path.Combine(itemFolder, itemName);
                        string fileExtension = Path.GetExtension(destFilePath);
                        File.Copy(fileName, destFilePath);

                        var solutionNode = container.CreateSolutionExplorerNodeModel(fileExtension);

                        //container.AddFileItem(fileItem);

                        container.AddChild(solutionNode);
                        solutionNode.Load(destFilePath);
                    }

                    // project.Save();
                }
            });
        }

        void AddNewProject(SolutionExplorerNodeModel node)
        {
            if (node == null)
                return;

            var slnPath = SolutionManager.SolutionFilePath;

            //show template dialog
            var newSolutionDialog = new NewItemWindowViewModel();
            newSolutionDialog.TemplateType = TemplateType.Project;
            newSolutionDialog.ItemName = "New project";
            newSolutionDialog.Location = Path.GetDirectoryName(slnPath);

            if (newSolutionDialog.ShowDialog() == true)
            {
                var fileName = newSolutionDialog.SelectedItemFilePath;
                var solution = SolutionManager.Solution;

                //we add the new project to the solution
                var solFolder = Path.GetDirectoryName(slnPath);
                var projName = Path.GetFileNameWithoutExtension(fileName);
                //todo: add to virtual folder
                var projItem = new SolutionProjectItem
                {
                    RelativePath = DirectoryName.GetRelativePath(solFolder, fileName)
                };
                solution.Children.Add(projItem);

                SolutionManager.SaveSolution();

                //load the project
                var projectModel = projItem.CreateSolutionExplorerNodeModel();

                node.AddChild(projectModel);
            }
        }

        async Task ShowProjectProperties(SolutionProjectNodeModel project)
        {
            if (project == null)
                return;

            // var item = e.Parameter as SolutionProjectNodeModel;
            var filePath = project.GetItemFullPath();
            var fileViewModel = Files.FirstOrDefault(fm => fm.FilePath == filePath);

            if (fileViewModel != null) // File is already open so show it to the user
            {
                ActiveDocument = fileViewModel;
                return;
            }

            var projectPropertiesDocument = new SolutionProjectPropertiesViewModel();
            projectPropertiesDocument.Project = (ProjectDocument)project.ProjectNode.Project;
            await projectPropertiesDocument.OpenFileAsync(filePath);
            IntegrateDocument(projectPropertiesDocument);
        }

        async Task ShowProperties(SolutionExplorerNodeModel node)
        {
            if (node == null)
                return;

            if (node is SolutionProjectNodeModel project)
            {
                await ShowProjectProperties(project);
            }
        }

        void ImportFromEagle()
        {

            try
            {
                var dlg = new EagleImporterViewModel();
                if (dlg.ShowDialog() == true)
                {
                    dlg.RunImport();
                }
            }
            catch (Exception ex)
            {
                MessageDialog.Show(ex.Message);
            }

        }

        void ChangeMode()
        {
            var canvasFileModel = activeDocument as CanvasDesignerFileViewModel;
            if (canvasFileModel != null)
                canvasFileModel.ChangeMode();
        }

        void CyclePlacementOrRotate()
        {
            var canvasFileModel = activeDocument as CanvasDesignerFileViewModel;
            if (canvasFileModel != null)
                canvasFileModel.CyclePlacementOrRotate();
        }

        void DeleteSelectedItems()
        {
            var canvasFileModel = activeDocument as CanvasDesignerFileViewModel;
            if (canvasFileModel != null)
                canvasFileModel.DeleteSelectedItems();
        }

        void CopySelectedItems()
        {
            var canvasFileModel = activeDocument as CanvasDesignerFileViewModel;
            if (canvasFileModel != null)
                canvasFileModel.CopySelectedItems();
        }

        void PasteSelectedItems()
        {
            var canvasFileModel = activeDocument as CanvasDesignerFileViewModel;
            if (canvasFileModel != null)
                canvasFileModel.PasteSelectedItems();
        }

        private void ShowToolWindow(IToolWindow toolWindow)
        {
            if (toolWindow == null)
                return;

            toolWindow.IsVisible = !toolWindow.IsVisible;
        }

        private void ShowAboutDialog()
        {
            try
            {
                var vm = new AboutViewModel();

                vm.ShowDialog();
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }
        }

        #region RequestClose [event]
        public void OnRequestClose()
        {
            try
            {
                if (shutDownInProgress == false)
                {
                    if (DialogCloseResult == null)
                        DialogCloseResult = true;      // Execute Close event via attached property

                    if (shuttingDownCancel == true)
                    {
                        shutDownInProgress = false;
                        shuttingDownCancel = false;
                        DialogCloseResult = null;
                    }
                    else
                    {
                        shutDownInProgress = true;

                        if (RequestClose != null)
                            RequestClose(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception exp)
            {
                shutDownInProgress = false;

                MessageDialog.Show(exp.Message);
            }
        }
        #endregion // RequestClose [event]

        public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // Close all open files and check whether application is ready to close
                if (CanCloseAndSaved() == true)
                {
                    OnRequestClose();

                    e.Cancel = false;
                }
                else
                    e.Cancel = ShuttingDownCancel = true;
            }
            catch
            {
            }
        }

        public void OnClosed()
        {
            try
            {
                // Save/initialize program options that determine global program behaviour
                SaveConfig();

                DisposeResources();
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }
        }

        private void DisposeResources()
        {
            try
            {
                foreach (var item in Files)
                {
                    try
                    {
                        item.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        void OnRequestExit()
        {
            var h = RequestExit;
            h?.Invoke(this, EventArgs.Empty);
        }

        private void SetActiveFileBaseDocument(IFileBaseViewModel vm)
        {
            try
            {
                ActiveDocument = vm;
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }
        }

        private string GetDefaultPath()
        {
            return _appCoreModel.MyDocumentsUserDir;
        }

        internal bool OnSaveCommandExecute(IFileBaseViewModel doc)
        {
            if (doc == null)
                return false;

            if (doc.CanSaveData)
            {
                return OnSaveDocumentFile(doc);
            }

            throw new NotSupportedException(( doc != null ? doc.ToString() : Strings.STR_MSG_UnknownDocumentType ));
        }

        internal bool OnSaveDocumentFile(IFileBaseViewModel fileToSave)
        {
            var filePath = fileToSave.FilePath;
            fileToSave.SaveFile(filePath);

            return true;
        }

        internal bool OnCloseSaveDirtyFile(IFileBaseViewModel fileToClose)
        {
            if (fileToClose.IsDirty == true &&
                    fileToClose.CanSaveData == true)
            {
                var res = MessageDialog.Show(string.Format(CultureInfo.CurrentCulture, Strings.STR_MSG_SaveChangesForFile, fileToClose.FileName),
                                                                    ApplicationTitle,
                                                                    XMessageBoxButton.YesNoCancel, XMessageBoxImage.Question
                                                                    );

                if (res == XMessageBoxResult.Cancel)
                    return false;

                if (res == XMessageBoxResult.Yes)
                {
                    return OnSaveCommandExecute(fileToClose);
                }
            }

            return true;
        }

        public bool Close(IFileBaseViewModel doc)
        {
            try
            {
                if (doc == null)
                    return false;

                if (!doc.CanClose())
                    return false;

                //todo: put the comment below back
                /*
                   if (OnCloseSaveDirtyFile(doc) == false)
                    return false;
                */

                var idx = files.IndexOf(doc);

                files.Remove(doc);
                doc.Dispose();

                if (idx >= 0)
                {
                    if (files.Count > idx)
                        ActiveDocument = files[idx];
                    else
                        if (files.Count > 1 && files.Count == idx)
                        ActiveDocument = files[idx - 1];
                    else
                         if (files.Count == 0)
                        ActiveDocument = null;
                    else
                        ActiveDocument = files[0];
                }

                return true;
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }

            // Throw an exception if this method does not know how the input document type is to be closed
            throw new NotSupportedException(doc.ToString());
        }

        /// <summary>
        /// This can be used to close the attached view via ViewModel
        /// 
        /// Source: http://stackoverflow.com/questions/501886/wpf-mvvm-newbie-how-should-the-viewmodel-close-the-form
        /// </summary>
        public bool? DialogCloseResult
        {
            get
            {
                return dialogCloseResult;
            }

            private set
            {
                if (dialogCloseResult != value)
                {
                    dialogCloseResult = value;
                    OnPropertyChanged(nameof(DialogCloseResult));
                }
            }
        }

        public bool? IsNotMaximized
        {
            get
            {
                return isNotMaximized;
            }

            set
            {
                if (isNotMaximized != value)
                {
                    isNotMaximized = value;
                    OnPropertyChanged(nameof(IsNotMaximized));
                }
            }
        }

        public bool CanCloseAndSaved()
        {
            if (shutDownInProgress)
                return true;

            try
            {
                if (files != null)               // Close all open files and make sure there are no unsaved edits
                {                                     // If there are any: Ask user if edits should be saved

                    var unsavedFiles = Files.Where(f => f.IsDirty && f.CanSaveData).ToList();
                    if (unsavedFiles.Count > 0)
                    {
                        var fileNames = unsavedFiles.Select(f => f.FileName).ToArray();
                        var str = string.Join(Environment.NewLine, fileNames);

                        //todo test what happens when we have a long list of names

#if !DEBUG
                        var canClose = true;
                        var res = MessageDialog.Show($"Save changes to the following items?" + Environment.NewLine + Environment.NewLine + str,
                                                     ApplicationTitle, XMessageBoxButton.YesNoCancel);
                        if (res == XMessageBoxResult.Cancel)
                            canClose = false;
                        else if (res == XMessageBoxResult.Yes)
                        {
                            canClose = unsavedFiles.TrueForAll(f => OnSaveCommandExecute(f));
                        }

                        if (!canClose)
                        {
                            shutDownInProgress = false;
                            return false;               // Cancel shutdown process (return false) if user cancels saving edits
                        }
#endif
                    }
                }
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }

            return true;
        }

        internal StartPageViewModel GetStartPage()
        {
            var startPage = files.OfType<StartPageViewModel>().FirstOrDefault();

            if (startPage == null)
            {
                startPage = new StartPageViewModel(_recentFiles);
                files.Add(startPage);
            }

            return startPage;
        }

        #endregion methods

        public void SaveConfig()
        {
            try
            {
                CreateAppDataFolder();

                //we want to ensure we have the options saved on close for now; consider removing this call in the future
                _settingsManager.SaveOptions(_appCoreModel.DirFileAppSettingsData);

                //save to profile settings from recent files model
                var profile = (Profile)_settingsManager.SessionData;
                var mruModel = (RecentFilesModel)_recentFiles;

                profile.MruList = mruModel.MruList.Select(m => new MruItem { IsPinned = m.IsPinned, FilePath = m.PathFileName }).ToList();

                _settingsManager.SaveProfileData(_appCoreModel.DirFileAppSessionData);
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }
        }

        public void SaveXmlLayout(string xmlLayout)
        {
            if (SolutionManager.IsSolutionOpen())
            {
                var slnFolder = Path.GetDirectoryName(SolutionManager.SolutionFilePath);
                if (!Directory.Exists(slnFolder))
                    return;

                var ideFolder = Path.Combine(slnFolder, AppHelpers.SolutionConfigFolderName);

                //ensure folder exists
                Directory.CreateDirectory(ideFolder);

                var layoutFilePath = Path.Combine(ideFolder, AppHelpers.LayoutFileName);

                File.WriteAllText(layoutFilePath, xmlLayout);
            }
        }

        private void LoadXmlLayout()
        {
            if (SolutionManager.IsSolutionOpen())
            {
                var slnFolder = Path.GetDirectoryName(SolutionManager.SolutionFilePath);
                if (!Directory.Exists(slnFolder))
                    return;

                var ideFolder = Path.Combine(slnFolder, AppHelpers.SolutionConfigFolderName);

                var layoutFilePath = Path.Combine(ideFolder, AppHelpers.LayoutFileName);

                if (!File.Exists(layoutFilePath))
                    return;

                var xmlLayout = File.ReadAllText(layoutFilePath);

                OnLoadLayoutRequested(xmlLayout);
            }
        }

        private void OnLoadLayoutRequested(string xmlLayout)
        {
            LoadLayoutRequested?.Invoke(this, xmlLayout);
        }

        public void LoadConfig()
        {
            _settingsManager.LoadOptions(_appCoreModel.DirFileAppSettingsData);
            _settingsManager.LoadProfileData(_appCoreModel.DirFileAppSessionData);

            _themesManager.SetSelectedTheme(SettingsManager.DefaultTheme);
            LoadSelectedTheme();
        }

        private void LoadSelectedTheme()
        {
            var resManager = ServiceProvider.Resolve<IResourceLocator>();
            resManager.SwitchToSelectedTheme();
        }
    }
}
