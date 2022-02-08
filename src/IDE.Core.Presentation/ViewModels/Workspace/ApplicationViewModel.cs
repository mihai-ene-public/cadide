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

    /// <summary>
    /// This class manages the complete application life cyle from start to end.
    /// It publishes the methodes, properties, and events necessary to integrate
    /// the application into a given shell (BootStrapper, App.xaml.cs etc).
    /// </summary>
    public partial class ApplicationViewModel : BaseViewModel,
                                                IViewModelResolver,
                                                IApplicationViewModel,
                                                IDocumentParent,
                                                IRegisterable
    {
        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        public event EventHandler RequestExit;

        /// <summary>
        /// The document with the current input focus has changed when this event fires.
        /// </summary>
        public event DocumentChangedEventHandler ActiveDocumentChanged;

        public event EventHandler SelectionChanged;

        public event EventHandler HighlightChanged;

        public event EventHandler<string> LoadLayoutRequested;

        #region constructor

        public ApplicationViewModel(IThemesManager _themesManager, ISettingsManager _settingsManager, IRecentFilesViewModel recentFilesViewModel)
        {
            settingsManager = _settingsManager;
            themesManager = _themesManager;
            recentFiles = recentFilesViewModel;

            dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
        }

        #endregion constructor

        #region fields

        bool? dialogCloseResult = null;
        bool? isNotMaximized = null;

        bool shutDownInProgress = false;
        bool shutDownInProgress_Cancel = false;

        ObservableCollection<IFileBaseViewModel> files = new ObservableCollection<IFileBaseViewModel>();
        ReadOnlyObservableCollection<IFileBaseViewModel> readonyFiles = null;

        IFileBaseViewModel activeDocument = null;
        ICommand mainWindowActivated = null;

        IDispatcherHelper dispatcher;
        #endregion fields

        ToolWindowRegistry toolRegistry => (ToolWindowRegistry)ApplicationServices.ToolRegistry;

        IDocumentTypeManager documentTypeManager => ApplicationServices.DocumentTypeManager;

        ISettingsManager settingsManager;//=> (SettingsManager)ApplicationServices.SettingsManager;

        IRecentFilesViewModel recentFiles;

        Profile profile => settingsManager.SessionData as Profile;

        IAppCoreModel appCoreModel => ApplicationServices.AppCoreModel;

        IThemesManager themesManager;//=> ApplicationServices.ThemesManager;

        #region events


        public void OnSelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }

        public void OnHighlightChanged(object sender, EventArgs e)
        {
            HighlightChanged?.Invoke(sender, e);
        }

        //public void OnLayoutLoaded(object sender, LoadLayoutEventArgs e)
        //{
        //    LayoutLoaded?.Invoke(sender, e);
        //}

        #endregion events

        #region Properties


        public ISettingsManager SettingsManager => settingsManager;

        public IThemesManager ThemesManager => themesManager;

        /// <summary>
        /// Gets an instance of the current application theme manager.
        /// </summary>
        public IThemesManager ApplicationThemes
        {
            get
            {
                return themesManager;
            }
        }


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

                    dispatcher.RunOnDispatcher(() =>
                    {
                        if (ActiveDocumentChanged != null)
                        {
                            ActiveDocumentChanged(this, new DocumentChangedEventArgs(activeDocument)); //this.ActiveDocument
                        }
                    });
                }
            }
        }

        private void ShowToolPropertiesForActiveDocument()
        {
            if (activeDocument == null)
                return;

            var toolRegistry = ApplicationServices.ToolRegistry;
            var tools = toolRegistry.Tools.OfType<IDocumentToolWindow>().ToList();
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

        private IDocumentType selectedOpenDocumentType;
        public IDocumentType SelectedOpenDocumentType
        {
            get
            {
                return selectedOpenDocumentType;
            }

            private set
            {
                if (selectedOpenDocumentType != value)
                {
                    selectedOpenDocumentType = value;
                    OnPropertyChanged(nameof(SelectedOpenDocumentType));
                }
            }
        }

        public IList<IDocumentType> DocumentTypes
        {
            get
            {
                return documentTypeManager.DocumentTypes;
            }
        }

        /// <summary>
        /// Principable data source for collection of documents managed in the the document manager (of AvalonDock).
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

        /// <summary>
        /// Principable data source for collection of tool window viewmodels
        /// whos view templating is managed in the the document manager of AvalonDock.
        /// </summary>
        public IList<IToolWindow> Tools
        {
            get
            {
                return toolRegistry.Tools;
            }
        }

        ///// <summary>
        ///// Expose command to load/save AvalonDock layout on application startup and shut-down.
        ///// </summary>
        //public IDockLayoutViewModel ADLayout
        //{
        //    get
        //    {
        //        return ApplicationServices.AvalonDockLayout;
        //    }
        //}

        public bool ShutDownInProgress_Cancel
        {
            get
            {
                return shutDownInProgress_Cancel;
            }

            set
            {
                shutDownInProgress_Cancel = value;
            }
        }

        public bool ShutDownInProgress => shutDownInProgress;

        #region ApplicationName
        /// <summary>
        /// Get the name of this application in a human read-able fashion
        /// </summary>
        public string ApplicationTitle
        {
            get
            {
                var slnPath = SolutionManager.SolutionFilePath;

                var title = appCoreModel.ApplicationTitle;

                if (!string.IsNullOrEmpty(slnPath))
                {
                    var slnName = Path.GetFileNameWithoutExtension(slnPath);
                    title = $"{title} - {slnName}";
                }

                return title;
            }
        }
        #endregion ApplicationName


        #endregion Properties

        #region methods


        /// <summary>
        /// Open a file supplied in <paramref name="filePath"/> (without displaying a file open dialog).
        /// </summary>
        /// <param name="filePath">file to open</param>
        public async Task<IFileBaseViewModel> Open(ISolutionExplorerNodeModel item, string filePath)
        {
            // Verify whether file is already open in editor, and if so, show it
            var fileViewModel = Files.FirstOrDefault(fm => fm.FilePath == filePath);

            if (fileViewModel != null) // File is already open so show it to the user
            {
                dispatcher.RunOnDispatcher(() => ActiveDocument = fileViewModel);
                return fileViewModel;
            }

            fileViewModel = await OpenDocument(item, filePath);

            IntegrateDocumentVM(fileViewModel, filePath);

            return fileViewModel;
        }

        //it opens both a solution and a file item by path
        async Task OpenSolutionInternal(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new Exception("Filepath is not specified");

            object docFile = null;
            IFileBaseViewModel fileViewModel = null;
            IDocumentModel dm = new DocumentModel();
            dm.SetFileNamePath(filePath, true);

            // 1st try to find a document type handler based on the supplied extension
            var docType = documentTypeManager.FindDocumentTypeByExtension(dm.FileExtension, true);

            // 2nd try to find a document type handler based on the name of the prefered viewer
            // (Defaults to EdiTextEditor if no name is given)
            if (docType != null)
            {
                if (docType.Key == SolutionExplorerViewModel.DocumentKey)
                {
                    CheckSolutionIsOpen();
                    docFile = toolRegistry.SolutionToolWindow;
                }
                else
                    docFile = Activator.CreateInstance(docType.ClassType);
            }
            else
            {
                // try to load a standard text file from the file system as a fallback method
                docFile = CreateDefaultDocViewModel();
            }

            fileViewModel = docFile as IFileBaseViewModel;

            if (fileViewModel != null)
            {
                await fileViewModel.OpenFileAsync(filePath);
            }
            else
            {
                if (docFile is SolutionExplorerViewModel)
                    await (docFile as SolutionExplorerViewModel).OpenFile(filePath);
            }

            IntegrateDocument(docFile, filePath);

            OnPropertyChanged(nameof(ApplicationTitle));

            LoadXmlLayout();
        }



        IFileBaseViewModel CreateDefaultDocViewModel()
        {
            return new SimpleTextDocumentViewModel();
        }

        void IntegrateDocument(object docFile, string filePath)
        {
            if (docFile is ToolViewModel)
            {
                var tool = docFile as ToolViewModel;
                tool.CanHide = true;
                tool.IsVisible = true;

                //if we are opening / creating a new solution, make output visible
                if (tool is SolutionExplorerViewModel)
                {
                    //show properties
                    var propertiesTool = toolRegistry.PropertiesToolWindow; //new PropertiesToolWindowViewModel();
                    propertiesTool.CanHide = true;
                    propertiesTool.IsVisible = true;
                }

                //we only add solution files to the MRU list
                if (docFile is SolutionExplorerViewModel)
                {
                    recentFiles.AddNewEntryIntoMRU(filePath);
                }
            }
            else if (docFile is IFileBaseViewModel)
            {
                var fileViewModel = docFile as IFileBaseViewModel;

                //fileViewModel.DocumentEvent += ProcessDocumentEvent;
                //fileViewModel.ProcessingResultEvent += vm_ProcessingResultEvent;

                files.Add(fileViewModel);

                SetActiveFileBaseDocument(fileViewModel);
            }

        }

        public async Task<IFileBaseViewModel> OpenDocument(ISolutionExplorerNodeModel item, string filePath = null)
        {
            IFileBaseViewModel fileViewModel = null;
            if (filePath == null)
                filePath = item.GetItemFullPath();
            IDocumentModel dm = new DocumentModel();
            dm.SetFileNamePath(filePath, true);

            // 1st try to find a document type handler based on the supplied extension
            var docType = documentTypeManager.FindDocumentTypeByExtension(dm.FileExtension, true);
            // 2nd try to find a document type handler based on the name of the prefered viewer
            // (Defaults to EdiTextEditor if no name is given)
            //if (docType == null)
            //    docType = documentTypeManager.FindDocumentTypeByKey(EdiViewModel.DefaultDocumentKey);

            if (docType != null)
            {
                fileViewModel = (IFileBaseViewModel)Activator.CreateInstance(docType.ClassType);
            }
            else
            {
                // try to load a standard text file from the file system as a fallback method
                fileViewModel = CreateDefaultDocViewModel();
            }
            fileViewModel.Item = item;
            await fileViewModel.OpenFileAsync(filePath);

            return fileViewModel;
        }

        public async Task<IFileBaseViewModel> OpenDocumentAsync(ISolutionExplorerNodeModel item, bool loadedForCompiler = false)
        {
            IFileBaseViewModel fileViewModel = null;
            var filePath = item.GetItemFullPath();
            IDocumentModel dm = new DocumentModel();
            dm.SetFileNamePath(filePath, true);

            // 1st try to find a document type handler based on the supplied extension
            var docType = documentTypeManager.FindDocumentTypeByExtension(dm.FileExtension, true);
            // 2nd try to find a document type handler based on the name of the prefered viewer
            // (Defaults to EdiTextEditor if no name is given)
            //if (docType == null)
            //    docType = documentTypeManager.FindDocumentTypeByKey(EdiViewModel.DefaultDocumentKey);

            if (docType != null)
            {
                fileViewModel = (IFileBaseViewModel)Activator.CreateInstance(docType.ClassType);
            }
            else
            {
                // try to load a standard text file from the file system as a fallback method
                fileViewModel = CreateDefaultDocViewModel();
            }

            fileViewModel.LoadedForCompiler = loadedForCompiler;
            fileViewModel.Item = item;
            await fileViewModel.OpenFileAsync(filePath);

            return fileViewModel;
        }

        IFileBaseViewModel IntegrateDocumentVM(IFileBaseViewModel fileViewModel, string filePath)
        {
            if (fileViewModel == null)
            {
                if (recentFiles.ContainsEntry(filePath))
                {
                    if (MessageDialog.Show(string.Format(Strings.STR_ERROR_LOADING_FILE_MSG, filePath),
                                               Strings.STR_ERROR_LOADING_FILE_CAPTION, XMessageBoxButton.YesNo) == XMessageBoxResult.Yes)
                    {
                        recentFiles.RemoveEntry(filePath);
                    }
                }

                return null;
            }

            //fileViewModel.DocumentEvent += ProcessDocumentEvent;
            // fileViewModel.ProcessingResultEvent += vm_ProcessingResultEvent;


            if (fileViewModel is ToolViewModel)
            {
                var tool = fileViewModel as ToolViewModel;
                tool.CanHide = true;
                tool.IsVisible = true;

                //if we are opening / creating a new solution, make output visible
                if (tool is SolutionExplorerViewModel)
                {
                    //show properties
                    var propertiesTool = new PropertiesToolWindowViewModel();
                    propertiesTool.CanHide = true;
                    propertiesTool.IsVisible = true;
                }

            }
            else
                files.Add(fileViewModel);

            SetActiveFileBaseDocument(fileViewModel);

            //we only add solution files to the MRU list
            if (fileViewModel is SolutionExplorerViewModel)
            {
                recentFiles.AddNewEntryIntoMRU(filePath);
            }
            return fileViewModel;
        }

        /// <summary>
        /// <seealso cref="IViewModelResolver"/> method for resolving
        /// AvalonDock contentid's against a specific viewmodel.
        /// </summary>
        /// <param name="content_id"></param>
        /// <returns></returns>
        public object ContentViewModelFromID(string content_id)
        {
            //this mwthod was called on startup on layout loaded

            // Query for a tool window and return it
            var anchorable_vm = Tools.FirstOrDefault(d => d.ContentId == content_id);

            if (anchorable_vm != null)
                return anchorable_vm;

            // Query for a matching document and return it
            //if (settingsManager.SettingData.ReloadOpenFilesOnAppStart == true)
            //always reload document
            return ReloadDocument(content_id);

        }

        void CheckSolutionIsOpen()
        {
            if (SolutionManager.IsSolutionOpen())
                throw new Exception("There is an already open solution. Please close current solution and try again");
        }

        #region NewCommand

        async Task CreateNewSolution(object parameter)
        {
            try
            {
                CheckSolutionIsOpen();

                var templateType = TemplateType.Solution;
                if (parameter != null)
                {
                    TemplateType t = TemplateType.Solution;
                    if (Enum.TryParse<TemplateType>(parameter.ToString(), out t))
                        templateType = t;
                }

                //show template dialog
                var newSolutionDialog = new NewItemWindowViewModel();
                newSolutionDialog.IsSolution = true;
                newSolutionDialog.TemplateType = templateType;
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
                //logger.Error(exp.Message, exp);
                MessageDialog.Show(exp.Message);
            }
        }

        #endregion NewCommand

        #region OpenCommand
        /// <summary>
        /// Opens a solution chosen from the dialog
        /// </summary>
        async Task OpenSolution()
        {
            try
            {
                CheckSolutionIsOpen();

                var typeOfDocument = nameof(TemplateType.Solution);
                var dlg = ServiceProvider.Resolve<IOpenFileDialog>();//new OpenFileDialog();
                IFileFilterEntries fileEntries = null;

                // Get filter strings for document specific filters or all filters
                // depending on whether type of document is set to a key or not.
                fileEntries = documentTypeManager.GetFileFilterEntries(typeOfDocument);
                dlg.Filter = fileEntries.GetFilterString();

                dlg.Multiselect = false;
                dlg.InitialDirectory = GetDefaultPath();

                if (dlg.ShowDialog() == true)
                {
                    // Smallest value in filterindex is 1
                    var docType = fileEntries.GetFileDocumentType(dlg.FilterIndex - 1);

                    var fileName = dlg.FileName;
                    await OpenSolutionInternal(fileName);
                }
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                MessageDialog.Show(ex.Message);
            }
        }

        #endregion OnOpen

        #region Application_Exit_Command
        private void ExitApplication()
        {
            try
            {
                //if (shutDownInProgress)
                //    return;

                //shutDownInProgress_Cancel = false;
                //OnRequestClose();

                OnRequestExit();
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                MessageDialog.Show(ex.Message);
            }
        }
        #endregion Application_Exit_Command

        private void ShowSettingsDialog()
        {
            try
            {
                // Initialize view model for editing settings
                var dlgVM = new SettingsDialogViewModel();
                dlgVM.LoadOptionsFromData(settingsManager.SettingData);

                dlgVM.ShowDialog();

                if (dlgVM.WindowCloseResult == true)
                {
                    dlgVM.SaveOptionsToData(settingsManager.SettingData);

                    settingsManager.SaveOptions(appCoreModel.DirFileAppSettingsData);

                    ApplySettings();
                }
            }
            catch (Exception exp)
            {
                //logger.Error(exp.Message, exp);
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
                var slnToolWindow = toolRegistry.SolutionToolWindow;
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

        void ShowStartPage()
        {
            var sp = GetStartPage();
            if (sp != null)
            {
                ActiveDocument = sp;
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
                //logger.Error(exp.Message, exp);
                MessageDialog.Show(exp.Message);
            }
        }

        void SaveCurrentDocument()
        {
            try
            {
                if (ActiveDocument != null)
                    OnSaveCommandExecute(ActiveDocument, false);
            }
            catch (Exception exp)
            {
                //logger.Error(exp.Message, exp);
                MessageDialog.Show(exp.Message);
            }
        }

        bool CanSaveCurrentDocument()
        {
            return ActiveDocument != null && ActiveDocument.CanSave();
        }

        /// <summary>
        /// Save all open files and current program settings
        /// </summary>
        void SaveAll()
        {
            try
            {
                // Save all edited documents
                if (files != null)               // Close all open files and make sure there are no unsaved edits
                {                                     // If there are any: Ask user if edits should be saved
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

        async Task RunBuild(SolutionExplorerNodeModel node)
        {
            try
            {
                if (node != null)
                {
                    var err = ServiceProvider.GetToolWindow<IErrorsToolWindowViewModel>();

                    dispatcher.RunOnDispatcher(() =>
                    {
                        err?.Clear();

                        //show output during build
                        var output = ApplicationServices.ToolRegistry.Output;
                        var outputTW = output as ToolViewModel;
                        output.Clear();
                        outputTW.IsVisible = true;
                        outputTW.IsActive = true;
                    });

                    await node.Build();


                    dispatcher.RunOnDispatcher(() =>
                    {
                        if (err != null)
                        {
                            if (err.Errors.Count > 0)
                            {
                                err.IsVisible = true;
                                err.IsActive = true;
                            }

                        }
                    });
                }
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
                    var err = ServiceProvider.GetToolWindow<IErrorsToolWindowViewModel>();

                    dispatcher.RunOnDispatcher(() =>
                    {
                        err?.Clear();

                        //show output during build
                        var output = ApplicationServices.ToolRegistry.Output;
                        var outputTW = output as ToolViewModel;
                        output.Clear();
                        outputTW.IsVisible = true;
                        outputTW.IsActive = true;
                    });

                    await node.Compile();

                    dispatcher.RunOnDispatcher(() =>
                    {
                        if (err != null)
                        {
                            if (err.Errors.Count > 0)
                            {
                                err.IsVisible = true;
                                err.IsActive = true;
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
                    //addRefDlg.ShowDialog();
                    //var addRefVM = addRefDlg.ViewModel;
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

                            //File.Copy(libRef.HintPath, refFile, true);
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

        //ProjectBaseFileRef GetSolutionFileItem(TemplateType templateType)
        //{
        //    switch (templateType)
        //    {
        //        case TemplateType.Symbol:
        //            return new ProjectSymbolFile();

        //        case TemplateType.Footprint:
        //            return new ProjectFootprintFile();

        //        case TemplateType.Model:
        //            return new ProjectModelFile();

        //        case TemplateType.Component:
        //            return new ProjectComponentFile();

        //        case TemplateType.Schematic:
        //            return new ProjectSchematicFile();

        //        case TemplateType.Board:
        //            return new ProjectBoardFile();

        //        case TemplateType.Font:
        //            return new ProjectFontFile();

        //        case TemplateType.Misc:
        //            return new ProjectGenericFile();
        //    }

        //    throw new NotSupportedException($"{templateType} is not suported");
        //}

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

                    container.AddChild(solutionNode);
                    solutionNode.Load(fileName);

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
                var fileEntries = documentTypeManager.GetFileFilterEntries();
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

        void AddExistingProject(SolutionExplorerNodeModel node)
        {
            MessageDialog.Show("TODO");
        }

        void AddNewProject(SolutionExplorerNodeModel node)
        {
            if (node == null)
                return;

            //we could use AddNewItem(templateType) but we will break some other things...

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

            var p = new SolutionProjectPropertiesViewModel();
            p.Project = (ProjectDocument)project.ProjectNode.Project;
            await p.OpenFileAsync(filePath);
            IntegrateDocumentVM(p, string.Empty);
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

        void MirrorXSelectedItems()
        {
            var canvasFileModel = activeDocument as CanvasDesignerFileViewModel;
            if (canvasFileModel != null)
                canvasFileModel.MirrorXSelectedItems();
        }

        void MirrorYSelectedItems()
        {
            var canvasFileModel = activeDocument as CanvasDesignerFileViewModel;
            if (canvasFileModel != null)
                canvasFileModel.MirrorYSelectedItems();
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

        void ChangeFootprintPlacement()
        {
            var canvasFileModel = activeDocument as CanvasDesignerFileViewModel;
            if (canvasFileModel != null)
                canvasFileModel.ChangeFootprintPlacement();
        }

        #region Application_About_Command

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

        public void ShowLicenseActivationDialog()
        {
            try
            {
                var vm = new LicenseActivationDialogViewModel();

                vm.ShowDialog();
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }
        }
        #endregion Application_About_Command


        #region Recent File List Pin Unpin Commands

        private void PinMruEntryToggle(MruItemViewModel mruEntry)
        {
            try
            {
                if (mruEntry == null)
                    return;

                recentFiles.PinUnpinEntry(!mruEntry.IsPinned, mruEntry.PathFileName);
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }
        }

        private void RemoveMRUEntry(MruItemViewModel mruEntry)
        {
            try
            {
                if (mruEntry == null)
                    return;

                recentFiles.RemoveEntry(mruEntry.PathFileName);
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }
        }

        #endregion Recent File List Pin Unpin Commands

        #region RequestClose [event]
        /// <summary>
        /// Method to be executed when user (or program) tries to close the application
        /// </summary>
        public void OnRequestClose()
        {
            try
            {
                if (shutDownInProgress == false)
                {
                    if (DialogCloseResult == null)
                        DialogCloseResult = true;      // Execute Close event via attached property

                    if (shutDownInProgress_Cancel == true)
                    {
                        shutDownInProgress = false;
                        shutDownInProgress_Cancel = false;
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
                //logger.Error(exp.Message, exp);
                MessageDialog.Show(exp.Message);
            }
        }

        private string GetDefaultPath()
        {
            var sPath = string.Empty;

            try
            {
                // Generate a default path from cuurently or last active document
                if (ActiveDocument != null)
                    sPath = ActiveDocument.GetFilePath();

                if (sPath == string.Empty)
                    sPath = appCoreModel.MyDocumentsUserDir;
                else
                {
                    try
                    {
                        if (Directory.Exists(sPath) == false)
                            sPath = appCoreModel.MyDocumentsUserDir;
                    }
                    catch
                    {
                        sPath = appCoreModel.MyDocumentsUserDir;
                    }
                }
            }
            catch (Exception exp)
            {
                //logger.Error(exp.Message, exp);
                MessageDialog.Show(exp.Message);
            }

            return sPath;
        }

        /// <summary>
        /// Attempt to save data in file when
        /// File>Save As... or File>Save command
        /// is executed.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="saveAsFlag"></param>
        /// <returns></returns>
        internal bool OnSaveCommandExecute(IFileBaseViewModel doc, bool saveAsFlag = false)
        {
            if (doc == null)
                return false;

            if (doc.CanSaveData == true)
            {
                var defaultFilter = GetDefaultFileFilter(doc, documentTypeManager);

                return OnSaveDocumentFile(doc, saveAsFlag, defaultFilter);
            }

            throw new NotSupportedException((doc != null ? doc.ToString() : Strings.STR_MSG_UnknownDocumentType));
        }

        /// <summary>
        /// Returns the default file extension filter strings
        /// that can be used for each corresponding document
        /// type (viewmodel), or an empty string if no document
        /// type (viewmodel) was matched.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        internal static string GetDefaultFileFilter(IFileBaseViewModel f, IDocumentTypeManager docManager)
        {
            if (f == null)
                return string.Empty;

            var filefilter = docManager.GetFileFilterEntries(f.DocumentTypeKey);

            if (filefilter != null)
                return filefilter.GetFilterString();

            return string.Empty;
        }

        internal bool OnSaveDocumentFile(IFileBaseViewModel fileToSave,
                                         bool saveAsFlag = false,
                                         string FileExtensionFilter = "")
        {
            var filePath = (fileToSave == null ? string.Empty : fileToSave.FilePath);

            // Offer SaveAs file dialog if file has never been saved before (was created with new command)
            if (fileToSave != null)
                saveAsFlag = saveAsFlag | !fileToSave.IsFilePathReal;

            try
            {
                if (filePath == string.Empty || saveAsFlag == true)   // Execute SaveAs function
                {
                    var dlg = ServiceProvider.Resolve<ISaveFileDialog>();//new SaveFileDialog();

                    dlg.FileName = Path.GetFileName(filePath);

                    dlg.InitialDirectory = GetDefaultPath();

                    if (string.IsNullOrEmpty(FileExtensionFilter) == false)
                        dlg.Filter = FileExtensionFilter;

                    if (dlg.ShowDialog() == true)     // SaveAs file if user OK'ed it so
                    {
                        filePath = dlg.FileName;

                        fileToSave.SaveFile(filePath);
                    }
                    else
                        return false;
                }
                else                                                  // Execute Save function
                    fileToSave.SaveFile(filePath);

                return true;
            }
            catch (Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }

            return false;
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

        /// <summary>
        /// Close the currently active file and set the file with the lowest index as active document.
        /// </summary>
        /// <param name="fileToClose"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get/set property to determine whether window is in maximized state or not.
        /// (this can be handy to determine when a resize grip should be shown or not)
        /// </summary>
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

        /// <summary>
        /// Check if pre-requisites for closing application are available.
        /// Save session data on closing and cancel closing process if necessary.
        /// </summary>
        /// <returns>true if application is OK to proceed closing with closed, otherwise false.</returns>
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
                startPage = new StartPageViewModel(recentFiles);
                files.Add(startPage);
            }

            return startPage;
        }

        /// <summary>
        /// Helper method for viewmodel resolution for avalondock contentids
        /// and specific document viewmodels. Careful: the Start Page is also
        /// a document but cannot be loaded, saved, or edit as other documents can.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Task<IFileBaseViewModel> ReloadDocument(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                switch (path)
                {
                    case StartPageViewModel.StartPageContentId: // Re-create start page content
                        {
                            return Task.Run(() => (IFileBaseViewModel)GetStartPage());
                        }

                    default:
                        if (path.Contains("<") == true && path.Contains(">") == true)
                        {
                            return null;
                        }

                        return Open(null, path);
                }
            }

            return Task.FromResult<IFileBaseViewModel>(null);
        }

        public void RegisterDocumentType(IDocumentTypeManager docTypeManager)
        {
        }

        #endregion methods
    }
}
