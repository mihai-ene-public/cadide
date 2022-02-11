namespace IDE.Core.ViewModels
{
    using System;
    using System.Globalization;
    using System.Windows.Input;
    using System.IO;
    using System.Diagnostics;
    using System.ComponentModel;
    using Documents;
    using Utilities;
    using Commands;
    using IDE.Core.Presentation.Resources;
    using IDE.Core.Errors;
    using System.Collections.Generic;
    using IDE.Core.Interfaces;
    using System.Linq;
    using System.Threading.Tasks;
    using IDE.Core.Types.Media;

    /// <summary>
    /// Base class that shares common properties, methods, and intefaces
    /// among viewmodels that represent documents
    /// (text file edits, Start Page, Program Settings).
    /// </summary>
    public abstract class FileBaseViewModel : PaneViewModel, IFileBaseViewModel
    {


        #region Constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="documentTypeKey"></param>
        public FileBaseViewModel(string docTypeKey)
            : this()
        {
            documentTypeKey = docTypeKey;
        }

        /// <summary>
        /// Standard class constructor.
        /// </summary>
        protected FileBaseViewModel()
        {
            clipBoard = ServiceProvider.Resolve<IClipboardAdapter>();
            _applicationViewModel = ServiceProvider.Resolve<IApplicationViewModel>();

            documentModel = new DocumentModel();
            PropertyChanged += FileBaseViewModel_PropertyChanged;
        }
        protected IClipboardAdapter clipBoard;

        protected readonly IApplicationViewModel _applicationViewModel;

        async void FileBaseViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsActive):
                    {
                        if (IsActive)
                        {
                            await Task.Run(() => RefreshFromCache());
                        }
                        break;
                    }
            }
        }

        protected virtual void RefreshFromCache()
        { }




        #endregion Constructors

        #region Fields

        protected object lockObject = new object();

        DocumentState documentState = DocumentState.IsLoading;

        ICommand openContainingFolderCommand = null;
        ICommand copyFullPathtoClipboard = null;

        protected string documentTypeKey = string.Empty;

        protected IDocumentModel documentModel = null;

        protected string defaultFileType = "sch";
        protected string defaultFileName = "Schema";

        protected string DocumentKey;// = "SchemaEditor";
        protected string Description;// = "Schematic files";
        protected string FileFilterName;// = "Schematic file";
        protected string DefaultFilter;// = "schematic";

        //FileLoader fileLoader;

        // protected bool loadAsync = false;

        #endregion Fields

        #region events
        ///// <summary>
        ///// This event is fired when a document tells the framework that is wants to be closed.
        ///// The framework can then close it and clean-up whatever is left to clean-up.
        ///// </summary>
        //public virtual event EventHandler<FileBaseEvent> DocumentEvent;

        ///// <summary>
        ///// Supports asynchrone processing by implementing a result event when processing is done.
        ///// </summary>
        //public event EventHandler<ProcessResultEvent> ProcessingResultEvent;
        #endregion events

        #region properties
        /// <summary>
        /// Gets the key that is associated with the type of this document.
        /// This key is relevant for the framework to implement the correct
        /// file open/save filter settings etc...
        /// </summary>
        public string DocumentTypeKey
        {
            get
            {
                return documentTypeKey;
            }
        }

        /// <summary>
        /// Get/set whether a given file path is a real existing path or not.
        /// 
        /// This is used to identify files that have never been saved and can
        /// those not be remembered in an MRU etc...
        /// </summary>
        public bool IsFilePathReal
        {
            get
            {
                if (documentModel != null)
                    return documentModel.IsReal;

                return false;
            }
        }

        #region Title
        /// <summary>
        /// Title is the string that is usually displayed - with or without dirty mark '*' - in the docking environment
        /// </summary>
        public override string Title
        {
            get
            {
                return FileName + (IsDirty ? "*" : string.Empty);
            }
        }
        #endregion

        /// <summary>
        /// Indicate whether error on load is displayed to user or not.
        /// </summary>
        protected bool CloseOnErrorWithoutMessage { get; set; }

        /// <summary>
        /// Gets the current state of the document. States may differ during
        /// initialization, loading, and other asynchron processings...
        /// </summary>
        public DocumentState State
        {
            get
            {
                lock (lockObject)
                {
                    return documentState;
                }
            }

            set
            {
                lock (lockObject)
                {
                    if (documentState != value)
                    {
                        documentState = value;

                        OnPropertyChanged(nameof(State));
                    }
                }
            }
        }

        public ISolutionProjectNodeModel ProjectNode
        {
            get
            {
                var sn = Item as SolutionExplorerNodeModel;
                if (sn != null)
                    return sn.ProjectNode;
                return null;
            }
        }

        INotifyPropertyChanged item;
        public INotifyPropertyChanged Item
        {
            get { return item; }
            set
            {
                if (item != null)
                    item.PropertyChanged -= Item_PropertyChanged;

                item = value;

                if (item != null)
                    item.PropertyChanged += Item_PropertyChanged;
            }
        }

        public virtual object Document { get { return null; } }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Item == null) return;

            if (e.PropertyName == "Name")
            {
                try
                {
                    var oldItemPath = FilePath;
                    var oldItemFolder = Path.GetDirectoryName(oldItemPath);
                    var oldExtension = Path.GetExtension(oldItemPath);

                    //since we have the object of a class in a higher dll we have to use reflection
                    var name = Item.GetType().GetProperty("Name", typeof(string)).GetValue(Item) as string;

                    //keep the old extension
                    var newFileName = Path.GetFileNameWithoutExtension(name) + oldExtension;

                    FilePath = Path.Combine(oldItemFolder, newFileName);
                }
                catch { }
            }
        }



        protected string filePath;

        public string FilePath
        {
            get
            {
                InternalGetFilePath();

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

        protected void InternalGetFilePath()
        {
            // filePath = "untitled";
            if (string.IsNullOrEmpty(filePath))
                filePath = GetDefaultFileNewName(1);
        }



        #region FileName
        /// <summary>
        /// FileName is the string that is displayed whenever the application refers to this file, as in:
        /// string.Format(CultureInfo.CurrentCulture, "Would you like to save the '{0}' file", FileName)
        /// 
        /// Note the absense of the dirty mark '*'. Use the Title property if you want to display the file
        /// name with or without dirty mark when the user has edited content.
        /// </summary>
        // abstract public string FileName { get; }

        public string FileName
        {
            get
            {
                var fp = FilePath;
                // This option should never happen - its an emergency break for those cases that never occur
                if (string.IsNullOrEmpty(fp))
                    return GetDefaultFileNewName(80085);

                if (fp.ToCharArray().Any(c => Path.GetInvalidPathChars().Any(s => s == c)))
                    return null; ;

                return Path.GetFileName(fp);
            }
        }

        #endregion FileName

        #region IsDirty


        bool isDirty = false;
        /// <summary>
        /// IsDirty indicates whether the file currently loaded
        /// in the editor was modified by the user or not.
        /// </summary>
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

        #endregion IsDirty

        #region CanSaveData
        /// <summary>
        /// Get whether edited data can be saved or not.
        /// A type of document does not have a save
        /// data implementation if this property returns false.
        /// (this is document specific and should always be overriden by descendents)
        /// </summary>
        public virtual bool CanSaveData { get { return true; } }

        #endregion CanSaveData

        #region Commands

        ICommand closeCommand;

        /// <summary>
        /// This command closes a single file. The binding for this is in the AvalonDock LayoutPanel Style.
        /// </summary>
        public virtual ICommand CloseCommand
        {
            get
            {
                if (closeCommand == null)
                {
                    closeCommand = CreateCommand(p => OnCloseCommand(),
                                                    p => CanClose());
                }

                return closeCommand;
            }
        }

        public IApplicationViewModel Workspace => _applicationViewModel;

        void OnCloseCommand()
        {
            _applicationViewModel.Close(this);
        }

        /// <summary>
        /// Get open containing folder command which will open
        /// the folder indicated by the path in windows explorer
        /// and select the file (if path points to one).
        /// </summary>
        public ICommand OpenContainingFolderCommand
        {
            get
            {
                //if (openContainingFolderCommand == null)
                //    openContainingFolderCommand = CreateCommand((p) => OnOpenContainingFolderCommand());

                return openContainingFolderCommand ?? (openContainingFolderCommand = CreateCommand((p) => OnOpenContainingFolderCommand()));
            }
        }


        /// <summary>
        /// Get CopyFullPathtoClipboard command which will copy
        /// the path of the executable into the windows clipboard.
        /// </summary>
        public ICommand CopyFullPathtoClipboard
        {
            get
            {
                if (copyFullPathtoClipboard == null)
                    copyFullPathtoClipboard = CreateCommand((p) => OnCopyFullPathtoClipboardCommand());

                return copyFullPathtoClipboard;
            }
        }


        #endregion commands

        /// <summary>
        /// Gets/sets a property to indicate whether this
        /// file was changed externally (by another editor) or not.
        /// 
        /// Setter can be used to override re-loading (keep current content)
        /// at the time of detection.
        /// </summary>
        public bool WasChangedExternally
        {
            get
            {
                if (documentModel == null)
                    return false;

                return documentModel.WasChangedExternally;
            }

            private set
            {
                if (documentModel == null)
                    return;

                if (documentModel.WasChangedExternally != value)
                    documentModel.WasChangedExternally = value;
            }
        }
        #endregion properties

        #region methods
        #region abstract methods
        /// <summary>
        /// Indicate whether document can be saved in the currennt state.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanSave()
        {
            return true;
        }

        /// <summary>
        /// Indicate whether document can be saved as.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanSaveAs()
        {
            return CanSave();
        }

        public virtual void ApplySettings()
        {
        }

        public async Task OpenFileAsync(string filePath)
        {
            FilePath = filePath;
            CloseOnErrorWithoutMessage = false;

            State = DocumentState.IsLoading;

            try
            {
                await OpenFileInternal(filePath);
            }
            finally
            {
                // Set this to invalid if viewmodel still things its loading...
                if (State == DocumentState.IsLoading)
                    State = DocumentState.IsInvalid;
            }

            await AfterLoadDocumentInternal();
        }

        //private void LoadFileAsync(string path)
        //{
        //    if (fileLoader != null)
        //    {

        //        if (MessageDialog.Show(
        //                "An operation is currently in progress. Would you like to cancel the current process?",
        //                "Processing...",
        //                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        //        {
        //            fileLoader.Cancel();
        //        }
        //    }

        //    fileLoader = new FileLoader();

        //    fileLoader.ProcessingResultEvent += FileLoaderLoadResultEvent;

        //    State = DocumentState.IsLoading;

        //    fileLoader.ExecuteAsynchronously(() =>
        //    {
        //        try
        //        {
        //            OpenFileInternal(path);
        //        }
        //        finally
        //        {
        //            // Set this to invalid if viewmodel still things its loading...
        //            if (State == DocumentState.IsLoading)
        //                State = DocumentState.IsInvalid;
        //        }
        //    },
        //                                   loadAsync);

        //}

        async Task OpenFileInternal(string openFilePath)
        {
            try
            {
                var isReal = File.Exists(openFilePath);
                documentModel.SetFileNamePath(openFilePath, isReal);

                if (IsFilePathReal)
                {
                    documentModel.SetIsReal(IsFilePathReal);
                    FilePath = openFilePath;
                    ContentId = filePath;
                    //IsDirty = false; // Mark document loaded from persistence as unedited copy (display without dirty mark '*' in name)

                    try
                    {
                        await LoadDocumentInternal(openFilePath);

                        // Set the correct actualy state of the model into the viewmodel
                        // to either allow editing or continue to block editing depending on what the model says

                        ApplySettings();

                        State = DocumentState.IsEditing;
                    }
                    catch (Exception ex)                 // File may be blocked by another process
                    {                    // Try read-only shared method and set file access to read-only

                        throw new Exception(Strings.STR_FILE_OPEN_ERROR_MSG_CAPTION, ex);
                    }
                    finally
                    {
                        IsDirty = false;
                    }
                }
                else
                    throw new FileNotFoundException(openFilePath);   // File does not exist
            }
            catch (Exception exp)
            {
                throw new Exception(Strings.STR_FILE_OPEN_ERROR_MSG_CAPTION, exp);
            }

        }

        protected virtual Task LoadDocumentInternal(string filePath)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterLoadDocumentInternal()
        {
            return Task.Run(() => EnableDocumentFileWatcher(true));
        }

        ///// <summary>
        ///// Method is executed when the background process finishes and returns here
        ///// because it was cancelled or is done processing.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void FileLoaderLoadResultEvent(object sender, ResultEvent e)
        //{
        //    fileLoader.ProcessingResultEvent -= FileLoaderLoadResultEvent;
        //    fileLoader = null;

        //    CommandManager.InvalidateRequerySuggested();

        //    // close documents automatically without message when re-loading on startup
        //    if (State == DocumentState.IsInvalid && CloseOnErrorWithoutMessage == true)
        //    {
        //        OnClose();
        //        return;
        //    }

        //    //// Continue processing in parent of this viewmodel if there is any such requested
        //    //OnFileProcessingResultEvent(e, TypeOfResult.FileLoad);
        //}

        ///// <summary>
        ///// Save this document as.
        ///// </summary>
        ///// <returns></returns>
        //public abstract bool SaveFile(string filePath);

        public bool SaveFile(string filePath)
        {
            try
            {
                SaveDocumentInternal(filePath);

                // Set new file name in viewmodel and model
                FilePath = filePath;
                ContentId = filePath;
                documentModel.SetFileNamePath(filePath, true);

                IsDirty = false;

                return true;
            }
            catch { throw; }
        }

        //protected virtual void BeforeSaveInternal()
        //{

        //}

        protected virtual void SaveDocumentInternal(string filePath)
        { }

        //protected virtual void AfterSaveInternal() { }



        /// <summary>
        /// Return the path of the file representation (if any).
        /// </summary>
        /// <returns></returns>
        public virtual string GetFilePath()
        {
            try
            {
                if (File.Exists(FilePath))
                    return Path.GetDirectoryName(FilePath);
            }
            catch
            {
            }

            return string.Empty;
        }

        #endregion abstract methods

        /// <summary>
        /// Is executed when the user wants to refresh/re-load
        /// the current content with the currently stored inforamtion.
        /// </summary>
        public virtual async Task ReOpen()
        {
            WasChangedExternally = false;

            await OpenFileAsync(FilePath);
        }

        /// <summary>
        /// Search for most inner exceptions and return it to caller.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public Exception GetInnerMostException(Exception exp)
        {
            if (exp != null)
            {
                while (exp.InnerException != null)
                    exp = exp.InnerException;
            }

            if (exp != null)
                return exp;

            return null;
        }

        /// <summary>
        /// Get a path that does not represent this document that indicates
        /// a useful alternative representation (eg: StartPage -> Assembly Path).
        /// </summary>
        /// <returns></returns>
        public virtual string GetAlternativePath()
        {
            return FilePath;
        }

        ///// <summary>
        ///// This method is executed to tell the surrounding framework to close the document.
        ///// </summary>
        //protected virtual void OnClose()
        //{
        //    if (DocumentEvent != null)
        //        DocumentEvent(this, new FileBaseEvent(FileEventType.CloseDocument));
        //}

        /// <summary>
        /// Indicate whether document can be closed or not.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanClose()
        {
            if (State == DocumentState.IsLoading)
                return false;

            return true;
        }

        private void OnCopyFullPathtoClipboardCommand()
        {
            try
            {
                clipBoard.SetText(FilePath);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Opens the folder in which this document is stored in the Windows Explorer.
        /// </summary>
        private void OnOpenContainingFolderCommand()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    // combine the arguments together it doesn't matter if there is a space after ','
                    var argument = @"/select, " + FilePath;

                    Process.Start("explorer.exe", argument);
                }
                else
                {
                    var parentDir = Directory.GetParent(FilePath).FullName;

                    if (Directory.Exists(parentDir) == false)
                        MessageDialog.Show(string.Format(CultureInfo.CurrentCulture, Strings.STR_ACCESS_DIRECTORY_ERROR, parentDir),
                                                        Strings.STR_FILE_FINDING_CAPTION
                                                        );
                    else
                    {
                        string argument = @"/select, " + parentDir;

                        Process.Start("EXPLORER.EXE", argument);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageDialog.Show(string.Format(CultureInfo.CurrentCulture, "{0}\n'{1}'.", ex.Message, (FilePath == null ? string.Empty : FilePath)),
                                                Strings.STR_FILE_FINDING_CAPTION
                                                );
            }
        }


        //public bool OnFileProcessingResultEvent(ResultEvent e, TypeOfResult typeOfResult)
        //{
        //    // Continue processing in parent of this viewmodel if there is any such requested
        //    if (ProcessingResultEvent != null)
        //    {
        //        ProcessingResultEvent(this, new ProcessResultEvent(e.Message, e.Error, e.Cancel, typeOfResult,
        //                                                           e.ResultObjects, e.InnerException));

        //        return true;
        //    }

        //    return false;
        //}

        protected string GetDefaultFileNewName(int iNewFileCounter,
                                               string newDefaultFileName = null,
                                               string newDefaultFileExtension = null
                                               )
        {
            if (newDefaultFileName != null)
                defaultFileName = newDefaultFileName;

            if (newDefaultFileExtension != null)
                defaultFileType = newDefaultFileExtension;

            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
                            this.defaultFileName,
                            (iNewFileCounter == 0 ? string.Empty : " " + iNewFileCounter.ToString()),
                            defaultFileType);
        }

        /// <summary>
        /// Set a file specific value to determine whether file
        /// watching is enabled/disabled for this file.
        /// </summary>
        /// <param name="IsEnabled"></param>
        /// <returns></returns>
        public bool EnableDocumentFileWatcher(bool IsEnabled)
        {
            // Activate file watcher for this document
            return documentModel.EnableDocumentFileWatcher(true);
        }




        public override void Dispose()
        {
            if (documentModel != null)
            {
                documentModel.Dispose();
                documentModel = null;
            }

            base.Dispose();
        }

        public abstract void RegisterDocumentType(IDocumentTypeManager docTypeManager);

        public IList<IErrorMessage> CompileErrors { get; set; } = new List<IErrorMessage>();

        //a flag that is true when we load a document for compiling or building
        //when loading a board for build we don't need to update connections(rastnets) or auto compile
        //we might need to repour polygons, though
        public bool LoadedForCompiler { get; set; } = false;

        public virtual Task<bool> Compile() { return Task.FromResult(true); }

        private IErrorMessage CreateMessage(MessageSeverity severity, string message, string fileName, string projectName, XRect? location = null)
        {
            return new ErrorMessage
            {
                Severity = severity,
                Description = message,
                File = fileName,
                Project = projectName,
                Location = new CanvasLocation
                {
                    File = this,
                    Location = location
                }
            };
        }

        public void AddCompileError(string message, string fileName, string projectName, XRect? location = null)
        {
            var em = CreateMessage(MessageSeverity.Error, message, fileName, projectName, location);
            CompileErrors.Add(em);
        }

        public void AddCompileWarning(string message, string fileName, string projectName, XRect? location = null)
        {
            var em = CreateMessage(MessageSeverity.Warning, message, fileName, projectName, location);
            CompileErrors.Add(em);
        }

        public abstract IList<IDocumentToolWindow> GetToolWindowsWhenActive();
        public void ShowVisibleToolsFiltered(IList<IDocumentToolWindow> visibleTools)
        {
            //show tools that are needed and set the document
            foreach (var tool in visibleTools)
            {
                tool.SetDocument(this);
                tool.IsVisible = IsToolWindowVisible(tool);
            }
        }

        protected virtual bool IsToolWindowVisible(IDocumentToolWindow toolWindow)
        {
            return true;
        }

        #endregion methods
    }
}
